namespace InventorySystem.ViewModel
{
    using GalaSoft.MvvmLight.Messaging;

    internal sealed class DeleteFromParentMessage<T> : MessageBase
    {
        public DeleteFromParentMessage(T sender, object target) : base(sender, target)
        {
            this.ObjectToDelete = sender;
        }

        public T ObjectToDelete { get; }
    }
}