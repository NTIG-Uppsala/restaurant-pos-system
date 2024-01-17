using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService db = new DatabaseService();
        private readonly ButtonDisplayLogicService buttonDisplayLogic = new ButtonDisplayLogicService();
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

            categoryButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
            productButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
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
            buttonDisplayLogic.productPanelPosition = 0;
            db.currentProducts = new ObservableCollection<Product>(db.products.Where(item => item.IsCommon));
            productButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button categoryButton && categoryButton.DataContext is DatabaseCategory selectedCategory)
            {
                // Filter products based on the selected category
                db.currentProducts = new ObservableCollection<Product>(db.products.Where(item => item.CategoryID == selectedCategory.Id));

                // Update the ItemsControl's ItemsSource with filtered products
                productButtonsControl.ItemsSource = db.currentProducts;

                buttonDisplayLogic.productPanelPosition = 0;
                // Updates the product page number based on the current page and total amount of pages
                ProductPageNumber.Text = Convert.ToString($"{(buttonDisplayLogic.productPanelPosition / buttonDisplayLogic.ProductLimit) + 1}/{(ButtonDisplayLogicService.GetClosestMultiple(db.currentProducts.Count, buttonDisplayLogic.ProductLimit) / buttonDisplayLogic.ProductLimit) + 1}");
            }
        }

        private void OnNextProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.currentProducts.Count <= buttonDisplayLogic.ProductLimit)
            {
                return;
            }

            var canGoToNextProductPage = (buttonDisplayLogic.productPanelPosition + buttonDisplayLogic.ProductLimit) <= ButtonDisplayLogicService.GetClosestMultiple(db.currentProducts.Count, buttonDisplayLogic.ProductLimit);
            if (canGoToNextProductPage)
            {
                buttonDisplayLogic.productPanelPosition += buttonDisplayLogic.ProductLimit;
            }
            else
            {
                buttonDisplayLogic.productPanelPosition = 0;
            }

            productButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnPreviousProductButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.currentProducts.Count <= buttonDisplayLogic.ProductLimit)
            {
                return;
            }

            if ((buttonDisplayLogic.productPanelPosition - buttonDisplayLogic.ProductLimit) >= 0)
            {
                buttonDisplayLogic.productPanelPosition -= buttonDisplayLogic.ProductLimit;
            }
            else
            {
                // Gets the first product posiiton of the last page
                buttonDisplayLogic.productPanelPosition = ButtonDisplayLogicService.GetClosestMultiple(db.currentProducts.Count, buttonDisplayLogic.ProductLimit);
            }
            productButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnNextCatagoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.categories.Count <= buttonDisplayLogic.CategoryLimit)
            {
                return;
            }

            var canGoToNextCategoryPage = (buttonDisplayLogic.categoryPanelPosition + buttonDisplayLogic.CategoryLimit) <= ButtonDisplayLogicService.GetClosestMultiple(db.categories.Count, buttonDisplayLogic.CategoryLimit);
            if (canGoToNextCategoryPage)
            {
                buttonDisplayLogic.categoryPanelPosition += buttonDisplayLogic.CategoryLimit;
            }
            else
            {
                buttonDisplayLogic.categoryPanelPosition = 0;
            }

            categoryButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
        }

        private void OnPreviousCatagoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (db.categories.Count <= buttonDisplayLogic.CategoryLimit)
            {
                return;
            }

            if ((buttonDisplayLogic.categoryPanelPosition - buttonDisplayLogic.CategoryLimit) >= 0)
            {
                buttonDisplayLogic.categoryPanelPosition -= buttonDisplayLogic.CategoryLimit;
            }
            else
            {
                // Gets the first category posiiton of the last page
                buttonDisplayLogic.categoryPanelPosition = ButtonDisplayLogicService.GetClosestMultiple(db.categories.Count, buttonDisplayLogic.CategoryLimit);
            }
            categoryButtonsControl.ItemsSource = buttonDisplayLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
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
