namespace InventorySystem.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class InventoryInfo
    {
        [DataMember]
        public List<ProductInfo> ProductInfos { get; set; }

        [DataMember]
        public DateTimeOffset LastUpdateTime { get; set; }
    }
}