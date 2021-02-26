namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// The VM for the Orders Overview container.
    /// </summary>
    public sealed class OrdersOverviewVM : ViewModelBase, IDisposable
    {
        private OrderDetailsVM selectedOrder;

        public OrdersOverviewVM()
        {
            this.MessengerInstance.Register<AddToNewOrderMessage>(this, AddToNewOrder);
            this.MessengerInstance.Register<AddToSelectedOrderMessage>(this, AddToSelectedOrder);
            this.MessengerInstance.Register<DeleteFromParentMessage<OrderDetailsVM>>(this, DeleteOrder);
        }

        public ObservableCollection<OrderDetailsVM> Orders { get; } = new ObservableCollection<OrderDetailsVM>();

        public OrderDetailsVM SelectedOrder
        {
            get => this.selectedOrder;
            set => this.Set(ref this.selectedOrder, value);
        }

        public void Dispose()
        {
            this.Cleanup();
            foreach (var orderDetailsVm in Orders)
            {
                orderDetailsVm.Dispose();
            }
        }

        private void AddToNewOrder(AddToNewOrderMessage message)
        {
            var newOrder = new OrderDetailsVM(this);
            this.Orders.Insert(0, newOrder);
            this.SelectedOrder = newOrder;

            var products = message.Content;
            this.SelectedOrder.AddProducts(products);
        }

        private void AddToSelectedOrder(AddToSelectedOrderMessage message)
        {
            if (this.SelectedOrder == null)
            {
                var newOrder = new OrderDetailsVM(this);
                this.Orders.Insert(0, newOrder);
                this.SelectedOrder = newOrder;
            }

            var products = message.Content;
            this.SelectedOrder.AddProducts(products);
        }

        private void DeleteOrder(DeleteFromParentMessage<OrderDetailsVM> message)
        {
            this.Orders.Remove(message.ObjectToDelete);
            message.ObjectToDelete.Dispose();
        }
    }
}