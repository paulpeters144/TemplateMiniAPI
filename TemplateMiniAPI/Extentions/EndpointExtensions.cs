using TemplateMiniAPI.Endpoints;

namespace TemplateMiniAPI.Extentions;

public static class EndpointExtensions
{
    public static WebApplication AddEndpoints(this WebApplication app)
    {
        new AccountEndpoint().MapEndpoint(app);

        return app;
    }
}
