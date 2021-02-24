namespace InventorySystem.Contract
{
    using System;
    using System.Runtime.Serialization;



    [DataContract]
    public class OperationResult
    {
        /// <summary>
        /// Gets or sets the error code of this operation result. Default is ErrorCode.Success.
        /// </summary>
        [DataMember]
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the last update to the inventory system was made.
        /// </summary>
        [DataMember]
        public DateTimeOffset LastUpdateTime { get; set; }
    }
}
