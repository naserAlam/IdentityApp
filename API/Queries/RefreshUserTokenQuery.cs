using API.DTOs.Account;
using MediatR;

namespace API.Queries
{
    public record RefreshUserTokenQuery(string Email) : IRequest<UserDto>;
}
