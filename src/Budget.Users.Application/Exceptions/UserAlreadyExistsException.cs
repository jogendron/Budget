using System;

namespace Budget.Users.Application.Exceptions
{
    public class UserAlreadyExistsException : UserApplicationException
    {
        public UserAlreadyExistsException() : base()
        {
        }

        public UserAlreadyExistsException(string message) : base(message)
        {
        }

        public UserAlreadyExistsException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}