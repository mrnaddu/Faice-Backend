using Faice_Backend.Dtos;
using Faice_Backend.Entities;

namespace Faice_Backend.Interfaces;

public interface IAccountAppService
{
    Task< LoginResponseDto> Login(
        LoginRequestDto input);

    Task<User> GetById(int id);
}
