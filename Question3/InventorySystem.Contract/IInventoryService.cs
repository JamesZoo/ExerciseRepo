namespace InventorySystem.Contract
{
    using System.ServiceModel;
    using System.Threading.Tasks;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IInventoryService
    {
        /// <summary>
        /// A lightweight operation intended for clients detecting server side changes.
        /// </summary>
        /// <returns>The asynchronous task of this operation.</returns>
        [OperationContract]
        Task<CheckUpdateResult> CheckUpdateAsync();

        /// <summary>
        /// An expensive operation to retrieve the latest inventory info. The client should only call this operation if CheckUpdateAsync detects a difference in LastUpdateTime.
        /// </summary>
        /// <returns>The asynchronous task of this operation</returns>
        [OperationContract]
        Task<GetInventoryInfoResult> GetInventoryInfoAsync();
    }
}
