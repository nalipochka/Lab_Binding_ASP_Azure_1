using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_Binding_ASP_Azure_1.Models.DTOs
{
    public class CreatePhotoDTO
    {
        [NotMapped]
        public IFormFile Photo { get; set; } = default!;    
        public string? Description { get; set; }
    }
}
