# Digital Thinkers test excercise

[![.NET](https://github.com/fuszenecker/DigitalThinkers/actions/workflows/dotnet.yml/badge.svg)](https://github.com/fuszenecker/DigitalThinkers/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/fuszenecker/DigitalThinkers/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/fuszenecker/DigitalThinkers/actions/workflows/codeql-analysis.yml)
[![Docker Image CI](https://github.com/fuszenecker/DigitalThinkers/actions/workflows/docker-image.yml/badge.svg)](https://github.com/fuszenecker/DigitalThinkers/actions/workflows/docker-image.yml)

## How to run the solution

You might want to install [.NET Core SDK 5](https://dotnet.microsoft.com/download/dotnet/5.0) first, and `sudo npm install nswag -g` next.

The service can be started

1. "locally" by running (might take a little time to compile the code):

    ```text
    dotnet run --project .\src\Api\DigitalThinkers.csproj
    ```

    or on Linux:

    ```text
    dotnet run --project ./src/Api/DigitalThinkers.csproj
    ```

2. Or you can start the service and run the tests with means of `docker-compose`:

    ```text
    docker-compose up --build
    ```

    Unfortunately, on Linux, the stable `docker-compose` is quite old, so it doesn't support `service_started`:

    ```text
    depends_on:
        api:
            # condition: service_healthy
            condition: service_started
    ```

    So if you want to use `docker-compose`, please use Windows and the latest Docker Desktop.

3. If you press `F5` button in `Visual Studio Code` or `Visual Studio`, the Swagger page will be shown in the browser window soon.

The Swagger page can be accessed through: `http://localhost:5000/swagger`.

There are two implementations:

1. The in-memory store, this is the default, as it is highly tested by unit tests,
2. and the persistent one (SQlite), it can be enabled by editing the `Startup.cs` in `src/Api`.

## Technologies involved

* Dotnet Core 5 WebAPI
* Swagger for interface description and visualization (`/swagger`)
* Docker and docker-compose
* Entity Framework Core with SQLite
* Serilog structured logging (console logger)
* Prometheus.NET for metrics (`/metrics`)
* Correlation ID middleware (`X-Correlation-ID` in header)
