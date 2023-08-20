using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Skills;
using ModelLibrary.DTOs.Tasks;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class ParametersService : IParametersService
    {
        private readonly WoTaasContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;
        private const string NotResourceAdaptivedMessage = "Not Resource (Workforce) adapt required skills task's";
        private IAlgorithmService algorithmService;
        public ParametersService(WoTaasContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAlgorithmService algorithmService)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
            this.algorithmService = algorithmService;
        }

        private async Task<bool> _ValidateTasksSkillRequireds(int ProjectId, List<ParameterResourceRequest> parameterResourcesRequest)
        {
            var Errors = new List<TaskSkillsRequiredErrorDTO>();

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();


            var AllWorkforcesSkills = db.Workforces
                .Include(sk => sk.WorkforceSkills).ThenInclude(ws => ws.Skill)

                .ToList();

            var WorkforceChoosed = AllWorkforcesSkills.Where(wf => parameterResourcesRequest.Select(
                    p => p.ResourceId).Contains(wf.Id) && wf.CloudId == cloudId && wf.IsDelete == false).ToArray();

            //var WorkforceNotChoosed = AllWorkforcesSkills.Where(wf => !parameterResourcesRequest.Select(
            //       p => p.ResourceId).Contains(wf.Id) && wf.CloudId == cloudId && wf.IsDelete == false).ToList();


            var Tasks = await db.Tasks
                .Include(t => t.TasksSkillsRequireds).ThenInclude(t => t.Skill)
                .Where(t => t.ProjectId == ProjectId).ToListAsync();

            var NotAdaptedTasks = new List<ModelLibrary.DBModels.Task>();

            foreach (var task in Tasks)
            {

                var skillsRequireds = task.TasksSkillsRequireds.ToList();
                var num_adapt = skillsRequireds.Count;
                var isAdapted = false;

                foreach (var wfk in WorkforceChoosed)
                {
                    var listSkills = wfk.WorkforceSkills.ToList();
                    var num_check = 0;
                    foreach (var rqSk in skillsRequireds)
                    {
                        for (int i = 0; i < listSkills.Count; i++)
                        {
                            if (listSkills[i].SkillId == rqSk.SkillId && listSkills[i].Level >= rqSk.Level)
                            {
                                num_check += 1;
                            }
                        }
                    }

                    if (num_check == num_adapt)
                    {
                        isAdapted = true;
                        break;
                    }

                }


                // If skills required task's not adaptive then throw
                if (isAdapted == false)
                {

                    NotAdaptedTasks.Add(task);

                    Errors.Add(new TaskSkillsRequiredErrorDTO
                    {
                        TaskId = task.Id,
                        SkillRequireds = mapper.Map<List<SkillRequiredDTO>>(task.TasksSkillsRequireds),
                        Messages = NotResourceAdaptivedMessage
                    });

                }



            }

            //var recommendResults = new List<RecomendWorkforceTaskParams>();

            //if (NotAdaptedTasks.Count > 0)
            //{
            //    recommendResults = await _RecomendWorkforceAdaptedTaskRequireSkill(NotAdaptedTasks, WorkforceNotChoosed);
            //}


            if (Errors.Count != 0)
            {

                throw new UnAuthorizedException(Errors);
            }
            return true;
        }


        private async Task<List<RecomendWorkforceTaskParams>> _RecomendWorkforceAdaptedTaskRequireSkill(List<ModelLibrary.DBModels.Task> Tasks, List<Workforce> workforces)
        {

            List<RecomendWorkforceTaskParams> RecomendWorkforceTaskParamsList = new();

            var _TaskQueue = Tasks.ToList();

            foreach (var wfk in workforces)
            {
                var TaskList = new List<TaskViewDTO>();
                foreach (var task in Tasks)
                {
                    var skillsRequireds = task.TasksSkillsRequireds;
                    var num_adapt = skillsRequireds.Count;

                    var listSkills = wfk.WorkforceSkills.ToList();
                    var num_check = 0;
                    foreach (var rqSk in skillsRequireds)
                    {
                        for (int i = 0; i < listSkills.Count; i++)
                        {
                            if (listSkills[i].SkillId == rqSk.SkillId && listSkills[i].Level >= rqSk.Level)
                            {
                                num_check += 1;
                            }
                        }
                    }

                    if (num_check == num_adapt)
                    {
                        TaskList.Add(new TaskViewDTO()
                        {
                            Id = task.Id,
                            Name = task.Name
                        });

                        // remove task be workforce adapted

                        if (_TaskQueue.Contains(task))
                        {
                            _TaskQueue.Remove(task);
                        }


                    }




                }

                if (TaskList.Count > 0)
                {
                    RecomendWorkforceTaskParamsList.Add(
                      new RecomendWorkforceTaskParams()
                      {
                          Tasks = TaskList,
                          Workforce = mapper.Map<WorkforceViewDTOResponse>(wfk)
                      });
                }

            }

            if (_TaskQueue.Count() > 0)
            {

                foreach (var task in _TaskQueue)
                {


                    var SkillDTORes = new List<SkillDTOResponse>();

                    var Skills = await db.Skills

                        .Where(t => t.TasksSkillsRequireds.Select(s => s.SkillId).Contains(t.Id)).ToListAsync();

                    foreach (var skill in task.TasksSkillsRequireds)
                    {
                        SkillDTORes.Add(new SkillDTOResponse()
                        {
                            Id = skill.SkillId,
                            Name = Skills.Where(s => s.Id == skill.SkillId).Select(s => s.Name).FirstOrDefault(),
                            Level = skill.Level
                        });
                    }

                    RecomendWorkforceTaskParamsList.Add(
                     new RecomendWorkforceTaskParams()
                     {
                         Tasks = new List<TaskViewDTO>() {
                             new TaskViewDTO()
                             {
                                 Id = task.Id,
                                 Name = task.Name
                              } },
                         NewWorkforce = new WorkforceSkillViewDTOResponse()
                         {
                             Name = "anonymous",
                             Skills = SkillDTORes

                         }
                     });
                }
            }

            return RecomendWorkforceTaskParamsList;
        }

        private async Task<ParameterRequestDTO> _validatePramater(ParameterRequestDTO parameterRequest)
        {
            await _ValidateTasksSkillRequireds(parameterRequest.ProjectId, parameterRequest.ParameterResources);
            return parameterRequest;
        }

        public async Task<ParameterDTO> SaveParams(ParameterRequestDTO parameterRequest)
        {
            // Is validate Resource parameter minimize adaptive Resource Task
            parameterRequest = await _validatePramater(parameterRequest);
            var parameterRequestDTO = mapper.Map<Parameter>(parameterRequest);
            var paramsEntity = await db.Parameters.AddAsync(parameterRequestDTO);

            await db.SaveChangesAsync();
            var parameterDTO = mapper.Map<ParameterDTO>(paramsEntity.Entity);
            return parameterDTO;
        }

        public async Task<List<WorkforceViewDTOResponse>> GetWorkforceParameter(string project_id)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                //QUERY WORKFORCE IN PARAMETER TABLE WITH PROJECT ID
                var parameter_resources = (project_id == null) ? null : await db.Workforces.Include(s => s.ParameterResources).ThenInclude(s => s.Parameter)
                    .Where(p => p.ParameterResources.Any(x => x.Parameter.ProjectId.ToString().Equals(project_id))).ToListAsync();
                var queryDTOResponse = parameter_resources.Select(s => new WorkforceViewDTOResponse
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList();
                return queryDTOResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



    }
}

