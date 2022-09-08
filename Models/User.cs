namespace rpg.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }

        public string profilePicturePath { get; set; }  

        public byte[] PasswordSalt { get; set; }

        //con esto se hace la relacion automaticamente
        public List<Character>? Characters { get; set; }

    }
}
