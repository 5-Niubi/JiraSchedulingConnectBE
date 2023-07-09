using AutoMapper;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;
using ModelLibrary.DTOs.Tasks;
using static ModelLibrary.DTOs.PertSchedule.TasksPertCreateTask;

namespace ModelLibrary.DTOs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Project, ProjectListHomePageDTO>();
            CreateMap<ProjectsListCreateProject, Project>();
            CreateMap<Project, ProjectDetailDTO>();
            CreateMap<WorkforceDTO, Workforce>();

            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();
            CreateMap<SkillsListCreateSkill.Request, Skill>();
            CreateMap<SkillRequest, TasksSkillsRequired>();
            CreateMap<PrecedenceRequest, TaskPrecedence>();

            CreateMap<TasksSkillsRequired, SkillRequest>();
            CreateMap<TaskPrecedence, PrecedenceRequest>();

            CreateMap<TasksSkillsRequired, SkillRequiredDTO>();
            CreateMap<TaskPrecedence, PrecedenceDTO>();

            CreateMap<TasksPertCreateTask.TaskRequest, DBModels.Task>();
            CreateMap<DBModels.Task, TasksPertCreateTask.TaskRequest>();
            CreateMap<DBModels.Task, TaskDetailDTO>();
            CreateMap<TaskDetailDTO, DBModels.Task>();



            CreateMap<Schedule, ScheduleResultSolutionDTO>();
            CreateMap<Workforce, WorkforceScheduleResultDTO>();
        }
    }
}
