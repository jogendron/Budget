using Budget.Cqrs.Commands;

namespace Budget.Users.Application.Commands.Subscribe
{

    public class SubscribeCommand : ICommand
    {
        public SubscribeCommand()
        {
        }

        public SubscribeCommand(
            string userName,
            string firstName,
            string lastName,
            string email,
            string password
        )
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

}