using TemplateMiniAPI.Common;
using TemplateMiniAPI.Common.Filters;
using TemplateMiniAPI.Common.Responses;
using TemplateMiniAPI.Features.Auth.Contracts;
using TemplateMiniAPI.Services;

namespace TemplateMiniAPI.Features.Auth;

public class AuthEndpoints : IEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", Login)
            .Produces<AccessTokenResponse>()
            .WithName("LogIn")
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .WithOpenApi();

        group.MapGet("/verify", Verify)
            .Produces<BasicResponse>()
            .WithName("VerifyAuth")
            .RequireAuthorization()
            .WithOpenApi();
    }

    private static IResult Login(LoginRequest request, ITokenService tokenService)
    {
        var token = tokenService.GenerateToken(request.Email);
        return TypedResults.Ok(new AccessTokenResponse { AccessToken = token });
    }

    private static IResult Verify(HttpContext context)
    {
        return TypedResults.Ok(new BasicResponse("worked!!!"));
    }
}
