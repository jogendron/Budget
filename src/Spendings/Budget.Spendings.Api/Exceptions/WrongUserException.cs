namespace Budget.Spendings.Api.Exceptions;

public class WrongUserException : Exception
{
    public WrongUserException() : base()
    {
    }

    public WrongUserException(string message) : base(message)
    {
    }

    public WrongUserException(string message, Exception inner) : base(message, inner)
    {
    }
}