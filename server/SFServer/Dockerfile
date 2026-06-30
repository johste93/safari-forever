FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SFServer.dll"]

COPY wwwroot/ /app/wwwroot
COPY Properties/ /app/Properties

ENV ASPNETCORE_URLS http://*:80

# Expose port 80 for the application.
EXPOSE 80

ADD ca-certificate.crt /usr/local/share/ca-certificates/ca-certificate.crt
RUN chmod 644 /usr/local/share/ca-certificates/ca-certificate.crt && update-ca-certificates
