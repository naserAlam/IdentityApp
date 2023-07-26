using API.DTOs.Account;
using MediatR;

namespace API.Queries
{
    public record GetAllUsersQuery(int PageNumber, int PageSize) : IRequest<PagedUserDto>;
}
