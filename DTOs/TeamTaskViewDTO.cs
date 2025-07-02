namespace TeamTaskManagement.Api.DTOs
{
    public class TeamTaskViewDTO
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; }
        public string? TaskDescription { get; set; }
        public DateTime? Deadline { get; set; }
        public string Status { get; set; }
        public string AssignedUserId { get; set; }
        public string AssignedUserName { get; set; }
        public DateTime AssignedAt { get; set; }

    }
}
