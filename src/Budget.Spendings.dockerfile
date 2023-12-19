# ====================
# Build image with Sdk
# ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

COPY Budget.Spendings.sln .
COPY Shared/. ./Shared
COPY Spendings/. ./Spendings

RUN dotnet restore Budget.Spendings.sln

WORKDIR /source/Spendings/Budget.Spendings.Api
RUN dotnet publish -c release -o /app --no-restore

# ======================================
# Final image with Runtime and Users API
# ======================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Budget.Spendings.Api.dll"]

EXPOSE 7000