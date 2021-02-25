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
        public async Task<CheckUpdateResult> CheckUpdateAsync()
        {
            await Task.Delay(3000).ConfigureAwait(false);
            return new CheckUpdateResult()
            {
                LastUpdateTime = DateTimeOffset.Now,
            };
        }

        public async Task<GetInventoryInfoResult> GetInventoryInfoAsync()
        {
            await Task.Delay(3000).ConfigureAwait(false);
            return new GetInventoryInfoResult()
            {
                InventoryInfo = new InventoryInfo()
                {
                
                    ProductInfos = new List<ProductInfo>()
                    {
                        new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Victorian Coffee Beans 2KG", Quantity = (100, Unit.Package) },
                        new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "A2 Full-Cream Milk 2L", Quantity = (50, Unit.Bottle) },
                        new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Truss Tomato", Quantity = (100, Unit.Kilogram) },
                        new ProductInfo() { ProductCode = Guid.NewGuid(), ProductName = "Iceberg Lettuce", Quantity = 200 },
                    },

                    LastUpdateTime = DateTimeOffset.Now,
                }
            };
        }
    }
}
