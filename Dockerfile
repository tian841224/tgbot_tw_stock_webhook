FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
# 更新並安裝必要的依賴
RUN apt-get update && apt-get install -y \
    wget \
    gnupg \
    curl \
    unzip
# 直接下載並安裝 ChromeDriver 和 Chromium
RUN curl -sS -o - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google-chrome.list \
    && apt-get update \
    && apt-get install -y google-chrome-stable \
    && CHROME_DRIVER_VERSION=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE) \
    && wget -N https://chromedriver.storage.googleapis.com/${CHROME_DRIVER_VERSION}/chromedriver_linux64.zip -P ~/tmp \
    && unzip ~/tmp/chromedriver_linux64.zip -d ~/tmp \
    && mv ~/tmp/chromedriver /usr/local/bin/chromedriver \
    && chmod +x /usr/local/bin/chromedriver
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TGBot_TW_Stock_Webhook.csproj", "./"]
RUN dotnet restore "./TGBot_TW_Stock_Webhook.csproj"
COPY . .
RUN dotnet build "TGBot_TW_Stock_Webhook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TGBot_TW_Stock_Webhook.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGBot_TW_Stock_Webhook.dll"]