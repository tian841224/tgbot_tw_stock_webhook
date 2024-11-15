#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
    chromium \
    chromium-driver
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TGBot_TW_Stock_Webhook.csproj", "."]
RUN dotnet restore "./TGBot_TW_Stock_Webhook.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./TGBot_TW_Stock_Webhook.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TGBot_TW_Stock_Webhook.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGBot_TW_Stock_Webhook.dll"]