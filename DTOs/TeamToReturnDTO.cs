using TeamTaskManagement.Core.Entities.Identity;

namespace TeamTaskManagement.Api.DTOs
{
    public class TeamToReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UserTeamDTO> UserTeams { get; set; }
    }
}
