using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace PointOfSaleSystem
{
    public class DatabaseService
    {
        public ObservableCollection<Product> Products = new();
        public ObservableCollection<Product> CurrentProducts = new();
        public ObservableCollection<DatabaseCategory> Categories = new();

        public async Task GenerateDatabase(string usedData)
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

            var ListOfProducts = await LoadProductsFromTxtAsync(usedData);
            var ListOfCategories = await LoadCategoriesFromTxtAsync(usedData);

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

        public void LoadProductsFromDatabase(string usedData)
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
                        var Color = Categories[categoryId - 1].Color;

                        // Create a product object and add it to the ObservableCollection
                        newproducts.Add(new Product(productId, productName, productPrice, categoryId, priority, isCommon, Color));
                    }
                }

                // Sort and filter the products
                var newProductsFiltered = new ObservableCollection<Product>(newproducts.OrderBy(item => item.Name));
                newProductsFiltered = new ObservableCollection<Product>(newProductsFiltered.OrderByDescending(item => item.Priority));

                // Update the products and currentProducts lists
                if (!Products.SequenceEqual(newProductsFiltered))
                {
                    Products = newProductsFiltered;
                    CurrentProducts = new ObservableCollection<Product>(newProductsFiltered.Where(item => item.IsCommon));
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                MessageBox.Show($"Error loading items from database: {ex.Message}");
            }
        }

        public async Task<List<DatabaseProduct>> LoadProductsFromTxtAsync(string usedData)
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

        public async Task<List<DatabaseCategory>> LoadCategoriesFromTxtAsync(string usedData)
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

        public void AddOrderToDatabase(ObservableCollection<DisplayedItem> order)
        {
            // Check if there are products in the order
            if (order.Count == 0)
            {
                MessageBox.Show("Please add products to the order before paying.");
                return;
            }

            // Calculate the total price of the order
            double totalOrderPrice = order.Sum(item => item.ProductAmount * item.ItemPrice);

            // Create a new order
            DatabaseOrder newOrder = new DatabaseOrder
            {
                TableId = 1,
                Price = totalOrderPrice,
                IsPaid = true
            };
            int orderId;
            // Add the new order to the database
            using (var db = new POSContext())
            {
                db.Orders.Add(newOrder);
                db.SaveChanges();

                orderId = newOrder.Id;

                // Link products to the order in the ProductsInOrder table
                foreach (var productItem in order)
                {
                    // Create a new ProductsInOrder entry for each product in the order
                    DatabaseProductsInOrder productsInOrder = new DatabaseProductsInOrder
                    {
                        OrderId = orderId,
                        ProductId = productItem.ProductId,
                        Amount = productItem.ProductAmount
                    };

                    // Add the entry to the ProductsInOrder table
                    db.ProductsInOrder.Add(productsInOrder);
                }

                db.SaveChanges();
            }
        }
    }

    // DbContext class for interacting with the database
    public class POSContext : DbContext
    {
        public DbSet<DatabaseProduct> Products { get; set; }
        public DbSet<DatabaseCategory> Categories { get; set; }
        public DbSet<DatabaseOrder> Orders { get; set; }
        public DbSet<DatabaseProductsInOrder> ProductsInOrder { get; set; }
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

    public class DatabaseOrder
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public double Price { get; set; }
        public bool IsPaid { get; set; } 
    }

    public class DatabaseProductsInOrder
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
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
}
