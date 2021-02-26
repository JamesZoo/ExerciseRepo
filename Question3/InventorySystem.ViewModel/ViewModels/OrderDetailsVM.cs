namespace InventorySystem.ViewModel
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
    }

    /// <summary>
    /// VM for each individual order (i.e. the order card containing product order details).
    /// </summary>
    public sealed class OrderDetailsVM : ViewModelBase, IDisposable
    {
        private readonly object owner;
        private readonly Dictionary<Guid, ProductOrderDetailsVM> productOrderLookup = new Dictionary<Guid, ProductOrderDetailsVM>();

        private OrderStatus orderStatus;
        private DateTimeOffset processedTime = DateTimeOffset.MinValue;
        private bool isSelected;

        public OrderDetailsVM(object owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.RemoveOrderCommand = new RelayCommand(this.RemoveOrder, this.CanRemoveOrder);
            this.ProcessOrderCommand = new RelayCommand(this.ProcessOrder, this.CanProcessOrder);

            this.MessengerInstance.Register<DeleteFromParentMessage<ProductOrderDetailsVM>>(this, this.DeleteProductOrder);
        }

        public DateTimeOffset CreationTime { get; } = DateTimeOffset.Now;

        public ObservableCollection<ProductOrderDetailsVM> ProductOrders { get; } = new ObservableCollection<ProductOrderDetailsVM>();

        public OrderStatus OrderStatus
        {
            get => this.orderStatus;
            set => this.Set(ref this.orderStatus, value);
        }

        public DateTimeOffset ProcessedTime
        {
            get => this.processedTime;
            set => this.Set(ref this.processedTime, value);
        }

        public ICommand RemoveOrderCommand { get; }

        public ICommand ProcessOrderCommand { get; }

        public void Dispose()
        {
            this.Cleanup();

            foreach (var productOrderDetailsVm in ProductOrders)
            {
                productOrderDetailsVm.Dispose();
            }
        }

        public void AddProducts(IEnumerable<ProductInfoVM> productInfos)
        {

        }

        private void RemoveOrder()
        {
            this.MessengerInstance.Send(new DeleteFromParentMessage<OrderDetailsVM>(this, this.owner));
        }

        private bool CanRemoveOrder()
        {
            // TODO: do not let remove when processing the order.
            return true;
        }

        private void ProcessOrder()
        {
        }

        private bool CanProcessOrder()
        {
            // TODO:
            // While processing, do not process again.
            // Once order completed, do not process.
            return true;
        }

        private void DeleteProductOrder(DeleteFromParentMessage<ProductOrderDetailsVM> message)
        {
            this.ProductOrders.Remove(message.ObjectToDelete);
            message.ObjectToDelete.Dispose();
        }
    }
}