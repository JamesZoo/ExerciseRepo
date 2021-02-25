

namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    using InventorySystem.Client;
    using InventorySystem.Contract;

    public enum ConnectionStatus
    {
        Unknown,
        Disconnected,
        Connecting,
        Connected,
    }

    /// <summary>
    /// A monitor on an Inventory service.
    ///
    /// The monitoring is done on the background, while notification events for any changes are raised on the UI thread.
    /// </summary>
    public sealed class InventoryMonitor : IDisposable
    {
        private readonly Dispatcher dispatcher;
        private readonly IInventoryServiceClient inventoryServiceClient;
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

        public InventoryMonitor(string inventoryServiceUri, Dispatcher dispatcher)
        {
            _ = inventoryServiceUri ?? throw new ArgumentNullException(nameof(inventoryServiceUri));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.inventoryServiceClient = InventoryServiceClientFactory.Instance.CreateAutoRecoveryClient(inventoryServiceUri);
        }

        public event EventHandler<ConnectionStatus> ConnectionStatusChanged;

        public event EventHandler<List<ProductInfo>> ProductInfoChanged;

        public void Dispose()
        {
            this.monitoringTaskCancellation.Cancel();
            this.monitoringTaskCancellation.Dispose();
            this.inventoryServiceClient.Dispose();
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
                    this.FireConnectionStatusChangedEventOnDispatcher(connectionStatus);
                }
            }

            try
            {
                UpdateConnectionStatus(ConnectionStatus.Disconnected);
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (connectionStatus == ConnectionStatus.Disconnected)
                    {
                        UpdateConnectionStatus(ConnectionStatus.Connecting);
                    }

                    cancellationToken.ThrowIfCancellationRequested();
                    var operationResult = await this.inventoryServiceClient.CheckUpdateAsync().ConfigureAwait(false);
                    if (operationResult.ErrorCode == ErrorCode.Disconnected)
                    {
                        UpdateConnectionStatus(ConnectionStatus.Disconnected);
                    }
                    else
                    {
                        UpdateConnectionStatus(ConnectionStatus.Connected);
                        if (operationResult.ErrorCode == ErrorCode.Success)
                        {
                            if (this.lastUpdateTime < operationResult.LastUpdateTime)
                            {
                                cancellationToken.ThrowIfCancellationRequested();
                                var getInventoryInfoResult = await this.inventoryServiceClient.GetInventoryInfoAsync().ConfigureAwait(false);
                                if (getInventoryInfoResult.ErrorCode == ErrorCode.Disconnected)
                                {
                                    UpdateConnectionStatus(ConnectionStatus.Disconnected);
                                }
                                else
                                {
                                    this.lastUpdateTime = getInventoryInfoResult.LastUpdateTime;
                                    this.FireProductInfoChangedEventOnDispatcher(getInventoryInfoResult.ProductInfos);
                                }
                            }
                        }
                    }

                    await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (Exception ex) when (ex is TaskCanceledException || ex is OperationCanceledException)
            {
                // Ignore.
            }
        }

        private void FireConnectionStatusChangedEventOnDispatcher(ConnectionStatus connectionStatus)
        {
            this.dispatcher.InvokeAsync(() => this.ConnectionStatusChanged?.Invoke(this, connectionStatus));
        }

        private void FireProductInfoChangedEventOnDispatcher(IEnumerable<ProductInfo> productInfos)
        {
            this.dispatcher.InvokeAsync(() => this.ProductInfoChanged?.Invoke(this, productInfos.ToList()));
        }
    }
}
