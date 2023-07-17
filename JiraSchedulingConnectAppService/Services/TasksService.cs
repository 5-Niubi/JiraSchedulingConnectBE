using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Aspose.Tasks;
using AutoMapper;
using Humanizer;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.Invalidator;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Tasks;
using UtilsLibrary.Exceptions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JiraSchedulingConnectAppService.Services
{
    public class TasksService : ITasksService
    {
        public const string NotFoundMessage = "Task Not Found!";
        public const string NotFoundSkillMessage = "Skill Not Found!";
        public const string LevelSkillNotValidMessage = "Level Skill Not Valid!";
        public const string RequiredSkillNotValidMessage = "Skill Not Validate!";
        public const string PrecedenceMissingTaskMessage = "Task Not Set Precedence!";
        public const string RequiredSkillMissingTaskMessage = "Task Not Set Required SKill!";

        public const string ProjectNotFoundMessage = "Project Not Found!";
        public const string NotUniqueTaskNameMessage = "Task Name Must Unique!";
        public const string PredenceNotExitedMessage = "Predence Task  not valid!";
        public const string MilestoneNotValidMessage = "Milestone Task's not valid!";

        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public TasksService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.db = dbContext;
            this.mapper = mapper;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        private async Task<ModelLibrary.DBModels.Task> GetExitedTask(ModelLibrary.DBModels.Task task)
        {
            // validate exited name task  project's 
            var existedTask = await db.Tasks.FirstOrDefaultAsync(
                t => t.Name == task.Name
                & t.CloudId == task.CloudId
                & t.ProjectId == task.ProjectId
                & t.IsDelete == false);

            return existedTask;
        }

        private async Task<bool> _ClearTaskPrecedenceTask(int projectId) {


            //var UniqueTasks = new List<int>();
            //foreach (var taskPredencesTask in TaskPrecedencesTasksRequest)
            //{
            //    var taskId = taskPredencesTask.TaskId;
            //    if (!UniqueTasks.Contains(taskId))
            //    {
            //        UniqueTasks.Add(taskId);
            //    }
            //    foreach (var precedenceId in taskPredencesTask.TaskPrecedences)
            //    {
            //        if (!UniqueTasks.Contains(precedenceId))
            //        {
            //            UniqueTasks.Add(precedenceId);
            //        }
            //    }
            //}

            //// validated task exited
            //// validate precedence tasks exited
            var exitedTasks = await db.Tasks
                .Where(t => t.ProjectId == projectId && t.IsDelete == false)
                .ToListAsync();



            var exitedPrecedenceTasks = await db.TaskPrecedences
                .Where(t => exitedTasks.Select(et => et.Id).Contains(t.TaskId) || exitedTasks.Select(et => et.Id).Contains(t.PrecedenceId))
                .ToListAsync();


            // TODO: improve clean data -> not 
            db.RemoveRange(exitedPrecedenceTasks);
            await db.SaveChangesAsync();
            return true;
        }


        private async Task<bool> _ClearTaskSkillRequired(List<TaskSkillsRequiredRequestDTO> taskSkillsRequiredsRequest)
        {


            var requiredSkillsToRemove = await db.TasksSkillsRequireds
                .Where(t => taskSkillsRequiredsRequest.Select(f => f.TaskId).Contains(t.TaskId) & t.IsDelete == false)
                .ToListAsync();

            db.RemoveRange(requiredSkillsToRemove);
            await db.SaveChangesAsync();
            return true;
        }




        public async Task<TaskPertViewDTO> CreateTask(TaskCreatedRequest taskRequest)
        {

           
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            // define task
            var task = mapper.Map<ModelLibrary.DBModels.Task>(taskRequest);
            task.CloudId = cloudId;

            // validate exited name task  project's 
            await _ValidateExitedTaskName(task);
                
            // validate milestone task
            await _ValidateMilestoneTask(task);

            // validate exited predences in this project
            await _ValidatePrecedenceTask(task);

            // validate required skills task's
            await _ValidateSkillsRequired(task) ;

            // validate is DAG graph
            // TODO 

            // insert task
            var taskCreatedEntity = await db.Tasks.AddAsync(task);
            await db.SaveChangesAsync();

            var taskPertViewDTO = mapper.Map<TaskPertViewDTO>(taskCreatedEntity.Entity);
            return taskPertViewDTO;
          
        }

      

        public async Task<TaskPertViewDTO> GetTaskDetail(int Id)
        {

            var task = await db.Tasks
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .FirstOrDefaultAsync(
                    t => t.Id == Id
                    && t.IsDelete == false);

            if(task == null) {
                throw new Exception(NotFoundMessage);
            }

            var taskPertViewDTO = mapper.Map<TaskPertViewDTO> (task);

            return taskPertViewDTO;


        }

        public async Task<List<TaskPertViewDTO>> GetTasksPertChart(int projectId)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();


            // validate exited name task  project's 
            var existingProject = await db.Projects.FirstOrDefaultAsync(
                t => t.CloudId == cloudId
                & t.Id == projectId
                & t.IsDelete == false);

            if (existingProject == null)
            {
                throw new Exception(ProjectNotFoundMessage);
            }

            // Get all task in project
            var taskList =  await db.Tasks
                .Include(tp => tp.TaskPrecedenceTasks)
                .Include(tk => tk.TasksSkillsRequireds)
                .Where(t =>  t.ProjectId == projectId
                & t.IsDelete == false).ToListAsync();


            var taskPertViewDTO = mapper.Map<List<TaskPertViewDTO>>(taskList);

            return taskPertViewDTO;


        }




        private async Task<bool> _UpsertSkillRequireds(ModelLibrary.DBModels.Task task, List<TasksSkillsRequired> tasksSkillsRequireds) {
            // delete skillRequiredsToRemove  
            foreach (var oldSkillRequired in task.TasksSkillsRequireds)
            {
                var exitedSkillRequired = tasksSkillsRequireds.FirstOrDefault(news => news.SkillId == oldSkillRequired.SkillId);
                if (exitedSkillRequired != null)
                {
                    oldSkillRequired.Level = exitedSkillRequired.Level;

                }
                else
                {
                    oldSkillRequired.IsDelete = true;
                }

            }


            // update exited skill required task
            var skillRequiredsToUpdate = task.TasksSkillsRequireds.Select(newS => new TasksSkillsRequired { TaskId = task.Id, SkillId = newS.SkillId, Level = newS.Level })
            .ToList();

            db.UpdateRange(skillRequiredsToUpdate);
            await db.SaveChangesAsync();

            // add new skill required task
            var skillRequiredsToAdd = tasksSkillsRequireds
            .Where(newS => !task.TasksSkillsRequireds.Any(old => old.SkillId == newS.SkillId))
            .Select(newS => new TasksSkillsRequired { TaskId = task.Id, SkillId = newS.SkillId, Level = newS.Level })
            .ToList();


            db.TasksSkillsRequireds.AddRange(skillRequiredsToAdd);
            await db.SaveChangesAsync();

            return true;
        }


        private async Task<bool> _UpsertPrecedenceTasks(ModelLibrary.DBModels.Task task, List<TaskPrecedence> taskPrecedences)
        {
            // delete skillRequiredsToRemove  
            foreach (var oldPrecedenceTask in task.TaskPrecedenceTasks)
            {
                var exitedPrecedenceTask = taskPrecedences.FirstOrDefault(news => news.PrecedenceId == oldPrecedenceTask.PrecedenceId);
                if (exitedPrecedenceTask == null)
                {
                    exitedPrecedenceTask.IsDelete = true;

                }
               

            }

            // update exited precedence task
            db.UpdateRange(task.TaskPrecedenceTasks);
            await db.SaveChangesAsync();

            // add new skill required task
            var raskPrecedenceTasksToAdd = taskPrecedences
            .Where(newS => !task.TaskPrecedenceTasks.Any(old => old.PrecedenceId == newS.PrecedenceId))
            .Select(newS => new TaskPrecedence { TaskId = task.Id, PrecedenceId = newS.PrecedenceId })
            .ToList();


            db.TaskPrecedences.AddRange(raskPrecedenceTasksToAdd);
            await db.SaveChangesAsync();

            return true;
        }



        public async Task<TaskPertViewDTO> UpdateTask(TaskUpdatedRequest taskRequest)
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            // define task
            var changingTask = mapper.Map<ModelLibrary.DBModels.Task>(taskRequest);
            changingTask.CloudId = cloudId;

            // validated task exited
            var oldTask = await db.Tasks
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .FirstOrDefaultAsync(
                    t => t.Id == changingTask.Id
                    && t.IsDelete == false);

            if (oldTask == null) {
                throw new Exception(NotFoundMessage);
            }

            // validated task name exited
            await _ValidateExitedTaskName(changingTask);
            

            // validate exited predences in this project
            if (changingTask.TaskPrecedenceTasks != null) {
                await _ValidatePrecedenceTask(changingTask);
            }


            // validate required skills task's
            if (changingTask.TasksSkillsRequireds != null)
            {
                await _ValidateSkillsRequired(changingTask);
            }


            //TODO validate DAG

            changingTask.Name = (changingTask.Name == null) ?
                oldTask.Name : changingTask.Name;

            oldTask.Duration = (changingTask.Duration == null) ?
                oldTask.Duration : changingTask.Duration;

            oldTask.Duration = (changingTask.Duration == null) ?
                oldTask.Duration : changingTask.Duration;

            // update and insert required skills
            _UpsertSkillRequireds(oldTask, changingTask.TasksSkillsRequireds.ToList());


            // update and insert precedence task
            _UpsertPrecedenceTasks(oldTask, changingTask.TaskPrecedenceTasks.ToList()); 

            var taskPertViewDTO = mapper.Map<TaskPertViewDTO>(changingTask);
            return taskPertViewDTO;
        }

        private async Task<List<TaskPrecedenceDTO>> _SaveTasksPrecedencesTasks(List<TaskPrecedencesTaskRequestDTO> taskprecedencesTasksRequest)
        {

            // mapping task precedences request -> task precedences database
            List<TaskPrecedence> precedenceTasksToAdd = new List<TaskPrecedence>();
            foreach(var taskPrecedences in taskprecedencesTasksRequest) {
                foreach(var precedenceId in taskPrecedences.TaskPrecedences) {
                    precedenceTasksToAdd.Add(new TaskPrecedence()
                    {
                        TaskId = taskPrecedences.TaskId,
                        PrecedenceId = precedenceId
                    });
                }
                
            }

            // insert new precedence tasks
            await db.AddRangeAsync(precedenceTasksToAdd);
            await db.SaveChangesAsync();

            // TODO: review code -> mapping task precedences database -> task precedences for view
            var taskPrecedencesDTO = mapper.Map<List<TaskPrecedenceDTO>>(precedenceTasksToAdd);

            return taskPrecedencesDTO;


        }


        


        


        private async Task<List<TasksSkillsRequired>> _SaveTasksSkillsRequireds(List<TaskSkillsRequiredRequestDTO> taskSkillsRequiredsRequest)
        {

           


            // mapping task precedences request -> task precedences database
            List<TasksSkillsRequired> tasksSkillsRequiredsToAdd = new List<TasksSkillsRequired>();
            foreach (var taskSkillsRequired in taskSkillsRequiredsRequest)
            {
                foreach (var skill in taskSkillsRequired.SkillsRequireds)
                {
                    var taskSkillRequired = new TasksSkillsRequired()
                    {
                        TaskId = taskSkillsRequired.TaskId,
                        SkillId = skill.SkillId,
                        Level = skill.Level

                    };
                    tasksSkillsRequiredsToAdd.Add(taskSkillRequired);
                }

            }

            // insert new precedence tasks
            await db.AddRangeAsync(tasksSkillsRequiredsToAdd);
            await db.SaveChangesAsync();


            return tasksSkillsRequiredsToAdd;


        }



        public async Task<bool> SaveTasks(TasksSaveRequest TasksSaveRequest)
        {

            var projectId = TasksSaveRequest.ProjectId;

            var TaskPrecedenceTasksRequest = TasksSaveRequest.TaskPrecedenceTasks;
            var TaskSkillsRequiredsRequest = TasksSaveRequest.TaskSkillsRequireds;

            // TODO: all task setup on skill & precedence must exited on database
            // check all task setup precedence
            await _ValidateConfigTaskPrecedences(projectId, TaskPrecedenceTasksRequest);

            // check precedence task is validate
            await _ValidateExitedPrecedenceTask(TaskPrecedenceTasksRequest);

            // clean all precedence tasks of project id
            await _ClearTaskPrecedenceTask(projectId);


            // check all Tasks project's must setup Required Skills
            await _ValidateConfigAllTaskSkillsRequireds(projectId, TaskSkillsRequiredsRequest);


            // validate exited skill 
            await _ValidateExitedSkill(TaskSkillsRequiredsRequest);

            // clean all precedence tasks of project id
            await _ClearTaskSkillRequired(TaskSkillsRequiredsRequest);

            // save task skill required
            await _SaveTasksSkillsRequireds(TaskSkillsRequiredsRequest);

            // save task precedence
            await _SaveTasksPrecedencesTasks(TaskPrecedenceTasksRequest);

            return true;

        }



        private async Task<bool> _ValidatePrecedenceTask(ModelLibrary.DBModels.Task task)
        {

            var Errors = new List<TaskInputErrorDTO>();
            var ExitedTasks = await db.Tasks
                .Where(t => t.ProjectId == task.ProjectId & t.IsDelete == false)
                .ToListAsync();

            var PrecedenceTaskErrors = new List<TaskPrecedenceErrorDTO>();
            foreach (var precedenceTask in task.TaskPrecedenceTasks)
            {
                if (!ExitedTasks.Select(t => t.Id).Contains(precedenceTask.PrecedenceId))
                {

                    PrecedenceTaskErrors.Add(
                        new TaskPrecedenceErrorDTO
                        {
                            PrecedenceId = precedenceTask.PrecedenceId,
                            TaskId = precedenceTask.TaskId,
                            Messages = PredenceNotExitedMessage
                        });
 
                    
            }


              
            }

            if (PrecedenceTaskErrors.Count != 0)
            {
                throw new NotSuitableInputException(PrecedenceTaskErrors);
            }

            return true;

        }

        private async Task<bool> _ValidateMilestoneTask(ModelLibrary.DBModels.Task task)
        {
            // validate milestone task  project's 
            var existingMilestone = await db.Milestones.FirstOrDefaultAsync(
                t => t.Id == task.MilestoneId
                & t.ProjectId == task.ProjectId
                );

            if (existingMilestone == null)
            {
                throw new NotSuitableInputException(new TaskInputErrorDTO
                {
                    TaskId = task.Id,
                    MilestoneId = task.MilestoneId,
                    Messages = MilestoneNotValidMessage
                });
            }

            return true;
        }



        private async Task<bool> _ValidateSkillsRequired(ModelLibrary.DBModels.Task task)
        {
            var Errors = new List<TaskInputErrorDTO>();
            //validate exited on database
            var exitedSkills = await db.Skills
                .Where(s => s.CloudId == task.CloudId & s.IsDelete == false)
                .ToListAsync();

            
            foreach (var skill in task.TasksSkillsRequireds)
            {

                var SkillErrors = new List<SkillRequestErrorDTO>();
                if (!exitedSkills.Select(s => s.Id).Contains(skill.SkillId))
                {
                    SkillErrors.Add(
                        new SkillRequestErrorDTO
                        {
                            SkillId = skill.SkillId,
                            Messages = NotFoundMessage
                        });



                }

                if (skill.Level < 1 || skill.Level > 5)
                {
                    SkillErrors.Add(
                        new SkillRequestErrorDTO
                        {
                            SkillId = skill.SkillId,
                            Level = skill.Level,
                            Messages = LevelSkillNotValidMessage
                        }); 

                }

                if (SkillErrors.Count != 0) {
                    Errors.Add(new TaskInputErrorDTO
                    {
                        TaskId = task.Id,
                        SkillRequireds = SkillErrors,
                        Messages = RequiredSkillNotValidMessage

                    });
                }

            }

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);

            }
            return true;

        }

        private async Task<bool> _ValidateExitedTaskName(ModelLibrary.DBModels.Task task)
        {
            // validate exited name task  project's 
            var existingTask = await db.Tasks.FirstOrDefaultAsync(
                t => t.Name == task.Name
                & t.CloudId == task.CloudId
                & t.ProjectId == task.ProjectId
                & t.IsDelete == false);

            if (existingTask != null)
            {

                throw new NotSuitableInputException(
                    new TaskInputErrorDTO
                    {
                        TaskId = task.Id,
                        Messages = NotUniqueTaskNameMessage
                 });
                

            }

            return true;
        }

        private async Task<bool> _ValidateExitedSkill(List<TaskSkillsRequiredRequestDTO> taskSkillsRequiredsRequest)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();


            var Errors = new List<TaskSkillRequiredErrorDTO>();

            var exitedSkills = await db.Skills
                .Where(t => t.CloudId == cloudId && t.IsDelete == false)
                .ToListAsync();

            foreach (var taskSkillsRequired in taskSkillsRequiredsRequest)
            {
                var skillsRequired = taskSkillsRequired.SkillsRequireds;
                foreach (var skill in skillsRequired)
                    if (!exitedSkills.Select(t => t.Id).Contains(skill.SkillId))
                    {
                        Errors.Add(new TaskSkillRequiredErrorDTO
                        {
                            TaskId = taskSkillsRequired.TaskId,
                            SkillRequireds = mapper.Map<List<SkillRequiredDTO>>(skillsRequired),
                            Messages = skill.SkillId + "Not Found",

                        });
                    }

            }

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);

            }
            return true;

        }


        private async Task<bool> _ValidateConfigAllTaskSkillsRequireds(int ProjectId, List<TaskSkillsRequiredRequestDTO> taskSkillsRequiredsRequest)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();


            var Errors = new List<TaskInputErrorDTO>();

            var exitedTasks = await db.Tasks
                .Where(p => p.ProjectId == ProjectId)
                .ToListAsync();

            foreach (var task in exitedTasks)
            {
                if (!taskSkillsRequiredsRequest.Select(t => t.TaskId).Contains(task.Id))
                {
                    Errors.Add(
                        new TaskInputErrorDTO
                        {
                            TaskId = task.Id,
                            Messages = RequiredSkillMissingTaskMessage
                        }
                        );
                }
            }


            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);
            }

            return true;

        }

        private async Task<bool> _ValidateConfigTaskPrecedences(int ProjectId, List<TaskPrecedencesTaskRequestDTO> taskPrecedencesTasksRequest)
        {

            var Errors = new List<TaskInputErrorDTO>();

            var UniqueTasks = new List<int>();
            foreach (var taskPredencesTask in taskPrecedencesTasksRequest)
            {
                var taskId = taskPredencesTask.TaskId;
                if (!UniqueTasks.Contains(taskId))
                {
                    UniqueTasks.Add(taskId);
                }
                foreach (var precedenceId in taskPredencesTask.TaskPrecedences)
                {
                    if (!UniqueTasks.Contains(precedenceId))
                    {
                        UniqueTasks.Add(precedenceId);
                    }
                }
            }

            var exitedTasks = await db.Tasks
                .Where(p => p.ProjectId == ProjectId)
                .ToListAsync();

            foreach (var task in exitedTasks)
            {
                if (!UniqueTasks.Contains(task.Id))
                {

                    Errors.Add(
                        new TaskInputErrorDTO
                        {
                            TaskId = task.Id,
                            Messages = PrecedenceMissingTaskMessage
                        }
                        );
                }



            }

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);
            }

            return true;
        }

        private async Task<bool> _ValidateExitedPrecedenceTask(List<TaskPrecedencesTaskRequestDTO> taskprecedencesTasksRequest)
        {


            var Errors = new List<TaskInputErrorDTO>();

            // TODO: Is validate DAG

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);
            }

            return true;

        }


    }



}

