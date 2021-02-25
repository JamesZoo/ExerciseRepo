namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// VM for Inventory Overview.
    /// </summary>
    public sealed class InventoryOverviewVM : ViewModelBase, IDisposable
    {
        private readonly Dictionary<Guid, ProductInfoVM> productsLookup = new Dictionary<Guid, ProductInfoVM>();
        private readonly ObservableCollection<ProductInfoVM> products = new ObservableCollection<ProductInfoVM>();
        private bool _isReady = true;

        private bool hasSyncedBefore = false;

        public InventoryOverviewVM()
        {
            this.MessengerInstance.Register<UpdateInventoryMessage>(this, OnUpdateInventory);
            this.MessengerInstance.Register<UpdateConnectionStatusMessage>(this, OnConnectionStatusChanged);
        }

        public ObservableCollection<ProductInfoVM> Products => this.products;

        public bool IsReady
        {
            get => this._isReady;
            set => this.Set(ref _isReady, value);
        }

        public void Dispose()
        {
            this.MessengerInstance.Unregister(this);
        }

        private void OnUpdateInventory(UpdateInventoryMessage message)
        {
            var productInfos = message.Content.ProductInfos;

            // First remove products that no longer exist.
            var latestProductCodes = productInfos.Select(info => info.ProductCode).ToList();
            var productsToRemove = productsLookup.Keys.Except(latestProductCodes).ToList();
            foreach (var productCode in productsToRemove)
            {
                var productToRemove = productsLookup[productCode];
                productsLookup.Remove(productCode);
                this.products.Remove(productToRemove);
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
                    this.products.Add(newProductInfoVm);
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