# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CurlImpersonateApi.csproj", "."]
RUN dotnet restore "./CurlImpersonateApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./CurlImpersonateApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN curl -L -o curl-impersonate.tar.gz https://github.com/lwthiker/curl-impersonate/releases/download/v0.6.1/curl-impersonate-v0.6.1.x86_64-linux-gnu.tar.gz

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CurlImpersonateApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/curl-impersonate.tar.gz /tmp/curl-impersonate.tar.gz
USER root
RUN apt-get update && \
    apt-get install -y libnss3 nss-plugin-pem ca-certificates && \
    rm -rf /var/lib/apt/lists/*
RUN tar -xzvf /tmp/curl-impersonate.tar.gz -C /usr/local/bin && \
    rm /tmp/curl-impersonate.tar.gz
USER app
ENTRYPOINT ["dotnet", "CurlImpersonateApi.dll"]