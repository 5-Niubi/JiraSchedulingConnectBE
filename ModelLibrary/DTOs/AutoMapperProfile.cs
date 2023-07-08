using AutoMapper;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
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
            CreateMap<WorkforceDTO, Workforce>();

            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();
            CreateMap<SkillsListCreateSkill.Request, Skill>();


            CreateMap<SkillRequiredDTO, TasksSkillsRequired>();

            CreateMap<TasksPertCreateTask.Request, DBModels.Task>();
            CreateMap<DBModels.Task, TasksPertCreateTask.Request>();
            CreateMap<DBModels.Task, TaskDetailDTO>();
            CreateMap<TaskDetailDTO, DBModels.Task>();



            CreateMap<Schedule, ScheduleResultSolutionDTO>();
            CreateMap<Workforce, WorkforceScheduleResultDTO>();
        }
    }
}
