# Waslah API

Waslah is a transportation and route-planning backend for connecting passengers with stations, route segments, chained routes, agency trips, and ride requests. The project provides a secured ASP.NET Core API, administration capabilities, background job processing, email-based account workflows, and a separate ML.NET taxi-fare prediction service.

## Main capabilities

- User registration, email confirmation, login, JWT access tokens, refresh tokens, logout, and password recovery.
- Profile management, password changes, and previous ride history.
- Station management with city/location data and nearest-station lookup from coordinates.
- Route segment management and activation/deactivation.
- Location-based route planning using nearby stations and chained routes.
- Agency trip management, including trip photos and trip status.
- Authenticated ride-request creation and order history.
- Role and permission management with policy-based authorization.
- User administration, account status changes, and account unlocking.
- SQL Server persistence through Entity Framework Core and ASP.NET Core Identity.
- FluentValidation request validation and Mapster contract mapping.
- Structured error responses through a global exception handler and problem details.
- Serilog request logging.
- Hangfire background jobs with a protected dashboard.
- OpenAPI/Swagger documentation in the development environment.
- Standalone ML.NET taxi-fare prediction endpoint.

## Solution structure

| Project                      | Purpose                                                                   | Target framework |
| ---------------------------- | ------------------------------------------------------------------------- | ---------------- |
| `Waslah`                     | Main route-planning, authentication, administration, and ride-request API | .NET 9           |
| `TaxiFarePrediction_WebApi1` | Standalone ML.NET prediction API exposing `POST /predict`                 | .NET 8           |
| `AILayer`                    | Supporting AI/model integration library and serialized model assets       | .NET 9           |

The Visual Studio solution currently builds the `Waslah` API project. The taxi prediction project can be run independently when its model endpoint is needed.

## Architecture

The main API follows a layered structure:

- **Controllers** expose HTTP endpoints and translate service results into HTTP responses.
- **Contracts** contain request and response models plus FluentValidation validators.
- **Services** contain authentication, station, route, agency-trip, order, role, and user workflows.
- **Persistence** contains the EF Core `ApplicationDbContext`, entity configuration, and migrations.
- **Entities** represent users, roles, stations, routes, trips, orders, refresh tokens, and related domain data.
- **Authentication** provides JWT token generation and custom permission policies.
- **Middlewares and errors** handle authorization checks and consistent exception responses.

## API endpoint overview

The API uses JWT bearer authentication for protected operations. Permission-protected endpoints require the corresponding permission claim in addition to authentication.

### Authentication

| Method | Endpoint                          | Description                       |
| ------ | --------------------------------- | --------------------------------- |
| `POST` | `/Auth/registration`              | Register a user                   |
| `POST` | `/Auth/confirm-email`             | Confirm an email address          |
| `POST` | `/Auth/resend-confirmation-email` | Resend an email confirmation code |
| `POST` | `/Auth/login`                     | Authenticate and issue tokens     |
| `POST` | `/Auth/new-refresh`               | Refresh an access token           |
| `POST` | `/Auth/revoke-refresh-token`      | Revoke a refresh token            |
| `POST` | `/Auth/forget-password`           | Request a password reset code     |
| `POST` | `/Auth/reset-password`            | Reset a password                  |

### Passenger account and rides

| Method | Endpoint              | Description                           |
| ------ | --------------------- | ------------------------------------- |
| `GET`  | `/me`                 | Get the current user's profile        |
| `PUT`  | `/me`                 | Update the current user's profile     |
| `PUT`  | `/me/change-password` | Change the current user's password    |
| `GET`  | `/me/pervious-rides`  | Get the current user's previous rides |
| `POST` | `/api/RideRequests`   | Create a ride request                 |

### Stations and routes

| Method | Endpoint                                | Description                          |
| ------ | --------------------------------------- | ------------------------------------ |
| `GET`  | `/api/Stations/info`                    | Get station information              |
| `GET`  | `/api/Stations/All`                     | List stations                        |
| `GET`  | `/api/Stations/{locationId}`            | Get a station by location ID         |
| `POST` | `/api/Stations/Nearest`                 | Find stations near a coordinate      |
| `POST` | `/api/Stations`                         | Create a station                     |
| `PUT`  | `/api/Stations/{id}`                    | Update a station                     |
| `PUT`  | `/api/Stations/{id}/toggle-status`      | Enable or disable a station          |
| `GET`  | `/api/route/segment`                    | List route segments                  |
| `GET`  | `/api/route/segment/{id}`               | Get a route segment                  |
| `POST` | `/api/route/segment`                    | Create a route segment               |
| `PUT`  | `/api/route/segment/{id}`               | Update a route segment               |
| `PUT`  | `/api/route/segment/{id}/toggle-status` | Enable or disable a route segment    |
| `GET`  | `/api/route/plan/All`                   | List chained routes                  |
| `POST` | `/api/route/plan/ByLocation`            | Plan a route between two locations   |
| `GET`  | `/api/route/plan/{id}/Current-segments` | Get the current segments for a route |

### Administration and agencies

