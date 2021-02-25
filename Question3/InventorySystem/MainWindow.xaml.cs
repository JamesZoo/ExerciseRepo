
namespace InventorySystem
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Client;
    using GalaSoft.MvvmLight.Messaging;
    using ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IInventoryServiceClient inventoryServiceClient;
        private readonly InventoryMonitor inventoryMonitor;
        
        private readonly MainWindowVM mainWindowVm;

        public MainWindow()
        {
            InitializeComponent();

            inventoryServiceClient = InventoryServiceClientFactory.Instance.CreateAutoRecoveryClient("http://localhost:12345/WCF/InventoryService.svc");
            inventoryMonitor = new InventoryMonitor(this.inventoryServiceClient, this.Dispatcher, Messenger.Default);

            this.mainWindowVm = new MainWindowVM();
            this.DataContext = this.mainWindowVm;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.inventoryMonitor.StartMonitoring();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            this.mainWindowVm.Dispose();
            this.inventoryMonitor.Dispose();
            this.inventoryServiceClient.Dispose();
        }
    }
}
