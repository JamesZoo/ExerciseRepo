namespace InventorySystem.Service
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using InventorySystem.Contract;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class InventoryService : IInventoryService
    {
        public async Task<OperationResult> CheckUpdateAsync()
        {
            await Task.Delay(3000).ConfigureAwait(false);
            return new OperationResult()
            {
                LastUpdateTime = DateTimeOffset.Now,
            };
        }

        public async Task<GetInventoryInfoResult> GetInventoryInfoAsync()
        {
            await Task.Delay(3000).ConfigureAwait(false);
            return new GetInventoryInfoResult()
            {
                LastUpdateTime = DateTimeOffset.Now,
                ProductInfos = new List<ProductInfo>()
                {
                    new ProductInfo() { ProductId = 1, ProductName = "Victorian Coffee Beans 2KG", Quantity = (100, Unit.Package) },
                    new ProductInfo() { ProductId = 2, ProductName = "A2 Full-Cream Milk 2L", Quantity = (50, Unit.Bottle) },
                    new ProductInfo() { ProductId = 3, ProductName = "Truss Tomato", Quantity = (100, Unit.Kilogram) },
                    new ProductInfo() { ProductId = 4, ProductName = "Iceberg Lettuce", Quantity = 200 },
                },
            };
        }
    }
}
