namespace InventorySystem.ViewModel
{
    using System.Collections.Generic;
    using GalaSoft.MvvmLight.Messaging;

    internal sealed class AddToNewOrderMessage : GenericMessage<List<ProductInfoVM>>
    {
        public AddToNewOrderMessage(List<ProductInfoVM> products) : base(products)
        {
        }

        public AddToNewOrderMessage(object sender, List<ProductInfoVM> products) : base(sender, products)
        {
        }

        public AddToNewOrderMessage(object sender, object target, List<ProductInfoVM> products) : base(sender, target, products)
        {
        }
    }
}