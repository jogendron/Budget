namespace Budget.Spendings.Application.Exceptions;

public class SpendingBelongsToAnotherUserException : Exception
{
    public SpendingBelongsToAnotherUserException() : base()
    {
    }

    public SpendingBelongsToAnotherUserException(string message) : base(message)
    {
    }

    public SpendingBelongsToAnotherUserException(string message, Exception inner) : base(message, inner)
    {
    }
}
