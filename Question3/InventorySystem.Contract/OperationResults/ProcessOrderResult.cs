namespace InventorySystem.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class ProcessOrderResult : OperationResult
    {
        [DataMember]
        public DateTimeOffset ProcessedTime { get; set; }
    }
}