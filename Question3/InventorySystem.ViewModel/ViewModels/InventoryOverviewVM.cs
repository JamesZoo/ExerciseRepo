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

        public InventoryOverviewVM()
        {
            this.MessengerInstance.Register<UpdateInventoryMessage>(this, OnUpdateInventory);
        }

        public ObservableCollection<ProductInfoVM> Products => this.products;

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
    }
}