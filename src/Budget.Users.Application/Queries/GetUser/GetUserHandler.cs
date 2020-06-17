using System.Threading;
using System.Threading.Tasks;

using Budget.Cqrs.Queries;
using Budget.Users.Domain.Model.ReadModel;
using Budget.Users.Domain.Repositories.ReadModelRepositories;

namespace Budget.Users.Application.Queries.GetUser
{
    public class GetUserHandler : IQueryHandler<GetUserRequest, User>
    {
        public GetUserHandler(IReadModelUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        private IReadModelUserRepository UserRepository { get; }

        public Task<User> Handle(GetUserRequest request, CancellationToken cancellationToken)
        {
            return UserRepository.GetUser(request.UserName);
        }
    }
}