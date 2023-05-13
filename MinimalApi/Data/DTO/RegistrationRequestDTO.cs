namespace MinimalApi.Data.DTO
{
    public class RegistrationRequestDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; } 
    }
}
