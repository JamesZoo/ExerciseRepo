namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Client;
    using Contract;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Rejected,
    }

    /// <summary>
    /// VM for each individual order (i.e. the order card containing product order details).
    /// </summary>
    public sealed class OrderDetailsVM : ViewModelBase, IDisposable
    {
        private readonly object owner;
        private readonly Dictionary<Guid, ProductOrderDetailsVM> productOrderLookup = new Dictionary<Guid, ProductOrderDetailsVM>();

        private OrderStatus orderStatus;
        private DateTimeOffset processedTime = DateTimeOffset.MinValue;
        private bool isSelected;

        public OrderDetailsVM(object owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.RemoveOrderCommand = new RelayCommand(this.RemoveOrder, this.CanRemoveOrder);
            this.ProcessOrderCommand = new RelayCommand(this.ProcessOrder, this.CanProcessOrder);

            this.MessengerInstance.Register<DeleteFromParentMessage<ProductOrderDetailsVM>>(this, this.DeleteProductOrder);
            this.MessengerInstance.Register<UpdateInventoryMessage>(this, OnUpdateInventory);
        }

        public DateTimeOffset CreationTime { get; } = DateTimeOffset.Now;

        public ObservableCollection<ProductOrderDetailsVM> ProductOrders { get; } = new ObservableCollection<ProductOrderDetailsVM>();

        public OrderStatus OrderStatus
        {
            get => this.orderStatus;
            set => this.Set(ref this.orderStatus, value);
        }

        public DateTimeOffset ProcessedTime
        {
            get => this.processedTime;
            set => this.Set(ref this.processedTime, value);
        }

        public ICommand RemoveOrderCommand { get; }

        public ICommand ProcessOrderCommand { get; }

        public void Dispose()
        {
            this.Cleanup();

            foreach (var productOrderDetailsVm in ProductOrders)
            {
                productOrderDetailsVm.Dispose();
            }
        }

        public void AddProducts(IEnumerable<ProductInfoVM> productInfos)
        {
            foreach (var product in productInfos)
            {
                if (this.productOrderLookup.TryGetValue(product.ProductCode, out var productOrder))
                {
                    productOrder.ProductName = product.ProductName;
                    productOrder.MaxQuantity = product.Quantity;
                    if (productOrder.OrderQuantity > product.Quantity)
                    {
                        productOrder.OrderQuantity = product.Quantity;
                    }
                }
                else
                {
                    if (product.Quantity == 0)
                    {
                        continue;
                    }

                    var newProductOrder = new ProductOrderDetailsVM(product.ProductCode, this)
                    {
                        ProductName = product.ProductName,
                        OrderQuantity = 1,
                        MaxQuantity = product.Quantity,
                    };

                    this.productOrderLookup[product.ProductCode] = newProductOrder;
                    this.ProductOrders.Add(newProductOrder);
                }
            }
        }

        private void RemoveOrder()
        {
            this.MessengerInstance.Send(new DeleteFromParentMessage<OrderDetailsVM>(this, this.owner));
        }

        private bool CanRemoveOrder()
        {
            return this.OrderStatus != OrderStatus.Processing;
        }

        private async void ProcessOrder()
        {
            // Run asynchrnously and then marshal back to UI thread after await by using ConfigureAwait(true);

            this.OrderStatus = OrderStatus.Processing;

            var orderTransaction = new OrderTransaction()
            {
                ProductOrders = this.ProductOrders.Select(vm => new ProductOrder() { ProductCode = vm.ProductCode, OrderQuantity = vm.OrderQuantity }).ToList(),
            };

            using (var inventoryServiceClient = InventoryServiceClientFactory.Instance.CreateAutoRecoveryClient(HardCodedServerInfo.InventoryServiceEndpointUri))
            {
                try
                {
                    var result = await inventoryServiceClient.ProcessOrderAsync(orderTransaction).ConfigureAwait(true);
                    if (result.ErrorCode == ErrorCode.Disconnected)
                    {
                        this.OrderStatus = OrderStatus.Pending;
                    }
                    else if (result.ErrorCode == ErrorCode.Success)
                    {
                        this.OrderStatus = OrderStatus.Completed;
                        this.ProcessedTime = result.ProcessedTime;
                    }
                    else
                    {
                        this.OrderStatus = OrderStatus.Rejected;
                    }
                }
                catch (Exception ex)
                {
                    this.OrderStatus = OrderStatus.Rejected;
                    Trace.WriteLine(ex);
                    throw;
                }
            }
        }

        private bool CanProcessOrder()
        {
            return this.OrderStatus == OrderStatus.Pending && this.ProductOrders.Count > 0 && this.ProductOrders.All(productOrder => productOrder.OrderQuantity > 0);
        }

        private void DeleteProductOrder(DeleteFromParentMessage<ProductOrderDetailsVM> message)
        {
            this.ProductOrders.Remove(message.ObjectToDelete);
            message.ObjectToDelete.Dispose();
        }

        private void OnUpdateInventory(UpdateInventoryMessage message)
        {
            if (this.OrderStatus != OrderStatus.Pending)
            {
                return;
            }

            var productInfos = message.Content.ProductInfos;

            // First remove products that no longer exist.
            var latestProductCodes = productInfos.Select(info => info.ProductCode).ToList();
            var productsToRemove = productOrderLookup.Keys.Except(latestProductCodes).ToList();
            foreach (var productCode in productsToRemove)
            {
                var productToRemove = this.productOrderLookup[productCode];
                this.productOrderLookup.Remove(productCode);
                this.ProductOrders.Remove(productToRemove);
                productToRemove.Dispose();
            }

            // Then update existing products or add as new product.
            foreach (var productInfo in productInfos)
            {
                if (this.productOrderLookup.TryGetValue(productInfo.ProductCode, out var productOrder))
                {
                    productOrder.ProductName = productInfo.ProductName;
                    productOrder.MaxQuantity = productInfo.Quantity.Numeric;
                    if (productOrder.OrderQuantity > productInfo.Quantity.Numeric)
                    {
                        productOrder.OrderQuantity = productInfo.Quantity.Numeric;
                    }
                }
            }
        }
    }
}