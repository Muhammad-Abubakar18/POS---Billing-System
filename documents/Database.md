# DrMusa — Database Schema

## Tables

### Users
| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER PK | Auto-increment |
| Username | TEXT | Unique |
| PasswordHash | TEXT | SHA-256 hashed |
| FullName | TEXT | |
| Email | TEXT | Nullable |
| Phone | TEXT | Nullable |
| Role | INTEGER | Admin=1, Manager=2, Cashier=3 |
| IsActive | INTEGER | Bool |
| CreatedAt | TEXT | DateTime |
| LastLoginAt | TEXT | Nullable |

### Products
| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER PK | |
| Name | TEXT | Required, max 200 |
| Barcode | TEXT | Unique, nullable |
| Description | TEXT | Nullable |
| CategoryId | INTEGER FK | → Categories |
| PurchasePrice | REAL | decimal(18,2) |
| SellingPrice | REAL | decimal(18,2) |
| CurrentStock | INTEGER | |
| MinimumStock | INTEGER | Low-stock threshold |
| ImagePath | TEXT | Nullable |
| IsActive | INTEGER | Soft delete |

### Categories
| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER PK | |
| Name | TEXT | Required |
| Description | TEXT | Nullable |
| IsActive | INTEGER | |

### Customers
| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER PK | |
| Name | TEXT | |
| Phone | TEXT | |
| Email | TEXT | |
| Address | TEXT | |
| TotalPurchases | REAL | Running total |
| IsActive | INTEGER | |

### Suppliers
| Column | Type | Notes |
|--------|------|-------|
| Id | INTEGER PK | |
| Name | TEXT | |
| ContactPerson | TEXT | |
| Phone | TEXT | |
| Email | TEXT | |
| Address | TEXT | |
| IsActive | INTEGER | |

### Purchases / PurchaseItems
Tracks incoming stock from suppliers.

### Sales / SaleItems
Tracks POS billing transactions.

### Inventory
Full movement log: StockIn, StockOut, Adjustment, SaleReturn, PurchaseReturn.

### Payments
Linked to Sales — supports split payment.

### Settings
Key-value store for business configuration.

### UserLogs
Audit trail for every user login, logout, and critical action.
