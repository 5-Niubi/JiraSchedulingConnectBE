using AlgorithmLibrary;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Invalidation;
using ModelLibrary.DTOs.PertSchedule;
using UtilsLibrary.Exceptions;

namespace JiraSchedulingConnectAppService.Services
{
    public class TasksService : ITasksService
    {
        public const string TaskNotFoundMessage = "Task Not Found!";

        public const string SkillNotFoundMessage = "Skill Not Found!";
        public const string LevelSkillNotValidMessage = "Level Skill Not Valid!";
        public const string RequiredSkillNotValidMessage = "Skill Not Validate!";
        public const string RequiredSkillInputEmptyMessage = "Empty Required Skills";

        public const string MilestoneNotFoundMessage = "Group task not exited";
        public const string MilestoneNotEmptyMessage = "Group is not empty";


        public const string MissingMessage = "Missing Task!";


        public const string PrecedenceMissingTaskMessage = "Task Not Set Precedence!";
        public const string PrecedenceIsCycleMessage = "Tasks be cycle!";
        public const string RequiredSkillMissingTaskMessage = "Task Not Set Required SKill!";

        public const string ProjectNotFoundMessage = "Project Not Found!";
        public const string NotUniqueTaskNameMessage = "Task Name Is Exited!";
        public const string PredenceNotFoundMessage = "Predence Task  not Found!";
        public const string MilestoneNotValidMessage = "Group Task's not valid!";

        private readonly WoTaasContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public TasksService(WoTaasContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
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

        private async Task<bool> _ClearTaskPrecedenceTask(int projectId)
        {

            // validated task exited
            // validate precedence tasks exited
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

            await _ValidateEmptyInputTask(task);

            // validate exited name task  project's 
            //await _ValidateExitedTaskName(task);

            // validate milestone task
            await _ValidateMilestoneTask(task);

            // validate exited predences in this project
            await _ValidatePrecedenceTask(task);

            // validate required skills task's
            await _ValidateSkillsRequired(task);

            // validate is DAG graph
            // TODO 

            // insert task
            var taskCreatedEntity = await db.Tasks.AddAsync(task);
            await db.SaveChangesAsync();

            var taskPertViewDTO = mapper.Map<TaskPertViewDTO>(taskCreatedEntity.Entity);
            return taskPertViewDTO;

        }

        private async System.Threading.Tasks.Task<bool> _ValidateEmptyInputTask(ModelLibrary.DBModels.Task task)
        {

            var Messages = "";

            // validate duration

            if (task.Duration == null)
            {
                Messages += "Duration is Empty \n";

            }

            if (task.Duration == 0 || task.Duration < 0)
            {
                Messages += "Duration is not validate. Duration must > 0 \n";
            }

                if (task.MilestoneId == null)
            {
                Messages += "GroupId is Null\n";

            }

            if (task.Name == null || task.Name.Trim() == "")
            {
                Messages += "Name is Null \n";

            }

            if (task.ProjectId == null)
            {
                Messages += "ProjectId is Null \n";

            }

            if (task.TasksSkillsRequireds.Count() == 0)
            {
                Messages += "TasksSkillsRequireds is Empty \n";

            }


            if (Messages != "")
            {
                
                throw new NotSuitableInputException(new TaskInputErrorDTO()
                {
                    Messages = Messages
                });
            }


            return true;

        }

        public async Task<TaskPertViewDTO> GetTaskDetail(int Id)
        {

            var task = await db.Tasks
                .Include(t => t.TaskPrecedenceTasks)
                .Include(t => t.TasksSkillsRequireds)
                .FirstOrDefaultAsync(
                    t => t.Id == Id
                    && t.IsDelete == false);

            if (task == null)
            {
                throw new Exception(TaskNotFoundMessage);
            }

            var taskPertViewDTO = mapper.Map<TaskPertViewDTO>(task);

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
            var taskList = await db.Tasks
                .Include(tp => tp.TaskPrecedenceTasks)
                .Include(tk => tk.TasksSkillsRequireds)
                .Where(t => t.ProjectId == projectId
                & t.IsDelete == false).ToListAsync();


            var taskPertViewDTO = mapper.Map<List<TaskPertViewDTO>>(taskList);

            return taskPertViewDTO;


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
                && t.IsDelete == false
                && t.ProjectId == changingTask.ProjectId);

            if (oldTask == null)
            {
                throw new NotSuitableInputException(new TaskInputErrorDTO
                {
                    TaskId = changingTask.Id,
                    Messages = TaskNotFoundMessage
                });
            }


            if (changingTask.MilestoneId != null)
            {
                // Validate milestoneId exited
                await _ValidateExitedMilestone(taskRequest);

            }


            // validate required skills task's
            if (changingTask.TasksSkillsRequireds != null)
            {
                if (changingTask.TasksSkillsRequireds.Count() == 0)
                {

                    throw new NoSuitableWorkerException(RequiredSkillInputEmptyMessage);
                }

                await _ValidateSkillsRequired(changingTask);

            }

            // validate required skills task's
            //if (changingTask.TaskPrecedenceTasks != null)
            //{
            //    await _ValidatePrecedenceTask(changingTask);
            //}

            // TODO: not save precede
            //if (changingTask.TaskPrecedenceTasks != null)
            //{
            //    // clear
            //    await _ClearTaskPrecedenceByTaskId(taskRequest.Id);

            //    // insert new
            //    List<TaskPrecedence> precedenceTasksToAdd = new List<TaskPrecedence>();

            //    foreach (var precedence in taskRequest.Precedences)
            //    {
            //        precedenceTasksToAdd.Add(new TaskPrecedence()
            //        {
            //            TaskId = taskRequest.Id,
            //            PrecedenceId = precedence.PrecedenceId,
            //            IsDelete = false
            //        });
            //    }

            //    // insert new precedence tasks
            //    await db.AddRangeAsync(precedenceTasksToAdd);
            //    await db.SaveChangesAsync();

            //}

            if (changingTask.TasksSkillsRequireds != null)
            {

                await _ClearTaskSkillRequiredByTaskId(taskRequest.Id);

                // mapping task precedences request -> task precedences database
                List<TasksSkillsRequired> tasksSkillsRequiredsToAdd = new();

                foreach (var skill in taskRequest.SkillRequireds)
                {
                    var taskSkillRequired = new TasksSkillsRequired()
                    {
                        TaskId = taskRequest.Id,
                        SkillId = skill.SkillId,
                        Level = skill.Level,
                        IsDelete = false

                    };
                    tasksSkillsRequiredsToAdd.Add(taskSkillRequired);


                }

                // insert new precedence tasks
                await db.AddRangeAsync(tasksSkillsRequiredsToAdd);
                await db.SaveChangesAsync();
            }



            oldTask.Name = (changingTask.Name == null) ?
            oldTask.Name : changingTask.Name;

            oldTask.Duration = (changingTask.Duration == null) ?
                oldTask.Duration : changingTask.Duration;

            oldTask.MilestoneId = (changingTask.MilestoneId == null) ?
                oldTask.MilestoneId : changingTask.MilestoneId;


            db.Tasks.Update(oldTask);
            await db.SaveChangesAsync();

            var taskPertViewDTO = mapper.Map<TaskPertViewDTO>(oldTask);
            return taskPertViewDTO;
        }

