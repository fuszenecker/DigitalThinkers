FROM mcr.microsoft.com/dotnet/aspnet:5.0-focal AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build
WORKDIR /src
COPY ["api/DigitalThinkers.csproj", "api/"]
COPY ["domain/domain.csproj", "domain/"]
COPY ["dataaccess/dataaccess.csproj", "dataaccess/"]
RUN dotnet restore "api/DigitalThinkers.csproj"
COPY . .
RUN dotnet build "api/DigitalThinkers.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api/DigitalThinkers.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
HEALTHCHECK --interval=10s --timeout=5s --start-period=5s --retries=30 CMD curl http://localhost:5000/health
ENTRYPOINT ["dotnet", "DigitalThinkers.dll"]
