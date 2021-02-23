namespace InventorySystem.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.Runtime.Serialization;

    [DataContract]
    public sealed class GetInventoryInfoResult : OperationResult
    {
        [DataMember]
        public List<ProductInfo> ProductInfos { get; set; }
    }
}
