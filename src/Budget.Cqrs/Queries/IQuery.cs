using MediatR;

namespace Budget.Cqrs.Queries
{
    public interface IQuery<TResponse> : IRequest<TResponse> 
        where TResponse : class
    {

    }
}