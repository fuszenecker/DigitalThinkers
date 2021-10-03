FROM mcr.microsoft.com/dotnet/sdk:6.0 AS tests-build
WORKDIR /src
COPY . .
RUN dotnet restore IntegrationTests.csproj
RUN dotnet build IntegrationTests.csproj
ENTRYPOINT ["dotnet", "test"]
