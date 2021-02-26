namespace InventorySystem.Contract
{
    public enum ErrorCode
    {
        Success = 0,
        Disconnected,
        InvalidArgs,
        InsufficientQuantity,

        // Insert new error codes before ErrorCode.Unknown
        Unknown = 65535,
    }
}
