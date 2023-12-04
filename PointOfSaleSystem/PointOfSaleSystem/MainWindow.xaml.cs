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

        public MainWindow()
        {
            InitializeComponent();

            // Load items from JSON file
            LoadItemsFromJson();

            // Set the loaded items as the ItemsSource for the ItemsControl
            itemButtonsControl.ItemsSource = items;

            // Set up file system watcher
            InitializeFileSystemWatcher();

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
            Dispatcher.Invoke(() => LoadItemsFromJson());
        }

        private void LoadItemsFromJson()
        {
            try
            {
                // Read the JSON file
                string jsonContent = File.ReadAllText("items.json");

                // Deserialize the JSON content into a list of items
                List<Item> loadedItems = JsonConvert.DeserializeObject<List<Item>>(jsonContent);

                // Clear existing items and add the loaded items
                items.Clear();
                if (loadedItems == null)
                {
                    return;
                }


                foreach (var item in loadedItems)
                {
                    items.Add(item);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., file not found, invalid JSON format)
                MessageBox.Show($"Error loading items from JSON file: {ex.Message}");
            }
        }

        public class Item
        {
            public string Name { get; set; }
            public double Price { get; set; }

            public Item(string name, double price)
            {
                Name = name;
                Price = price;
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
