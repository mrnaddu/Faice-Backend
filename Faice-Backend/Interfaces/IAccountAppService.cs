using Faice_Backend.Dtos;
using Faice_Backend.Entities;

namespace Faice_Backend.Interfaces;

public interface IAccountAppService
{
    Task< LoginResponseDto> LoginAsync(
        LoginRequestDto input);

    Task<User> GetByIdAsync(int id);
}
