# ===============================
# Build angular frontend with sdk
# ===============================
FROM node:22.2-alpine as uiBuild
RUN npm install -g @angular/cli@17

RUN mkdir -p /app
COPY UI/Angular/. /app
WORKDIR /app/Angular

RUN npm i
RUN ng build --localize

# ===========================
# Build .net backend with Sdk
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS serverBuild
WORKDIR /source

COPY UI/Server/. /source
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

RUN mkdir /app/wwwroot/fr-CA
COPY --from=uiBuild /app/dist/budget/browser/fr-CA/. /app/wwwroot/fr-CA/.
RUN sed -i -e 's/https:\/\/denethor.jogendron.xyz\/en-US/https:\/\/denethor.jogendron.xyz\/fr-CA/g' /app/wwwroot/fr-CA/assets/configuration.json
RUN mkdir /app/wwwroot/en-US
COPY --from=uiBuild /app/dist/budget/browser/en-US/. /app/wwwroot/en-US/.

# ======================================
# Final image with Runtime and Users API
# ======================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=serverBuild /app ./
ENTRYPOINT ["dotnet", "Server.dll"]
EXPOSE 8080