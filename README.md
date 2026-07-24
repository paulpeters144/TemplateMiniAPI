# TemplateMiniAPI

A production-grade ASP.NET Core Minimal API template built as an educational project. It demonstrates a clean, feature-folder architecture designed for AWS Lambda deployment.

## Tech Stack

- **.NET 8.0** with C# and Minimal APIs
- **ASP.NET Core** (no controllers, no Startup.cs)
- **AWS Lambda** via `Amazon.Lambda.AspNetCoreServer.Hosting`
- **JWT Authentication** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **Swagger/OpenAPI** (`Swashbuckle.AspNetCore`)
- **FluentValidation** for request validation
- **Dependency Injection** with auto-discovery patterns

## Project Structure

```
TemplateMiniAPI/
├── Program.cs                          ← Composition root (~20 lines)
├── appsettings.json                    ← JWT config lives here
│
├── Configuration/
│   └── JwtSettings.cs                  ← Strongly-typed options class
│
├── Common/
│   ├── IEndpoint.cs                    ← Feature contract interface
│   ├── Filters/
│   │   └── ValidationFilter.cs         ← Generic validation filter
│   └── Responses/
│       └── BasicResponse.cs            ← Shared response DTOs
│
├── Extensions/
│   ├── ServiceCollectionExtensions.cs  ← DI registration (auth, JWT, Swagger)
│   └── EndpointExtensions.cs           ← Reflection-based auto-discovery
│
├── Features/
│   ├── Auth/
│   │   ├── AuthEndpoints.cs            ← POST /api/auth/login, GET /api/auth/verify
│   │   ├── Contracts/
│   │   │   ├── LoginRequest.cs
│   │   │   └── AccessTokenResponse.cs
│   │   └── Validators/
│   │       └── LoginRequestValidator.cs
│   └── Weather/
│       ├── WeatherEndpoints.cs         ← GET /api/weather
│       └── WeatherForecast.cs
│
└── Services/
    ├── ITokenService.cs                ← Token generation contract
    └── TokenService.cs                 ← JWT implementation
```

## Key Concepts

### Feature-Folder Architecture

Each feature is a self-contained vertical slice. To add a new feature, create a folder under `Features/` and implement `IEndpoint`:

```csharp
public class ProductsEndpoints : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", () => TypedResults.Ok(new[] { "Widget", "Gadget" }))
             .AllowAnonymous();
    }
}
```

That's it. No other files need to change. The auto-discovery system finds and registers all `IEndpoint` implementations automatically via reflection.

### Auto-Discovery

`EndpointExtensions.MapEndpoints()` scans the assembly at startup and calls `MapEndpoint()` on every `IEndpoint` implementation. No manual registration needed.

### Generic Validation

`ValidationFilter<T>` is a reusable `IEndpointFilter` that injects a FluentValidation `IValidator<T>` and returns a standard `ProblemDetails` 400 response on validation failure. Attach it to any endpoint:

```csharp
group.MapPost("/create", CreateWidget)
     .AddEndpointFilter<ValidationFilter<CreateWidgetRequest>>();
```

Validators auto-register via `AddValidatorsFromAssemblyContaining<Program>()`.

### JWT Authentication

- Settings bound from `appsettings.json` under the `"Jwt"` section (override via env vars in Lambda)
- `ITokenService` / `TokenService` handles token generation
- Swagger pre-configured with Bearer token support
- `.RequireAuthorization()` on any endpoint that needs auth

## Endpoints

| Method | Route             | Auth     | Description             |
|--------|-------------------|----------|-------------------------|
| GET    | /api/weather      | None     | Returns 5-day forecast  |
| POST   | /api/auth/login   | None     | Returns JWT token       |
| GET    | /api/auth/verify  | Required | Validates token         |

Swagger UI available at `/swagger` in non-production environments.

## Getting Started

```bash
# Clone the repository
git clone https://github.com/paulpeters144/TemplateMiniAPI.git
cd TemplateMiniAPI

# Run the API
cd TemplateMiniAPI
dotnet run
```

The API starts on `http://localhost:5162` (configured in `launchSettings.json`). Swagger opens automatically in your browser.

## Configuration

JWT settings in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your_secret_key_here_what_in_the_world",
    "Issuer": "TemplateMiniAPI",
    "Audience": "TemplateMiniAPI",
    "ExpiryHours": 10
  }
}
```

Override any value via environment variables for production:

```bash
export Jwt__Key="your-production-secret"
```

## AWS Lambda

The project includes `Amazon.Lambda.AspNetCoreServer.Hosting` for seamless Lambda deployment. Run `dotnet publish` and deploy the resulting artifact to Lambda. The `AddAWSLambdaHosting(LambdaEventSource.RestApi)` call in `ServiceCollectionExtensions.cs` wires everything up.

## Learning Path

1. **Start with `Program.cs`** — see how minimal the composition root is
2. **Read `Common/IEndpoint.cs`** — understand the feature contract
3. **Check `Extensions/EndpointExtensions.cs`** — see how auto-discovery works
4. **Look at `Features/Auth/`** — study a complete feature with validation
5. **Try adding a new feature** — follow the `ProductsEndpoints` example above
