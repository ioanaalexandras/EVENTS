using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RazorPagesEvents.Models
{
    public class EventUserAccess
    {
        public int Id { get; set; }
        [NotMapped]
        public string? UserEmail => User?.Email ?? string.Empty;

        // FK către Event
        public int EventId { get; set; }
        public Event Event { get; set; } = null!;

        // FK către utilizator
        public string UserId { get; set; } = null!;
        public IdentityUser User { get; set; } = null!;

        // Rol local: Manager / Asistent
        [Required]
        public string Role { get; set; } = null!;
    }
}
