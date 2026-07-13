# DrMusa
# Offline POS Billing & Inventory Management System

**Version:** 1.0  
**Platform:** Windows Desktop (Offline)  
**Framework:** .NET 8  
**IDE:** VS Code  
**Database:** SQLite

---

# 1. Project Overview

## Project Name

**DrMusa**

## Description

DrMusa is a professional offline Point of Sale (POS) and Billing System for Windows Desktop. It enables businesses to manage products, inventory, sales, purchases, customers, suppliers, reports, and receipt printing without requiring an internet connection.

---

# 2. Technology Stack

| Component | Technology |
|------------|------------|
| Language | C# |
| Framework | .NET 8 LTS |
| UI | WinForms |
| Database | SQLite |
| ORM | Entity Framework Core |
| Reporting | QuestPDF |
| Barcode | ZXing.Net |
| Logging | Serilog |
| Dependency Injection | Microsoft.Extensions.DependencyInjection |
| Validation | FluentValidation |
| IDE | VS Code |

---

# 3. Project Architecture

Clean Architecture

```
Presentation Layer
        │
Business Layer
        │
Data Access Layer
        │
SQLite Database
```

---

# 4. Project Structure

```
DrMusa/

│
├── src/
│
│   ├── DrMusa.Desktop/          # WinForms UI
│   │
│   │   ├── Forms/
│   │   │
│   │   ├── Dashboard/
│   │   ├── Billing/
│   │   ├── Products/
│   │   ├── Categories/
│   │   ├── Customers/
│   │   ├── Suppliers/
│   │   ├── Purchases/
│   │   ├── Inventory/
│   │   ├── Reports/
│   │   ├── Settings/
│   │   ├── Users/
│   │   └── Login/
│   │
│   │
│   ├── Components/
│   ├── Helpers/
│   ├── Assets/
│   ├── Resources/
│   └── Program.cs
│
│
│
│   ├── DrMusa.Business/
│   │
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   ├── Validators/
│   │   ├── DTOs/
│   │   ├── Mappers/
│   │   └── BusinessRules/
│
│
│
│   ├── DrMusa.Data/
│   │
│   │   ├── Context/
│   │   ├── Models/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   ├── Seed/
│   │   └── Migrations/
│
│
│
│   ├── DrMusa.Common/
│   │
│   │   ├── Constants/
│   │   ├── Enums/
│   │   ├── Exceptions/
│   │   ├── Utilities/
│   │   └── Extensions/
│
│
│
│   └── DrMusa.Tests/
│
│
├── database/
│
│   ├── DrMusa.db
│   └── Backup/
│
├── documents/
│
│   ├── PRD.md
│   ├── Database.md
│   ├── Modules.md
│   └── README.md
│
├── assets/
│
│   ├── logo/
│   ├── icons/
│   └── images/
│
├── appsettings.json
│
└── README.md

```

---

# 5. Modules

## Module 1 — Authentication

### Features

- Login
- Logout
- Change Password
- User Roles
- Remember Me
- Lock Application

---

## Module 2 — Dashboard

### Features

- Today's Sales
- Monthly Sales
- Total Products
- Total Customers
- Total Suppliers
- Recent Transactions
- Low Stock Alert
- Top Selling Products
- Sales Graph

---

## Module 3 — Product Management

### Features

- Add Product
- Edit Product
- Delete Product
- Search Product
- Barcode
- Product Image
- Product Category
- Purchase Price
- Selling Price
- Current Stock
- Minimum Stock

---

## Module 4 — Category Management

### Features

- Add Category
- Edit Category
- Delete Category
- Search Category

---

## Module 5 — Customer Management

### Features

- Add Customer
- Update Customer
- Delete Customer
- Customer Purchase History
- Search Customer

---

## Module 6 — Supplier Management

### Features

- Add Supplier
- Update Supplier
- Delete Supplier
- Search Supplier
- Supplier Purchase History

---

## Module 7 — Purchase Management

### Features

- Create Purchase
- Purchase Invoice
- Purchase History
- Purchase Return
- Update Stock Automatically

---

## Module 8 — Billing (POS)

### Features

- Barcode Search
- Product Search
- Shopping Cart
- Quantity Update
- Discount
- Tax
- Cash Payment
- Card Payment
- Split Payment (Future)
- Hold Bill
- Resume Bill
- Print Receipt
- Save Invoice

---

## Module 9 — Sales Management

### Features

- Sales History
- Invoice Search
- Reprint Invoice
- Sales Return
- Cancel Invoice

---

## Module 10 — Inventory Management

### Features

- Current Stock
- Stock In
- Stock Out
- Low Stock
- Stock Adjustment
- Inventory History

---

## Module 11 — Reports

### Reports

- Daily Sales Report
- Weekly Sales Report
- Monthly Sales Report
- Yearly Sales Report
- Purchase Report
- Profit Report
- Customer Report
- Supplier Report
- Inventory Report
- Low Stock Report

Export

- PDF
- Print

---

## Module 12 — Receipt Printing

### Features

- Thermal Printer
- A4 Invoice
- Business Logo
- Barcode
- QR Code (Future)
- Receipt Footer
- Print Preview

---

## Module 13 — User Management

### Features

- Add User
- Edit User
- Delete User
- User Roles
- Password Reset

---

## Module 14 — Settings

### Features

- Business Information
- Business Logo
- Receipt Header
- Receipt Footer
- Tax Settings
- Currency
- Printer Settings
- Backup Settings

---

## Module 15 — Backup & Restore

### Features

- Backup Database
- Restore Database
- Automatic Backup
- Manual Backup

---

# 6. Database Tables

- Users
- Products
- Categories
- Customers
- Suppliers
- Purchases
- PurchaseItems
- Sales
- SaleItems
- Inventory
- Payments
- Settings
- UserLogs

---

# 7. Development Phases

## Phase 1 (MVP)

- Authentication
- Dashboard
- Product Management
- Category Management
- Customer Management
- Supplier Management
- Billing
- Receipt Printing
- Inventory
- Reports

---

## Phase 2

- Purchase Management
- Sales Returns
- Purchase Returns
- User Management
- Backup & Restore

---

## Phase 3

- Expense Management
- Employee Management
- Payroll
- Loyalty Program
- Barcode Scanner Integration
- QR Code Support
- Analytics Dashboard
- Cloud Backup
- Multi-Branch Support

---

# 8. NuGet Packages

```
Microsoft.EntityFrameworkCore

Microsoft.EntityFrameworkCore.Sqlite

Microsoft.EntityFrameworkCore.Design

FluentValidation

Serilog

Serilog.Sinks.File

QuestPDF

ZXing.Net

Microsoft.Extensions.DependencyInjection
```

---

# 9. Development Order

1. Create Solution
2. Create Projects
3. Configure Dependency Injection
4. Configure SQLite
5. Authentication
6. Dashboard
7. Product Module
8. Category Module
9. Customer Module
10. Supplier Module
11. Purchase Module
12. Billing Module
13. Receipt Printing
14. Reports
15. Backup & Restore
16. Settings