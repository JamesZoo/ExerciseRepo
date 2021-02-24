using System;

namespace InventorySystem.TestWcfClient
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using InventorySystem.Client;


    class Program
    {
        static async Task Main(string[] args)
        {
            using (var client = InventoryServiceClientFactory.Instance.CreateAutoRecoveryClient("http://localhost:12345/WCF/InventoryService.svc"))
            {
                for (int i = 0; i < 100; ++i)
                {
                    var result = await client.CheckUpdateAsync().ConfigureAwait(false);
                    Console.WriteLine($"CheckUpdateAsync: ErrorCode: {result.ErrorCode}, LastUpdateTime: {result.LastUpdateTime}");

                    await Task.Delay(500).ConfigureAwait(false);
                }
            }

            Console.WriteLine("Press ENTER to end...");
            Console.ReadLine();
        }
    }
}
