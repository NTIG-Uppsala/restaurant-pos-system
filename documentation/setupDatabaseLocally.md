# Setup database
* Download [XAMPP](https://www.cs.virginia.edu/~up3f/cs4750/supplement/XAMPP-setup.html) with MySQL and apache
* Press config in the Apache module's column and press "Apache (httpd.conf)".
* Press Ctrl + F and search for "80" and replace it with "81"
* Save and exit
* Start MySQL and Apache from the XAMPP Control Panel
* Go to [localhost:81](http://localhost:81/) and go to `phpMyAdmin`
* Create a new database with the name `restaurant-POSS`
* Go to the SQL tab in the new database and input the following command
 
    ``` SQL
    CREATE TABLE products (
        ID INT AUTO_INCREMENT PRIMARY KEY,
        product VARCHAR(255),
        price DECIMAL(10, 2),
        categoryId INT
    );
    ```
* To add products you can go into the SQL tab and write the following line (change the sent values)
    ``` SQL
    INSERT INTO products (product, price, categoryId) VALUES ('ProductName', 20, 1);
    ```