namespace InventorySystem.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class OrderTransaction
    {
        [DataMember]
        public List<ProductOrder> ProductOrders { get; set; }
    }
}