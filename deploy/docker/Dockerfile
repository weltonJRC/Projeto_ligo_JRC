FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /app

COPY src/Jrc.LigoCampaignGateway.Domain/*.csproj src/Jrc.LigoCampaignGateway.Domain/
COPY src/Jrc.LigoCampaignGateway.Application/*.csproj src/Jrc.LigoCampaignGateway.Application/
COPY src/Jrc.LigoCampaignGateway.Infrastructure/*.csproj src/Jrc.LigoCampaignGateway.Infrastructure/
COPY src/Jrc.LigoCampaignGateway.Api/*.csproj src/Jrc.LigoCampaignGateway.Api/

RUN dotnet restore src/Jrc.LigoCampaignGateway.Api/Jrc.LigoCampaignGateway.Api.csproj

COPY src/ ./src/
RUN dotnet publish src/Jrc.LigoCampaignGateway.Api/Jrc.LigoCampaignGateway.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Jrc.LigoCampaignGateway.Api.dll"]
