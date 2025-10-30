using System.ComponentModel.DataAnnotations;

namespace Oneko_WebApp.Models
{    
    public record TicketForm(
        [Required(ErrorMessage = "First name is required.")] string FirstName,
        [Required(ErrorMessage = "Last name is required.")] string LastName,
        [Required(ErrorMessage = "Help is required.")] string Help
    );
}
