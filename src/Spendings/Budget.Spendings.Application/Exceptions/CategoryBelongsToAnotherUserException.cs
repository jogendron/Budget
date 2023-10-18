namespace Budget.Spendings.Application.Exceptions;

public class CategoryBelongsToAnotherUserException : Exception
{
    public CategoryBelongsToAnotherUserException() : base()
    {
    }

    public CategoryBelongsToAnotherUserException(string message) : base(message)
    {
    }

    public CategoryBelongsToAnotherUserException(string message, Exception inner) : base(message, inner)
    {
    }
}
