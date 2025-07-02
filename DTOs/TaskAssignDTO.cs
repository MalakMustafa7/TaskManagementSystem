namespace TeamTaskManagement.Api.DTOs
{
    public class TaskAssignDTO
    {
        public string AppUserId { get; set; }
        public int TeamId { get; set; }
        public int TaskTemplateId { get; set; }
    }
}
