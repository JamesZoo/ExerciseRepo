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
    }
}
