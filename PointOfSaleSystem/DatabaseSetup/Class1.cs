using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class POSSContext : DbContext
{
    public DbSet<Item> Products { get; set; }

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

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int CategoryId { get; set; }
}