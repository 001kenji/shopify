<div align="center">

# Shop Management System

**A modern web platform for browsing inventory, booking items for pickup, and requesting deliveries.**

Built with ASP.NET Core MVC, Entity Framework Core, ASP.NET Core Identity, and Tailwind CSS.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core)
[![EF Core](https://img.shields.io/badge/EF%20Core-ORM-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://learn.microsoft.com/ef/core)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind%20CSS-v4-38BDF8?style=flat-square&logo=tailwindcss&logoColor=white)](https://tailwindcss.com/)
[![License](https://img.shields.io/badge/License-MIT-C07D22?style=flat-square)](#license)

</div>

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Screenshots](#screenshots)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Database Setup](#database-setup)
  - [Running the App](#running-the-app)
- [Project Structure](#project-structure)
- [Booking & Delivery Lifecycles](#booking--delivery-lifecycles)
- [Configuration](#configuration)
- [Tailwind CSS Build](#tailwind-css-build)
- [Deployment](#deployment)
- [Terms & Privacy](#terms--privacy)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Overview

**Shop Management System** is a full-stack web application that lets customers browse a live product catalogue, reserve items for in-store pickup (**bookings**), and request home delivery on eligible products (**deliveries**). Shop administrators get a dashboard to manage inventory, confirm bookings, and progress deliveries through their lifecycle.

The project is built as a **lightweight alternative** to heavyweight e-commerce platforms — it focuses on the reservation + delivery workflow rather than online payments, making it ideal for local shops, pharmacies, grocers, or any business where customers prefer to see live stock before committing.

---

## Features

### Customer-facing

- 🛍️ **Product catalogue** — browse items with images, prices, stock levels, and categories
- 🔍 **Search & filter** — by product name, category, and sortable columns (name, price, quantity)
- 📅 **Bookings** — reserve items for pickup with a chosen pickup window
- 🚚 **Delivery requests** — available on deliverable items within a configurable radius
- 📊 **Personal dashboard** — track bookings and deliveries with live status updates
- 👤 **Account management** — profile, email, password, 2FA, and personal data controls
- 🌓 **Polished light theme** — clean white + golden-brown palette, mobile-first responsive

### Admin-facing

- 📦 **Inventory management** — full CRUD with image upload
- 🧾 **Booking management** — confirm, complete, or cancel incoming bookings
- 🚚 **Delivery management** — confirm, dispatch, complete, or cancel deliveries
- 🔔 **Status workflows** — colour-coded chips and row accents for at-a-glance scanning
- 🧑‍💼 **User oversight** — view customer details attached to each order

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 8 (ASP.NET Core) |
| **Web framework** | ASP.NET Core MVC + Razor Pages |
| **ORM** | Entity Framework Core |
| **Database** | SQL Server (LocalDB / SQL Express / Azure SQL) |
| **Auth** | ASP.NET Core Identity |
| **Styling** | Tailwind CSS v4 (CLI) |
| **Icons** | Heroicons (inline SVG) |
| **Client validation** | jQuery Validation / Unobtrusive |
| **File uploads** | `IFormFile` + `wwwroot/media` |

---

## Screenshots

> _Add screenshots here. Recommended: homepage hero, shop grid, booking confirmation, admin dashboard._

| Home | Shop | Booking |
|---|---|---|
| ![](docs/screenshots/home.png) | ![](docs/screenshots/shop.png) | ![](docs/screenshots/booking.png) |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) (any edition, including LocalDB)
- [Node.js 18+](https://nodejs.org/) *(only if you're rebuilding Tailwind)*
- [Git](https://git-scm.com/)

### Installation

```bash
# Clone the repository
git clone https://github.com/<your-username>/shop-management-system.git
cd shop-management-system

# Restore .NET dependencies
dotnet restore

# Install Tailwind (optional — only for CSS development)
npm install
```

### Database Setup

1. Update the connection string in `appsettings.json` (see [Configuration](#configuration)).
2. Apply EF Core migrations:

```bash
dotnet ef database update
```

3. *(Optional)* Seed sample data — check for a `DbInitializer` or use your own seed.

### Running the App

```bash
dotnet run
```

The app will start on `https://localhost:5001` (or the port shown in the console).

If you want to rebuild Tailwind CSS while you work:

```bash
# In a second terminal
npm run css:watch
```

---

## Project Structure

```
Shop_Management_System/
├── Areas/
│   └── Identity/               # ASP.NET Core Identity Razor Pages
│       └── Pages/
│           ├── Account/        # Login, Register, 2FA, Manage
│           └── ...
├── Controllers/
│   ├── HomeController.cs       # Landing & privacy pages
│   ├── InventoriesController.cs
│   ├── BookingsController.cs
│   ├── DeliveriesController.cs
│   └── ShopController.cs       # Public product browsing
├── Data/
│   └── ApplicationDbContext.cs
├── Extensions/
│   └── EnumExtensions.cs       # GetDisplayName() for enums
├── Models/
│   ├── Inventory.cs
│   ├── Booking.cs
│   ├── Delivery.cs
│   └── Enums/                  # CategoriesChoices, BookingStatus, DeliveryStatus
├── Views/
│   ├── Home/                   # Index, Privacy (Terms)
│   ├── Inventories/            # Admin CRUD
│   ├── Bookings/
│   ├── Deliveries/
│   ├── Shop/                   # Public shop
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _LoginPartial.cshtml
│       ├── _StatusMessage.cshtml
│       └── Error.cshtml
├── wwwroot/
│   ├── css/
│   │   ├── site.css            # Tailwind input file
│   │   └── output.css          # Built output (linked in layout)
│   ├── js/
│   ├── lib/                    # jQuery, jQuery Validation, Bootstrap icons
│   └── media/                  # Uploaded product images
├── appsettings.json
├── appsettings.Development.json
├── package.json                # Tailwind CLI scripts
├── tailwind.config.js          # (v3 only; v4 uses @theme in site.css)
└── Shop_Management_System.csproj
```

---

## Booking & Delivery Lifecycles

Both flows use a **status enum** rendered consistently across the app.

### Bookings

```
Pending ──▶ Confirmed ──▶ Completed
   │              │
   └──────────────┴──▶ Cancelled
```

- **Pending** — customer submitted a reservation; awaiting admin review
- **Confirmed** — admin approved; item is held for pickup
- **Completed** — customer collected the item
- **Cancelled** — cancelled by customer or admin before pickup

### Deliveries

```
Pending ──▶ Confirmed ──▶ Out for Delivery ──▶ Delivered
   │              │
   └──────────────┴──▶ Cancelled
```

- **Pending** — customer submitted a delivery request
- **Confirmed** — admin approved the delivery
- **Out for Delivery** — courier dispatched
- **Delivered** — received by customer
- **Cancelled** — cancelled before dispatch

Status colors are consistent throughout the UI: **yellow** for pending, **green** for confirmed, **blue** for in-progress/completed, **gray** for delivered, **red** for cancelled.

---

## Configuration

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShopManagementSystem;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Secrets

For production, do **not** commit real connection strings or keys. Use:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-connection-string>"
```

Or environment variables:

```bash
export ConnectionStrings__DefaultConnection="<your-connection-string>"
```

---

## Tailwind CSS Build

This project uses **Tailwind CSS v4** with the CSS-first `@theme` configuration. All theme tokens (including the custom `golden-*` palette) live in `wwwroot/css/site.css`:

```css
@import "tailwindcss";

@theme {
    --color-golden-50:  #fdf9f0;
    --color-golden-100: #faf0d7;
    --color-golden-200: #f4dfae;
    --color-golden-300: #ecc87b;
    --color-golden-400: #e3ad4e;
    --color-golden-500: #d99a2e;
    --color-golden-600: #c07d22;
    --color-golden-700: #a0611e;
    --color-golden-800: #834d1f;
    --color-golden-900: #6c401d;
}
```

### Scripts

```bash
npm run css:watch    # Rebuild on save (development)
npm run css:build    # Minified production build
```

The compiled file is `wwwroot/css/output.css`, which is what `_Layout.cshtml` links to. **Never link `site.css` directly** — it's the Tailwind input, not the final stylesheet.

---

## Deployment

### Azure App Service

```bash
dotnet publish -c Release -o ./publish
az webapp deploy --resource-group <rg> --name <app> --src-path ./publish
```

Set the production connection string in **Configuration → Connection strings** on the App Service.

### Docker (example)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Shop_Management_System.dll"]
```

### Before deploying

- [ ] Set `ASPNETCORE_ENVIRONMENT=Production`
- [ ] Configure a real SMTP sender for Identity emails
- [ ] Enable HTTPS redirection & HSTS
- [ ] Run `npm run css:build` and commit `output.css` (or run it in CI)
- [ ] Apply migrations: `dotnet ef database update`
- [ ] Verify the `wwwroot/media/` folder has write permissions

---

## Terms & Privacy

The app ships with a full **Terms & Conditions** page (`/Home/Privacy`) and a **Privacy Policy** linked throughout the registration and account flows. Both are editable directly in the Razor views:

- `Views/Home/Privacy.cshtml` — Terms & Conditions (with ToC and anchor links)
- Registration flows include passive consent text linking to the terms

If you fork this project for your own shop, **replace the placeholder contact details** in the policy pages with your real business information before going live.

---

## Contributing

Contributions are welcome. To keep the codebase consistent:

1. **Fork** the repo and create a feature branch: `git checkout -b feature/my-thing`
2. **Follow the existing patterns:**
   - Razor views use Tailwind utility classes (`.card`, `.btn-primary`, `.form-input` are defined in `site.css` via `@layer components`)
   - Status enums use `GetDisplayName()` from `Shop_Management_System.Extensions`
   - Never link `site.css` — always `output.css`
3. **Run the Tailwind watcher** while developing views: `npm run css:watch`
4. **Commit** with clear messages and open a **Pull Request**

### Coding conventions

- **C#** — file-scoped namespaces, async/await everywhere, `Include()` on navigation properties
- **Razor** — prefer local variables at the top of `@{ }` blocks over inline logic in markup
- **Tailwind** — group utilities by layout → spacing → typography → color → state
- **Accessibility** — always include `aria-label` on icon-only buttons and `title` on truncated text

---

## License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## Contact

**Shop Management System**
Nairobi, Kenya
📧 [support@example.com](mailto:support@example.com)

> Replace this section with your real contact details and repository URL before publishing.

---

<div align="center">

Made with ☕ and a lot of golden-600.

</div>