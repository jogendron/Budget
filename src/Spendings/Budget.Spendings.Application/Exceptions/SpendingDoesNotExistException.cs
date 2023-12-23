namespace Budget.Spendings.Application.Exceptions;

public class SpendingDoesNotExistException : Exception
{
    public SpendingDoesNotExistException() : base()
    {
    }

    public SpendingDoesNotExistException(string message) : base(message)
    {
    }

    public SpendingDoesNotExistException(string message, Exception inner) : base(message, inner)
    {
    }
}
