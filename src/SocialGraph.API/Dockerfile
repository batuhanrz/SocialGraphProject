# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY src/SocialGraph.API/*.csproj ./src/SocialGraph.API/
RUN dotnet restore ./src/SocialGraph.API/SocialGraph.API.csproj

# Copy everything else and build
COPY src/SocialGraph.API/ ./src/SocialGraph.API/
RUN dotnet publish ./src/SocialGraph.API/SocialGraph.API.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=build /app/out .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "SocialGraph.API.dll"]
