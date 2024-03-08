using Faice_Backend.Dtos;
using Faice_Backend.Interfaces;

namespace Faice_Backend.Services;

public class AccountAppService
    : IAccountAppService
{
    public AccountAppService()
    {
    }

    public Task<LoginResponseDto> Login(LoginRequestDto input)
    {
        throw new NotImplementedException();
    }
}
