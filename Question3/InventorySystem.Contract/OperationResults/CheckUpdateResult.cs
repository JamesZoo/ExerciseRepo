namespace InventorySystem.Contract
{
    using System;
    using System.Runtime.Serialization;

    public class CheckUpdateResult : OperationResult
    {
        
        /// <summary>
        /// Gets or sets the timestamp indicating when the last update to the inventory system was made.
        /// </summary>
        [DataMember]
        public DateTimeOffset LastUpdateTime { get; set; }
    }
}