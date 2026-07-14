# DrMusa
# Fast Food POS Billing System
## Offline POS Billing & Inventory Management System

**Version:** 1.1
**Platform:** Windows Desktop (Offline)
**Framework:** .NET 8
**IDE:** VS Code
**Database:** SQLite

---

# 1. Project Overview

## Project Name

**DrMusa**

## Description

DrMusa is a professional offline Point of Sale (POS) and Billing System for Windows Desktop, purpose-built for **Fast Food** businesses. It enables businesses to manage products, inventory, sales, and receipt printing without requiring an internet connection. The system is focused on fast, image-driven order taking at the counter — no customer, supplier, or purchase tracking is required.

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

## Module 5 — Billing (POS)

The core of DrMusa — a fast, touch/click-friendly ordering screen for cashiers.

### Features

- **Image-Based Ordering** — Products are displayed as image tiles grouped by category; cashier clicks a product image to add it to the cart
- Barcode Search
- Product Search (by name, as a fallback to image tap)
- Shopping Cart (live running list of items, quantities, and line totals)
- Quantity Update (increment/decrement or edit quantity per cart line)
- Remove Item from Cart
- Discount (per item / per bill)
- Tax
- Cash Payment
- Card Payment
- Split Payment (Future)
- Hold Bill
- Resume Bill
- **Complete Order & Print** — clicking Print finalizes the order, saves it to the database as a completed Sale, and sends the receipt to the printer
- Print Receipt
- Save Invoice

### Order Flow

1. Cashier taps product images as the customer orders; each tap adds the item to the cart
2. Cashier adjusts quantities, applies discount/tax if needed
3. Cashier selects payment method (Cash / Card)
4. Cashier clicks **Print**
5. System saves the order (Sale + SaleItems) to the database
6. System sends the receipt to the printer

---

## Module 6 — Sales Management

### Features

- Sales History
- Invoice Search
- Reprint Invoice
- Sales Return
- Cancel Invoice

---

## Module 7 — Inventory Management

### Features

- Current Stock
- Stock In
- Stock Out
- Low Stock
- Stock Adjustment
- Inventory History

---

## Module 8 — Reports

### Reports

- Daily Sales Report
- Weekly Sales Report
- Monthly Sales Report
- Yearly Sales Report
- Profit Report
- Inventory Report
- Low Stock Report

Export

- PDF
- Print

---

## Module 9 — Receipt Printing

### Features

- Thermal Printer
- A4 Invoice
- Business Logo
- Barcode
- QR Code (Future)
- Receipt Footer
- Print Preview

---

## Module 10 — User Management

### Features

- Add User
- Edit User
- Delete User
- User Roles
- Password Reset

---

## Module 11 — Settings

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

## Module 12 — Backup & Restore

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
- Billing (image-based ordering)
- Receipt Printing
- Inventory
- Reports

---

## Phase 2

- Sales Returns
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
9. Billing Module (image-based ordering)
10. Receipt Printing
11. Inventory
12. Reports
13. Backup & Restore
14. Settings