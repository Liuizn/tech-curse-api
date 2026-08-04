namespace tech_curse_api.src.Domain.Exceptions;

public class BadRequestExecption : Exception
{
    public BadRequestExecption(string message) : base(message)
    {
    }
}
