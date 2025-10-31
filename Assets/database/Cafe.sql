IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'cafe')
CREATE DATABASE cafe;

DROP TABLE IF EXISTS users;
CREATE TABLE users(
    id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    profile_img NVARCHAR(255) NULL,
    role NVARCHAR(20) NOT NULL DEFAULT 'User',
    status NVARCHAR(15) NOT NULL DEFAULT 'Pending',
    reg_date DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

-- User with profile image
INSERT INTO users (username, password, profile_img, role, status) 
VALUES ('vattra.com', '7d1598927ed9c9b579b548beec60215e2f83c31a307ff0a8407f3fbcde6315bb', 'profiles/Vattra.png', 'admin', 'Active');

DECLARE @fromDate DATETIME = '2025-10-28 00:00:00.000'
DECLARE @toDate DATETIME = '2025-11-29 00:00:00.000'

SELECT TOP 5
    p.name,
    SUM(od.quantity) as Q
FROM order_details od
INNER JOIN products p ON p.Id = od.product_id
INNER JOIN orders o ON o.Id = od.order_id
WHERE od.created_at BETWEEN @fromDate AND @toDate
GROUP BY p.name  -- Remove the duplicate p.name
ORDER BY Q DESC;

DECLARE @fromDate DATETIME = '2025-10-28 00:00:00.000'
DECLARE @toDate DATETIME = '2025-11-29 23:59:59.999'
SELECT created_at, sum(total_amount) From [Orders]
                                            WHERE created_at between @fromDate and @toDate group by created_at;
select  * from orders;

SELECT TOP 5
    p.name as ProductName,
    SUM(od.quantity) as TotalQuantityOrdered
FROM order_details od
INNER JOIN products p ON p.id = od.product_id  -- Only need this join!
WHERE od.created_at BETWEEN @fromDate AND @toDate
GROUP BY p.name
ORDER BY TotalQuantityOrdered DESC;

SELECT
    p.name as ProductName,
    (COALESCE(SUM(i.stock_in), 0) - COALESCE(SUM(i.stock_out), 0)) as CurrentStock,
    p.price as UnitPrice
FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
GROUP BY p.id, p.name, p.price
HAVING (COALESCE(SUM(i.stock_in), 0) - COALESCE(SUM(i.stock_out), 0)) <= 100
ORDER BY CurrentStock DESC;
SELECT
    p.id,
    p.name as ProductName,
    p.price as UnitPrice,
    COALESCE(SUM(i.stock_in), 0) as TotalStockIn,
    COALESCE(SUM(i.stock_out), 0) as TotalStockOut,
    (COALESCE(SUM(i.stock_in), 0) - COALESCE(SUM(i.stock_out), 0)) as CurrentStock
FROM products p
LEFT JOIN inventory i ON p.id = i.product_id
GROUP BY p.id, p.name, p.price
ORDER BY p.name;