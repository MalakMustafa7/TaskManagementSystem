using System.ComponentModel.DataAnnotations;

namespace TeamTaskManagement.Api.DTOs
{
    public class TeamDto
    {
        [Required]
        public string Name { get; set; }
    }
}
