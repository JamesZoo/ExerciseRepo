namespace InventorySystem.Contract
{
    public enum ErrorCode
    {
        Success = 0,
        Disconnected,
        ProductCodeNotFound,
        InsufficientQuantity,
        DuplicateProductCode,

        // Insert new error codes before ErrorCode.Unknown
        Unknown = 65535,
    }
}
