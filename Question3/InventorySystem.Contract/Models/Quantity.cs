namespace InventorySystem.Contract
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class Quantity
    {
        /// <summary>
        /// Gets or sets the numeric part of the quantity.
        /// </summary>
        [DataMember]
        public uint Numeric { get; set; }

        [DataMember]
        public Unit Unit { get; set; }

        public static implicit operator Quantity((uint numeric, Unit unit) input)
            => new Quantity()
            {
                Numeric = input.numeric,
                Unit = input.unit,
            };

        public static implicit operator Quantity(uint numeric)
            => new Quantity()
            {
                Numeric = numeric,
                Unit = Unit.Unit,
            };
    }
}
