namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    /// <summary>
    /// VM for Inventory Overview.
    /// </summary>
    public sealed class InventoryOverviewVM : ViewModelBase, IDisposable
    {
        private readonly Dictionary<Guid, ProductInfoVM> productsLookup = new Dictionary<Guid, ProductInfoVM>();
        
        private bool isReady = true;
        private bool hasSyncedBefore = false;

        public InventoryOverviewVM()
        {
            this.NewOrderCommand = new RelayCommand(this.CreateNewOrder, this.HasAnySelectedProducts);
            this.UpdateOrderCommand = new RelayCommand(this.UpdateOrder, this.HasAnySelectedProducts);

            this.MessengerInstance.Register<UpdateInventoryMessage>(this, OnUpdateInventory);
            this.MessengerInstance.Register<UpdateConnectionStatusMessage>(this, OnConnectionStatusChanged);
        }

        public ObservableCollection<ProductInfoVM> Products { get; } = new ObservableCollection<ProductInfoVM>();

        public ICommand NewOrderCommand { get; }

        public ICommand UpdateOrderCommand { get; }

        public bool IsReady
        {
            get => this.isReady;
            set => this.Set(ref isReady, value);
        }

        public void Dispose()
        {
            this.Cleanup();
        }

        private void CreateNewOrder()
        {
            this.MessengerInstance.Send(new AddToNewOrderMessage(this.Products.Where(product => product.IsSelected).ToList()));
        }

        private void UpdateOrder()
        {
            this.MessengerInstance.Send(new AddToSelectedOrderMessage(this.Products.Where(product => product.IsSelected).ToList()));
        }

        private bool HasAnySelectedProducts()
        {
            return this.Products.Any(product => product.IsSelected);
        }

        private void OnUpdateInventory(UpdateInventoryMessage message)
        {
            var productInfos = message.Content.ProductInfos;

            // First remove products that no longer exist.
            var latestProductCodes = productInfos.Select(info => info.ProductCode).ToList();
            var productsToRemove = this.productsLookup.Keys.Except(latestProductCodes).ToList();
            foreach (var productCode in productsToRemove)
            {
                var productToRemove = this.productsLookup[productCode];
                this.productsLookup.Remove(productCode);
                this.Products.Remove(productToRemove);
            }

            // Then update existing products or add as new product.
            foreach (var productInfo in productInfos)
            {
                if (this.productsLookup.TryGetValue(productInfo.ProductCode, out var productInfoVm))
                {
                    productInfoVm.UpdateFromModel(productInfo);
                }
                else
                {
                    var newProductInfoVm = ProductInfoVM.CreateFromModel(productInfo);
                    this.productsLookup[productInfo.ProductCode] = newProductInfoVm;
                    this.Products.Add(newProductInfoVm);
                }
            }
        }

        private void OnConnectionStatusChanged(UpdateConnectionStatusMessage message)
        {
            switch (message.Content)
            {
                case ConnectionStatus.Unknown:
                case ConnectionStatus.Disconnected:
                case ConnectionStatus.Connecting:
                    this.IsReady = false;
                    break;
                case ConnectionStatus.Syncing:
                case ConnectionStatus.OutOfSync:
                    this.IsReady = this.hasSyncedBefore;
                    break;
                case ConnectionStatus.Synced:
                    this.IsReady = true;
                    this.hasSyncedBefore = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}