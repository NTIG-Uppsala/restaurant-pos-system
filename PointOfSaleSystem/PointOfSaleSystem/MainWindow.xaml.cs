using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private double total = 0;
        private ObservableCollection<Item> items = new ObservableCollection<Item>();

        public MainWindow()
        {
            InitializeComponent();

            GenerateDatabase();

            // Load items from the Database
            LoadItemsFromDatabase();

            // Set the loaded items as the ItemsSource for the ItemsControl
            itemButtonsControl.ItemsSource = items;
        }

        public void GenerateDatabase()
        {
            List<DatabaseItem> ListOfProducts = GetDefaultProducts();
            try
            {
                using var db = new POSSContext();
                db.Database.EnsureCreated();

                var existingProductNames = db.Products.Select(p => p.Name).ToList();

                foreach (DatabaseItem newProduct in ListOfProducts)
                {
                    if (!existingProductNames.Contains(newProduct.Name))
                    {
                        db.Add(newProduct);
                    }
                }

                db.SaveChanges();

            }
            catch (Exception error)
            {
                MessageBox.Show($"An error occured during database setup: {error.Message}");
            }
        }

        public List<DatabaseItem> GetDefaultProducts()
        {
            List<DatabaseItem> Products = new List<DatabaseItem>();

            Products.Add(new DatabaseItem { Name = "Bearnaise", Price = 10, CategoryId = 6 });
            Products.Add(new DatabaseItem { Name = "Citronmajonnäs", Price = 10, CategoryId = 6 });
            Products.Add(new DatabaseItem { Name = "Chimichurri", Price = 10, CategoryId = 6 });

            Products.Add(new DatabaseItem { Name = "Räkor 200 g", Price = 120, CategoryId = 5 });
            Products.Add(new DatabaseItem { Name = "Rökta räkor 200 g", Price = 120, CategoryId = 5 });
            Products.Add(new DatabaseItem { Name = "Havskräfta", Price = 38, CategoryId = 5 });
            Products.Add(new DatabaseItem { Name = "Halv krabba", Price = 140, CategoryId = 5 });
            Products.Add(new DatabaseItem { Name = "Halv hummer", Price = 235, CategoryId = 5 });

            Products.Add(new DatabaseItem { Name = "Svamptartar", Price = 135, CategoryId = 1 });
            Products.Add(new DatabaseItem { Name = "Laxsashimi", Price = 145, CategoryId = 1 });
            Products.Add(new DatabaseItem { Name = "Kammusslor", Price = 170, CategoryId = 1 });

            Products.Add(new DatabaseItem { Name = "Crème brûlée", Price = 90, CategoryId = 3 });
            Products.Add(new DatabaseItem { Name = "Hallonpavlova", Price = 110, CategoryId = 3 });
            Products.Add(new DatabaseItem { Name = "Passionfruktssorbet", Price = 50, CategoryId = 3 });

            Products.Add(new DatabaseItem { Name = "Glas Maison Sans Pareil Sauvignon Blanc", Price = 135, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "Flaska Maison Sans Pareil Sauvignon Blanc", Price = 495, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "Paolo Leo Calaluna Fiano", Price = 405, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "Glas Friedrich-Wilhelm-Gymnasium Riesling Trocken", Price = 125, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "Flaska Friedrich-Wilhelm-Gymnasium Riesling Trocken", Price = 455, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "Glas Jean-Claude Boisset, Pinot Noir", Price = 135, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "Flaska Jean-Claude Boisset, Pinot Noir", Price = 495, CategoryId = 7 });
            Products.Add(new DatabaseItem { Name = "I Castei Ripasso", Price = 520, CategoryId = 7 });

            Products.Add(new DatabaseItem { Name = "Capricciosa", Price = 90, CategoryId = 4 });
            Products.Add(new DatabaseItem { Name = "Calzone", Price = 90, CategoryId = 4 });
            Products.Add(new DatabaseItem { Name = "Margarita", Price = 90, CategoryId = 4 });
            Products.Add(new DatabaseItem { Name = "Hawaii", Price = 90, CategoryId = 4 });
            Products.Add(new DatabaseItem { Name = "Vesuvio", Price = 90, CategoryId = 4 });


            Products.Add(new DatabaseItem { Name = "Dagens Lunch", Price = 85, CategoryId = 2 });
            Products.Add(new DatabaseItem { Name = "Räksallad", Price = 120, CategoryId = 2 });
            Products.Add(new DatabaseItem { Name = "Husmanstallrik", Price = 125, CategoryId = 2 });
            Products.Add(new DatabaseItem { Name = "Fish 'n' Chips", Price = 125, CategoryId = 2 });
            Products.Add(new DatabaseItem { Name = "Grillad Tonfisksallad", Price = 190, CategoryId = 2 });
            Products.Add(new DatabaseItem { Name = "Steam Tartar", Price = 185, CategoryId = 2 });

            Products.Add(new DatabaseItem { Name = "Cider", Price = 74, CategoryId = 8 });
            Products.Add(new DatabaseItem { Name = "Grängesberg", Price = 30, CategoryId = 8 });
            Products.Add(new DatabaseItem { Name = "Cruzcampo", Price = 65, CategoryId = 8 });
            Products.Add(new DatabaseItem { Name = "Janne Shuffle", Price = 59, CategoryId = 8 });
            Products.Add(new DatabaseItem { Name = "Bistro Lager", Price = 65, CategoryId = 8 });
            Products.Add(new DatabaseItem { Name = "Wisby Pils", Price = 69, CategoryId = 8 });
            Products.Add(new DatabaseItem { Name = "Sleepy Bulldog Pale Ale", Price = 75, CategoryId = 8 });

            Products.Add(new DatabaseItem { Name = "Kaffe", Price = 32, CategoryId = 9 });
            Products.Add(new DatabaseItem { Name = "Espresso", Price = 32, CategoryId = 9 });
            Products.Add(new DatabaseItem { Name = "Dubbel Espresso", Price = 36, CategoryId = 9 });
            Products.Add(new DatabaseItem { Name = "Macchiato", Price = 34, CategoryId = 9 });
            Products.Add(new DatabaseItem { Name = "Dubbel Macchiato", Price = 42, CategoryId = 9 });
            Products.Add(new DatabaseItem { Name = "Cappuccino", Price = 44, CategoryId = 9 });
            Products.Add(new DatabaseItem { Name = "Te", Price = 34, CategoryId = 9 });

            return Products;
        }

        private void LoadItemsFromDatabase()
        {
            ObservableCollection<Item> newItems = new ObservableCollection<Item>();
            try
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POSS");
                Directory.CreateDirectory(path);
                var DbPath = System.IO.Path.Join(path, "POSS.db");

                string connectionString = $"Data Source={DbPath}";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM products";
                    using (var command = new SqliteCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int itemId = Convert.ToInt32(reader["ID"]);
                                string itemName = Convert.ToString(reader["Name"]);
                                double itemPrice = Convert.ToDouble(reader["price"]);
                                int categoryId = Convert.ToInt32(reader["CategoryId"]);

                                var productHasNoName = string.IsNullOrEmpty(itemName);

                                if (productHasNoName)
                                {
                                    itemName = "NO GIVEN NAME";
                                    itemPrice = 0;
                                    categoryId = 0;
                                }

                                // Create an Item object and add it to the ObservableCollection
                                newItems.Add(new Item(itemId, itemName, itemPrice, categoryId));
                            }
                        }
                    }
                }

                if (!items.SequenceEqual(newItems))
                {
                    items = newItems;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                MessageBox.Show($"Error loading items from database: {ex.Message}");
            }
        }

        public class Item
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public double Price { get; set; }
            public int CategoryID { get; set; }

            public Item(int id, string name, double price, int categoryID)
            {
                ID = id;
                Name = name;
                Price = price;
                CategoryID = categoryID;
            }
        }

        private void OnItemButtonClick(object sender, RoutedEventArgs e)
        {
            // Retrieve the item price from the button's tag
            double itemPrice = ((Item)((Button)sender).DataContext).Price;

            // Update the total
            total += itemPrice;
            totalPrice.Content = total.ToString("0.00") + " kr";
        }

        private void ResetOrder(object sender, RoutedEventArgs e)
        {
            total = 0;
            totalPrice.Content = total.ToString("0.00") + " kr";
        }

        public class POSSContext : DbContext
        {
            public DbSet<DatabaseItem> Products { get; set; }

            public string DbPath { get; }

            public POSSContext()
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POSS");
                Directory.CreateDirectory(path);
                DbPath = System.IO.Path.Join(path, "POSS.db");
            }

            // The following configures EF to create a Sqlite database file in the
            // special "local" folder for your platform.
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite($"Data Source={DbPath}");
        }

        public class DatabaseItem
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public double Price { get; set; }
            public int CategoryId { get; set; }
        }
    }
}
