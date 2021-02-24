using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.Client
{
    using Contract;

    public interface IInventoryServiceClientFactory
    {
        IInventoryServiceClient CreateAutoRecoveryClient(string endpointUri);
    }

    public class InventoryServiceClientFactory : IInventoryServiceClientFactory
    {
        private static readonly Lazy<InventoryServiceClientFactory> lazyInstance =
            new Lazy<InventoryServiceClientFactory>(() => new InventoryServiceClientFactory(), isThreadSafe:true);

        public static IInventoryServiceClientFactory Instance => InventoryServiceClientFactory.lazyInstance.Value;

        internal InventoryServiceClientFactory()
        {
        }

        public IInventoryServiceClient CreateAutoRecoveryClient(string endpointUri)
        {
            _ = endpointUri ?? throw new ArgumentNullException(nameof(endpointUri));
            return new AutoRecoveryClient(this, endpointUri);
        }

        internal InventoryServiceClient CreateClient(string endpointUri)
        {
            _ = endpointUri ?? throw new ArgumentNullException(nameof(endpointUri));
            return new InventoryServiceClient(endpointUri);
        }
    }
}
