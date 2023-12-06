using System;
using System.Linq;

List<Item> ListOfProducts = new List<Item>();

ListOfProducts.Add(new Item { Name = "Bearnaise", Price = 10, CategoryId = 6});
ListOfProducts.Add(new Item { Name = "Citronmajonnäs", Price = 10, CategoryId = 6 });
ListOfProducts.Add(new Item { Name = "Chimichurri", Price = 10, CategoryId = 6 });

ListOfProducts.Add(new Item { Name = "Räkor 200 g", Price = 120, CategoryId = 5 });
ListOfProducts.Add(new Item { Name = "Rökta räkor 200 g", Price = 120, CategoryId = 5 });
ListOfProducts.Add(new Item { Name = "Havskräfta", Price = 38, CategoryId = 5 });
ListOfProducts.Add(new Item { Name = "Halv krabba", Price = 140, CategoryId = 5 });
ListOfProducts.Add(new Item { Name = "Halv hummer", Price = 235, CategoryId = 5 });

ListOfProducts.Add(new Item { Name = "Svamptartar", Price = 135, CategoryId = 1 });
ListOfProducts.Add(new Item { Name = "Laxsashimi", Price = 145, CategoryId = 1 });
ListOfProducts.Add(new Item { Name = "Kammusslor", Price = 170, CategoryId = 1 });

ListOfProducts.Add(new Item { Name = "Crème brûlée", Price = 90, CategoryId = 3 });
ListOfProducts.Add(new Item { Name = "Hallonpavlova", Price = 110, CategoryId = 3 });
ListOfProducts.Add(new Item { Name = "Passionfruktssorbet", Price = 50, CategoryId = 3 });

ListOfProducts.Add(new Item { Name = "Glas Maison Sans Pareil Sauvignon Blanc", Price = 135, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "Flaska Maison Sans Pareil Sauvignon Blanc", Price = 495, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "Paolo Leo Calaluna Fiano", Price = 405, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "Glas Friedrich-Wilhelm-Gymnasium Riesling Trocken", Price = 125, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "Flaska Friedrich-Wilhelm-Gymnasium Riesling Trocken", Price = 455, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "Glas Jean-Claude Boisset, Pinot Noir", Price = 135, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "Flaska Jean-Claude Boisset, Pinot Noir", Price = 495, CategoryId = 7 });
ListOfProducts.Add(new Item { Name = "I Castei Ripasso", Price = 520, CategoryId = 7 });

ListOfProducts.Add(new Item { Name = "Capricciosa", Price = 90, CategoryId = 4 });
ListOfProducts.Add(new Item { Name = "Calzone", Price = 90, CategoryId = 4 });
ListOfProducts.Add(new Item { Name = "Margarita", Price = 90, CategoryId = 4 });
ListOfProducts.Add(new Item { Name = "Hawaii", Price = 90, CategoryId = 4 });
ListOfProducts.Add(new Item { Name = "Vesuvio", Price = 90, CategoryId = 4 });


ListOfProducts.Add(new Item { Name = "Dagens Lunch", Price = 85, CategoryId = 2 });
ListOfProducts.Add(new Item { Name = "Räksallad", Price = 120, CategoryId = 2 });
ListOfProducts.Add(new Item { Name = "Husmanstallrik", Price = 125, CategoryId = 2 });
ListOfProducts.Add(new Item { Name = "Fish 'n' Chips", Price = 125, CategoryId = 2 });
ListOfProducts.Add(new Item { Name = "Grillad Tonfisksallad", Price = 190, CategoryId = 2 });
ListOfProducts.Add(new Item { Name = "Steam Tartar", Price = 185, CategoryId = 2 });

ListOfProducts.Add(new Item { Name = "Cider", Price = 74, CategoryId = 8 });
ListOfProducts.Add(new Item { Name = "Grängesberg", Price = 30, CategoryId = 8 });
ListOfProducts.Add(new Item { Name = "Cruzcampo", Price = 65, CategoryId = 8 });
ListOfProducts.Add(new Item { Name = "Janne Shuffle", Price = 59, CategoryId = 8 });
ListOfProducts.Add(new Item { Name = "Bistro Lager", Price = 65, CategoryId = 8 });
ListOfProducts.Add(new Item { Name = "Wisby Pils", Price = 69, CategoryId = 8 });
ListOfProducts.Add(new Item { Name = "Sleepy Bulldog Pale Ale", Price = 75, CategoryId = 8 });

ListOfProducts.Add(new Item { Name = "Kaffe", Price = 32, CategoryId = 9 });
ListOfProducts.Add(new Item { Name = "Espresso", Price = 32, CategoryId = 9 });
ListOfProducts.Add(new Item { Name = "Dubbel Espresso", Price = 36, CategoryId = 9 });
ListOfProducts.Add(new Item { Name = "Macchiato", Price = 34, CategoryId = 9 });
ListOfProducts.Add(new Item { Name = "Dubbel Macchiato", Price = 42, CategoryId = 9 });
ListOfProducts.Add(new Item { Name = "Cappuccino", Price = 44, CategoryId = 9 });
ListOfProducts.Add(new Item { Name = "Te", Price = 34, CategoryId = 9 });

try
{
    using var db = new POSSContext();
    db.Database.EnsureCreated();

    // Note: This sample requires the database to be created before running.
    Console.WriteLine($"Database path: {db.DbPath}.");

    Console.WriteLine("Querying for all products");
    var existingProductNames = db.Products.Select(p => p.Name).ToList();

    foreach (Item newProduct in ListOfProducts)
    {
        if (!existingProductNames.Contains(newProduct.Name))
        {
            db.Add(newProduct);
        }
    }

    db.SaveChanges();

    foreach (var product in db.Products)
    {
        Console.WriteLine($"Name: {product.Name}\nPrice: {product.Price}\nCategoryId: {product.CategoryId}");
    }
}
catch (Exception error)
{
    Console.WriteLine($"An error occured during database setup: {error.Message}");
}
