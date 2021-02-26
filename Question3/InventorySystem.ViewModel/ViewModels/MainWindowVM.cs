namespace InventorySystem.ViewModel
{
    using System;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// VM for main window.
    /// </summary>
    public sealed class MainWindowVM : ViewModelBase, IDisposable
    {
        private string connectionStatus = "Unknown";
        private DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;

        public MainWindowVM()
        {
            this.MessengerInstance.Register<UpdateInventoryMessage>(this, OnUpdateInventory);
            this.MessengerInstance.Register<UpdateConnectionStatusMessage>(this, OnReceiveUpdateConnectionStatusMessage);
        }

        /// <summary>
        /// Gets InventoryOverview view model.
        /// </summary>
        public InventoryOverviewVM InventoryOverviewVm { get; } = new InventoryOverviewVM();

        /// <summary>
        /// Gets OrdersOverview view model.
        /// </summary>
        public OrdersOverviewVM OrdersOverviewVm { get; } = new OrdersOverviewVM();

        public string ConnectionStatus
        {
            get => this.connectionStatus;
            set => this.Set(ref this.connectionStatus, value);
        }

        public DateTimeOffset LastUpdateTime
        {
            get => this.lastUpdateTime;
            set => this.Set(ref this.lastUpdateTime, value);
        }

        public void Dispose()
        {
            this.Cleanup();
            this.InventoryOverviewVm.Dispose();
            this.OrdersOverviewVm.Dispose();
        }

        private void OnReceiveUpdateConnectionStatusMessage(UpdateConnectionStatusMessage message)
        {
            switch (message.Content)
            {
                case ViewModel.ConnectionStatus.Unknown:
                    this.ConnectionStatus = "Unknown";
                    break;
                case ViewModel.ConnectionStatus.Disconnected:
                    this.ConnectionStatus = "Disconnected";
                    break;
                case ViewModel.ConnectionStatus.Connecting:
                    this.ConnectionStatus = "Connecting to Inventory";
                    break;
                case ViewModel.ConnectionStatus.Syncing:
                    this.ConnectionStatus = "Synchronizing with Inventory";
                    break;
                case ViewModel.ConnectionStatus.Synced:
                    this.ConnectionStatus = "Synchronized";
                    break;
                case ViewModel.ConnectionStatus.OutOfSync:
                    this.ConnectionStatus = "Failed to Synchronize";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.Content), message.Content, "Undefined connection status detected.");
            }
        }

        private void OnUpdateInventory(UpdateInventoryMessage message)
        {
            this.LastUpdateTime = message.Content.LastUpdateTime;
        }
    }
}
