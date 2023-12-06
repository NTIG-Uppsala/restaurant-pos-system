using System;
using System.Linq;

try
{
    using var db = new POSSContext();
    db.Database.EnsureCreated();

    // Note: This sample requires the database to be created before running.
    Console.WriteLine($"Database path: {db.DbPath}.");

    // Create
    Console.WriteLine("Inserting a new prod");
    db.Add(new Item { Name = "Kaffe", Price = 20, CategoryId = 4 });
    db.SaveChanges();

    // Read
    Console.WriteLine("Querying for all products");
    var products = db.Products;
    foreach (var product in products)
    {
        Console.WriteLine($"Name: {product.Name}\nPrice: {product.Price}\nCategoryId: {product.CategoryId}");
    }
}
catch (Exception error)
{
    Console.WriteLine($"An error occured during database setup: {error.Message}");
}