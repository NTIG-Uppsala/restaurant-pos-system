﻿using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


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

            // Load items from JSON file
            LoadItemsFromDatabase();

            // Set the loaded items as the ItemsSource for the ItemsControl
            itemButtonsControl.ItemsSource = items;

            // Set up file system watcher
            InitializeFileSystemWatcher();

        }

        private void InitializeFileSystemWatcher()
        {
            fileSystemWatcher = new FileSystemWatcher
            {
                Path = Environment.CurrentDirectory,
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
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                connection.Open();

                string query = "SELECT * FROM products";
                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataReader reader = command.ExecuteReader();

                // Clear existing items
                items.Clear();

                while (reader.Read())
                {
                    int itemId = Convert.ToInt32(reader["ID"]);
                    string? itemName = Convert.ToString(reader["Product"]);
                    double itemPrice = Convert.ToDouble(reader["Price"]);
                    int categoryId = Convert.ToInt32(reader["Category_ID"]);

                    // Create an Item object and add it to the ObservableCollection
                    items.Add(new Item(itemId, itemName, itemPrice, categoryId));
                }

                reader.Dispose();
                connection.Dispose();
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
