# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY src/SocialGraph.AI/*.csproj ./src/SocialGraph.AI/
RUN dotnet restore ./src/SocialGraph.AI/SocialGraph.AI.csproj

# Copy everything else and build
COPY src/SocialGraph.AI/ ./src/SocialGraph.AI/
RUN dotnet publish ./src/SocialGraph.AI/SocialGraph.AI.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "SocialGraph.AI.dll"]
