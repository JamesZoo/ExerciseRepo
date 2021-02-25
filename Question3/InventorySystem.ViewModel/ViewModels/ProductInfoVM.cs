namespace InventorySystem.ViewModel
{
    using System;
    using Contract;
    using GalaSoft.MvvmLight;

    /// <summary>
    /// The VM for each product and quality in Inventory Overview.
    /// </summary>
    public sealed class ProductInfoVM : ObservableObject
    {
        private string productName;
        private uint quantity;
        private string displayUnit;

        public Guid ProductCode { get; set; }

        public string ProductName
        {
            get => this.productName;
            set => this.Set(ref this.productName, value);
        }

        public uint Quantity
        {
            get => this.quantity;
            set => this.Set(ref this.quantity, value);
        }

        public string DisplayUnit
        {
            get => this.displayUnit;
            set => this.Set(ref this.displayUnit, value);
        }

        public static ProductInfoVM CreateFromModel(ProductInfo model)
        {
            return new ProductInfoVM()
            {
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                Quantity = model.Quantity.Numeric,
                DisplayUnit = model.Quantity.Numeric > 1
                    ? ToPluralUnit(model.Quantity.Unit)
                    : ToSingularUnit(model.Quantity.Unit),
            };
        }

        public void UpdateFromModel(ProductInfo model)
        {
            if (this.ProductCode != model.ProductCode)
            {
                throw new InvalidOperationException($"ProductCode mismatch.");
            }

            ProductName = model.ProductName;
            Quantity = model.Quantity.Numeric;
            DisplayUnit = model.Quantity.Numeric > 1
                ? ToPluralUnit(model.Quantity.Unit)
                : ToSingularUnit(model.Quantity.Unit);
        }

        public override string ToString()
        {
            return $"{this.ProductName}: {this.Quantity} {this.DisplayUnit}";
        }

        private static string ToSingularUnit(Unit unit)
        {
            return unit.ToString();
        }

        private static string ToPluralUnit(Unit unit)
        {
            switch (unit)
            {
                case Unit.Unit:
                    return "Units";
                case Unit.Kilogram:
                    return "Kilograms";
                case Unit.Bottle:
                    return "Bottles";
                case Unit.Package:
                    return "Packages";
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }
    }
}