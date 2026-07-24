
using FluentValidation;

namespace TemplateMiniAPI.Endpoints;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(p => p.Email).EmailAddress().NotEmpty();
        RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(16).WithMessage("Your password length must not exceed 16.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).");
    }
}

public class LoginFilter : IEndpointFilter
{
    private readonly IValidator<LoginRequest> _validator;
    public LoginFilter(IValidator<LoginRequest> validator)
    {
        _validator = validator;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var login = ctx.Arguments.FirstOrDefault(a => a?.GetType() == typeof(LoginRequest)) as LoginRequest;
        var result = await _validator.ValidateAsync(login ?? new LoginRequest());
        if (!result.IsValid)
        {
            var error = result.ToDictionary().ToDictionary(
                kvp => char.ToLower(kvp.Key[0]) + kvp.Key[1..],
                kvp => kvp.Value);

            return Results.Json(error, statusCode: 400);
        }
        return await next(ctx);
    }
}
