namespace InventorySystem.ViewModel
{
    using System;
    using System.Windows.Input;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Command;

    /// <summary>
    /// VM for each product and order quantity line in OrderDetails view.
    /// </summary>
    public sealed class ProductOrderDetailsVM : ViewModelBase, IDisposable
    {
        private readonly object owner;
        private string productName;
        private uint orderQuantity;
        private uint maxQuantity;

        public ProductOrderDetailsVM(Guid productCode, object owner)
        {
            this.ProductCode = productCode;
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            this.RemoveFromOrderCommand = new RelayCommand(this.RemoveFromOrder);
        }

        public Guid ProductCode { get; }

        public string ProductName
        {
            get => this.productName;
            set => this.Set(ref this.productName, value);
        }

        public uint OrderQuantity
        {
            get => this.orderQuantity;
            set => this.Set(ref this.orderQuantity, value);
        }

        public uint MaxQuantity
        {
            get => this.maxQuantity;
            set => this.Set(ref this.maxQuantity, value);
        }

        public ICommand RemoveFromOrderCommand { get; }

        public void Dispose()
        {
            this.Cleanup();
        }

        private void RemoveFromOrder()
        {
            this.MessengerInstance.Send(new DeleteFromParentMessage<ProductOrderDetailsVM>(this, this.owner));
        }
    }
}