# Update Database

## Database from default

* When the program is run without an existing database a database is created from a `.txt` file

* The `.txt` files separates values by the keyword "_SPLIT_HERE_"

* One is located in the `PointOfSaleSystem` project and the other in the `TestSystem` project

* The `.txt` file is chosen by the `.env` file located in the same folder

## Update Database

Firstly download the EF Core by using the following command.

```
dotnet tool install --global dotnet-ef --version 7.0.14
```

When the database structure is changed you need to create a [migration](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=dotnet-core-cli) file. use this command in the terminal and change 'NAME OF MIGRATION' to what has been done for example AddCategoryTable.

``` sh
dotnet ef migrations add 'NAME OF MIGRATION'
``` 

The database will now update next time the program is run but if you want to manually update the database use the following command.

```
dotnet ef database update
```

## Update products and Categories

1. Download DB Browser [Here](https://sqlitebrowser.org/dl/) and install it

    It should look like this ![](images/dbbrowser.JPG)

2. Click "Open Database" and select its location

3. Navigate to the existing database
    * If program has been run the database will be created at `C:\Users\%UserName%\AppData\Local\Restaurant-POS\`

4. To add products you can go into the SQL tab and write the following line (change the sent values. priority is 0 to 10 where 10 is the highest priority, isCommon is 1 for true or 0 for false) 
    ``` SQL
    INSERT INTO products (product, price, categoryId, priority, isCommon) VALUES ('ProductName', 20, 1, 5, 1);
    ```

5. To add categories you can go into the SQL tab and write the following line (change the sent values. Colors are taken from [Here](https://learn.microsoft.com/en-us/dotnet/api/system.windows.media.colors?view=windowsdesktop-8.0#remarks)) 
    ``` SQL
    INSERT INTO categories (name, color) VALUES ('CategoryName', 'Red');
    ```