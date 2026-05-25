# IndPubBack

IndPubBack is a .NET 10 Web API pet project for an online book publishing and reading platform. It focuses on backend architecture, authentication, content management, and user engagement features such as subscriptions, library tracking, likes, reviews, comments, and search.

This project is a strong resume piece because it demonstrates real-world backend patterns: layered architecture, JWT authentication, EF Core with PostgreSQL, Azure Blob Storage integration, validation, and Swagger-based API documentation.

## Highlights

- JWT-based authentication with refresh token flow
- User registration, login, profile management, and account deletion
- Book CRUD operations for authenticated users
- Book interactions: like / unlike
- Personal library management with reading status tracking
- Author subscriptions
- Search by filters across books
- Reviews, comments, notifications, chapters, tags, genres, and categories
- Azure Blob Storage integration for media files
- Swagger UI for API exploration
- CORS configuration for frontend integration

## Tech Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10
- PostgreSQL via Npgsql
- JWT Bearer Authentication
- Azure Blob Storage
- Swashbuckle (Swagger)
- SignalR - in proggress for real-time notifications
- ImageSharp

## Architecture

The solution uses a layered backend structure:

- `Controllers` expose HTTP endpoints
- `Services` contain business logic
- `Repositories` handle data access
- `Infrastructure` contains validation and storage integrations
- `Models` define the domain entities
- `DTO` objects are used for request and response contracts

## Main API Areas

- `api/register`, `api/login`, `api/refresh`, `api/logout`
- `api/user`
- `api/book`
- `api/book-interaction`
- `api/library`
- `api/subscriptions`
- `api/search`

## Prerequisites

- .NET 10 SDK
- PostgreSQL
- Azure Storage Emulator / Azurite for local development, or a real Azure Storage account

## Configuration

### `appsettings.json`
Configure the following values:

- `ConnectionStrings:IndPubConnection` - PostgreSQL connection string
- `AppSettings:AccessToken` - JWT signing key
- `AppSettings:Issuer` - JWT issuer
- `AppSettings:Audience` - JWT audience
- `Cors:AllowedOrigins` - frontend origins allowed to call the API

### `appsettings.Development.json`
For local development, Azure Blob Storage is configured with:

- `AzureStorage:ConnectionString = UseDevelopmentStorage=true`
- container and folder names for book covers and profile pictures

## Local Setup

1. Install the .NET 10 SDK.
2. Start PostgreSQL and create the database used by `IndPubConnection`.
3. Start Azurite if you want to use local blob storage.
4. Update configuration values in `appsettings.json` and `appsettings.Development.json` as needed.
5. Run database migrations.
6. Start the API:

```bash
dotnet run
```

7. Open Swagger UI in development mode to test endpoints.

## Why this project is valuable for a resume

IndPubBack shows that the author can build a production-style backend, not just a demo app. It combines authentication, storage, relational data modeling, and structured business logic in a clean layered design. The project is especially useful to show:

- backend API design skills
- work with authentication and authorization
- PostgreSQL and EF Core experience
- cloud storage integration
- maintainable architecture
- practical social/content-platform features

## Notes

- The repository is configured for CORS and JWT-protected endpoints.
- Some endpoints require an authenticated user.
- Swagger is enabled in development.
