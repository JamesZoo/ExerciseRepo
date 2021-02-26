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
        private bool isReady;

        private bool hasSyncedBefore;

        public OrdersOverviewVM()
        {
            this.MessengerInstance.Register<AddToNewOrderMessage>(this, AddToNewOrder);
            this.MessengerInstance.Register<AddToSelectedOrderMessage>(this, AddToSelectedOrder);
            this.MessengerInstance.Register<DeleteFromParentMessage<OrderDetailsVM>>(this, DeleteOrder);
            this.MessengerInstance.Register<UpdateConnectionStatusMessage>(this, OnConnectionStatusChanged);
        }

        public ObservableCollection<OrderDetailsVM> Orders { get; } = new ObservableCollection<OrderDetailsVM>();

        public OrderDetailsVM SelectedOrder
        {
            get => this.selectedOrder;
            set => this.Set(ref this.selectedOrder, value);
        }

        public bool IsReady
        {
            get => this.isReady;
            set => this.Set(ref this.isReady, value);
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

        private void OnConnectionStatusChanged(UpdateConnectionStatusMessage message)
        {
            switch (message.Content)
            {
                case ConnectionStatus.Unknown:
                case ConnectionStatus.Disconnected:
                case ConnectionStatus.Connecting:
                    this.IsReady = false;
                    break;
                case ConnectionStatus.Syncing:
                case ConnectionStatus.OutOfSync:
                    this.IsReady = this.hasSyncedBefore;
                    break;
                case ConnectionStatus.Synced:
                    this.IsReady = true;
                    this.hasSyncedBefore = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}