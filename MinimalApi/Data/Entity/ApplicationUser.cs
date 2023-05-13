using Microsoft.AspNetCore.Identity;

namespace MinimalApi.Data.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateOfRegistration { get; set; } 
        public bool IsActive { get; set; } = true; 
    }
}
