namespace InventorySystem.View.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using InventorySystem.ViewModel;
    using LiveSearchTextBoxControl;

    /// <summary>
    /// Interaction logic for InventoryOverview.xaml
    /// </summary>
    public partial class InventoryOverview : UserControl
    {
        private readonly CollectionViewSource inventoryDataGridViewSource;
        public InventoryOverview()
        {
            InitializeComponent();
            this.inventoryDataGridViewSource = this.Resources["InventoryDataGridViewSource"] as CollectionViewSource;
        }

        private void LiveSearchTextBox_OnFilter(object sender, RoutedEventArgs e)
        {
            var searchBox = (LiveSearchTextBox)sender;

            this.inventoryDataGridViewSource.View.Filter = 
                obj =>
                {
                    var productInfoVm = obj as ProductInfoVM;
                    if (productInfoVm == null)
                    {
                        return true;
                    }

                    return productInfoVm.ProductName.IndexOf(searchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
                };
        }
    }
}
