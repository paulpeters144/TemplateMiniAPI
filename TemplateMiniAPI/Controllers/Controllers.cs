using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TemplateMiniAPI.Endpoints;

namespace TemplateMiniAPI.Controllers;

public class AccessTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
}

public class BasicResponse
{
    public string Message { get; set; } = string.Empty;
    public BasicResponse(string message)
    {
        Message = message;
    }
}

public class WeatherForecast
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}


public class WeatherController
{
    public IResult GetWeather(IConfiguration config)
    {
        var test = config.GetValue<string>("key");
        var weatherResult = getWeather();
        if (weatherResult.DidError)
        {
            return TypedResults.StatusCode(500);
        }
        return TypedResults.Ok(weatherResult.Value);
    }

    private TResult<WeatherForecast[]> getWeather()
    {
        try
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                });
            return forecast.ToArray();
        }
        catch (Exception ex)
        {
            return Error.Exception(ex);
        }
    }
}

public class AccountController
{
    public IResult Login(HttpContext context, [FromBody] LoginRequest request)
    {
        var token = generateJwtToken();
        var response = new AccessTokenResponse { AccessToken = token };
        return TypedResults.Ok(response);
    }

    public IResult AuthEndPoint(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"].ToString();
        return TypedResults.Ok(new BasicResponse("worked!!!"));
    }

    private string generateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var encodedKey = Encoding.UTF8.GetBytes("your_secret_key_here_what_in_the_world");
        var symmetricKey = new SymmetricSecurityKey(encodedKey);
        var algorithm = SecurityAlgorithms.HmacSha256Signature;
        var credentials = new SigningCredentials(symmetricKey, algorithm);
        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "username")
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddHours(10),
            SigningCredentials = credentials
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
