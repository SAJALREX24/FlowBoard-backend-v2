# FlowBoard Backend

ASP.NET Core 8 microservices backend for FlowBoard — a Trello-style task management platform built for the BridgeLabz × Capgemini .NET Full Stack Fellowship.

## Architecture

8 processes run inside a single Docker container managed by supervisord:

| Service | Port | Responsibility |
|---|---|---|
| Gateway (YARP) | 8080 | Public entry point, JWT validation, reverse proxy |
| Auth | 5048 | Registration, login, JWT generation, BCrypt hashing |
| Workspace | 5277 | Workspace CRUD, workspace membership and roles |
| Board | 5030 | Board CRUD, board membership and roles |
| List | 5266 | List/column CRUD, drag-drop positioning, archiving |
| Card | 5024 | Card CRUD, fractional positioning, archiving |
| Comment | 5198 | Threaded comments, soft delete |
| Label | 5027 | Labels, checklist items |

All traffic enters through the gateway on port 8080. The gateway validates JWT tokens and routes requests to the appropriate microservice via YARP reverse proxy. Services communicate internally via 127.0.0.1 — they are never directly exposed to the internet.

## Tech Stack

- **Framework**: ASP.NET Core 8 Web API
- **ORM**: Entity Framework Core 8 with Npgsql (PostgreSQL)
- **Auth**: JWT Bearer (HMAC-SHA256), BCrypt.Net password hashing (workFactor 12)
- **Gateway**: YARP Reverse Proxy 2.2.0
- **Database**: PostgreSQL via Neon (one database per service — 7 total)
- **Container**: Docker + Supervisord (single container, 8 processes)
- **API Docs**: Swagger/OpenAPI on each service
- **Rate Limiting**: ASP.NET Core built-in rate limiter on auth endpoints (10 req/min)

## Architecture Decisions

**Database-per-service**: Each microservice owns its schema independently. No cross-service foreign keys — only logical references via integer IDs. This enforces bounded context isolation and allows each service to evolve its schema independently.

**JWT at gateway only**: Services trust forwarded requests. JWT validation happens once at the edge, reducing redundancy and keeping individual services stateless.

**Fractional positioning**: Cards and lists use floating-point position values with midpoint calculation for drag-drop ordering (e.g., inserting between position 100 and 200 gives 150). This avoids batch position updates on every reorder operation.

**Synchronous REST for service-to-service**: Direct HTTP via 127.0.0.1 inside the container. Appropriate for MVP scope. RabbitMQ/MassTransit would be added in v2 for async notification dispatch.

**Single container deployment**: All 8 processes run in one Docker container managed by supervisord with staggered startup delays. This fits free-tier hosting constraints (512MB RAM) while maintaining microservices architecture at the code level.

## Running Locally

Prerequisites: .NET 8 SDK, PostgreSQL (or use the Neon connection strings), 8 terminal tabs.

```bash
# Start services first (any order)
cd services/FlowBoard.Auth && dotnet run        # port 5048
cd services/FlowBoard.Workspace && dotnet run   # port 5277
cd services/FlowBoard.Board && dotnet run       # port 5030
cd services/FlowBoard.List && dotnet run        # port 5266
cd services/FlowBoard.Card && dotnet run        # port 5024
cd services/FlowBoard.Comment && dotnet run     # port 5198
cd services/FlowBoard.Label && dotnet run       # port 5027

# Start gateway last
cd gateway/FlowBoard.Gateway && dotnet run      # port 5011 (local) / 8080 (production)
```

All API calls go through the gateway: `http://localhost:5011/api/...`

## Deployment (Render)

Deployed as a single Render Web Service using Docker runtime.
All 8 processes start automatically via supervisord.

Required Render environment variables:
```
Jwt__Key        = <your-jwt-secret>
Jwt__Issuer     = FlowBoard.Auth
Jwt__Audience   = FlowBoard.Clients
ASPNETCORE_ENVIRONMENT = Production
```

## Frontend

Angular 18 frontend repo: https://github.com/SAJALREX24/FlowBoard-frontend

## v2 Roadmap

- Real-time updates via ASP.NET Core SignalR
- Async notification dispatch via RabbitMQ + MassTransit
- File attachments via Azure Blob Storage
- Full-text card search via Elasticsearch
- Polly circuit breaker for service resilience
- Docker Compose for local multi-service orchestration
