namespace InventorySystem.ViewModel
{
    using System.Collections.Generic;
    using GalaSoft.MvvmLight.Messaging;

    internal sealed class AddToSelectedOrderMessage : GenericMessage<List<ProductInfoVM>>
    {
        public AddToSelectedOrderMessage(List<ProductInfoVM> products) : base(products)
        {
        }

        public AddToSelectedOrderMessage(object sender, List<ProductInfoVM> products) : base(sender, products)
        {
        }

        public AddToSelectedOrderMessage(object sender, object target, List<ProductInfoVM> products) : base(sender, target, products)
        {
        }
    }
}