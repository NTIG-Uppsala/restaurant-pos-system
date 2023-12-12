# Setup database

## Database from default

* When the program is run without an existing database a database is created from a `.txt` file

* The `.txt` files separates values by the keyword "_SPLIT_HERE_"

* One is located in the `PointOfSaleSystem` project and the other in the `TestSystem` project

* The `.txt` file is chosen by the `.env` file located in the same folder

## DB Browser

1. Download DB Browser [Here](https://sqlitebrowser.org/dl/) and install it

    It should look like this ![](images/dbbrowser.JPG)

2. Click "Open Database" and select its location

3. Navigate to the existing database
    * If program has been run the database will be created at `C:\Users\%UserName%\AppData\Local\Restaurant-POS\`

4. To add products you can go into the SQL tab and write the following line (change the sent values)
    ``` SQL
    INSERT INTO products (product, price, categoryId) VALUES ('ProductName', 20, 1);
    ```
