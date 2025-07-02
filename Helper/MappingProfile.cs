using AutoMapper;
using TeamTaskManagement.Api.DTOs;
using TeamTaskManagement.Core.Entities.Identity;

namespace TeamTaskManagement.Api.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<TeamDto,Team>().ReverseMap();
            CreateMap<UserTeamDTO,UserTeam>().ReverseMap();
            CreateMap<Team,TeamToReturnDTO>().ReverseMap();
            CreateMap<Team, TeamListDTO>().ReverseMap();
            CreateMap<UserTeam, TeamListDTO>()
                    .ForMember(t => t.Id, ut => ut.MapFrom(u => u.Team.Id))
                    .ForMember(t => t.Name, ut => ut.MapFrom(u => u.Team.Name));

            CreateMap<TaskTemplate, TaskTemplateDTO>().ReverseMap();
            CreateMap<TaskAssignment,TaskAssignDTO>().ReverseMap();

            CreateMap<TaskAssignment, TeamTaskViewDTO>()
                     .ForMember(tv => tv.TaskId, ta => ta.MapFrom(tt => tt.TaskTemplateId))
                     .ForMember(tv => tv.TaskTitle, ta => ta.MapFrom(tt => tt.TaskTemplate.Title))
                     .ForMember(tv => tv.TaskDescription, ta => ta.MapFrom(tt => tt.TaskTemplate.Description))
                     .ForMember(tv => tv.Deadline, ta => ta.MapFrom(tt => tt.TaskTemplate.Deadline))
                     .ForMember(tv => tv.AssignedUserId, ta => ta.MapFrom(au => au.AppUserId))
                     .ForMember(tv => tv.AssignedUserName, ta => ta.MapFrom(au => au.UserTeam.AppUser.FullName));

            CreateMap<TaskAssignment, UserTaskViewDTO>()
                    .ForMember(ut => ut.TaskId, ta => ta.MapFrom(tt => tt.TaskTemplateId))
                     .ForMember(ut => ut.TaskTitle, ta => ta.MapFrom(tt => tt.TaskTemplate.Title))
                     .ForMember(ut => ut.TaskDescription, ta => ta.MapFrom(tt => tt.TaskTemplate.Description))
                     .ForMember(ut => ut.Deadline, ta => ta.MapFrom(tt => tt.TaskTemplate.Deadline));
        }
    }
}