| Method | Endpoint                              | Description                      |
| ------ | ------------------------------------- | -------------------------------- |
| `GET`  | `/api/AgencyTrips`                    | List agency trips                |
| `GET`  | `/api/AgencyTrips/{id}`               | Get an agency trip               |
| `POST` | `/api/AgencyTrips`                    | Create an agency trip            |
| `PUT`  | `/api/AgencyTrips/{id}`               | Update an agency trip            |
| `PUT`  | `/api/AgencyTrips/{id}/photo`         | Update an agency trip photo      |
| `PUT`  | `/api/AgencyTrips/{id}/toggle-status` | Enable or disable an agency trip |
| `GET`  | `/api/Users`                          | List users                       |
| `GET`  | `/api/Users/{id}`                     | Get user details                 |
| `POST` | `/api/Users`                          | Create a user                    |
| `PUT`  | `/api/Users/{id}`                     | Update a user                    |
| `PUT`  | `/api/Users/{id}/toggle-status`       | Enable or disable a user         |
| `PUT`  | `/api/Users/{id}/unlock`              | Unlock a user                    |
| `GET`  | `/api/Roles`                          | List roles                       |
| `GET`  | `/api/Roles/{id}`                     | Get role details                 |
| `POST` | `/api/Roles`                          | Create a role                    |
| `PUT`  | `/api/Roles/{id}`                     | Update a role                    |
| `PUT`  | `/api/Roles/{id}/toggle-status`       | Enable or disable a role         |

## Prerequisites

- Windows, Linux, or macOS
- .NET 9 SDK for the main API
- .NET 8 SDK if running `TaxiFarePrediction_WebApi1`
- SQL Server or SQL Server Express
- A configured SMTP provider for email confirmation and password recovery
- Optional: the .NET Entity Framework CLI for database migrations

Verify the SDK installation with:

```bash
dotnet --list-sdks
```

## Configuration

Do not use the committed development credentials in a shared or production environment. Configure secrets with user secrets, environment variables, or a deployment secret store.

The main API expects these settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<SQL Server connection string>",
    "HangfireConnection": "<Hangfire SQL Server connection string>"
  },
  "Jwt": {
    "Key": "<long random signing key>",
    "Issuer": "Waslah",
    "Audience": "WaslahUsers",
    "ExpiresIn": 30
  },
  "MailSettings": {
    "Mail": "<SMTP username>",
    "DisplayName": "Waslah",
    "Password": "<SMTP password>",
    "Host": "<SMTP host>",
    "Port": 587
  },
  "HangFireSettings": {
    "Username": "<dashboard username>",
    "password": "<dashboard password>"
  }
}
```

The API enables permissive CORS through the `AllowAll` policy for the current development setup. Restrict allowed origins before deploying publicly.

## Database setup

From the repository root, restore dependencies and apply the existing migrations:

```bash
dotnet restore WaslahApi.sln
dotnet tool install --global dotnet-ef
dotnet ef database update --project Waslah/Waslah.csproj
```

The persistence layer includes migrations for the application schema, Identity users and roles, route and station data, agency trips, and related domain changes. The project also contains seeded Identity configuration for the default roles and administrator account; replace development credentials before using the system outside a local environment.

## Running the main API

```bash
dotnet run --project Waslah/Waslah.csproj
```

When running in the Development environment, OpenAPI is available at `/openapi/v1.json` and the Swagger UI is available at `/swagger`. The Hangfire dashboard is available at `/jobs` and is protected by the configured basic-authentication credentials.

Use the HTTPS URL printed by `dotnet run` when calling the API. Authenticate through `/Auth/login`, then send the returned access token as:

```http
Authorization: Bearer <access-token>
```

## Running the taxi-fare prediction API

The prediction service is separate from the main `Waslah` API and loads `TaxiFarePrediction.mlnet` at startup.

```bash
dotnet run --project TaxiFarePrediction_WebApi1/TaxiFarePrediction_WebApi1.csproj
```

Prediction requests are sent to:

```http
POST /predict
Content-Type: application/json

{
  "vendor_id": "<vendor>",
  "rate_code": 1,
  "passenger_count": 1,
  "trip_distance": 5.2,
  "payment_type": "<payment-type>",
  "fare_amount": 0
}
```

The response contains the ML.NET model output, including the predicted `Score`. The training source path in the generated training file is machine-specific; retraining requires updating it to a valid local dataset path.

## Validation and error handling

Requests are validated with FluentValidation. Successful operations use standard HTTP responses such as `200 OK`, `201 Created`, and `204 No Content`. Domain and validation failures are returned as problem-details responses through the shared result/error handling abstractions. Unexpected exceptions are logged and processed by the global exception handler.

## Development notes

- Keep database credentials, JWT keys, SMTP passwords, and Hangfire dashboard credentials out of source control.
- Limit CORS origins and use HTTPS outside local development.
- Protect and monitor the Hangfire dashboard in deployed environments.
- Keep the ML.NET model file beside the prediction service output so it can be loaded at startup.
- Add automated integration tests around authentication, permission policies, route planning, and ride creation before production deployment.

## License

No license file is currently included in the repository. Add a license before distributing the project publicly.
