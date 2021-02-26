using System;

namespace InventorySystem.TestWcfClient
{
    using System.Linq;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Contract;
    using InventorySystem.Client;


    class Program
    {
        static async Task Main(string[] args)
        {
            using (var client = InventoryServiceClientFactory.Instance.CreateAutoRecoveryClient("http://localhost:12345/WCF/InventoryService.svc"))
            {
                for (int i = 0; i < 100; ++i)
                {
                    var checkUpdateResult = await client.CheckUpdateAsync().ConfigureAwait(false);
                    Console.WriteLine($"CheckUpdateAsync: ErrorCode: {checkUpdateResult.ErrorCode}, LastUpdateTime: {checkUpdateResult.LastUpdateTime}");
                    await Task.Delay(500).ConfigureAwait(false);
                    if (checkUpdateResult.ErrorCode == ErrorCode.Disconnected)
                    {
                        continue;
                    }

                    var getInventoryInfoResult = await client.GetInventoryInfoAsync().ConfigureAwait(false);
                    Console.WriteLine($"GetInventoryInfoAsync: ErrorCode: {getInventoryInfoResult.ErrorCode}, LastUpdateTime: {getInventoryInfoResult.InventoryInfo.LastUpdateTime}");
                    if (checkUpdateResult.ErrorCode == ErrorCode.Disconnected)
                    {
                        continue;
                    }

                    var productOrders = getInventoryInfoResult.InventoryInfo.ProductInfos
                        .Where(product => product.Quantity.Numeric >= 1)
                        .Take(3)
                        .Select(product => new ProductOrder() {ProductCode = product.ProductCode, OrderQuantity = 1})
                        .ToList();

                    var processOrderResult = await client.ProcessOrderAsync(new OrderTransaction() { ProductOrders = productOrders });
                    Console.WriteLine($"ProcessOrderAsync: ErrorCode: {processOrderResult.ErrorCode}");

                    await Task.Delay(500).ConfigureAwait(false);
                }
            }

            Console.WriteLine("Press ENTER to end...");
            Console.ReadLine();
        }
    }
}
