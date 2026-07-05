# RESILIENCE

## Handling External Inventory Service

If the inventory system becomes a separate service, I would use the Outbox Pattern together with asynchronous message processing.

The purchase transaction would first be committed successfully in the wallet database. After the transaction commits, an outbox event would be created and processed to notify the inventory service.

This approach prevents inconsistent states caused by partial failures.

## Duplicate Requests

Each client request should contain a unique idempotency key.

If the same request is received multiple times due to retries or network failures, the previously stored response should be returned instead of executing the operation again.

## Crash Recovery

Because wallet data is stored in PostgreSQL, committed transactions remain durable after unexpected application crashes.

Incomplete transactions are automatically rolled back by the database, preventing partial updates.

## Audit Trail

All currency operations are recorded in the Transaction table.

This audit history helps identify unexpected balance changes and provides traceability for every wallet operation.

## Future Improvements

* Outbox Pattern
* Distributed Saga Pattern
* Background Worker for retry processing
* Message Queue (RabbitMQ/Kafka)
* Idempotency Key Store
