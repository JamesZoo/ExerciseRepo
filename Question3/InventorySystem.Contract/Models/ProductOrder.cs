namespace InventorySystem.Contract
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ProductOrder
    {
        [DataMember]
        public Guid ProductCode { get; set; }

        [DataMember]
        public uint OrderQuantity { get; set; }
    }
}