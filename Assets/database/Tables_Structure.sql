CREATE DATABASE IF NOT EXISTS cafe;
use cafe;
-- Drop tables if they exist (in correct order to handle foreign key constraints)
DROP TABLE IF EXISTS order_details;
DROP TABLE IF EXISTS payments;
DROP TABLE IF EXISTS inventory;
DROP TABLE IF EXISTS products;
DROP TABLE IF EXISTS orders;
DROP TABLE IF EXISTS categories;
DROP TABLE IF EXISTS suppliers;
DROP TABLE IF EXISTS users;

-- 1. Create Parents Tables
CREATE TABLE categories
(
    id          int primary key identity (1,1),
    name        varchar(50) NOT NULL,
    description varchar(255),
    created_at  DATETIME DEFAULT GETDATE()
);

CREATE TABLE suppliers
(
    id           int primary key identity (1,1),
    name         varchar(50) NOT NULL,
    contact_name varchar(50),
    phone        varchar(50),
    email        varchar(50),
    address      varchar(255),
    created_at   DATETIME DEFAULT GETDATE()
);

CREATE TABLE users
(
    id            int primary key identity (1,1),
    username      varchar(50),
    password varchar(255),
    role          varchar(50),
    status        varchar(50),
    profile_img   varchar(255),
    reg_date DATETIME DEFAULT GETDATE()
);

-- 2. Create CHILD tables (depend on parents)
CREATE TABLE products
(
    id            INT PRIMARY KEY IDENTITY (1,1),
    name          VARCHAR(50),
    categories_id INT,
    price         DECIMAL(10, 2),
    discount      DECIMAL(5, 2),
    supplier_id   INT,
    created_at    DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (categories_id) REFERENCES categories (id),
    FOREIGN KEY (supplier_id) REFERENCES suppliers (id)
);

CREATE TABLE orders
(
    id           INT PRIMARY KEY IDENTITY (1,1),
    order_date   DATETIME DEFAULT GETDATE(),
    status       VARCHAR(50) NOT NULL DEFAULT 'Processing',
    total_amount DECIMAL(10, 2) NOT NULL CHECK (total_amount >= 0),
    note         varchar(50),
    created_at   DATETIME DEFAULT GETDATE(),
    CONSTRAINT CHK_status CHECK (status IN ('Processing', 'Completed', 'Cancelled'))
);

-- 3. Create GRANDCHILD tables (depend on children)
CREATE TABLE order_details
(
    id         INT PRIMARY KEY IDENTITY (1,1),
    order_id   INT NOT NULL,
    product_id INT NOT NULL,
    quantity   INT,
    unit_price DECIMAL(10, 2) NOT NULL CHECK (unit_price >= 0),
    sub_total  DECIMAL(10, 2) NOT NULL CHECK (sub_total >= 0),
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (order_id) REFERENCES orders (id) ON DELETE CASCADE,
    FOREIGN KEY (product_id) REFERENCES products (id),
    CONSTRAINT CHK_ValidSubTotal CHECK (sub_total = quantity * unit_price)
);

CREATE TABLE payments
(
    id INT PRIMARY KEY IDENTITY (1,1),
    order_id INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL,
    payment_method VARCHAR(50) NOT NULL DEFAULT 'Cash',
    payment_date DATETIME DEFAULT GETDATE(),
    transaction_code VARCHAR(50) DEFAULT 'None',
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (order_id) REFERENCES orders(id),
    CONSTRAINT CHK_PaymentMethod CHECK (payment_method IN ('Cash', 'Card', 'Mobile')),
    CONSTRAINT CHK_PaymentStatus CHECK (status IN ('Pending', 'Paid', 'Failed', 'Refunded'))
);

CREATE TABLE inventory
(
    id INT PRIMARY KEY IDENTITY (1,1),
    product_id INT NOT NULL,
    stock_in INT,
    stock_out INT,
    created_at DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (product_id) REFERENCES products(id)
);
