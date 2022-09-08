namespace rpg.Services.AuthService
{
    public interface IAuthRepository
    {
        Task<ServiceResponse<int>> Register(User user, string password); //GENERATES THE ID OF THE USER
        Task<ServiceResponse<List<KeyValuePair<string, string>>>> Login(string username, string password);
        Task<bool> UserExists(string username);
    }
}
