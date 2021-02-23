namespace InventorySystem.Contract
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class ProductInfo
    {
        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public Quantity Quantity { get; set; }
    }
}
