namespace TeamPulse.Application.Exceptions;

public class AppException : Exception
{
    public string Code { get; }

    public AppException(string code)
    {
        Code = code;
    }
}