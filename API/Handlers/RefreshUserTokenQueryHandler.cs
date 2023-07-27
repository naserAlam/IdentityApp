using API.DTOs.Account;
using API.Queries;
using API.Repositories;
using API.Services;
using MediatR;

namespace API.Handlers
{
    public class RefreshUserTokenQueryHandler : IRequestHandler<RefreshUserTokenQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly JWTService _jwtService;

        public RefreshUserTokenQueryHandler(IUserRepository userRepository, JWTService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<UserDto> Handle(RefreshUserTokenQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null) return null;
            return new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                JWT = _jwtService.CreateJWT(user)
            };
        }
    }
}
