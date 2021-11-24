# ----- Base image -----

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# ----- Build image -----

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnet-build
WORKDIR /src
COPY ["Api/ServiceTemplate.csproj", "Api/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["DataAccess/DataAccess.csproj", "DataAccess/"]
COPY ["UnitTests/UnitTests.csproj", "UnitTests/"]
RUN dotnet restore "Api/ServiceTemplate.csproj"
RUN dotnet restore "UnitTests/UnitTests.csproj"
COPY . .
RUN dotnet build "Api/ServiceTemplate.csproj" -c Release -o /app/build
RUN dotnet test "UnitTests/UnitTests.csproj" -c Release
RUN dotnet publish "Api/ServiceTemplate.csproj" -c Release -o /app/publish

# ----- Final image -----

FROM base AS final
WORKDIR /app
COPY --from=dotnet-build /app/publish .
ENTRYPOINT ["dotnet", "ServiceTemplate.dll"]
