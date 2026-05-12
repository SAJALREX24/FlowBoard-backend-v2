# Build 2026-05-13-00-49
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy the entire solution
COPY . .

# Publish Gateway
RUN dotnet publish gateway/FlowBoard.Gateway/FlowBoard.Gateway.csproj -c Release -o /app/gateway

# Publish Services
RUN dotnet publish services/FlowBoard.Auth/FlowBoard.Auth.csproj -c Release -o /app/auth
RUN dotnet publish services/FlowBoard.Workspace/FlowBoard.Workspace.csproj -c Release -o /app/workspace
RUN dotnet publish services/FlowBoard.Board/FlowBoard.Board.csproj -c Release -o /app/board
RUN dotnet publish services/FlowBoard.List/FlowBoard.List.csproj -c Release -o /app/list
RUN dotnet publish services/FlowBoard.Card/FlowBoard.Card.csproj -c Release -o /app/card
RUN dotnet publish services/FlowBoard.Comment/FlowBoard.Comment.csproj -c Release -o /app/comment
RUN dotnet publish services/FlowBoard.Label/FlowBoard.Label.csproj -c Release -o /app/label

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app

# Install supervisor to manage multiple processes
RUN apk add --no-cache supervisor

# Copy published outputs from build stage
COPY --from=build /app/gateway /app/gateway
COPY --from=build /app/auth /app/auth
COPY --from=build /app/workspace /app/workspace
COPY --from=build /app/board /app/board
COPY --from=build /app/list /app/list
COPY --from=build /app/card /app/card
COPY --from=build /app/comment /app/comment
COPY --from=build /app/label /app/label

# Copy supervisord configuration
COPY supervisord.conf /etc/supervisor/conf.d/supervisord.conf

# Expose the API Gateway port
EXPOSE 10000

# Environment variables for Production
ENV ASPNETCORE_ENVIRONMENT=Production

# CRITICAL MEMORY OPTIMIZATIONS FOR 512MB CONTAINER
# Force Workstation GC (saves massive memory at the cost of some throughput)
ENV DOTNET_gcServer=0
# Tell .NET to aggressively release memory back to the OS
ENV DOTNET_System_GC_RetainVM=0


# Start Supervisor
CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/conf.d/supervisord.conf"]

