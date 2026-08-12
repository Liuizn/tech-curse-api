namespace tech_curse_api.src.Domain.Exceptions;

public class GatewayTimeoutException : Exception
{
    public GatewayTimeoutException(string message) : base(message)
    {
    }
}
