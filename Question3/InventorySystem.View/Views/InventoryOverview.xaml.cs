namespace InventorySystem.View.Views
{
    using System.Windows.Controls;
    
    using InventorySystem.ViewModel;

    /// <summary>
    /// Interaction logic for InventoryOverview.xaml
    /// </summary>
    public partial class InventoryOverview : UserControl
    {
        public InventoryOverview()
        {
            InitializeComponent();
        }

        public void UpdateConnectionStatus(ConnectionStatus connectionStatus)
        {
            // Todo: move the handling to VM once the VM is created.
            this.ConnectionStatusText.Text = connectionStatus.ToString();
        }
    }
}
