
namespace InventorySystem
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly InventoryMonitor inventoryMonitor;

        public MainWindow()
        {
            inventoryMonitor = new InventoryMonitor("http://localhost:12345/WCF/InventoryService.svc", this.Dispatcher);

            inventoryMonitor.ConnectionStatusChanged += InventoryMonitor_ConnectionStatusChanged;
            inventoryMonitor.ProductInfoChanged += InventoryMonitor_ProductInfoChanged;
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.inventoryMonitor.StartMonitoring();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            this.inventoryMonitor.Dispose();
        }

        private void InventoryMonitor_ConnectionStatusChanged(object sender, ConnectionStatus e)
        {
            this.InventoryOverview.UpdateConnectionStatus(e);
        }

        private void InventoryMonitor_ProductInfoChanged(object sender, System.Collections.Generic.List<Contract.ProductInfo> e)
        {
            throw new NotImplementedException();
        }
    }
}
