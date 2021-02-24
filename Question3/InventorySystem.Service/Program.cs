using System;

namespace InventorySystem.Service
{
    using System.ServiceModel;
    using System.ServiceModel.Description;

    using InventorySystem.Contract;

    class Program
    {
        static void Main(string[] args)
        {
            // Step 1: Create a URI to serve as the base address.
            ServiceHost selfHost = new ServiceHost(typeof(InventoryService));

            try
            {
                // Step 3: Add a service endpoint.
                var endpoint = selfHost.AddServiceEndpoint(typeof(IInventoryService), new WSHttpBinding(), "http://localhost:12345/WCF/InventoryService.svc");

                Console.WriteLine($"InventoryService endpoint: {endpoint.ListenUri}");

                // Step 4: Enable metadata exchange.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.HttpsGetEnabled = true;
                smb.HttpGetUrl = endpoint.ListenUri;
                selfHost.Description.Behaviors.Add(smb);

                // Step 5: Start the service.
                selfHost.Open();
                Console.WriteLine("The service is ready.");

                // Close the ServiceHost to stop the service.
                Console.WriteLine("Press <Enter> to terminate the service.");
                Console.WriteLine();
                Console.ReadLine();
                selfHost.Close();
            }
            catch (CommunicationException ce)
            {
                Console.WriteLine("An exception occurred: {0}", ce.Message);
                selfHost.Abort();
            }
        }
        
    }
}
