using AutoMapper;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TeamTaskManagement.Api.DTOs;
using TeamTaskManagement.Api.Errors;
using TeamTaskManagement.Api.Helper;
using TeamTaskManagement.Core.Entities.Identity;
using TeamTaskManagement.Core.Repositories;
using TeamTaskManagement.Core.Service;
using TeamTaskManagement.Core.Specifications;

namespace TeamTaskManagement.Api.Controllers
{ 
    public class TeamController : ApiBaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITeamService _teamService;

        public TeamController(IUnitOfWork unitOfWork,IMapper mapper,UserManager<AppUser> userManager,ITeamService teamService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _teamService = teamService;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> CreateTeam(TeamDto model)
        {
            var mappedTeam = _mapper.Map<TeamDto,Team>(model);
            await _unitOfWork.Repository<Team>().AddAsync(mappedTeam);
            var Result = await _unitOfWork.CompeleteAsync();
            if(Result<=0) return BadRequest(new ApiResponse(400, "Failed to create the Team"));
            return Ok(new { message = "Team created successfully", teamId = mappedTeam.Id });
        }
        [Authorize(Roles = "Admin")]
        [CachedAttribute(300)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<TeamListDTO>>> GetTeams([FromQuery] TeamSpecParams specParams)
        {
            var Spec = new TeamSpecification(specParams);
            var teams = await _unitOfWork.Repository<Team>().GetAllWithSpecAsync(Spec);
            if (teams == null || !teams.Any()) return NotFound(new ApiResponse(404, "No Teams Are Found."));
            var mappedTeams = _mapper.Map<IReadOnlyList<TeamListDTO>>(teams);
            return Ok(mappedTeams);
        }
        [Authorize(Roles = "Admin,TeamLeader")]
        [HttpGet("{teamId}")]
        public async Task<ActionResult<TeamToReturnDTO>> GetTeam(int teamId)
        {
            var Spec = new TeamSpecification(teamId);
            var team = await _unitOfWork.Repository<Team>().GetEntityWithSpecAsync(Spec);
            if(team==null) return NotFound(new ApiResponse(404, "Team Not Found"));
            var returnedTeam = _mapper.Map<TeamToReturnDTO>(team);
            return Ok(returnedTeam);
        }
        [Authorize(Roles = "TeamLeader,Admin")]
        [HttpPost("AddMember")]
        public async Task<ActionResult<string>>AddMember(UserTeamDTO model)
        {
            var mappedMember = _mapper.Map<UserTeam>(model);
            var Result = await _teamService.AddMemberToTeam(mappedMember);
            return Result switch
            {
                "Team Not Found" => NotFound(new ApiResponse(404, "Team Not Found")),
                "Member Not Found" => NotFound(new ApiResponse(404, "User Not Found")),
                "exist" =>BadRequest(new ApiResponse(400, "Member Alredy Exist")),
                "Fail" => BadRequest(new ApiResponse(400, "Failed to add member")),
                _ => Ok("Member added successfully")
            };
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<UserTeamDTO>>UpdateMemberRole(UserTeamDTO model)
        {
            var mappedMember = _mapper.Map<UserTeam>(model);
            var updatedOne = await _teamService.UpdateRole(mappedMember);
            if (updatedOne == null) return NotFound(new ApiResponse(404, "Team or User not found, or Member does not exist in team."));
            var returnedMember = _mapper.Map<UserTeamDTO>(updatedOne);
            return Ok(returnedMember);
        }
        [Authorize(Roles = "Admin,TeamLeader")]
        [HttpGet("{teamId}/members")]
        [CachedAttribute(100)]
        public async Task<ActionResult<IReadOnlyList<UserTeamDTO>>> GetTeamMembers(int teamId)
        {
            var Spec = new UserTeamSpec(teamId);
            var members = await _unitOfWork.Repository<UserTeam>().GetAllWithSpecAsync(Spec);
            if (members == null || !members.Any()) return NotFound(new ApiResponse(404, "No members found for this team."));
            var mappedMembers = _mapper.Map<IReadOnlyList<UserTeamDTO>>(members);
            return Ok(mappedMembers);
        }
        [Authorize]
        [HttpGet("{userId}/userteams")]
        [CachedAttribute(100)]
        public async Task<ActionResult<IReadOnlyList<TeamListDTO>>> GetUsersTeams(string userId)
        {
            var spec = new UserTeamSpec(userId);
            var Teams =await _unitOfWork.Repository<UserTeam>().GetAllWithSpecAsync(spec);
            if(Teams == null || !Teams.Any()) return NotFound(new ApiResponse(404, "This User Was Not At Any Team Now"));
            var mappedTeams = _mapper.Map<IReadOnlyList<TeamListDTO>>(Teams);
            return Ok(mappedTeams);

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{teamId}/{userId}")]
        public async Task<ActionResult<UserTeamDTO>> DeleteMember(int teamId,string userId)
        {
            var deletedMember =await _teamService.DeleteMemberFromTeamAsync(teamId, userId);
            if (deletedMember == null) return  NotFound(new ApiResponse(404, "Team or User not found, or Member does not exist in team."));
            var returnedMember = _mapper.Map<UserTeamDTO>(deletedMember);
            return Ok(returnedMember);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{teamId}")]
        public async Task<ActionResult<TeamListDTO>> DeletTeam(int teamId)
        {
            var deletedTeam =await _teamService.DeleteTeam(teamId);
            if (deletedTeam == null) return NotFound(new ApiResponse(404, "Team Not Found"));
            var returnedTeam = _mapper.Map<TeamListDTO>(deletedTeam);
            return Ok(returnedTeam);
        }


    }
}
