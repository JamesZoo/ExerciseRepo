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

        public MainWindowVM()
        {
            this.MessengerInstance.Register<UpdateConnectionStatusMessage>(this, OnReceiveUpdateConnectionStatusMessage);
        }

        /// <summary>
        /// Gets InventoryOverview view model.
        /// </summary>
        public InventoryOverviewVM InventoryOverviewVm { get; } = new InventoryOverviewVM();

        public string ConnectionStatus
        {
            get => this.connectionStatus;
            set => this.Set(ref this.connectionStatus, value);
        }

        public void Dispose()
        {
            this.MessengerInstance.Unregister(this);
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
    }
}
