namespace TemplateMiniAPI.Common.Responses;

public class BasicResponse
{
    public string Message { get; set; } = string.Empty;

    public BasicResponse(string message)
    {
        Message = message;
    }
}
