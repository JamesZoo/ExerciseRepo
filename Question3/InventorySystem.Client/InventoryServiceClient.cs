namespace InventorySystem.Client
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
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
        
        public System.Threading.Tasks.Task<OperationResult> CheckUpdateAsync()
        {
            return base.Channel.CheckUpdateAsync();
        }
        
        public System.Threading.Tasks.Task<GetInventoryInfoResult> GetInventoryInfoAsync()
        {
            return base.Channel.GetInventoryInfoAsync();
        }
    }
}
