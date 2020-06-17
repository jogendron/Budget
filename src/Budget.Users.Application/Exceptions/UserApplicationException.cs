using System;

namespace Budget.Users.Application.Exceptions
{
    public abstract class UserApplicationException : Exception
    {
        public UserApplicationException() : base()
        {
        }

        public UserApplicationException(string message) : base(message)
        {
        }

        public UserApplicationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}