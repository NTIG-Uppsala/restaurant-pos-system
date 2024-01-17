using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService DatabaseLogic = new DatabaseService();
        private readonly ButtonDisplayLogicService ButtonDisplayLogic = new ButtonDisplayLogicService();
        private ObservableCollection<DisplayedItem> ProductWindowItems = new ObservableCollection<DisplayedItem>();
        private string UsedData;

        public MainWindow()
        {
            InitializeComponent();

            DotNetEnv.Env.Load();
            UsedData = Environment.GetEnvironmentVariable("USEDDATA");

            DatabaseLogic.GenerateDatabase(UsedData).Wait();

            // Load categories into the Categories property
            DatabaseLogic.Categories = DatabaseService.LoadCategoriesFromDatabase();

            // Load products from the Database
            DatabaseLogic.LoadProductsFromDatabase(UsedData);

            categoryButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedCategories(DatabaseLogic.Categories, CategoryPageNumber);
            productButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedProducts(DatabaseLogic.CurrentProducts, ProductPageNumber);
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
            var addedItem = ProductWindowItems.FirstOrDefault(item => item.ProductName == productName);

            if (addedItem != null)
            {
                addedItem.ProductAmount += 1;
                addedItem.ProductPrice = (addedItem.ItemPrice * addedItem.ProductAmount).ToString("0.00") + " kr";
            }
            else
            {
                string totalProductPrice = productPrice.ToString("0.00") + " kr";
                ProductWindowItems.Add(new DisplayedItem(productName, totalProductPrice, 1, productPrice));
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
            ButtonDisplayLogic.ProductPanelPosition = 0;
            DatabaseLogic.CurrentProducts = new ObservableCollection<Product>(DatabaseLogic.Products.Where(item => item.IsCommon));
            productButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedProducts(DatabaseLogic.CurrentProducts, ProductPageNumber);
        }

        private void OnCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button categoryButton && categoryButton.DataContext is DatabaseCategory selectedCategory)
            {
                // Filter products based on the selected category
                DatabaseLogic.CurrentProducts = new ObservableCollection<Product>(DatabaseLogic.Products.Where(item => item.CategoryID == selectedCategory.Id));

                // Update the ItemsControl's ItemsSource with filtered products
                productButtonsControl.ItemsSource = DatabaseLogic.CurrentProducts;

                // Sets the position in the new view to the first page
                ButtonDisplayLogic.ProductPanelPosition = 0;

                // Updates the product page number based on the current page and total amount of pages
                var currentPageNumber = (ButtonDisplayLogic.ProductPanelPosition / ButtonDisplayLogicService.ProductLimit) + 1;
                var totalCategoryButtons = ButtonDisplayLogicService.GetClosestMultiple(DatabaseLogic.CurrentProducts.Count, ButtonDisplayLogicService.ProductLimit);
                var totalPageNumber = (totalCategoryButtons / ButtonDisplayLogicService.ProductLimit) + 1;

                ProductPageNumber.Text = Convert.ToString($"{currentPageNumber}/{totalPageNumber}");
            }
        }

        private void OnNextProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (DatabaseLogic.CurrentProducts.Count <= ButtonDisplayLogicService.ProductLimit)
            {
                return;
            }

            var nextPagePosition = ButtonDisplayLogic.ProductPanelPosition + ButtonDisplayLogicService.ProductLimit;
            var canGoToNextProductPage = nextPagePosition <= ButtonDisplayLogicService.GetClosestMultiple(DatabaseLogic.CurrentProducts.Count, ButtonDisplayLogicService.ProductLimit);
            if (canGoToNextProductPage)
            {
                ButtonDisplayLogic.ProductPanelPosition += ButtonDisplayLogicService.ProductLimit;
            }
            else
            {
                ButtonDisplayLogic.ProductPanelPosition = 0;
            }

            productButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedProducts(DatabaseLogic.CurrentProducts, ProductPageNumber);
        }

        private void OnPreviousProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (DatabaseLogic.CurrentProducts.Count <= ButtonDisplayLogicService.ProductLimit)
            {
                return;
            }

            if ((ButtonDisplayLogic.ProductPanelPosition - ButtonDisplayLogicService.ProductLimit) >= 0)
            {
                ButtonDisplayLogic.ProductPanelPosition -= ButtonDisplayLogicService.ProductLimit;
            }
            else
            {
                // Gets the first product posiiton of the last page
                ButtonDisplayLogic.ProductPanelPosition = ButtonDisplayLogicService.GetClosestMultiple(DatabaseLogic.CurrentProducts.Count, ButtonDisplayLogicService.ProductLimit);
            }
            productButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedProducts(DatabaseLogic.CurrentProducts, ProductPageNumber);
        }

        private void OnNextCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (DatabaseLogic.Categories.Count <= ButtonDisplayLogicService.CategoryLimit)
            {
                return;
            }

            var nextPagePosition = ButtonDisplayLogic.CategoryPanelPosition + ButtonDisplayLogicService.CategoryLimit;
            var canGoToNextCategoryPage = nextPagePosition <= ButtonDisplayLogicService.GetClosestMultiple(DatabaseLogic.Categories.Count, ButtonDisplayLogicService.CategoryLimit);
            if (canGoToNextCategoryPage)
            {
                ButtonDisplayLogic.CategoryPanelPosition += ButtonDisplayLogicService.CategoryLimit;
            }
            else
            {
                ButtonDisplayLogic.CategoryPanelPosition = 0;
            }

            categoryButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedCategories(DatabaseLogic.Categories, CategoryPageNumber);
        }

        private void OnPreviousCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (DatabaseLogic.Categories.Count <= ButtonDisplayLogicService.CategoryLimit)
            {
                return;
            }

            if ((ButtonDisplayLogic.CategoryPanelPosition - ButtonDisplayLogicService.CategoryLimit) >= 0)
            {
                ButtonDisplayLogic.CategoryPanelPosition -= ButtonDisplayLogicService.CategoryLimit;
            }
            else
            {
                // Gets the first category posiiton of the last page
                ButtonDisplayLogic.CategoryPanelPosition = ButtonDisplayLogicService.GetClosestMultiple(DatabaseLogic.Categories.Count, ButtonDisplayLogicService.CategoryLimit);
            }
            categoryButtonsControl.ItemsSource = ButtonDisplayLogic.GetDisplayedCategories(DatabaseLogic.Categories, CategoryPageNumber);
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
