namespace InventorySystem.ViewModel
{
    using GalaSoft.MvvmLight.Messaging;

    internal sealed class UpdateConnectionStatusMessage : GenericMessage<ConnectionStatus>
    {
        public UpdateConnectionStatusMessage(ConnectionStatus connectionStatus) : base(connectionStatus)
        {
        }

        public UpdateConnectionStatusMessage(object sender, ConnectionStatus connectionStatus) : base(sender, connectionStatus)
        {
        }

        public UpdateConnectionStatusMessage(object sender, object target, ConnectionStatus connectionStatus) : base(sender, target, connectionStatus)
        {
        }
    }
}