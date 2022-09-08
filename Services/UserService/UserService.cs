using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rpg.Data;
using rpg.Dtos.Character;
using rpg.Dtos.User;

namespace rpg.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;



        //constructor por automapper
        public UserService(IMapper mapper, DataContext context) 
        {
            _mapper = mapper;
            _context = context;

        }

        public async Task<ServiceResponse<GetUserDto>> GetById(int userId)
        {
            var serviceResponse = new ServiceResponse<GetUserDto>();
            try
            {
                var dbUser = await _context.Users
                     .FirstOrDefaultAsync(c => c.Id == userId);

                serviceResponse.Data = _mapper.Map<GetUserDto>(dbUser);
                if (serviceResponse.Data == null)
                {
                    serviceResponse.Message = "The id does not correspond to any user";
                    serviceResponse.Success = false;
                }

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> UpdateImageAsync(int userId, string imgPath)
        {
            var response = new ServiceResponse<string>();
            var user = new User() { Id = userId, profilePicturePath = imgPath };
            try
            {
                _context.Users.Attach(user);
                _context.Entry(user).Property(x => x.profilePicturePath).IsModified = true;
                _context.SaveChanges();
                response.Success = true;
                response.Message = "Succesfully updated picture";
            } catch ( Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
