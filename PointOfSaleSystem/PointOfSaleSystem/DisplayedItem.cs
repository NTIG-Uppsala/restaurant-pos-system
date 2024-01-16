using System.ComponentModel;

namespace PointOfSaleSystem
{
    public class DisplayedItem : INotifyPropertyChanged
    {
        private string? productName;
        public string? ProductName
        {
            get { return productName; }
            set
            {
                if (value != productName)
                {
                    productName = value;
                    OnPropertyChanged(nameof(ProductName));
                }
            }
        }

        private string? productPrice;
        public string? ProductPrice
        {
            get { return productPrice; }
            set
            {
                if (value != productPrice)
                {
                    productPrice = value;
                    OnPropertyChanged(nameof(ProductPrice));
                }
            }
        }

        private int productAmount;
        public int ProductAmount
        {
            get { return productAmount; }
            set
            {
                if (value != productAmount)
                {
                    productAmount = value;
                    OnPropertyChanged(nameof(ProductAmount));
                }
            }
        }

        private double itemPrice;
        public double ItemPrice
        {
            get { return itemPrice; }
            set
            {
                if (value != itemPrice)
                {
                    itemPrice = value;
                    OnPropertyChanged(nameof(ItemPrice));
                }
            }
        }

        public DisplayedItem(string? name, string? totalPrice, int amount, double price)
        {
            ProductName = name;
            ProductPrice = totalPrice;
            ProductAmount = amount;
            ItemPrice = price;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
