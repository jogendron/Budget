using Budget.Cqrs.Queries;

using Budget.Users.Domain.ReadModel;

namespace Budget.Users.Application.Queries.GetUser
{
    public class GetUserRequest : IQuery<User>
    {
        public GetUserRequest()
        {
        }
        
        public GetUserRequest(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; set; }
    }
}