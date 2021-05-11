# ====================
# Build image with Sdk
# ====================
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY Budget.Users.Deploy.sln .
COPY Budget.Cqrs/. ./Budget.Cqrs/
COPY Budget.EventSourcing/. ./Budget.EventSourcing/
COPY Budget.Users.Api/. ./Budget.Users.Api/
COPY Budget.Users.Application/. ./Budget.Users.Application/
COPY Budget.Users.Domain/. ./Budget.Users.Domain/
COPY Budget.Users.InMemoryAdapters/. ./Budget.Users.InMemoryAdapters/
COPY Budget.Users.KafkaAdapters/. ./Budget.Users.KafkaAdapters/
COPY Budget.Users.MongoDbAdapters/. ./Budget.Users.MongoDbAdapters/
COPY Budget.Users.PostgresAdapters/. ./Budget.Users.PostgresAdapters/

RUN dotnet restore Budget.Users.Deploy.sln

WORKDIR /source/Budget.Users.Api
RUN dotnet publish -c release -o /app --no-restore

# ======================================
# Final image with Runtime and Users API
# ======================================
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Budget.Users.Api.dll"]

RUN apt-get update
RUN apt-get -y install librdkafka1

EXPOSE 5000