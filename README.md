# OnlineShop - Advanced Programming Workshop (C#)

This repository contains a console-based online shop implemented with an MVC-style layered architecture in C#.

## Projects

- `src/OnlineShop.Core`: domain entities, enums, validation, business rules
- `src/OnlineShop.Data`: JSON persistence, DTOs, mappers
- `src/OnlineShop.Controllers`: use-cases and controllers
- `src/OnlineShop.App`: console UI (home/register/login/buyer area/manager CLI)
- `src/OnlineShop.Tests`: xUnit tests

## Quick Start

```bash
dotnet build OnlineShop.slnx
dotnet test src/OnlineShop.Tests/OnlineShop.Tests.csproj
dotnet run --project src/OnlineShop.App
```

## Default Accounts

- Manager: `admin` / `admin`
- Seeded buyer: `ali_reza` / `Passw0rd1`

## Notes

- Runtime data is stored in `data/store.json`.
- The project includes product inheritance, manager request approval flows, filtering/pagination, cart/checkout, comments, ratings, and credit charge requests.
