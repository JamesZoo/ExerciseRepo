

namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using GalaSoft.MvvmLight.Messaging;
    using InventorySystem.Client;
    using InventorySystem.Contract;

    public enum ConnectionStatus
    {
        Unknown,
        Disconnected,
        Connecting,
        Syncing,
        OutOfSync,
        Synced,
    }

    /// <summary>
    /// A monitor on an Inventory service.
    ///
    /// The monitoring is done on the background, while notification events for any changes are raised on the UI thread.
    /// </summary>
    public sealed class InventoryMonitor : IDisposable
    {
        private readonly IInventoryServiceClient inventoryServiceClient;
        private readonly Dispatcher dispatcher;
        private readonly IMessenger messenger;

        private readonly CancellationTokenSource monitoringTaskCancellation = new CancellationTokenSource();

        /// <summary>
        /// This field is used to check whether this monitor's understanding of the inventory is up to date.
        ///
        /// This check mechanism relies on the fact that the InventoryService's CheckUpdateAsync() and GetInventoryInfoAsync() both returns the latest timestamp of the inventory.
        /// And it is assumed that the service updates its timestamp whenever there has been a change (e.g. processing an order, or replenishing the inventory).
        ///
        /// Since the service is the only source of truth, comparing a previously obtained "lastUpdateTime" with service's can tell us when is the right moment to refresh Inventory information from the service.
        /// </summary>
        DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;

        public InventoryMonitor(IInventoryServiceClient inventoryServiceClient, Dispatcher dispatcher, IMessenger messenger)
        {
            this.inventoryServiceClient = inventoryServiceClient ?? throw new ArgumentNullException(nameof(inventoryServiceClient));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.messenger = messenger ?? throw new ArgumentNullException(nameof(messenger));
        }

        public void Dispose()
        {
            this.monitoringTaskCancellation.Cancel();
            this.monitoringTaskCancellation.Dispose();
        }

        public void StartMonitoring()
        {
            Task.Run(async () => await this.MonitoringTask(this.monitoringTaskCancellation.Token));
        }

        private async Task MonitoringTask(CancellationToken cancellationToken)
        {
            ConnectionStatus connectionStatus = ConnectionStatus.Unknown;

            void UpdateConnectionStatus(ConnectionStatus newStatus)
            {
                if (connectionStatus != newStatus)
                {
                    connectionStatus = newStatus;
                    SendMessageOnDispatcher(new UpdateConnectionStatusMessage(connectionStatus));
                }
            }

            // Business logic for synchronizing the inventory:
            //
            // The first time will try retrieving data straight away.
            //
            // After that periodically check whether there is any inventory update.
            // only retrieve data if the inventory is updated.
            try
            {
                bool isInitialDataRetrieved = false;
                TimeSpan reconnectDelay = TimeSpan.FromMilliseconds(5000);
                TimeSpan checkUpdateDelay = TimeSpan.FromMilliseconds(2500);
                while (!cancellationToken.IsCancellationRequested)
                {
                    switch (connectionStatus)
                    {
                        case ConnectionStatus.Unknown:
                        {
                            UpdateConnectionStatus(ConnectionStatus.Connecting);
                            break;
                        }
                        case ConnectionStatus.Disconnected:
                        {
                            await Task.Delay(reconnectDelay, cancellationToken).ConfigureAwait(false);
                            UpdateConnectionStatus(ConnectionStatus.Connecting);
                            break;
                        }
                        case ConnectionStatus.Connecting:
                            if (isInitialDataRetrieved)
                            {
                                var checkUpdateResult = await this.inventoryServiceClient.CheckUpdateAsync().ConfigureAwait(false);
                                if (checkUpdateResult.ErrorCode == ErrorCode.Disconnected)
                                {
                                    UpdateConnectionStatus(ConnectionStatus.Disconnected);
                                }
                                else
                                {
                                    UpdateConnectionStatus(this.lastUpdateTime < checkUpdateResult.LastUpdateTime
                                        ? ConnectionStatus.Syncing
                                        : ConnectionStatus.Synced);
                                }
                            }
                            else
                            {
                                UpdateConnectionStatus(ConnectionStatus.Syncing);
                            }

                            break;

                        case ConnectionStatus.Syncing:
                        {
                            var getInventoryInfoResult = await this.inventoryServiceClient.GetInventoryInfoAsync().ConfigureAwait(false);
                            if (getInventoryInfoResult.ErrorCode == ErrorCode.Disconnected)
                            {
                                UpdateConnectionStatus(ConnectionStatus.Disconnected);
                            }
                            else
                            {
                                if (getInventoryInfoResult.ErrorCode == ErrorCode.Success)
                                {
                                    UpdateConnectionStatus(ConnectionStatus.Synced);
                                    this.lastUpdateTime = getInventoryInfoResult.InventoryInfo.LastUpdateTime;
                                    SendMessageOnDispatcher(new UpdateInventoryMessage(getInventoryInfoResult.InventoryInfo));
                                    isInitialDataRetrieved = true;
                                }
                                else
                                {
                                    UpdateConnectionStatus(ConnectionStatus.OutOfSync);
                                }
                            }

                            break;
                        }
                        case ConnectionStatus.OutOfSync:
                        {
                            await Task.Delay(checkUpdateDelay, cancellationToken).ConfigureAwait(false);
                            UpdateConnectionStatus(ConnectionStatus.Syncing);
                            break;
                        }
                        case ConnectionStatus.Synced:
                        {
                            await Task.Delay(checkUpdateDelay, cancellationToken).ConfigureAwait(false);

                            var checkUpdateResult =
                                await this.inventoryServiceClient.CheckUpdateAsync().ConfigureAwait(false);
                            if (checkUpdateResult.ErrorCode == ErrorCode.Disconnected)
                            {
                                UpdateConnectionStatus(ConnectionStatus.Disconnected);
                            }
                            else
                            {
                                if (this.lastUpdateTime < checkUpdateResult.LastUpdateTime)
                                {
                                    UpdateConnectionStatus(ConnectionStatus.Syncing);
                                }
                                else
                                {
                                    UpdateConnectionStatus(ConnectionStatus.Synced);
                                }
                            }

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                // Ignore.
            }
        }

        private void SendMessageOnDispatcher<TMessage>(TMessage message)
            where TMessage : MessageBase
        {
            this.dispatcher.InvokeAsync(() => this.messenger.Send(message));
        }
    }
}
