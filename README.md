# ☕ OOAD Cafe Shop Management

## 📝 Description

**OOAD Cafe Shop Management** is a desktop application built with **C# (WinForms)** and **SQL Server** for managing a coffee shop.
It allows users to handle sales, products, inventory, and user authentication (login, registration, and role-based access).

## ⚙️ Prerequisites

Before running the project, ensure you have the following installed:

- **Microsoft SQL Server** (Instance name: `SQLEXPRESS`)
- **Database name:**`cafe`
- **Visual Studio 2022** (or newer)
- **.NET Framework 4.8** or compatible runtime
- Optionally, you can run the prebuilt executable `OOADCafeShopManagement.exe` in the `bin` folder.

## Installation

1. Clone the repository: `https://github.com/VattraF4/CafeShopManagement.git`
2. Navigate to the project directory: `cd CafeShopManagement`

## Configuration

1. Open **SQL Server Management Studio (SSMS)**.
2. Make sure your SQL Server instance name is `.\SQLEXPRESS`.
3. Create a new database named `cafe`.
4. Execute the following scripts in this order:
   - `Tables_Structure.sql` → creates all database tables.
   - `Insert_Data.sql` → inserts sample data, including default users.

## Usage

### You can run the project in two ways:

### Option 1: Run the built executable

- Navigate to the `bin` folder.
- Open `OOADCafeShopManagement.exe`.
- Log in using:
  ```
  Username: vattra.com
  Password: V123$

  ```

### Option 1: Run from Visual Studio

1. Open the solution file OOADCafeShopManagement.sln in Visual Studio 2022.
2. Press F5 to build and run the project.
3. Use the same test credentials to log in.

## Contributing

Contributions are welcome!
If you'd like to contribute:

1. Fork this repository.
2. Create a new branch for your feature or bug fix.
3. Commit your changes.
4. Submit a pull request with a clear description of your improvement.

## License

This project is licensed under the MIT License.
You are free to use, modify, and distribute this project with proper attribution.

## Contact

**Author**: Ra Vattra
**GitHub**: [VattraF4](http://github.com/VattraF4)
**Portfolio**: [Ra Vattra](https://vattraf4.github.io/Ra-Vattra-Resume-Kh-and-Eng/)
**Email:** [ravattrasmartboy@gmail.com](https://)
