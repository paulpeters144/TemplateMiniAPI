using TemplateMiniAPI.Controllers;

namespace TemplateMiniAPI.Endpoints;

public class AccountEndpoint : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        var weatherController = new WeatherController();
        app.MapGet("/weather", new WeatherController().GetWeather)
            .Produces<WeatherForecast[]>()
            .WithName("GetWeather")
            .AllowAnonymous()
            .WithOpenApi();

        var accountController = new AccountController();
        app.MapPost("/login", accountController.Login)
            .Produces<AccessTokenResponse>()
            .WithName("LogIn")
            .AllowAnonymous()
            .AddEndpointFilter<LoginFilter>()
            .WithOpenApi();

        app.MapGet("/auth", accountController.AuthEndPoint)
            .Produces<BasicResponse>()
            .WithName("Auth")
            .RequireAuthorization()
            .WithOpenApi();
    }
}


public class TVoid { }

public readonly struct TResult
{
    public static TVoid Void => new TVoid();
}

public sealed record Error(string Code, string? Description)
{
    public static Error NotImplemented()
    {
        try { throw new NotImplementedException(); }
        catch (Exception ex) { return Exception(ex); }
    }

    public static Error Exception(Exception ex) => new Error(ex.GetType().Name, ex.Message);
}

public readonly struct TResult<TValue>
{
    private readonly TValue? _value = default;
    private readonly Error? _error = default;

    public bool DidError => _error != null;

    public Error Error => _error!;
    public TValue Value => _value!;

    public static TVoid Void => new TVoid();

    private TResult(Error error)
    {
        _error = error;
    }

    private TResult(TValue value)
    {
        _value = value;
    }

    public static implicit operator TResult<TValue>(TValue value) => new(value);
    public static implicit operator TResult<TValue>(Error err) => new(err);
}