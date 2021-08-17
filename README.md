# Digital Thinkers test excercise

The service can be started

1. "locally" by running:

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

    So if you want tu use `docker-compose`, please use Windows and Docker Desktop.

3. If you press `F5` in Visual Studio Code or Visual Studio, the Swagger page will be shown in the browser window.

The Swagger page can be accessed through: `http://localhost:5000/swagger`.
