# Ding Payment Processor

A clean architecture-based payment processing system built in C# with .NET 10. This project implements a transactional account system with persistent storage using SQLite, providing a Web API for interaction and data management.

## Project Structure

```
Ding.PaymentProcessor
├── Ding.PaymentProcessor.Domain/           # Core business logic and entities
├── Ding.PaymentProcessor.Application/      # Application services and use cases
├── Ding.PaymentProcessor.Infrastructure/   # Data access and persistence
├── Ding.PaymentProcessor.Api/              # Web API presentation layer
├── Ding.PaymentProcessor.Domain.UnitTests/       # Domain layer tests
└── Ding.PaymentProcessor.Application.UnitTests/  # Application layer tests
```

## Technology Stack

- **Language**: C#
- **Framework**: .NET 10
- **Database**: SQLite
- **Testing**: NUnit
- **Architecture Pattern**: Clean Architecture

## Architectural Decisions

### 1. Web API with Clean Architecture
A Web API project structure was created to provide a user-friendly interface for interacting with the payment processor. This allows clients to make HTTP requests to perform account operations and view transaction data, while maintaining a clean separation of concerns across layers.

### 2. SQLite for Data Persistence
SQLite was chosen as the database solution for its simplicity and ease of integration. Unlike in-memory databases, SQLite provides actual persistent storage, making it ideal for demonstrating data durability and recovery capabilities.

### 3. Database Seeding Strategy
The database is pre-seeded with sample accounts and transactions during initialization. This approach ensures the system has realistic data for demonstration purposes and eliminates the need to expand the scope of the assignment with account creation business logic.

### 4. Dependency Injection via Route Parameters
The `Account` entity and its associated transactions are resolved from the API endpoint route parameters. This design respects the existing `IAccountService` interface method definitions while still enabling dependency injection throughout the application layer.

### 5. Captured Console Output for API Responses
To provide meaningful HTTP responses from the API endpoints (rather than only console logging), the console output from operations is captured and returned as part of the API response. This ensures clients receive useful feedback from their requests.

## Design Constraints & Solutions

| Constraint | Solution |
|-----------|----------|
| Cannot modify `IAccountService` interface | Inject dependencies through route parameters and resolve at the controller level |
| Data must persist across application restarts | Use SQLite database with automatic migrations and seeding |
| No external account creation scope | Pre-seed database with sample accounts during initialization |
| API endpoints should return meaningful responses | Capture console output from service operations and return as response body |

## Architecture Layers

### Domain Layer (`Ding.PaymentProcessor.Domain`)
Contains core business entities and interfaces:
- `Account` - Represents a customer account
- `Transaction` - Represents account transactions
- `IAccountService` - Interface defining account operations

### Application Layer (`Ding.PaymentProcessor.Application`)
Implements business logic and application services:
- Service implementations for account operations
- Business rule validation
- Transaction management

### Infrastructure Layer (`Ding.PaymentProcessor.Infrastructure`)
Handles data persistence and external services:
- SQLite database context and configuration
- Entity mappings and seeding strategy
- Repository implementations

### Presentation Layer (`Ding.PaymentProcessor.Api`)
Exposes functionality through Web API:
- REST endpoints for account operations
- Dependency injection configuration
- Request routing and response handling

## Database Design

### Entities

**Accounts Table**
- `Id` (GUID) - Primary key
- `AccountNumber` (string) - Unique account identifier
- `Balance` (decimal) - Current account balance
- `CreatedAt` (DateTime) - Account creation timestamp

**Transactions Table**
- `Id` (GUID) - Primary key
- `AccountId` (GUID) - Foreign key to Accounts
- `Amount` (decimal) - Transaction amount
- `Description` (string) - Transaction details
- `CreatedAt` (DateTime) - Transaction timestamp

### Database Initialization
The database is automatically created and seeded with sample accounts and transactions on application startup. This ensures a consistent initial state for demonstration and testing.

## Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio / Visual Studio Code (optional)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/hututuiandrei/DingAssignment.git
   cd DingAssignment
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Run the API**
   ```bash
   dotnet run --project Ding.PaymentProcessor.Api
   ```

## Testing

The project includes comprehensive unit tests using NUnit:

- **Domain Tests** (`Ding.PaymentProcessor.Domain.UnitTests`) - Test core business logic and validation
- **Application Tests** (`Ding.PaymentProcessor.Application.UnitTests`) - Test service implementations and use cases

Run all tests:
```bash
dotnet test
```

Run tests for a specific project:
```bash
dotnet test Ding.PaymentProcessor.Domain.UnitTests
dotnet test Ding.PaymentProcessor.Application.UnitTests
```
