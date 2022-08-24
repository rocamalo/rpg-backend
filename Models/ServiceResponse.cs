namespace rpg.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true; //result tambien se le llama
        public string Message { get; set; } = string.Empty;
    }
}
