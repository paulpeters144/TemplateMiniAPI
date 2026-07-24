using TemplateMiniAPI.Common;

namespace TemplateMiniAPI.Features.Weather;

public class WeatherEndpoints : IEndpoint
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public void MapEndpoint(WebApplication app)
    {
        var group = app.MapGroup("/api/weather").WithTags("Weather");

        group.MapGet("/", GetWeather)
            .Produces<WeatherForecast[]>()
            .WithName("GetWeather")
            .AllowAnonymous()
            .WithOpenApi();
    }

    private static IResult GetWeather()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            });

        return TypedResults.Ok(forecast.ToArray());
    }
}
