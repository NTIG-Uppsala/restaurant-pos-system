using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MySql.Data.MySqlClient;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private double total = 0;
        private ObservableCollection<Item> items = new ObservableCollection<Item>();
        private const string ConnectionString = "Server=localhost;Database=restaurant-poss;Uid=root;Pwd=;";
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            // Load items from the Database
            LoadItemsFromDatabase();

            // Set the loaded items as the ItemsSource for the ItemsControl
            itemButtonsControl.ItemsSource = items;

            SetTimer();
        }

        private void LoadItemsFromDatabase()
        {
            ObservableCollection<Item> newItems = new ObservableCollection<Item>();
            try
            {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string query = "SELECT * FROM products";
                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    int itemId = Convert.ToInt32(reader["ID"]);
                    string? itemName = Convert.ToString(reader["product"]);
                    double itemPrice = Convert.ToDouble(reader["price"]);
                    int categoryId = Convert.ToInt32(reader["categoryId"]);

                    var productHasNoName = itemName == null || itemName == "";

                    if (productHasNoName)
                    {
                        itemName = "NO GIVEN NAME";
                        itemPrice = 0;
                        categoryId = 0;
                    }

                    // Create an Item object and add it to the ObservableCollection
                    newItems.Add(new Item(itemId, itemName, itemPrice, categoryId));
                }

                reader.Dispose();
                connection.Dispose();

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

        private void SetTimer()
        {
            // Set up the timer to reload items every 10 minutes (adjust the interval as needed)
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(10);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Reload items from the database
            LoadItemsFromDatabase();

            itemButtonsControl.ItemsSource = items;
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
    }
}
