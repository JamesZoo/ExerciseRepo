namespace InventorySystem.ViewModel
{
    using Contract;
    using GalaSoft.MvvmLight.Messaging;

    internal sealed class UpdateInventoryMessage : GenericMessage<InventoryInfo>
    {
        public UpdateInventoryMessage(InventoryInfo inventory) : base(inventory)
        {
        }

        public UpdateInventoryMessage(object sender, InventoryInfo inventory) : base(sender, inventory)
        {
        }

        public UpdateInventoryMessage(object sender, object target, InventoryInfo inventory) : base(sender, target, inventory)
        {
        }
    }
}