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
        private ObservableCollection<Item> items = new();
        public ObservableCollection<CategoryItem> Categories { get; set; }
        private string usedData;
        private int categoryPanelPosition = 0;
        private readonly int CategoryLimit = 7;

        public MainWindow()
        {
            InitializeComponent();

            DotNetEnv.Env.Load();
            usedData = Environment.GetEnvironmentVariable("USEDDATA");

            GenerateDatabase().Wait();

            // Load items from the Database
            LoadItemsFromDatabase();

            // Load categories into the Categories property
            Categories = LoadCategories();
            categorysButtonsControl.ItemsSource = GetDisplayedCategories();
            itemButtonsControl.ItemsSource = items;
        }

        public async Task GenerateDatabase()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POS");
            var DbPath = System.IO.Path.Join(path, $"{usedData}.db");

            if (File.Exists(DbPath))
            {
                return;
            }

            List<DatabaseItem> ListOfProducts = await LoadProductsFromTxtAsync();
            List<CategoryItem> ListOfCategories = await LoadCategoriesFromTxtAsync();

            try
            {
                using var db = new POSContext();
                db.Database.EnsureCreated();

                foreach (DatabaseItem newProduct in ListOfProducts)
                {
                    db.Add(newProduct);
                }

                foreach (CategoryItem newCategory in ListOfCategories)
                {
                    db.Add(newCategory);
                }

                db.SaveChanges();

            }
            catch (Exception error)
            {
                MessageBox.Show($"An error occured during database setup: {error.Message}");
            }
        }

        private void LoadItemsFromDatabase()
        {
            ObservableCollection<Item> newItems = new ObservableCollection<Item>();
            try
            {
                var folder = Environment.SpecialFolder.LocalApplicationData;
                var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POS");
                Directory.CreateDirectory(path);
                var DbPath = System.IO.Path.Join(path, $"{usedData}.db");

                string connectionString = $"Data Source={DbPath}";

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM products";
                    using var command = new SqliteCommand(query, connection);
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int itemId = Convert.ToInt32(reader["ID"]);
                        string itemName = Convert.ToString(reader["Name"]);
                        double itemPrice = Convert.ToDouble(reader["price"]);
                        int categoryId = Convert.ToInt32(reader["CategoryId"]);
                        int priority = Convert.ToInt32(reader["Priority"]);
                        bool isCommon = Convert.ToBoolean(reader["IsCommon"]);

                        var productHasNoName = string.IsNullOrEmpty(itemName);

                        if (productHasNoName)
                        {
                            itemName = "NO GIVEN NAME";
                            itemPrice = 0;
                            categoryId = 0;
                        }

                        // Create an Item object and add it to the ObservableCollection
                        newItems.Add(new Item(itemId, itemName, itemPrice, categoryId, priority, isCommon));
                    }
                }

                ObservableCollection<Item> newItemsFiltered = new ObservableCollection<Item>(newItems.OrderByDescending(item => item.Priority));

                if (!items.SequenceEqual(newItemsFiltered))
                {
                    items = newItemsFiltered;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., database connection issues)
                MessageBox.Show($"Error loading items from database: {ex.Message}");
            }
        }

        private static ObservableCollection<CategoryItem> LoadCategories()
        {
            ObservableCollection<CategoryItem> categories = new ObservableCollection<CategoryItem>();

            try
            {
                using var db = new POSContext();
                categories = new ObservableCollection<CategoryItem>(db.Categories.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }

            return categories;
        }

        private dynamic GetDisplayedCategories()
        {
            var newCategory = Categories.Skip(categoryPanelPosition).Take(CategoryLimit);

            return newCategory;
        }

        public async Task<List<DatabaseItem>> LoadProductsFromTxtAsync()
        {
            var products = new List<DatabaseItem>();

            try
            {
                var lines = File.ReadAllLines($"Product{usedData}.txt");

                // Skip header line
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var data = line.Split("_SPLIT_HERE_");

                    if (data.Length >= 3)
                    {
                        var product = new DatabaseItem
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

        public async Task<List<CategoryItem>> LoadCategoriesFromTxtAsync()
        {
            var categories = new List<CategoryItem>();

            try
            {
                var lines = File.ReadAllLines($"Category{usedData}.txt");

                // Skip header line
                for (int i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var data = line.Split("_SPLIT_HERE_");

                    if (data.Length >= 2)
                    {
                        var category = new CategoryItem
                        {
                            Name = Convert.ToString(data[0]),
                            Color = Convert.ToString(data[1]),
                        };
                        categories.Add(category);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            return categories;
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

        private void OnReturnButtonClick(object sender, RoutedEventArgs e)
        {
            itemButtonsControl.ItemsSource = items;
        }
        
        private void OnCategoryButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button categoryButton && categoryButton.DataContext is CategoryItem selectedCategory)
            {
                // Filter items based on the selected category
                var filteredItems = items.Where(item => item.CategoryID == selectedCategory.Id).ToList();

                // Update the ItemsControl's ItemsSource with filtered items
                itemButtonsControl.ItemsSource = filteredItems;
            }
        }

        private void OnNextCatagoryButtonClick(object sender, RoutedEventArgs e)
        {
            if ((categoryPanelPosition + CategoryLimit) <= ClosestMultipleOf7(Categories.Count))
            {
                categoryPanelPosition += CategoryLimit;
            }
            else
            {
                categoryPanelPosition = 0;
            }

            categorysButtonsControl.ItemsSource = GetDisplayedCategories();
        }

        private void OnPreviousCatagoryButtonClick(object sender, RoutedEventArgs e)
        {
            if ((categoryPanelPosition - CategoryLimit) >= 0)
            {
                categoryPanelPosition -= CategoryLimit;
            }
            else
            {
                categoryPanelPosition = ClosestMultipleOf7(Categories.Count);
            }
            categorysButtonsControl.ItemsSource = GetDisplayedCategories();
        }

        static int ClosestMultipleOf7(int number)
        {
            int quotient = number / 7;
            int lowerMultiple = 7 * quotient;

            return lowerMultiple;
        }
    }
    public class POSContext : DbContext
    {
        public DbSet<DatabaseItem> Products { get; set; }
        public DbSet<CategoryItem> Categories { get; set; }

        public string DbPath { get; }

        public POSContext()
        {
            DotNetEnv.Env.Load();
            string usedData = Environment.GetEnvironmentVariable("USEDDATA");

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = System.IO.Path.Join(Environment.GetFolderPath(folder), "Restaurant-POS");
            Directory.CreateDirectory(path);
            DbPath = System.IO.Path.Join(path, $"{usedData}.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    public class DatabaseItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
        public int Priority { get; set; }
        public bool IsCommon { get; set; }
        public CategoryItem Category { get; set; }
    }

    public class CategoryItem
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Color { get; set; }

        // Navigation property to link categories to items
        public List<DatabaseItem> Items { get; set; }
    }
    public class Item
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int CategoryID { get; set; }
        public int Priority { get; set; }
        public bool IsCommon { get; set; }

        public Item(int id, string name, double price, int categoryID, int priority, bool isCommon)
        {
            ID = id;
            Name = name;
            Price = price;
            CategoryID = categoryID;
            Priority = priority;
            IsCommon = isCommon;
        }
    }
}
