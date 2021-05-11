# ====================
# Build image with Sdk
# ====================
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY Budget.Gateway.sln .
COPY Budget.Gateway.Api/. ./Budget.Gateway.Api/

RUN dotnet restore Budget.Gateway.sln

WORKDIR /source/Budget.Gateway.Api
RUN dotnet publish -c release -o /app --no-restore

# ======================================
# Final image with Runtime and Users API
# ======================================
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Budget.Gateway.Api.dll"]

EXPOSE 7000