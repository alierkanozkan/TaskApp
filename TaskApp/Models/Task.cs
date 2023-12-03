using Microsoft.AspNetCore.Identity;

namespace TaskApp.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
