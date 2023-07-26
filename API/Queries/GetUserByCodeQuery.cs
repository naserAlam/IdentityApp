using API.Models;
using MediatR;

namespace API.Queries
{
    public record GetUserByCodeQuery(int Code) : IRequest<User>;
}
