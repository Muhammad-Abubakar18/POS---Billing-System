# DrMusa — Offline POS & Billing System

## Overview

**DrMusa** is a professional offline Point of Sale and Billing System for Windows Desktop built with **.NET 8**, **WinForms**, **SQLite**, and **Clean Architecture**.

---

## Technology Stack

| Layer | Technology |
|-------|-----------|
| Language | C# 12 |
| Framework | .NET 8 LTS |
| UI | WinForms |
| Database | SQLite |
| ORM | Entity Framework Core 8 |
| Validation | FluentValidation 11 |
| Logging | Serilog 4 |
| DI | Microsoft.Extensions.DependencyInjection 8 |

---

## Quick Start

```bash
# 1. Restore packages
dotnet restore

# 2. Run migrations (first time)
cd src/DrMusa.Data
dotnet ef migrations add InitialCreate
dotnet ef database update

# 3. Run the application
cd ../DrMusa.Desktop
dotnet run
```

**Default Login:**
- Username: `admin`
- Password: `admin123`

---

## Project Structure

```
DrMusa/
├── src/
│   ├── DrMusa.Desktop/     # WinForms UI (Presentation)
│   ├── DrMusa.Business/    # Services, DTOs, Validators (Business)
│   ├── DrMusa.Data/        # EF Core, Models, Repositories (Data)
│   ├── DrMusa.Common/      # Enums, Constants, Utilities (Shared)
│   └── DrMusa.Tests/       # xUnit Tests
├── database/               # SQLite DB file + Backups
├── documents/              # PRD and technical docs
├── assets/                 # Logos, icons, images
├── appsettings.json
└── DrMusa.sln
```

---

## Modules

| # | Module | Status |
|---|--------|--------|
| 1 | Authentication | Phase 1 |
| 2 | Dashboard | Phase 1 |
| 3 | Product Management | Phase 1 |
| 4 | Category Management | Phase 1 |
| 5 | Customer Management | Phase 1 |
| 6 | Supplier Management | Phase 1 |
| 7 | Purchase Management | Phase 2 |
| 8 | Billing (POS) | Phase 1 |
| 9 | Sales Management | Phase 1 |
| 10 | Inventory Management | Phase 1 |
| 11 | Reports | Phase 1 |
| 12 | Receipt Printing | Phase 1 |
| 13 | User Management | Phase 2 |
| 14 | Settings | Phase 1 |
| 15 | Backup & Restore | Phase 2 |

---

## License

Private — All rights reserved.
