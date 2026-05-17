# BoxInventory

Inventory management API with .NET, MongoDB, and Keycloak SSO.

## Requirements

- .NET 10 SDK
- Docker (for MongoDB)

## Quick Start (local)

```bash
# Start MongoDB
docker compose up -d

# Run the API
dotnet run --project src/BoxInventory.Api
```

The API starts at `http://localhost:5000`. Swagger UI at `/swagger`.

## Deploy with Docker

```bash
# Configure environment
cp .env.example .env
# Edit .env with your values

# Build and run everything
docker compose -f docker-compose.deploy.yml up -d
```

## Configuration

All settings via environment variables or `appsettings.json`:

| Variable | Default | Description |
|---|---|---|
| `MongoDb__ConnectionString` | `mongodb://localhost:27017` | MongoDB connection string |
| `MongoDb__DatabaseName` | `box_inventory` | Database name |
| `Jwt__Authority` | `https://sso.allue.eu/realms/master` | Keycloak realm URL |
| `Jwt__Audience` | `account` | JWT audience |
| `Cors__AllowedOrigins__0` | `*` | Allowed CORS origin |
