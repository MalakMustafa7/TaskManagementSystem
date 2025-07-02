using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagement.Api.DTOs
{
    public class TaskTemplateDTO
    {
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
