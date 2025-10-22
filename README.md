# 🗾 **Sales API – Developer Evaluation**

## 📘 Overview

This project implements a **Sales Management API** designed with **Domain-Driven Design (DDD)** and **CQRS (Command Query Responsibility Segregation)** principles.  
It simulates a real-world e-commerce or ERP sales module where each **Sale** can have multiple **Products**, with support for **discounts**, **cancellation**, and **domain event publishing** (via Serilog logging).

The goal is to demonstrate architectural quality, testability, and scalability — following the best practices described in the initial documentation (`overview.md`, `general-api.md`, etc.).

---

## 🧩 Features

-   ✅ CRUD operations for **Sales**
-   ✅ **Domain-driven design** (aggregates, value objects, and invariants)
-   ✅ **CQRS pattern** with MediatR (commands and queries separated)
-   ✅ **Pagination, sorting, and filtering** via query parameters
-   ✅ **Domain events**: `SaleCreated`, `SaleModified`, `SaleCancelled`, `ItemCancelled`
-   ✅ **Serilog logging** for structured event publication
-   ✅ Unit testing ready
-   ✅ Modular project organization (DDD-style folder structure)

---

## ⚙️ Tech Stack

| Layer         | Technology            |
| ------------- | --------------------- |
| **Framework** | .NET 8.0              |
| **ORM**       | Entity Framework Core |
| **Database**  | PostgreSQL            |
| **Logging**   | Serilog               |
| **Mediator**  | MediatR               |
| **Mapping**   | AutoMapper            |
| **Testing**   | xUnit + NSubstitute   |

---

## 🧱 Project Structure

```
root
├── src/
│   ├── Ambev.DeveloperEvaluation.WebApi/          → ASP.NET Core Web API (Controllers, Swagger)
│   ├── Ambev.DeveloperEvaluation.Application/  → CQRS, DTOs, Handlers, Validators
│   ├── Ambev.DeveloperEvaluation.Domain/       → Entities, Value Objects, Enums, Domain Events
│   ├── Ambev.DeveloperEvaluation.ORM/          → EF Core Context & Configurations
│   ├── Ambev.DeveloperEvaluation.IoC/          → Dependency Injection
│   └── Ambev.DeveloperEvaluation.Common/       → Cross-cutting helpers, extensions
│
└── tests/
    └── Ambev.DeveloperEvaluation.Tests/        → Unit and integration tests
```

---

## ⚡ CQRS Implementation

### Commands

| Command             | Responsibility                                  |
| ------------------- | ----------------------------------------------- |
| `CreateSaleCommand` | Creates a new sale and emits `SaleCreatedEvent` |
| `UpdateSaleCommand` | Updates sale data and emits `SaleModifiedEvent` |
| `CancelSaleCommand` | Cancels a sale and emits `SaleCancelledEvent`   |

### Queries

| Query              | Responsibility                                              |
| ------------------ | ----------------------------------------------------------- |
| `GetSalesQuery`    | Returns a paginated list of sales with filters and ordering |
| `GetSaleByIdQuery` | Returns sale details by ID                                  |

---

## 🔍 Pagination, Filtering and Sorting

Following `general-api.md`, endpoints support:

| Parameter         | Description   | Example                                    |
| ----------------- | ------------- | ------------------------------------------ |
| `_page`           | Page number   | `?_page=2`                                 |
| `_size`           | Page size     | `?_size=10`                                |
| `_order`          | Sort fields   | `?_order=date desc,totalAmount asc`        |
| `_minX` / `_maxX` | Range filters | `?_minDate=2024-01-01&_maxDate=2024-12-31` |
| `field=value`     | Exact filters | `?status=Cancelled&customerId=...`         |

Filtering and ordering are implemented dynamically via **Expression Trees** in the `GetSalesQueryHandler`.

---

## 🗾 Domain Events

The events are collected by the domain entities, persisted via EF Core, and then dispatched through the `DomainEventDispatcher`, which logs them via **Serilog**.

Example log:

```
[22:45:10 INF] 📣 Domain event published: SaleCreatedEvent | Payload: { SaleId = 45a..., SaleNumber = "S-2025-001", Date = 2025-10-20T22:45:10Z }
```

---

## 🔰 Logging (Serilog)

The API uses **Serilog** for structured, multi-sink logging.

**Outputs**

-   Console (`.WriteTo.Console()`)
-   File (`logs/log-<date>.txt`)

**Sample log format**

```
[2025-10-20 22:45:10 INF] SaleCreatedEvent | SaleId: 45a..., SaleNumber: S-2025-001
```

---

## 🚀 Running the Application

### 1️⃣ Configure Database

1. Open the `template/backend` folder in a terminal and run: `docker compose up -d` to run the containers with de database and API.
2. Install dotnet-ef tool if you don't have it: `dotnet tool install --global dotnet-ef`
3. Run the migrations to update the database: `dotnet ef database update -c DefaultContext -s .\src\Ambev.DeveloperEvaluation.WebApi\Ambev.DeveloperEvaluation.WebApi.csproj`

### 2️⃣ Swagger

Once running, access Swagger UI at:

```
http://localhost:8080/swagger
```

### 3️⃣ Follow the api logs

You can see the API logs with: `docker compose logs -f ambev.developerevaluation.webapi`

---

## 🤪 Tests and coverage

To run the tests and the coverage report, run the script `coverage-report.bat` inside `template/backend`. Then open the generated `index.html` file inside `TestResults` folder.

---

## 👨‍💻 Author

**Vitor Lacerda**  
Fullstack Software Engineer — .NET | Angular  
GitHub: [@vitorlacerda](https://github.com/vitorlacerda)
