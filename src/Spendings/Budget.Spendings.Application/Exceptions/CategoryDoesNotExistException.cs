namespace Budget.Spendings.Application.Exceptions;

public class CategoryDoesNotExistException : Exception
{
    public CategoryDoesNotExistException() : base()
    {
    }

    public CategoryDoesNotExistException(string message) : base(message)
    {
    }

    public CategoryDoesNotExistException(string message, Exception inner) : base(message, inner)
    {
    }
}
