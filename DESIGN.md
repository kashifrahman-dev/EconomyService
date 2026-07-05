# DESIGN

## Architecture

The project follows a simple layered architecture:

* Controller Layer – Handles HTTP requests and responses.
* Data Layer – Entity Framework Core with ApplicationDbContext.
* Model Layer – Wallet and Transaction entities.
* DTO Layer – Request and response models.
* Service Layer – Prepared for future business logic separation.

## Datastore Choice

PostgreSQL was selected because it provides:

* ACID transactions
* Strong consistency
* Reliable persistence
* Good support with Entity Framework Core

## API Design

The service exposes the following endpoints:

* POST /v1/wallets/{playerId}/credit
* GET /v1/wallets/{playerId}
* POST /v1/wallets/{playerId}/purchase
* POST /v1/wallets/{playerId}/claim-reward
* GET /v1/wallets/{playerId}/transactions

## Atomicity

Purchase operations validate balance before deducting currency.

If balance is insufficient, the transaction is rejected and no wallet data is modified.

Reward claims are validated to prevent duplicate claims.

## Validation

The application validates:

* Positive amounts
* Available wallet
* Sufficient balance
* Duplicate reward claims

## Durability

Data is stored in PostgreSQL, ensuring persistence across application restarts.

Entity Framework Core manages database transactions and persistence.
