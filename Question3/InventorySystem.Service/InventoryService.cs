namespace InventorySystem.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using InventorySystem.Contract;

    /// <summary>
    /// A naive implementation of InventoryService with a single instance and in-memory data.
    ///
    /// A sophisticated WCF service shim should be have the instance context mode of per session or per call, and ideally stateless.
    /// The consolidation of the data interaction should be performed in a database or DB server.
    ///
    /// Due to time limitation, here I chose to simulate DB consolidation using a single instance,
    /// whereas data consolidation is achieved via sync lock rather than transaction.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class InventoryService : IInventoryService
    {
        private readonly object dataLock = new object();
        private readonly Dictionary<Guid, ProductInfo> products;
        private DateTimeOffset lastUpdateTime = DateTimeOffset.MinValue;
        
        public InventoryService()
        {
            // The data initialization here is only possible because of the single instance.
            var productInfos = new List<ProductInfo>()
            {
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Victorian Coffee Beans 2KG", Quantity = (100, Unit.Package) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Bega Vintage Cheese 200g", Quantity = (1, Unit.Package) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "A2 Full-Cream Milk 2L", Quantity = (50, Unit.Bottle) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "A2 Light Milk 2L", Quantity = (1, Unit.Bottle) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Truss Tomato", Quantity = (100, Unit.Kilogram) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Lebanese Cucumber", Quantity = (120, Unit.Kilogram) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "King Prawn", Quantity = (100, Unit.Kilogram) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Banana Prawn", Quantity = (1, Unit.Kilogram) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Truss Tomato", Quantity = (80, Unit.Kilogram) },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Iceberg Lettuce", Quantity = 200 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Watermelon", Quantity = 1 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple Watch Series 6", Quantity = 50 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 11", Quantity = 10 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 11 Pro", Quantity = 10 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 11 Pro Max", Quantity = 20 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 11 Mini", Quantity = 1 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 12 Mini", Quantity = 10 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 12", Quantity = 100 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 12 Pro", Quantity = 10 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple iPhone 12 Pro Max", Quantity = 10 },
                new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Apple Watch Series 5", Quantity = 1 },
            };

            this.products = productInfos.ToDictionary(product => product.ProductCode);
            this.lastUpdateTime = DateTimeOffset.Now;
        }

        public async Task<CheckUpdateResult> CheckUpdateAsync()
        {
            await Task.Delay(1500).ConfigureAwait(false);

            lock (this.dataLock)
            {
                return new CheckUpdateResult()
                {
                    LastUpdateTime = this.lastUpdateTime,
                };
            }
        }

        public async Task<GetInventoryInfoResult> GetInventoryInfoAsync()
        {
            await Task.Delay(1500).ConfigureAwait(false);

            lock (this.dataLock)
            {
                return new GetInventoryInfoResult()
                {
                    InventoryInfo = new InventoryInfo()
                    {
                        ProductInfos = this.products.Values.ToList(),
                        LastUpdateTime = this.lastUpdateTime,
                    }
                };
            }
        }

        public async Task<ProcessOrderResult> ProcessOrderAsync(OrderTransaction orderTransaction)
        {
            await Task.Delay(1500).ConfigureAwait(false);

            if (orderTransaction == null || orderTransaction.ProductOrders == null)
            {
                return new ProcessOrderResult() { ErrorCode = ErrorCode.InvalidArgs };
            }

            lock (this.dataLock)
            {
                foreach (var productOrder in orderTransaction.ProductOrders)
                {
                    if (!this.products.TryGetValue(productOrder.ProductCode, out var productInfo) || productOrder.OrderQuantity > productInfo.Quantity.Numeric)
                    {
                        return new ProcessOrderResult() { ErrorCode = ErrorCode.InsufficientQuantity };
                    }
                }

                foreach (var productOrder in orderTransaction.ProductOrders)
                {
                    this.products[productOrder.ProductCode].Quantity.Numeric -= productOrder.OrderQuantity;
                }

                this.lastUpdateTime = DateTimeOffset.Now;

                return new ProcessOrderResult() { ErrorCode = ErrorCode.Success, ProcessedTime = this.lastUpdateTime };
            }
        }
    }
}
