using rpg.Dtos.User;
using rpg.Models;

namespace rpg.Services.UserService
{
    public interface IUserService
    {
        Task<ServiceResponse<GetUserDto>> GetById(int userId);
        Task<ServiceResponse<string>> UpdateImageAsync(int userId, string imgPath);
    }
}
