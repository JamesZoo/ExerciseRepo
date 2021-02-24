namespace InventorySystem.TestWcfClient.Proxy
{
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using InventorySystem.Contract;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    internal class InventoryServiceClient : ClientBase<IInventoryService>, IInventoryService
    {
        
        public InventoryServiceClient(Binding binding, EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
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
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
    }
}
}
