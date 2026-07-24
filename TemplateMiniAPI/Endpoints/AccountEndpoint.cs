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