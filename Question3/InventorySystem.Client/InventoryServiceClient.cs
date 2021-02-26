namespace InventorySystem.Client
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;
    using InventorySystem.Contract;

    public interface IInventoryServiceClient : IInventoryService, IDisposable
    {
    }

    [System.Diagnostics.DebuggerStepThrough]
    internal sealed class InventoryServiceClient : ClientBase<IInventoryService>, IInventoryServiceClient
    {
        
        public InventoryServiceClient(string endpointUri) :
            base(new WSHttpBinding(), new EndpointAddress(endpointUri))
        {
        }
        
        public Task<CheckUpdateResult> CheckUpdateAsync()
        {
            return base.Channel.CheckUpdateAsync();
        }
        
        public Task<GetInventoryInfoResult> GetInventoryInfoAsync()
        {
            return base.Channel.GetInventoryInfoAsync();
        }

        public Task<ProcessOrderResult> ProcessOrderAsync(OrderTransaction orderTransaction)
        {
            return base.Channel.ProcessOrderAsync(orderTransaction);
        }
    }
}
