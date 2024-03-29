﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService DatabaseLogic = new();
        private readonly ButtonDisplayLogicService ButtonDisplayLogic = new();
        private readonly ObservableCollection<DisplayedItem> ProductWindowItems = new();
        private readonly string UsedData;
        private DisplayedItem? CurrentlyEditing;

        public MainWindow()
        {
            InitializeComponent();

            DotNetEnv.Env.Load();
            var envData = Environment.GetEnvironmentVariable("USEDDATA");
            
            if (envData != null)
            {
                UsedData = envData;
            }
            else
            {
                MessageBox.Show("There is no USEDDATA in the env file");
            }

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
            int productId = ((Product)((Button)sender).DataContext).ID;

            UpdateProductWindow(productName, productPrice, productId);

            double totalFromProductWindow = ProductWindowItems.Sum(x => x.ProductAmount * x.ItemPrice);
            totalPrice.Content = totalFromProductWindow.ToString("0.00") + " kr";
        }

        private void UpdateProductWindow(string productName, double productPrice, int productId, int changeAmountBy = 1)
        {
            // Find the first item in the ProductWindowItems collection where the ProductName matches the specified productName
            var addedItem = ProductWindowItems.FirstOrDefault(item => item.ProductName == productName);

            if (addedItem != null)
            {
                if ((addedItem.ProductAmount + changeAmountBy) >= 1)
                {
                    addedItem.ProductAmount += changeAmountBy;
                }
                addedItem.ProductPrice = (addedItem.ItemPrice * addedItem.ProductAmount).ToString("0.00") + " kr";
            }
            else
            {
                string totalProductPrice = productPrice.ToString("0.00") + " kr";
                ProductWindowItems.Add(new DisplayedItem(productName, totalProductPrice, 1, productPrice, productId));
            }

            productWindow.ItemsSource = ProductWindowItems;
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            if (ProductWindowItems.Count == 0)
            {
                return;
            }
            if (MessageBox.Show("Are you sure you want to the reset the unpaid order?",
                    "Reset warning", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            ResetOrder();
        }

        private void ResetOrder()
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

        private void OnPayOrderClick(object sender, RoutedEventArgs e)
        {
            DatabaseLogic.AddOrderToDatabase(ProductWindowItems);

            // Clear the current order in the UI
            ResetOrder();
        }

        private void OnDecreaseAmountClick(object sender, RoutedEventArgs e)
        {
            // Retrieve the product name and price from the button's tag
            string productName = ((DisplayedItem)((Button)sender).DataContext).ProductName!;
            double productPrice = ((DisplayedItem)((Button)sender).DataContext).ItemPrice;
            int productId = ((DisplayedItem)((Button)sender).DataContext).ProductId;

            UpdateProductWindow(productName, productPrice, productId, -1);

            double totalFromProductWindow = ProductWindowItems.Sum(x => x.ProductAmount * x.ItemPrice);
            totalPrice.Content = totalFromProductWindow.ToString("0.00") + " kr";
        }

        private void OnIncreaseAmountClick(object sender, RoutedEventArgs e)
        {
            // Retrieve the product name and price from the button's tag
            string productName = ((DisplayedItem)((Button)sender).DataContext).ProductName!;
            double productPrice = ((DisplayedItem)((Button)sender).DataContext).ItemPrice;
            int productId = ((DisplayedItem)((Button)sender).DataContext).ProductId;

            UpdateProductWindow(productName, productPrice, productId);

            double totalFromProductWindow = ProductWindowItems.Sum(x => x.ProductAmount * x.ItemPrice);
            totalPrice.Content = totalFromProductWindow.ToString("0.00") + " kr";
        }

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            // Gets the name of the item which edit button has been pressed
            string productName = ((DisplayedItem)((Button)sender).DataContext).ProductName!;
            if (CurrentlyEditing == null)
            {
                CurrentlyEditing = ProductWindowItems.FirstOrDefault(item => item.ProductName == productName)!;
                AmountEditor.Visibility = Visibility.Visible;
                return;
            }

            if (CurrentlyEditing.ProductName != productName)
            {
                CurrentlyEditing = ProductWindowItems.FirstOrDefault(item => item.ProductName == productName)!;
                QuantityKeypadResult.Text = "";
                return;
            }

            CurrentlyEditing = null;
            QuantityKeypadResult.Text = "";
            AmountEditor.Visibility = Visibility.Hidden;
        }
        private void OnQuantityKeyClick(object sender, RoutedEventArgs e)
        {
            string buttonValue = (e.Source as Button)!.Content.ToString()!;

            QuantityKeypadResult.Text += buttonValue;
        }

        private void OnQuantityBackClick(object sender, RoutedEventArgs e)
        {
            string currentText = QuantityKeypadResult.Text;
            if (currentText.Length > 0)
            {
                QuantityKeypadResult.Text = currentText.Remove(currentText.Length - 1);
            }
        }

        private void OnQuantityEnterClick(object sender, RoutedEventArgs e)
        {
            // Does nothing if there is no amount in the quantity editor
            if (QuantityKeypadResult.Text == "" || QuantityKeypadResult.Text == "0")
            {
                MessageBox.Show("Please specify an amount before pressing enter");
                return;
            }

            // Tries to convert the input to an int
            try
            {
                // Gets the new amount from the input
                CurrentlyEditing.ProductAmount = Convert.ToInt32(QuantityKeypadResult.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Too many items, no items were added");
                return;
            }

            double totalFromProductWindow = ProductWindowItems.Sum(x => x.ProductAmount * x.ItemPrice);
            totalPrice.Content = totalFromProductWindow.ToString("0.00") + " kr";

            CurrentlyEditing.ProductPrice = (CurrentlyEditing.ItemPrice * CurrentlyEditing.ProductAmount).ToString("0.00") + " kr";

            productWindow.ItemsSource = ProductWindowItems;

            CurrentlyEditing = null;
            AmountEditor.Visibility = Visibility.Hidden;

            QuantityKeypadResult.Text = "";
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
