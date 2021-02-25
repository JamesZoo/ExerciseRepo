namespace InventorySystem.Contract
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class ProductInfo
    {
        [DataMember]
        public Guid ProductCode { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public Quantity Quantity { get; set; }
    }
}
