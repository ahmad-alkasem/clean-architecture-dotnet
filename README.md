<div align="center">

# Clean Architecture · .NET 10

**A reference backend showing Clean Architecture, CQRS, and a validation pipeline done right.**

[![CI](https://github.com/ahmad-alkasem/clean-architecture-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/ahmad-alkasem/clean-architecture-dotnet/actions/workflows/ci.yml)

![.NET](https://img.shields.io/badge/.NET%2010-252555?style=flat-square&logo=dotnet&logoColor=white)
![MediatR](https://img.shields.io/badge/MediatR%20CQRS-252555?style=flat-square)
![FluentValidation](https://img.shields.io/badge/FluentValidation-252555?style=flat-square)
![EF Core](https://img.shields.io/badge/EF%20Core-252555?style=flat-square&logo=dotnet&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-252555?style=flat-square)

</div>

## Overview

A small, production-shaped slice of a backend built the way I structure real systems: dependencies point inward, the domain owns its invariants, and use cases are isolated as CQRS commands and queries dispatched through MediatR. The web layer stays thin — it only maps HTTP to requests.

## Architecture

```
WebApi          →  endpoints, DI composition, exception handling
  │
Application     →  CQRS handlers, DTOs, validators, pipeline behaviors
  │
Domain          →  entities, aggregates, business invariants (no dependencies)
  ▲
Infrastructure  →  EF Core, repositories (implements Application interfaces)
```

The dependency rule is enforced by project references: `Domain` depends on nothing, `Application` depends only on `Domain`, and `Infrastructure` + `WebApi` sit on the outside.

## What it demonstrates

- **CQRS** — writes (`CreateProductCommand`, `CreateCategoryCommand`) and reads (`GetProductsQuery`, `GetProductByIdQuery`, `GetCategoriesQuery`) are separate paths with their own handlers.
- **Aggregate relationships** — a `Product` references a `Category` by id; creating a product checks the category exists across aggregate boundaries and returns a not-found result (mapped to 404) when it does not.
- **Paged reads** — the product list returns a `PagedResult<T>` with page metadata, driven by `?page` and `?pageSize` query parameters and clamped to safe bounds.
- **Caching pipeline** — a MediatR behavior caches queries that opt in through `ICachedQuery` (the category list), and creating a category invalidates the cached entry.
- **Validation pipeline** — a MediatR `IPipelineBehavior` runs FluentValidation before any handler executes, so handlers never re-check input.
- **Rich domain** — `Product` is created through a factory that guards its own invariants; you cannot build an invalid aggregate.
- **Result pattern** — handlers return `Result<T>` for expected outcomes (invalid input, not found) instead of throwing, and the API maps each `Error` to an RFC 7807 `ProblemDetails` with the right status code.
- **Domain events** — aggregates raise events (`ProductCreatedDomainEvent`); the `DbContext` collects them and dispatches through MediatR after a successful save, while the domain itself stays free of any framework dependency.
- **Persistence behind an interface** — `IProductRepository` lives in `Application`; EF Core implements it in `Infrastructure`. It runs on PostgreSQL with EF Core migrations, and falls back to an in-memory provider when no connection string is configured.
- **Thin transport** — MVC controllers translate HTTP to MediatR requests and domain failures to clean problem responses, with the mapping centralized in a base controller.
- **Observability** — Serilog structured logging, a correlation id propagated through `X-Correlation-ID` and the log context, and a `/health` check wired to the database.
- **API documentation** — a generated OpenAPI document with an interactive Scalar reference in development.

## Tests

46 tests run on every push through GitHub Actions.

- **Unit** (`Application.UnitTests`) — domain invariants, the FluentValidation rules, the CQRS handlers with substituted dependencies, and the MediatR validation behavior.
- **Integration** (`WebApi.IntegrationTests`) — the HTTP endpoints exercised end to end through `WebApplicationFactory`, each test isolated by its own in-memory database.

```bash
dotnet test
```

## Run it

```bash
dotnet run --project src/WebApi
```

```bash
# create a category, then a product that belongs to it
curl -X POST http://localhost:5000/api/categories \
  -H "Content-Type: application/json" \
  -d '{ "name": "Peripherals" }'

curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -d '{ "name": "Keyboard", "price": 45.00, "stockQuantity": 12, "categoryId": "<id>" }'

# list products (paged)
curl "http://localhost:5000/api/products?page=1&pageSize=10"

# fetch one by id (404 with a problem+json body when it does not exist)
curl http://localhost:5000/api/products/{id}
```

With no connection string configured, `dotnet run` uses an in-memory database, so it starts with zero infrastructure.

## Run with Docker

The API and a PostgreSQL database come up together with one command:

```bash
docker compose up --build
```

The API applies its EF Core migrations on startup and is then served at `http://localhost:8080`. Liveness is exposed at `/health`, and the interactive API reference is at `/scalar/v1` in development.

## Project layout

```
src/
├─ Domain/          Entities, aggregates, invariants, Result/Error
├─ Application/     Commands, queries, DTOs, validators, behaviors, events
├─ Infrastructure/  EF Core DbContext, repositories, migrations
└─ WebApi/          Controllers + composition root
tests/
├─ Application.UnitTests/     Domain, validators, handlers, behavior, events
└─ WebApi.IntegrationTests/   Endpoints through WebApplicationFactory
Dockerfile · docker-compose.yml   API + PostgreSQL
```

## License

Released under the [MIT License](LICENSE).

---

<div align="center">
<sub>Built by <a href="https://ahmad-alkasem.me">Ahmad Alkasem</a> — Software Engineer & .NET Backend Engineer</sub>
</div>
