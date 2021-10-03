FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS tests-build
WORKDIR /src
COPY . .
RUN dotnet restore "IntegrationTests.csproj"
RUN dotnet build "IntegrationTests.csproj"
ENTRYPOINT ["dotnet", "test"]
