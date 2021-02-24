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
                    try
                    {
                        var result = await client.GetInventoryInfoAsync().ConfigureAwait(false);
                        Console.WriteLine(result.LastUpdateTime);
                    }
                    catch (Exception ex) when (ex is CommunicationException || ex is TimeoutException)
                    {
                        Console.WriteLine(ex.GetType().FullName);
                    }

                    await Task.Delay(500).ConfigureAwait(false);
                }
            }

            Console.WriteLine("Press ENTER to end...");
            Console.ReadLine();
        }
    }
}
