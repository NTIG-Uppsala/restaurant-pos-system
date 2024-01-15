using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private double total = 0;
        private readonly DatabaseService db = new DatabaseService();
        private readonly BusinessLogicService businessLogic = new BusinessLogicService();

        public MainWindow()
        {
            InitializeComponent();
            

            DotNetEnv.Env.Load();
            db.usedData = Environment.GetEnvironmentVariable("USEDDATA");

            db.GenerateDatabase().Wait();

            // Load categories into the Categories property
            db.categories = DatabaseService.LoadCategoriesFromDatabase();

            // Load products from the Database
            db.LoadProductsFromDatabase();

            categoryButtonsControl.ItemsSource = businessLogic.GetDisplayedCategories(db.categories, CategoryPageNumber);
            productButtonsControl.ItemsSource = businessLogic.GetDisplayedProducts(db.currentProducts, ProductPageNumber);
        }

        private void OnProductButtonClick(object sender, RoutedEventArgs e)
        {
            // Retrieve the product price from the button's tag
            var productPrice = ((Product)((Button)sender).DataContext).Price;

            // Update the total
            total += productPrice;
            totalPrice.Content = total.ToString("0.00") + " kr";
        }

        private void ResetOrder(object sender, RoutedEventArgs e)
        {
            total = 0;
            totalPrice.Content = total.ToString("0.00") + " kr";
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

    public class BusinessLogicService
    {
        public int categoryPanelPosition = 0;
        public readonly int CategoryLimit = 5;
        public int productPanelPosition = 0;
        public readonly int ProductLimit = 35;

        // Get a dynamic list of displayed categories
        public dynamic GetDisplayedCategories(ObservableCollection<DatabaseCategory> categories, TextBlock CategoryPageNumber)
        {
            // Get a subset of categories based on the current panel position
            var newCategory = categories.Skip(categoryPanelPosition).Take(CategoryLimit);

            var currentCategoryPage = (categoryPanelPosition / CategoryLimit) + 1;
            var totalCategoryPages = (GetClosestMultiple(categories.Count, CategoryLimit) / CategoryLimit) + 1;

            CategoryPageNumber.Text = Convert.ToString($"{currentCategoryPage}/{totalCategoryPages}");

            return newCategory;
        }

        // Get a dynamic list of displayed products
        public dynamic GetDisplayedProducts(ObservableCollection<Product> currentProducts, TextBlock ProductPageNumber)
        {
            // Get a subset of currentProducts based on the current panel position
            var newDisplayedProducts = currentProducts.Skip(productPanelPosition).Take(ProductLimit);

            var currentProductPage = (productPanelPosition / ProductLimit) + 1;
            var totalProductPages = (GetClosestMultiple(currentProducts.Count, ProductLimit) / ProductLimit) + 1;

            ProductPageNumber.Text = Convert.ToString($"{currentProductPage}/{totalProductPages}");

            return newDisplayedProducts;
        }

        // Helper method to get the closest multiple of a divisor for a dividend
        public static int GetClosestMultiple(int dividend, int divisor)
        {
            var quotient = dividend / divisor;
            var lowerMultiple = divisor * quotient;

            return lowerMultiple;
        }
    }

    public class DatabaseService
    {
        public ObservableCollection<Product> products = new();
        public ObservableCollection<Product> currentProducts = new();
        public ObservableCollection<DatabaseCategory> categories;
        public string usedData;



        public async Task GenerateDatabase()
        {
            // Set up paths
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POS");
            var DbPath = System.IO.Path.Join(path, $"{usedData}.db");

            // If the database file exists, do nothing
            if (File.Exists(DbPath))
            {
                return;
            }

            var ListOfProducts = await LoadProductsFromTxtAsync();
            var ListOfCategories = await LoadCategoriesFromTxtAsync();

            try
            {
                // Create a new POSContext to connect to the database
                using var db = new POSContext();
                db.Database.EnsureCreated();

                // Add products to the database
                foreach (var newProduct in ListOfProducts)
                {
                    db.Add(newProduct);
                }

                // Add categories to the database
                foreach (var newCategory in ListOfCategories)
                {
                    db.Add(newCategory);
                }

                // Save changes to the database
                db.SaveChanges();

            }
            catch (Exception error)
            {
                MessageBox.Show($"An error occured during database setup: {error.Message}");
            }
        }

        public static ObservableCollection<DatabaseCategory> LoadCategoriesFromDatabase()
        {
            var newCategories = new ObservableCollection<DatabaseCategory>();

            try
            {
                // Create a new POSContext to connect to the database
                using var db = new POSContext();
                // Load categories from the database
                newCategories = new ObservableCollection<DatabaseCategory>(db.Categories.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }

            return newCategories;
        }

        public void LoadProductsFromDatabase()
        {
            var newproducts = new ObservableCollection<Product>();
            try
            {
                // Set up paths
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POS");
                Directory.CreateDirectory(path);
                var DbPath = System.IO.Path.Join(path, $"{usedData}.db");

                var connectionString = $"Data Source={DbPath}";

                // Connect to the SQLite database
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // Select all products from the "products" table
                    var query = "SELECT * FROM products";
                    using var command = new SqliteCommand(query, connection);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        // Read product details from the database
                        var productId = Convert.ToInt32(reader["ID"]);
                        var productName = Convert.ToString(reader["Name"]);
                        var productPrice = Convert.ToDouble(reader["price"]);
                        var categoryId = Convert.ToInt32(reader["CategoryId"]);
                        var priority = Convert.ToInt32(reader["Priority"]);
                        var isCommon = Convert.ToBoolean(reader["IsCommon"]);

                        var productHasNoName = string.IsNullOrEmpty(productName);

                        if (productHasNoName)
                        {
                            productName = "NO GIVEN NAME";
                            productPrice = 0;
                            categoryId = 0;
                        }

                        // Get the color of the product's category
                        var Color = categories[categoryId - 1].Color;

                        // Create a product object and add it to the ObservableCollection
                        newproducts.Add(new Product(productId, productName, productPrice, categoryId, priority, isCommon, Color));
                    }
                }

                // Sort and filter the products
                var newProductsFiltered = new ObservableCollection<Product>(newproducts.OrderBy(item => item.Name));
                newProductsFiltered = new ObservableCollection<Product>(newProductsFiltered.OrderByDescending(item => item.Priority));

                // Update the products and currentProducts lists
                if (!products.SequenceEqual(newProductsFiltered))
                {
                    products = newProductsFiltered;
                    currentProducts = new ObservableCollection<Product>(newProductsFiltered.Where(item => item.IsCommon));
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                MessageBox.Show($"Error loading items from database: {ex.Message}");
            }
        }

        public async Task<List<DatabaseProduct>> LoadProductsFromTxtAsync()
        {
            var products = new List<DatabaseProduct>();

            try
            {
                var lines = File.ReadAllLines($"Product{usedData}.txt");

                // Skip header line
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var data = line.Split("_SPLIT_HERE_");

                    if (data.Length >= 5)
                    {
                        // Create a DatabaseProduct object and add it to the list
                        var product = new DatabaseProduct
                        {
                            Name = Convert.ToString(data[0]),
                            Price = Convert.ToDouble(data[1]),
                            CategoryId = Convert.ToInt32(data[2]),
                            Priority = Convert.ToInt32(data[3]),
                            IsCommon = Convert.ToBoolean(data[4])
                        };
                        products.Add(product);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return products;
        }

        public async Task<List<DatabaseCategory>> LoadCategoriesFromTxtAsync()
        {
            var newCategories = new List<DatabaseCategory>();

            try
            {
                var lines = File.ReadAllLines($"Category{usedData}.txt");

                // Skip header line
                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var data = line.Split("_SPLIT_HERE_");

                    if (data.Length >= 2)
                    {
                        // Create a DatabaseCategory object and add it to the list
                        var category = new DatabaseCategory
                        {
                            Name = Convert.ToString(data[0]),
                            Color = Convert.ToString(data[1]),
                        };
                        newCategories.Add(category);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return newCategories;
        }
    }

    // DbContext class for interacting with the database
    public class POSContext : DbContext
    {
        public DbSet<DatabaseProduct> Products { get; set; }
        public DbSet<DatabaseCategory> Categories { get; set; }

        public string DbPath { get; }

        public POSContext()
        {
            DotNetEnv.Env.Load();
            var usedData = Environment.GetEnvironmentVariable("USEDDATA");

            // Set up paths
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POS");
            Directory.CreateDirectory(path);
            DbPath = System.IO.Path.Join(path, $"{usedData}.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }

    // DatabaseProduct class represents a product in the database
    public class DatabaseProduct
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public int Priority { get; set; }
        public bool IsCommon { get; set; }
        public DatabaseCategory Category { get; set; }
    }

    // DatabaseCategory class represents a category in the database
    public class DatabaseCategory
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }

        // Navigation property to link categories to products
        public List<DatabaseProduct> Products { get; set; }
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
