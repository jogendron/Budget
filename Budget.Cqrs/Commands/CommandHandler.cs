using MediatR;

namespace Budget.Cqrs.Commands
{
    public abstract class CommandHandler<TRequest> : AsyncRequestHandler<TRequest> where TRequest : IRequest
    {
        
    }
}