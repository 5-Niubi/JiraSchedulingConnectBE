using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Humanizer;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.PertSchedule;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace JiraSchedulingConnectAppService.Services
{
    public class TasksService : ITasksService
    {
        public const string NotFoundMessage = "Task Not Found!";
        public const string PrecedenceMissingTaskMessage = "Some Task Not Set Precedence!";
        public const string ProjectNotFoundMessage = "Project Not Found!";
        public const string NotUniqueTaskNameMessage = "Task Name Must Unique!";
        public const string PredenceNotExitedMessage = "Predence Task  not valid!";
        public const string RequiredSkillNotValidMessage = "Required Skill not valid!";
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


        private async Task<bool> _ValidatePrecedenceTask(ModelLibrary.DBModels.Task task) {

            var precedenceIds = new HashSet<int>(task.TaskPrecedenceTasks.Select(p => p.PrecedenceId));
            var exitedPrecedences = await db.Tasks
                .Where(t => precedenceIds.Contains(t.Id) & t.ProjectId == task.ProjectId & t.IsDelete==false)
                .ToListAsync();

            if (exitedPrecedences.Count != task.TaskPrecedenceTasks.Count)
            {
                throw new Exception(PredenceNotExitedMessage);
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
                throw new Exception(MilestoneNotValidMessage);
            }
            return true;
        }
        


        private async Task<bool> _ValidateSkillsRequired(ModelLibrary.DBModels.Task task) {

            //validate exited on database
            var RequiredSkillsId = new HashSet<int>(task.TasksSkillsRequireds.Select(p => p.SkillId));
            var exitedSkills = await db.Skills
                .Where(s => RequiredSkillsId.Contains(s.Id) & s.CloudId == task.CloudId & s.IsDelete == false)
                .ToListAsync();

            if (exitedSkills.Count != RequiredSkillsId.Count)
            {
                throw new Exception(RequiredSkillNotValidMessage);
            }



            // validate level skill
            foreach (var skill in task.TasksSkillsRequireds)
            {
                if (skill.Level < 1 || skill.Level > 5)
                {
                    throw new Exception(RequiredSkillNotValidMessage);
                }
            }

            return true;
       
        }

        private async Task<bool> _IsExitedTaskName(ModelLibrary.DBModels.Task task) {
            // validate exited name task  project's 
            var existingTask = await db.Tasks.FirstOrDefaultAsync(
                t => t.Name == task.Name
                & t.CloudId == task.CloudId
                & t.ProjectId == task.ProjectId
                & t.IsDelete == false);

            if (existingTask != null)
            {
                return true;
            }

            return false;
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

        private async Task<bool> _ClearTaskPrecedenceTask(List<TaskPrecedence> taskPrecedences) {

            // TODO: improve clean data -> not 
            db.RemoveRange(taskPrecedences);
            await db.SaveChangesAsync();
            return true;
        }


        private async Task<bool> _ClearTaskSkillRequired(List<TasksSkillsRequired> tasksSkillsRequireds)
        {

            // TODO: improve clean data -> not 
            db.RemoveRange(tasksSkillsRequireds);
            await db.SaveChangesAsync();
            return true;
        }




        public async Task<TaskPertViewDTO> CreateTask(TaskCreatedRequest taskRequest)
        {

            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                // define task
                var task = mapper.Map<ModelLibrary.DBModels.Task>(taskRequest);
                task.CloudId = cloudId;

                // validate exited name task  project's 
                var isExited = await _IsExitedTaskName(task);
                if (isExited) {
                    throw new Exception(NotUniqueTaskNameMessage);
                }

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
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
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



        // TODO
        public Task<bool> DeleteTask(int Id)
        {
            throw new NotImplementedException();
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
            bool isExitedName = await _IsExitedTaskName(changingTask);
            if (isExitedName)
            {
                throw new Exception(NotUniqueTaskNameMessage);
            }


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

        public async Task<List<TaskPrecedenceDTO>> SaveTasksPrecedencesTasks(TasksPrecedencesSaveRequest pertRequest)
        {

            var UniqueTasks = new List<int>();
            foreach(var precedenceT in pertRequest.TaskPrecedences) {
                var taskId = precedenceT.TaskId;
                var precedenceId = precedenceT.PrecedenceId;
                if (!UniqueTasks.Contains(taskId)) {
                    UniqueTasks.Add(taskId);
                }

                if (!UniqueTasks.Contains(precedenceId))
                {
                    UniqueTasks.Add(precedenceId);
                }
            }

            // validated task exited
            // validate precedence tasks exited
            var exitedTasks = await db.Tasks
                .Where(s => s.ProjectId == pertRequest.ProjectId & s.IsDelete == false)
                .ToListAsync();

            if (exitedTasks.Count > UniqueTasks.Count)
            {
                throw new Exception(PrecedenceMissingTaskMessage);
            }

            if (exitedTasks.Count < UniqueTasks.Count)
            {
                throw new Exception(NotFoundMessage);
            }



            // TODO: validate DAG graph
            // TODO: validate  edge node in graph

            var exitedPrecedenceTasks = await db.TaskPrecedences
                .Where(s => UniqueTasks.Contains(s.TaskId) | UniqueTasks.Contains(s.TaskId))
                .ToListAsync();

            //var exitedPrecedenceTasks = await db.TaskPrecedences
            //    .Where(s => s.IsDelete == true)
            //    .ToListAsync();


            // clean all precedence tasks of project id
            await _ClearTaskPrecedenceTask(exitedPrecedenceTasks);


            // mapping task precedences request -> task precedences database
            var precedenceTasksToAdd = mapper.Map<List<TaskPrecedence>>(pertRequest.TaskPrecedences);

            // insert new precedence tasks
            await db.AddRangeAsync(precedenceTasksToAdd);
            await db.SaveChangesAsync();

            // TODO: review code -> mapping task precedences database -> task precedences for view
            var taskPrecedencesDTO = mapper.Map<List<TaskPrecedenceDTO>>(precedenceTasksToAdd);
            
            return taskPrecedencesDTO;


        }


        private async Task<List<TaskPrecedenceDTO>> _SaveTasksPrecedencesTasks(List<TaskPrecedencesTaskRequestDTO> taskprecedencesTasksRequest)
        {

            var UniqueTasks = new List<int>();
            foreach (var taskPredencesTask in taskprecedencesTasksRequest)
            {
                var taskId = taskPredencesTask.TaskId;
                if (!UniqueTasks.Contains(taskId))
                {
                    UniqueTasks.Add(taskId);
                }
                foreach (var precedenceId in taskPredencesTask.TaskPrecedences) {
                    if (!UniqueTasks.Contains(precedenceId))
                    {
                        UniqueTasks.Add(precedenceId);
                    }
                }
      
                
            }

            // validated task exited
            // validate precedence tasks exited
            var exitedTasks = await db.Tasks
                .Where(t => UniqueTasks.Contains(t.Id) & t.IsDelete == false)
                .ToListAsync();

            if (exitedTasks.Count > UniqueTasks.Count)
            {
                throw new Exception(PrecedenceMissingTaskMessage);
            }

            if (exitedTasks.Count < UniqueTasks.Count)
            {
                throw new Exception(NotFoundMessage);
            }



            // TODO: validate DAG graph
            // TODO: validate  edge node in graph

            var exitedPrecedenceTasks = await db.TaskPrecedences
                .Where(s => UniqueTasks.Contains(s.TaskId) | UniqueTasks.Contains(s.TaskId))
                .ToListAsync();

            //var exitedPrecedenceTasks = await db.TaskPrecedences
            //    .Where(s => s.IsDelete == true)
            //    .ToListAsync();


            // clean all precedence tasks of project id
            await _ClearTaskPrecedenceTask(exitedPrecedenceTasks);


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



        private async Task<List<TasksSkillsRequired>> _SaveTasksSkillsRequireds(List<TaskSkillsRequiredRequestDTO> taskSkillsRequiredRequests)
        {

            var uniqueSkills = new List<int>();
            foreach (var taskSkillsRequired in taskSkillsRequiredRequests)
            {
                var skillsRequired = taskSkillsRequired.SkillsRequireds;
                foreach (var skill in skillsRequired)
                    if (!uniqueSkills.Contains(skill.SkillId))
                {
                        uniqueSkills.Add(skill.SkillId);
                }
  

            }

            // validate skills exited
            var exitedSkills = await db.Skills
                .Where(t => uniqueSkills.Contains(t.Id) & t.IsDelete == false)
                .ToListAsync();

            if (exitedSkills.Count != uniqueSkills.Count)
            {
                throw new Exception("SKill not found");
            }


            // TODO: validate DAG graph
            // TODO: validate  edge node in graph

            
            


            // get all required skills by task ids

            // validate skills exited
            var requiredSkillsToRemove = await db.TasksSkillsRequireds
                .Where(t => taskSkillsRequiredRequests.Select(f=> f.TaskId).Contains( t.TaskId) & t.IsDelete == false)
                .ToListAsync();
        
            // clean all precedence tasks of project id
            await _ClearTaskSkillRequired(requiredSkillsToRemove);


            // mapping task precedences request -> task precedences database
            List<TasksSkillsRequired> tasksSkillsRequiredsToAdd = new List<TasksSkillsRequired>();
            foreach (var taskSkillsRequired in taskSkillsRequiredRequests)
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

            var TaskPrecedenceTasks = TasksSaveRequest.TaskPrecedenceTasks;
            await _SaveTasksPrecedencesTasks(TaskPrecedenceTasks);


            var TaskSkillsRequireds = TasksSaveRequest.TaskSkillsRequireds;
            await _SaveTasksSkillsRequireds(TaskSkillsRequireds);

            return true;

        }




            //    // Validate task precedence must exited

            //    // Validate task skill must exited





            //    // if task not exited -> create

            //    // if task exited -> update

            //    return output;
            //}
        }
}

