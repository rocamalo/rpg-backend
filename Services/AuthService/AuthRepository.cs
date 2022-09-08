using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using rpg.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace rpg.Services.AuthService
{
    public class AuthRepository : IAuthRepository
    {

        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //TODO create a token registing a new user too to login
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> response = new ServiceResponse<int>();
            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "User already exists.";
                return response;
            }

            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash; //AQUI HACEMOS USO DE LAS VARIABLES CREADAS POR EL OUT
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            response.Data = user.Id;
            response.Message = "User " + user.Username + " created succesfully";
            return response;
        }

        public async Task<ServiceResponse<List<KeyValuePair<string, string>>>> Login(string username, string password)
        {
            var response = new ServiceResponse<List<KeyValuePair<string, string>>>();
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username.ToLower()));

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
            }
            else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) //compare incoming pass with the stored pass and stored salt
            {
                response.Success = false;
                response.Message = "Wrong password.";
            }
            else
            {
                response.Data = CreateToken(user);
                
            }

            return response;
        }

        

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) //CON OUT DECLARAMOS ESTAS VARIABLES DENTRO DE LOS MISMOS PARAMETROS, INCLUSO ESTAS VARIABLES SE PUEDEN UTILIZAR MAS ADELANTE DEL CODIGO
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash); //compares the stored password with the upcoming pass
            }
        }

        private List<KeyValuePair<string, string>> CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), //user id
                new Claim(ClaimTypes.Name, user.Username) //user name
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value)); //we take our secret key from appsettings.json

            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            var list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("id", user.Id.ToString()));
            list.Add(new KeyValuePair<string, string>("token", tokenHandler.WriteToken(token))); //we return the token as string
            list.Add(new KeyValuePair<string, string>("username", user.Username));
            list.Add(new KeyValuePair<string, string>("imgpath", user.profilePicturePath));

            return list; 
        }


    }
}
