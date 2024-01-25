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

        private int productId;
        public int ProductId
        {
            get { return productId; }
            set
            {
                if (value != productId)
                {
                    productId = value;
                    OnPropertyChanged(nameof(ProductId));
                }
            }
        }

        private string decreaseAmountId;
        public string DecreaseAmountId
        {
            get { return decreaseAmountId; }
            set
            {
                if (value != decreaseAmountId)
                {
                    decreaseAmountId = value;
                    OnPropertyChanged(nameof(DecreaseAmountId));
                }
            }
        }

        private string increaseAmountId;
        public string IncreaseAmountId
        {
            get { return increaseAmountId; }
            set
            {
                if (value != increaseAmountId)
                {
                    increaseAmountId = value;
                    OnPropertyChanged(nameof(IncreaseAmountId));
                }
            }
        }

        private string amountTextBlockId;
        public string AmountTextBlockId
        {
            get { return amountTextBlockId; }
            set
            {
                if (value != amountTextBlockId)
                {
                    amountTextBlockId = value;
                    OnPropertyChanged(nameof(AmountTextBlockId));
                }
            }
        }

        private string nameTextBlockId;
        public string NameTextBlockId
        {
            get { return nameTextBlockId; }
            set
            {
                if (value != nameTextBlockId)
                {
                    nameTextBlockId = value;
                    OnPropertyChanged(nameof(NameTextBlockId));
                }
            }
        }

        private string priceTextBlockId;
        public string PriceTextBlockId
        {
            get { return priceTextBlockId; }
            set
            {
                if (value != priceTextBlockId)
                {
                    priceTextBlockId = value;
                    OnPropertyChanged(nameof(PriceTextBlockId));
                }
            }
        }

        public DisplayedItem(string? name, string? totalPrice, int amount, double price, int productId)
        {
            ProductName = name;
            ProductPrice = totalPrice;
            ProductAmount = amount;
            ItemPrice = price;
            ProductId = productId;
            DecreaseAmountId = name + "DecreaseAmountButton";
            IncreaseAmountId = name + "IncreaseAmountButton";
            NameTextBlockId = name + "NameTextBlock";
            AmountTextBlockId = name + "AmountTextBlock";
            PriceTextBlockId = name + "PriceTextBlock";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
