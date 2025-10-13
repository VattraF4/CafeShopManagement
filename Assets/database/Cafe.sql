IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'cafe')
CREATE DATABASE cafe;

DROP TABLE IF EXISTS users;
CREATE TABLE users(
    id INT PRIMARY KEY IDENTITY(1,1),
    username NVARCHAR(50) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    profile_img NVARCHAR(255) NULL,
    role NVARCHAR(20) NOT NULL DEFAULT 'User',
    status NVARCHAR(15) NOT NULL DEFAULT 'Active',
    reg_date DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

-- User with profile image
INSERT INTO users (username, password, profile_img, role, status) 
VALUES ('vattra.com', '7d1598927ed9c9b579b548beec60215e2f83c31a307ff0a8407f3fbcde6315bb', 'profiles/Vattra.png', 'admin', 'Active');

--Select all users
SELECT * FROM users;

