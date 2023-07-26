using API.Models;
using API.Queries;
using API.Repositories;
using API.Services;
using MediatR;

namespace API.Handlers
{
    public class GetUserByCodeQueryHandler : IRequestHandler<GetUserByCodeQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly JWTService _jwtService;

        public GetUserByCodeQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> Handle(GetUserByCodeQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByCodeAsync(request.Code);
            if (user == null) { return null; }
            return user;
        }
    }
}
