using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;

#nullable disable

namespace PointOfSaleSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the 'Categories' table already exists
            var tableExists = migrationBuilder.IsTableExists("Categories");

            if (!tableExists)
            {
                migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });
            }

            tableExists = migrationBuilder.IsTableExists("Orders");

            if (!tableExists)
            {
                migrationBuilder.CreateTable(
                    name: "Orders",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "INTEGER", nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        TableId = table.Column<int>(type: "INTEGER", nullable: false),
                        Price = table.Column<double>(type: "REAL", nullable: false),
                        IsPaid = table.Column<bool>(type: "INTEGER", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Orders", x => x.Id);
                    });
            }

            tableExists = migrationBuilder.IsTableExists("ProductsInOrder");

            if (!tableExists)
            {
                migrationBuilder.CreateTable(
               name: "ProductsInOrder",
               columns: table => new
               {
                   Id = table.Column<int>(type: "INTEGER", nullable: false)
                       .Annotation("Sqlite:Autoincrement", true),
                   OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                   ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                   Amount = table.Column<int>(type: "INTEGER", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_ProductsInOrder", x => x.Id);
               });
            }

            tableExists = migrationBuilder.IsTableExists("Products");

            if (!tableExists)
            {
                migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<double>(type: "REAL", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCommon = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            }

            migrationBuilder.Sql("DROP INDEX IF EXISTS [IX_Products_CategoryId]");
            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.IsTableExists("Orders"))
            {
                migrationBuilder.DropTable(
                    name: "Orders");
            }
            if (migrationBuilder.IsTableExists("ProductsInOrder"))
            {
                migrationBuilder.DropTable(
                    name: "ProductsInOrder");
            }
            if (migrationBuilder.IsTableExists("Products"))
            {
                migrationBuilder.DropTable(
                    name: "Products");
            }
            if (migrationBuilder.IsTableExists("Categories"))
            {
                migrationBuilder.DropTable(
                    name: "Categories");
            }
        }
    }

    public static class MigrationBuilderExtensions
    {
        public static bool IsTableExists(this MigrationBuilder migrationBuilder, string tableName)
        {
            var activeProvider = migrationBuilder.ActiveProvider;

            if (activeProvider.Equals("Microsoft.EntityFrameworkCore.Sqlite"))
            {
                var context = new POSContext();
                var connection = context.Database.GetDbConnection();

                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'";
                    using (var reader = command.ExecuteReader())
                    {
                        return reader.HasRows;
                    }
                }
            }

            return false;
        }
    }
}
