# Economy Service

## Overview

This project is a backend wallet/economy service developed as part of a Backend Developer technical assessment.

The service provides a simple game economy where players can:

* Earn virtual currency
* Purchase items
* Claim one-time rewards
* View wallet information
* View transaction history

The project is built using:

* ASP.NET Core 8 Web API
* Entity Framework Core
* PostgreSQL
* Docker
* Swagger (OpenAPI)

---

## Features

* Credit currency to player wallet
* Purchase items with balance validation
* Claim rewards only once per player
* View wallet details
* View transaction history
* PostgreSQL persistent storage
* Entity Framework Core migrations
* Docker support

---

## Tech Stack

* ASP.NET Core 8
* C#
* Entity Framework Core
* PostgreSQL
* Docker
* Swagger

---

## How to Run

### 1. Start PostgreSQL

```bash
docker start postgres-db
```

### 2. Apply Database Migration

```bash
dotnet ef database update
```

### 3. Run the Application

```bash
dotnet run
```

### 4. Open Swagger

```
https://localhost:7055/swagger
```

---

## Available APIs

* POST /v1/wallets/{playerId}/credit
* GET /v1/wallets/{playerId}
* POST /v1/wallets/{playerId}/purchase
* POST /v1/wallets/{playerId}/claim-reward
* GET /v1/wallets/{playerId}/transactions

---

## Author

Md Kashif Rahman
