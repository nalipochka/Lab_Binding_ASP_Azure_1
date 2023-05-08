using System.ComponentModel.DataAnnotations.Schema;

namespace Lab_Binding_ASP_Azure_1.Models.DTOs
{
    public class EditPhotoDTO
    {
        [NotMapped]
        public int Id { get; set; }
        public string? Filename { get; set; } = default!;
        public string? PhotoUrl { get; set; } = default!;
        public string? Description { get; set; } 
    }
}
