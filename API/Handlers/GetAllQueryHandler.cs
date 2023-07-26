using API.DTOs.Account;
using API.Models;
using API.Queries;
using API.Repositories;
using MediatR;

namespace API.Handlers
{
    public class GetAllQueryHandler : IRequestHandler<GetAllUsersQuery, PagedUserDto>
    {
        private readonly IUserRepository _userRepository;

        public GetAllQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedUserDto> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            int totalUsersCount = await _userRepository.GetTotalUsersCountAsync();
            var users = await _userRepository.GetPagedUsersAsync(request.PageNumber, request.PageSize);
            var response = new PagedUserDto()
            {
                TotalCount = totalUsersCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Users = users
            };

            return response;
        }
    }
}
