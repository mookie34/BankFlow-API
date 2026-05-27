# BankFlow API

Loan Management System built with .NET 9, Clean Architecture, and CQRS.

## Overview

BankFlow is a REST API that simulates core banking operations: loan disbursement, amortization schedule generation, payment processing, and portfolio management. Built as a demonstration of enterprise-grade .NET architecture patterns applied to real financial domain logic.

## Architecture

```
src/
├── BankFlow.Domain          # Entities, enums, interfaces (zero dependencies)
├── BankFlow.Application     # CQRS commands/queries, DTOs, validators, mappings
├── BankFlow.Infrastructure  # EF Core DbContext, repositories, Unit of Work
└── BankFlow.API             # Controllers, Program.cs, Swagger config

tests/
└── BankFlow.UnitTests       # 52 tests covering domain entities and handlers
```

**Patterns:** Clean Architecture, CQRS (MediatR), Repository, Unit of Work, DDD

## Tech Stack

| Layer | Technologies |
|-------|-------------|
| API | .NET 9, ASP.NET Core Web API, Swagger/OpenAPI |
| Application | MediatR 12, AutoMapper 13, FluentValidation 11 |
| Infrastructure | Entity Framework Core 8, SQL Server |
| Testing | xUnit, Moq, FluentAssertions |
| DevOps | Docker (multi-stage), Docker Compose, GitHub Actions CI/CD |

## Getting Started

### Prerequisites

- .NET 9 SDK
- SQL Server (or Docker)

### Run with Docker Compose

```bash
docker-compose up --build
```

API available at `http://localhost:5000/swagger`

### Run locally

```bash
# Update connection string in appsettings.json
dotnet restore
dotnet build
dotnet run --project src/BankFlow.API
```

### Run tests

```bash
dotnet test tests/BankFlow.UnitTests --verbosity normal
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/customers | List all customers |
| GET | /api/customers/{id} | Get customer by ID |
| POST | /api/customers | Create customer |
| GET | /api/loans/{id} | Get loan details with amortization schedule |
| GET | /api/loans/customer/{customerId} | Get all loans for a customer |
| POST | /api/loans | Create and activate a loan |
| POST | /api/payments | Process a loan payment |

## Domain Model

- **Customer** — Borrower with identification, credit score, and contact info
- **Loan** — Tracks amount, interest rate, term, status (Pending → Active → PaidOff), and outstanding balance
- **LoanSchedule** — Amortization table with principal/interest split per installment
- **Payment** — Records each payment with automatic distribution between principal and interest

## CI/CD

GitHub Actions pipeline runs on every push to `main`:

1. Restore → Build → Run 52 unit tests
2. Build Docker image (tests run inside multi-stage build)
3. Tag image with commit SHA

## Author

**Santiago Mazo Padierna** — Backend Software Engineer | 11+ years in .NET & Banking  
[santiagomazo34@gmail.com](mailto:santiagomazo34@gmail.com)
