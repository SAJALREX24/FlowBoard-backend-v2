# FlowBoard Backend — Deployment Preparation (Ready to Run)

You are preparing the FlowBoard ASP.NET Core 8 microservices backend for deployment on Render.
The project runs all 8 processes (1 YARP gateway + 7 services) inside ONE Docker container managed by supervisord.
All packages are already in the .csproj files. All migrations exist. Nothing needs to be installed manually.

Execute every task in order. Do not skip any task.

---

## TASK 1 — Remove test_auth_publish folder from git tracking

This folder contains compiled binaries that should never be in version control.

Run:
```
git rm -r --cached test_auth_publish/
```

Then open .gitignore (it exists at the repo root) and add this line at the very bottom:
```
test_auth_publish/
```

Save the .gitignore file.

---

## TASK 2 — Fix connection strings in all 7 appsettings.json files

Replace the existing SQL Server connection strings with the real Neon PostgreSQL URLs.
Edit each file and replace ONLY the connection string value. Keep everything else exactly as-is.
Make sure each file remains valid JSON after editing.

FILE: services/FlowBoard.Auth/appsettings.json
Set "AuthDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_auth?sslmode=require&channel_binding=require

FILE: services/FlowBoard.Workspace/appsettings.json
Set "WorkspaceDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_workspace?sslmode=require&channel_binding=require

FILE: services/FlowBoard.Board/appsettings.json
Set "BoardDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_board?sslmode=require&channel_binding=require

FILE: services/FlowBoard.List/appsettings.json
Set "ListDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_list?sslmode=require&channel_binding=require

FILE: services/FlowBoard.Card/appsettings.json
Set "CardDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_card?sslmode=require&channel_binding=require

FILE: services/FlowBoard.Comment/appsettings.json
Set "CommentDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_comment?sslmode=require&channel_binding=require

FILE: services/FlowBoard.Label/appsettings.json
Set "LabelDb" value to:
postgresql://neondb_owner:npg_9xjrIQuWCS2t@ep-cool-sky-apuol1yk-pooler.c-7.us-east-1.aws.neon.tech/flowboard_label?sslmode=require&channel_binding=require

After editing all 7 files, verify each is valid JSON (check opening and closing braces are intact).

---

## TASK 3 — Sanitize JWT key

The JWT secret is currently hardcoded in plain text in two files on a public GitHub repo.
Replace it with a placeholder — the real key will be set as a Render environment variable.

FILE: gateway/FlowBoard.Gateway/appsettings.json
Find the "Key" field inside the "Jwt" section.
Change its value to exactly: SET_VIA_RENDER_ENVIRONMENT_VARIABLE

FILE: services/FlowBoard.Auth/appsettings.json
Find the "Key" field inside the "Jwt" section.
Change its value to exactly: SET_VIA_RENDER_ENVIRONMENT_VARIABLE

Do not touch "Issuer" or "Audience" — only "Key".

---

## TASK 4 — Remove UseHttpsRedirection from all 7 service Program.cs files

Inside a Docker container on Render, HTTPS termination happens at Render's edge not inside the container.
This line causes problems in containerized environments.

Find and DELETE the line app.UseHttpsRedirection(); from each of these files:
- services/FlowBoard.Auth/Program.cs
- services/FlowBoard.Workspace/Program.cs
- services/FlowBoard.Board/Program.cs
- services/FlowBoard.List/Program.cs
- services/FlowBoard.Card/Program.cs
- services/FlowBoard.Comment/Program.cs
- services/FlowBoard.Label/Program.cs

Do NOT touch gateway/FlowBoard.Gateway/Program.cs — the gateway does not have this line.

---

## TASK 5 — Verify Dockerfile

Read the Dockerfile and verify:

1. Final stage has EXPOSE 8080 — if it says 5011 change it to 8080
2. Build stage publishes all 8 projects (gateway + 7 services)
3. Runtime stage copies all 8 published outputs
4. supervisord is installed: RUN apk add --no-cache supervisor
5. CMD starts supervisord: CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]

Fix anything wrong. Confirm if all correct.

---

## TASK 6 — Verify supervisord.conf

Read supervisord.conf and verify:

1. Gateway listens on 0.0.0.0:8080 (NOT 5011)
   Expected: command=dotnet FlowBoard.Gateway.dll --urls "http://0.0.0.0:8080"

2. All 7 services have staggered startup:
   - auth: no sleep (starts immediately)
   - workspace: sleep 5
   - board: sleep 10
   - list: sleep 15
   - card: sleep 20
   - comment: sleep 25
   - label: sleep 30

3. All services bind to 0.0.0.0 in their --urls flag

Fix anything wrong. Confirm if all correct.

---

## TASK 7 — Add README.md to backend repo root

Create file README.md at the repo root with this content:

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

---

## TASK 8 — Build verification

Run:
```
dotnet build FlowBoard.sln
```

If build succeeds with 0 errors, proceed to Task 9.
If there are errors, fix them and rebuild before proceeding.
Report: error count, warning count, build time.

---

## TASK 9 — Commit and push

```
git add .
git status
git commit -m "Deploy: Neon PostgreSQL connection strings, sanitize JWT key, remove HTTPS redirect, remove test binaries, add README"
git push
```

Report:
1. All files changed (from git status output)
2. Confirmation that git push succeeded
3. The commit hash

---

## Final summary

After all tasks complete write a summary with:

1. test_auth_publish removed from git tracking: YES/NO
2. All 7 connection strings updated with Neon URLs: YES/NO — list each file
3. JWT key sanitized in gateway and auth: YES/NO
4. UseHttpsRedirection removed from all 7 services: YES/NO
5. Dockerfile issues found and fixed: list what changed or "all correct"
6. supervisord.conf issues found and fixed: list what changed or "all correct"
7. README.md created: YES/NO
8. Build result: X errors, Y warnings
9. Git push: commit hash and confirmation

End with:
"Deploy prep complete. Next steps: (1) Go to render.com, New Web Service, connect SAJALREX24/FlowBoard-backend, Runtime: Docker, Port: 8080. (2) Add environment variables: Jwt__Key, Jwt__Issuer=FlowBoard.Auth, Jwt__Audience=FlowBoard.Clients, ASPNETCORE_ENVIRONMENT=Production. (3) Click Deploy. (4) Wait 10 minutes for first build. (5) Test with POST https://your-render-url.onrender.com/api/auth/login"
