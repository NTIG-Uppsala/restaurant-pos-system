using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService db = new DatabaseService();
        private readonly BusinessLogicService businessLogic = new BusinessLogicService();
        private ObservableCollection<DisplayedItem> ProductWindowItems = new ObservableCollection<DisplayedItem>();
        private string usedData;

        public MainWindow()
        {
            InitializeComponent();

            DotNetEnv.Env.Load();
            usedData = Environment.GetEnvironmentVariable("USEDDATA");

            db.GenerateDatabase(usedData).Wait();

            // Load categories into the Categories property
            db.categories = DatabaseService.LoadCategoriesFromDatabase();

            // Load products from the Database
            db.LoadProductsFromDatabase(usedData);

            categoryButtonsControl.ItemsSource = businessLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
            productButtonsControl.ItemsSource = businessLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
            productWindow.ItemsSource = ProductWindowItems;
        }

        private void OnProductButtonClick(object sender, RoutedEventArgs e)
        {
            // Retrieve the product name and price from the button's tag
            string productName = ((Product)((Button)sender).DataContext).Name;
            double productPrice = ((Product)((Button)sender).DataContext).Price;

            UpdateProductWindow(productName, productPrice);

            double totalFromProductWindow = ProductWindowItems.Sum(x => x.ProductAmount * x.ItemPrice);
            totalPrice.Content = totalFromProductWindow.ToString("0.00") + " kr";
        }

        private void UpdateProductWindow(string productName, double productPrice)
        {
            // Find the first item in the ProductWindowItems collection where the ProductName matches the specified productName
            var addedItem = ProductWindowItems.FirstOrDefault(x => x.ProductName == productName);

            if (addedItem != null)
            {
                addedItem.ProductAmount += 1;
                addedItem.ProductPrice = (addedItem.ItemPrice * addedItem.ProductAmount).ToString("0.00") + " kr";
            }
            else
            {
                string totalPrice = productPrice.ToString("0.00") + " kr";

                ProductWindowItems.Add(new DisplayedItem(productName, totalPrice, 1, productPrice));
            }

            productWindow.ItemsSource = ProductWindowItems;
        }

        private void ResetOrder(object sender, RoutedEventArgs e)
        {
            ProductWindowItems.Clear();
            productWindow.ItemsSource = ProductWindowItems;
            double totalFromProductWindow = ProductWindowItems.Sum(x => x.ProductAmount * x.ItemPrice);
            totalPrice.Content = totalFromProductWindow.ToString("0.00") + " kr";
        }

        private void OnReturnButtonClick(object sender, RoutedEventArgs e)
        {
            businessLogic.productPanelPosition = 0;
            db.currentProducts = new ObservableCollection<Product>(db.products.Where(item => item.IsCommon));
            productButtonsControl.ItemsSource = businessLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button categoryButton && categoryButton.DataContext is DatabaseCategory selectedCategory)
            {
                // Filter products based on the selected category
                db.currentProducts = new ObservableCollection<Product>(db.products.Where(item => item.CategoryID == selectedCategory.Id));

                // Update the ItemsControl's ItemsSource with filtered products
                productButtonsControl.ItemsSource = db.currentProducts;

                businessLogic.productPanelPosition = 0;
                ProductPageNumber.Text = Convert.ToString($"{(businessLogic.productPanelPosition / businessLogic.ProductLimit) + 1}/{(BusinessLogicService.GetClosestMultiple(db.currentProducts.Count, businessLogic.ProductLimit) / businessLogic.ProductLimit) + 1}");
            }
        }

        private void OnNextProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.currentProducts.Count <= businessLogic.ProductLimit)
            {
                return;
            }

            if ((businessLogic.productPanelPosition + businessLogic.ProductLimit) <= BusinessLogicService.GetClosestMultiple(db.currentProducts.Count, businessLogic.ProductLimit))
            {
                businessLogic.productPanelPosition += businessLogic.ProductLimit;
            }
            else
            {
                businessLogic.productPanelPosition = 0;
            }

            productButtonsControl.ItemsSource = businessLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnPreviousProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.currentProducts.Count <= businessLogic.ProductLimit)
            {
                return;
            }

            if ((businessLogic.productPanelPosition - businessLogic.ProductLimit) >= 0)
            {
                businessLogic.productPanelPosition -= businessLogic.ProductLimit;
            }
            else
            {
                businessLogic.productPanelPosition = BusinessLogicService.GetClosestMultiple(db.currentProducts.Count, businessLogic.ProductLimit);
            }
            productButtonsControl.ItemsSource = businessLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnNextCatagoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.categories.Count <= businessLogic.CategoryLimit)
            {
                return;
            }

            if ((businessLogic.categoryPanelPosition + businessLogic.CategoryLimit) <= BusinessLogicService.GetClosestMultiple(db.categories.Count, businessLogic.CategoryLimit))
            {
                businessLogic.categoryPanelPosition += businessLogic.CategoryLimit;
            }
            else
            {
                businessLogic.categoryPanelPosition = 0;
            }

            categoryButtonsControl.ItemsSource = businessLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
        }

        private void OnPreviousCatagoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.categories.Count <= businessLogic.CategoryLimit)
            {
                return;
            }

            if ((businessLogic.categoryPanelPosition - businessLogic.CategoryLimit) >= 0)
            {
                businessLogic.categoryPanelPosition -= businessLogic.CategoryLimit;
            }
            else
            {
                businessLogic.categoryPanelPosition = BusinessLogicService.GetClosestMultiple(db.categories.Count, businessLogic.CategoryLimit);
            }
            categoryButtonsControl.ItemsSource = businessLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
        }
    }

    // Product class represents a product in the user interface
    public class Product
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryID { get; set; }
        public int Priority { get; set; }
        public bool IsCommon { get; set; }
        public string? Color { get; set; }

        public Product(int id, string name, double price, int categoryID, int priority, bool isCommon, string? color)
        {
            ID = id;
            Name = name;
            Price = price;
            CategoryID = categoryID;
            Priority = priority;
            IsCommon = isCommon;
            Color = color;
        }
    }
}
