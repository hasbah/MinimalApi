namespace MinimalApi.Data.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; } 
        public string Surname { get; set; }
        public DateTime DateOfRegistration { get; set; }
    }
}
