﻿using AutoMapper;
using ModelLibrary.DBModels;
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
            CreateMap<ProjectsListCreateProject.Request, Project>();
            CreateMap<Project, ProjectDetailDTO>();
            CreateMap<WorkforceDTO, Workforce>();

            CreateMap<Skill, SkillDTO>();
            CreateMap<SkillDTO, Skill>();

            CreateMap<TasksListCreateTask.Request, DBModels.Task>();
            CreateMap<DBModels.Task, TaskDetailDTO>();

            CreateMap<SkillsListCreateSkill.Request, Skill>();
        }
    }
}
