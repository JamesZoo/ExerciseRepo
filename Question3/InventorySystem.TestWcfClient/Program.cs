using System;

namespace InventorySystem.TestWcfClient
{
    using System.ServiceModel;
    using System.Threading.Tasks;
    using Proxy;

    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new InventoryServiceClient(new WSHttpBinding(), new EndpointAddress("http://localhost:12345/WCF/InventoryService.svc"));

            try
            {
                var result = await client.GetInventoryInfoAsync().ConfigureAwait(false);
                Console.WriteLine(result.LastUpdateTime);
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine(ex);
            }

            Console.WriteLine("Press ENTER to end...");
            Console.ReadLine();
        }
    }
}
