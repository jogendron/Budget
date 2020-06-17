using MediatR;

namespace Budget.Cqrs.Queries
{
    public interface IQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {

    }
}