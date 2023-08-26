namespace Budget.Spendings.Application.Exceptions;

public class SpendingCategoryAlreadyExistsException : Exception
{
    public SpendingCategoryAlreadyExistsException() : base()
    {
    }

    public SpendingCategoryAlreadyExistsException(string message) : base(message)
    {
    }

    public SpendingCategoryAlreadyExistsException(string message, Exception inner) : base(message, inner)
    {
    }
}
