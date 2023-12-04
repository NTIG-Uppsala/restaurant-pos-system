using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.Json;
using System.Text.Json.Serialization;
using MySql.Data.MySqlClient;


namespace PointOfSaleSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double total = 0;
        private ObservableCollection<Item> items = new ObservableCollection<Item>();
        private FileSystemWatcher fileSystemWatcher;
        private const string ConnectionString = "Server=localhost;Database=restaurant-poss;Uid=root;Pwd=;";

        public MainWindow()
        {
            InitializeComponent();

            LoadDefault();

            // Load items from JSON file
            LoadItemsFromDatabase();

            // Set the loaded items as the ItemsSource for the ItemsControl
            itemButtonsControl.ItemsSource = items;

            // Set up file system watcher
            InitializeFileSystemWatcher();

        }

        public class data
        {
            public string Name { get; set; }
            public double Price { get; set; }
        }

        private void LoadDefault()
        {
            List<data> _data = new List<data>();

            _data.Add(new data()
            {
                Name = "Kaffe",
                Price = 20.00
            });

            _data.Add(new data()
            {
                Name = "Bulle",
                Price = 25.00
            });

            _data.Add(new data()
            {
                Name = "Kaka",
                Price = 10.00
            });

            string json = System.Text.Json.JsonSerializer.Serialize(_data);
            File.WriteAllText(@"items.json", json);
        }

        private void InitializeFileSystemWatcher()
        {
            fileSystemWatcher = new FileSystemWatcher
            {
                Path = Environment.CurrentDirectory, // Change this path if needed
                Filter = "items.json",
                NotifyFilter = NotifyFilters.LastWrite
            };

            fileSystemWatcher.Changed += FileSystemWatcher_Changed;

            // Enable the watcher
            fileSystemWatcher.EnableRaisingEvents = true;
        }



        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1500);
            // Handle file change event (reload items)
            Dispatcher.Invoke(() => LoadItemsFromDatabase());
        }

        private void LoadItemsFromDatabase()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM products";
                    MySqlCommand command = new MySqlCommand(query, connection);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        // Clear existing items
                        items.Clear();

                        while (reader.Read())
                        {
                            int itemId = Convert.ToInt32(reader["ID"]);
                            string itemName = reader["Product"].ToString();
                            double itemPrice = Convert.ToDouble(reader["Price"]);
                            int categoryId = Convert.ToInt32(reader["Category_ID"]);

                            // Create an Item object and add it to the ObservableCollection
                            items.Add(new Item(itemId, itemName, itemPrice, categoryId));
                        }
                    }
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
    }
}
