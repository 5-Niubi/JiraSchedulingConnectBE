using AutoMapper;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;
using ModelLibrary.DTOs.Tasks;

namespace ModelLibrary.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectListHomePageDTO>();
            CreateMap<ProjectsListCreateProject, Project>();
            CreateMap<Project, ProjectDetailDTO>();
            CreateMap<WorkforceDTORequest, Workforce>();
            CreateMap<Workforce, WorkforceDTORequest>();
            CreateMap<Workforce, WorkforceDTOResponse>();
            CreateMap<WorkforceDTOResponse, Workforce>();
            CreateMap<EquipmentDTOResponse, Equipment>();
            CreateMap<Equipment, EquipmentDTOResponse>();
            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();
            CreateMap<SkillCreatedRequest, Skill>();

            CreateMap<TaskPrecedenceDTO, TaskPrecedence>();
            CreateMap<TaskPrecedence, TaskPrecedenceDTO>();

            CreateMap<TasksSkillsRequired, SkillRequiredDTO>();
            CreateMap<SkillRequiredDTO, TasksSkillsRequired>();

            CreateMap<DBModels.Task, TaskPertViewDTO>()
                .ForMember(tp => tp.Precedences, t => t.MapFrom(t => t.TaskPrecedenceTasks))
                .ForMember(tp => tp.SkillRequireds, t => t.MapFrom(t => t.TasksSkillsRequireds));

            // request to object database
            CreateMap<SkillRequiredRequestDTO, TasksSkillsRequired>();
            CreateMap<TasksSkillsRequired, SkillRequiredRequestDTO>();

            CreateMap<PrecedenceRequestDTO, TaskPrecedence>();
            CreateMap<TaskPrecedence, PrecedenceRequestDTO>();

            CreateMap<DBModels.Task, TaskCreatedRequest>()
                .ForMember(tr => tr.Precedences, t => t.MapFrom(t => t.TaskPrecedenceTasks))
                .ForMember(tr => tr.SkillRequireds, t => t.MapFrom(t => t.TasksSkillsRequireds));

            CreateMap<TaskCreatedRequest, DBModels.Task > ()
                .ForMember(tr => tr.TasksSkillsRequireds, t => t.MapFrom(t => t.SkillRequireds))
                .ForMember(tr => tr.TaskPrecedenceTasks, t => t.MapFrom(t => t.Precedences));


            CreateMap<TaskUpdatedRequest, DBModels.Task>()
                .ForMember(tr => tr.TasksSkillsRequireds, t => t.MapFrom(t => t.SkillRequireds))
                .ForMember(tr => tr.TaskPrecedenceTasks, t => t.MapFrom(t => t.Precedences));

            CreateMap<DBModels.Task, TaskUpdatedRequest>()
                .ForMember(tr => tr.SkillRequireds, t => t.MapFrom(t => t.TasksSkillsRequireds))
                .ForMember(tr => tr.Precedences, t => t.MapFrom(t => t.TaskPrecedenceTasks));

            CreateMap<Schedule, ScheduleResultSolutionDTO>();
            CreateMap<Workforce, WorkforceScheduleResultDTO>();
            CreateMap<Schedule, ScheduleResultSolutionDTO>();
        }
    }
}
