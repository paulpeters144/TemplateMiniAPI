namespace TemplateMiniAPI.Services;

public interface ITokenService
{
    string GenerateToken(string username);
}
