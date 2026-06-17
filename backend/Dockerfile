FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["EncurtadorUrl.sln", "./"]
COPY ["EncurtadorURL.Api/EncurtadorURL.Api.csproj", "EncurtadorURL.Api/"]
COPY ["EncurtadorURL.Application/EncurtadorURL.Application.csproj", "EncurtadorURL.Application/"]
COPY ["EncurtadorURL.Domain/EncurtadorURL.Domain.csproj", "EncurtadorURL.Domain/"]
COPY ["EncurtadorURL.Infrastructure/EncurtadorURL.Infrastructure.csproj", "EncurtadorURL.Infrastructure/"]

RUN dotnet restore "EncurtadorURL.Api/EncurtadorURL.Api.csproj"

COPY . .
WORKDIR /src/EncurtadorURL.Api
RUN dotnet publish "EncurtadorURL.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Install curl for Docker health checks
RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*

ARG ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_ENVIRONMENT=${ENVIRONMENT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "EncurtadorURL.Api.dll"]