        private async Task<List<TaskPrecedenceDTO>> _SaveTasksPrecedencesTasks(List<TaskPrecedencesTaskRequestDTO> taskprecedencesTasksRequest)
        {

            // mapping task precedences request -> task precedences database
            List<TaskPrecedence> precedenceTasksToAdd = new();
            foreach (var taskPrecedences in taskprecedencesTasksRequest)
            {
                foreach (var precedenceId in taskPrecedences.TaskPrecedences)
                {
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
            List<TasksSkillsRequired> tasksSkillsRequiredsToAdd = new();
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


        public async Task<bool> _UpdateTasks(List<ModelLibrary.DBModels.Task> Tasks)
        {

            db.Tasks.UpdateRange(Tasks);
            await db.SaveChangesAsync();
            return true;

        }



        public async Task<bool> SaveTasksPertChart(TasksSaveRequest TasksSaveRequest)
        {



            // validate project exited by cloud id
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var projectInDB = await db.Projects.FirstOrDefaultAsync(p => p.Id == TasksSaveRequest.ProjectId && p.CloudId == cloudId) ??
            throw new NotFoundException($"Can not find project :{TasksSaveRequest.ProjectId}");


            var projectId = TasksSaveRequest.ProjectId;
            var TaskPrecedenceTasksRequest = TasksSaveRequest.TaskPrecedenceTasks;
            var TaskSkillsRequiredsRequest = TasksSaveRequest.TaskSkillsRequireds;


            // TODO: all task setup on skill & precedence must exited on database
            // check all task setup precedence
            await _ValidateConfigTaskPrecedences(projectId, TaskPrecedenceTasksRequest);

        
            // check precedence task is validate
            await _ValidateDAG(TaskPrecedenceTasksRequest);

            // validate exited skill 
            await _ValidateExitedSkill(TaskSkillsRequiredsRequest);

            // check all Tasks project's must setup Required Skills
            await _ValidateConfigAllTaskSkillsRequireds(projectId, TaskSkillsRequiredsRequest);


            // clean all precedence tasks of project id
            await _ClearTaskPrecedenceTask(projectId);

            // clean all precedence tasks of project id
            await _ClearTaskSkillRequired(TaskSkillsRequiredsRequest);

            // save task skill required
            await _SaveTasksSkillsRequireds(TaskSkillsRequiredsRequest);

            // save task precedence
            await _SaveTasksPrecedencesTasks(TaskPrecedenceTasksRequest);

            return true;

        }

        //private async System.Threading.Tasks.Task _ValidateMilestoneUpdate(TaskUpdatedRequest taskUpdatedRequest)
        //{
        //    var jwt = new JWTManagerService(httpContext);
        //    var cloudId = jwt.GetCurrentCloudId();

        //    // validate milestone task  not empty
        //    if (taskUpdatedRequest.MilestoneId == null)
        //    {
        //        throw new NotSuitableInputException(new TaskInputErrorDTO
        //        {
        //            TaskId = taskUpdatedRequest.Id,
        //            MilestoneId = taskUpdatedRequest.MilestoneId,
        //            Messages = MilestoneNotEmptyMessage
        //        });
        //    }

        //    var milestoneId = await db.Milestones.FirstOrDefaultAsync(
        //        t => t.Id == taskUpdatedRequest.MilestoneId
        //        && t.ProjectId == taskUpdatedRequest.ProjectId);

        //    if(milestoneId == null)
        //    {
        //        throw new NotSuitableInputException(MilestoneNotFoundMessage);
        //    }
        //}

        public async Task<bool> DeleteTask(int TaskId)
        {

            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            // validated task exited
            var exitedTask = await db.Tasks
                .FirstOrDefaultAsync(
                    t => t.Id == TaskId
                    && t.IsDelete == false);

            if (exitedTask == null)
            {
                throw new NotSuitableInputException(TaskNotFoundMessage);
            }

            else
            {
                exitedTask.IsDelete = true;
                exitedTask.DeleteDatetime = DateTime.Now;

                // update delete task
                db.UpdateRange(exitedTask);
                await db.SaveChangesAsync();

                // clear required skill 
                await _ClearTaskSkillRequiredByTaskId(exitedTask.Id);


                // clear precedence
                await _ClearTaskPrecedenceByTaskId(exitedTask.Id);


            }
            return true;

        }


        private async Task<bool> _ClearTaskSkillRequiredByTaskId(int TaskId)
        {


            var requiredSkillsToRemove = await db.TasksSkillsRequireds
               .Where(t => t.TaskId == TaskId)
               .ToListAsync();

            db.RemoveRange(requiredSkillsToRemove);
            await db.SaveChangesAsync();
            return true;
        }

        private async Task<bool> _ClearTaskPrecedenceByTaskId(int TaskId)
        {

            var exitedPrecedenceTasks = await db.TaskPrecedences
                .Where(t => t.TaskId == TaskId || t.PrecedenceId == TaskId)
                .ToListAsync();


            // TODO: improve clean data -> not 
            db.RemoveRange(exitedPrecedenceTasks);
            await db.SaveChangesAsync();
            return true;
        }






        //public async Task<bool> TasksSaveRequestV2(TasksSaveRequestV2 TasksSaveRequest)
        //{

        //    var jwt = new JWTManagerService(httpContext);
        //    var cloudId = jwt.GetCurrentCloudId();


        //    var projectId = TasksSaveRequest.ProjectId;

        //    var TasksRequests = TasksSaveRequest.TaskSaveRequests;

        //    var ListTask = new List<ModelLibrary.DBModels.Task>();
        //    var TaskPrecedenceTasksRequest = new List<TaskPrecedencesTaskRequestDTO>();
        //    var TaskSkillsRequiredsRequest = new List<TaskSkillsRequiredRequestDTO>();
        //    foreach (var task in TasksRequests)
        //    {


        //        ListTask.Add(new ModelLibrary.DBModels.Task()
        //        {

        //            Id = task.TaskId,
        //            Name = task.Name,
        //            Duration = task.Duration,
        //            CloudId = cloudId,
        //            ProjectId = projectId,
        //            Milestone = new Milestone()
        //            {
        //                Id = task.MileStoneId
        //            }
        //        });

        //        TaskPrecedenceTasksRequest.Add(new TaskPrecedencesTaskRequestDTO()
        //        {
        //            TaskId = task.TaskId,
        //            TaskPrecedences = task.TaskPrecedences
        //        });

        //        TaskSkillsRequiredsRequest.Add(new TaskSkillsRequiredRequestDTO()
        //        {
        //            TaskId = task.TaskId,
        //            SkillsRequireds = task.TaskSkillsRequireds
        //        });

        //    }


        //    // TODO: all task setup on skill & precedence must exited on database
        //    // check all task setup precedence
        //    await _ValidateConfigTaskPrecedences(projectId, TaskPrecedenceTasksRequest);

        //    // check precedence task is validate
        //    await _ValidateExitedPrecedenceTask(TaskPrecedenceTasksRequest);

        //    // clean all precedence tasks of project id
        //    await _ClearTaskPrecedenceTask(projectId);


        //    // check all Tasks project's must setup Required Skills
        //    await _ValidateConfigAllTaskSkillsRequireds(projectId, TaskSkillsRequiredsRequest);


        //    // validate exited skill 
        //    await _ValidateExitedSkill(TaskSkillsRequiredsRequest);


        //    // update task info
        //    await _UpdateTasks(ListTask);

        //    // clean all precedence tasks of project id
        //    await _ClearTaskSkillRequired(TaskSkillsRequiredsRequest);

        //    // save task skill required
        //    await _SaveTasksSkillsRequireds(TaskSkillsRequiredsRequest);

        //    // save task precedence
        //    await _SaveTasksPrecedencesTasks(TaskPrecedenceTasksRequest);





        //    //db.UpdateRange()
        //    //    //var TaskSkillsRequiredsRequest = TasksSaveRequest.TaskSkillsRequireds;

        //    //    // TODO: all task setup on skill & precedence must exited on database
        //    //    // check all task setup precedence
        //    //    await _ValidateConfigTaskPrecedences(projectId, TaskPrecedenceTasksRequest);

        //    //    // check precedence task is validate
        //    //    await _ValidateExitedPrecedenceTask(TaskPrecedenceTasksRequest);

        //    //    // clean all precedence tasks of project id
        //    //    await _ClearTaskPrecedenceTask(projectId);


        //    //    // check all Tasks project's must setup Required Skills
        //    //    await _ValidateConfigAllTaskSkillsRequireds(projectId, TaskSkillsRequiredsRequest);


        //    //    // validate exited skill 
        //    //    await _ValidateExitedSkill(TaskSkillsRequiredsRequest);

        //    //    // clean all precedence tasks of project id
        //    //    await _ClearTaskSkillRequired(TaskSkillsRequiredsRequest);

        //    //    // save task skill required
        //    //    await _SaveTasksSkillsRequireds(TaskSkillsRequiredsRequest);

        //    //    // save task precedence
        //    //    await _SaveTasksPrecedencesTasks(TaskPrecedenceTasksRequest);

        //    return true;

        //}



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
                            Messages = PredenceNotFoundMessage
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
            // validate milestone task  not empty
            if(task.MilestoneId == null)
            {
                throw new NotSuitableInputException(new TaskInputErrorDTO
                {
                    TaskId = task.Id,
                    MilestoneId = task.MilestoneId,
                    Messages = MilestoneNotEmptyMessage
                });
            }

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
                            Messages = SkillNotFoundMessage
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

                if (SkillErrors.Count != 0)
                {
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

        private async Task<bool> _ValidateExitedMilestone(TaskUpdatedRequest task)
        {
            // validate exited name task  project's 
            var existingMileStone = await db.Milestones.FirstOrDefaultAsync(
                t => t.ProjectId == task.ProjectId && t.Id == task.MilestoneId);


            if (existingMileStone == null)
            {

                throw new NotSuitableInputException(
                    new TaskInputErrorDTO
                    {
                        TaskId = task.Id,
                        Messages = MilestoneNotValidMessage
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
                            SkillRequired = mapper.Map<SkillRequiredDTO>(skill),
                            Messages = skill.SkillId + " Not Found",

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

            // Check all task exited in project must include required skill
            foreach (var task in exitedTasks)
            {
                var taskSkillReq = taskSkillsRequiredsRequest.Where(t => t.TaskId == task.Id).FirstOrDefault();
                if (taskSkillReq == null || taskSkillReq.SkillsRequireds.Count == 0)
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

        private async Task<bool> _ValidateDAG(List<TaskPrecedencesTaskRequestDTO> taskprecedencesTasksRequest)
        {


            var Errors = new List<TaskSaveInputErrorDTO>();

            // TODO: Is validate DAG
            var graph = new DirectedGraph(0);

            graph.LoadData(taskprecedencesTasksRequest);


            var isDAG = graph.IsDAG();
            if (isDAG == false)
            {
                Errors.Add(new TaskSaveInputErrorDTO()
                {
                    Messages = PrecedenceIsCycleMessage
                });
            }

            if (Errors.Count != 0)
            {
                throw new NotSuitableInputException(Errors);
            }

            return true;

        }


        private async Task<bool> _ValidateTasksExited(int ProjectId, List<ModelLibrary.DBModels.Task> Tasks)
        {

            var Errors = new List<TaskInputErrorDTO>();

            var exitedTasks = await db.Tasks
                .Where(p => p.ProjectId == ProjectId && p.IsDelete == true)
                .ToListAsync();

            foreach (var task in exitedTasks)
            {
                if (!Tasks.Select(t => t.Id).Contains(task.Id))
                {

                    Errors.Add(
                        new TaskInputErrorDTO
                        {
                            TaskId = task.Id,
                            Messages = MissingMessage
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


    }



}

