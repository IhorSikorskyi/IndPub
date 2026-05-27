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
- SignalR - in progress for real-time notifications
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
5. Run database migrations:

```bash
dotnet ef database update
```

6. Start the API:

```bash
dotnet run
```

7. Open Swagger UI in development mode to test endpoints.

## Docker Setup

You can also run the project using Docker Compose without installing PostgreSQL or .NET SDK locally.

1. Make sure Docker and Docker Compose are installed.
2. Update configuration values in `appsettings.json` as needed.
3. Start all services:

```bash
docker-compose up
```

4. The API will be available at `http://localhost:5000` for HTTP and `https://localhost:5001` for HTTPS.
5. Azurite (Azure Storage Emulator) is included in the Docker Compose configuration for local blob storage.

For development with hot reload, use:
```bash
docker-compose -f docker-compose.yaml -f docker-compose.override.yaml up
```
