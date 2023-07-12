using AlgorithmServiceServer.DTOs.AlgorithmController;
using AlgorithmServiceServer.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using Newtonsoft.Json;
using RcpspAlgorithmLibrary;
using RcpspAlgorithmLibrary.GA;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace AlgorithmServiceServer.Services
{
    public class AccessDataToComputeService : IAccessDataToComputeService
    {
        private readonly JiraDemoContext db;
        private readonly HttpContext http;
        private readonly IMapper mapper;
        public AccessDataToComputeService(JiraDemoContext db, IHttpContextAccessor httpAccessor, IMapper mapper)
        {
            this.db = db;
            http = httpAccessor.HttpContext;
            this.mapper = mapper;
        }

        public async Task<List<ScheduleResultSolutionDTO>> GetDataToCompute(int parameterId)
        {
            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var inputTo = new InputToORDTO();
            var parameterLatest = db.Parameters.Where(p => p.Id == parameterId)
                .Include(p => p.Project).FirstOrDefault();
            if (parameterLatest == null)
            {
                throw new NotFoundException($"Can not find parameter with id: {parameterId}");
            }
            var projectFromDB = parameterLatest.Project;
            var taskFromDB = db.Tasks.Where(t => t.ProjectId == parameterLatest.ProjectId)
               .Include(t => t.TasksSkillsRequireds).Include(t => t.TaskPrecedenceTasks).ToList();


            var workerFromDB = db.Workforces.Where(w => w.CloudId == cloudId)
                .Include(w => w.WorkforceSkills)
                .ToList();
            var skillFromDB = db.Skills.Where(s => s.CloudId == cloudId).ToList();

            var functionFromDB = db.Functions.Where(f => f.CloudId == cloudId).ToList();
            var equipmentsFromDB = db.Equipments.Where(e => e.CloudId == cloudId)
                .Include(eq => eq.EquipmentsFunctions)
                .ToList();

            inputTo.StartDate = (DateTime)projectFromDB.StartDate;
            inputTo.Deadline = (int)projectFromDB.Deadline.Value
                .Subtract(projectFromDB.StartDate.Value).TotalDays;

            inputTo.Budget = (int)parameterLatest.Budget;
            inputTo.TaskList = taskFromDB.ToList();
            inputTo.WorkerList = workerFromDB;
            inputTo.SkillList = skillFromDB;
            inputTo.FunctionList = functionFromDB;
            inputTo.EquipmentList = equipmentsFromDB;

            var converter = new AlgorithmConverter(inputTo, mapper);

            var outputToAlgorithm = converter.ToOR();
            var ga = new GAExecution();
            ga.SetParam(outputToAlgorithm);
            var algorithmOutputRaws = ga.Run();
            var algorithmOutputConverted = new List<OutputFromORDTO>();

            var scheduleResultDTOs = new List<ScheduleResultSolutionDTO>();
            await db.Database.BeginTransactionAsync();
            try
            {

                foreach (var algOutRaw in algorithmOutputRaws)
                {
                    var algOutConverted = converter.FromOR(algOutRaw.Genes,
                        new int[0], algOutRaw.TaskBegin, algOutRaw.TaskFinish);
                    algorithmOutputConverted.Add(algOutConverted);


                    InsertScheduleIntoDB(parameterId, algOutConverted, scheduleResultDTOs);
                }
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                await db.Database.RollbackTransactionAsync();
            }
            finally
            {
                await db.Database.CloseConnectionAsync();
            }
            return scheduleResultDTOs;
        }

        public Task<List<ScheduleResultSolutionDTO>> GetDataToCompute(int projectId, int parameterId)
        {
            throw new NotImplementedException();
        }

        private async void InsertScheduleIntoDB(int parameterId, OutputFromORDTO algOutConverted,
            List<ScheduleResultSolutionDTO> scheduleResultDTOs)
        {
            // Insert result into Schedules table
            var schedule = new Schedule();
            schedule.ParameterId = parameterId;
            schedule.Duration = algOutConverted.timeFinish;
            schedule.Cost = algOutConverted.totalSalary;
            schedule.Quality = algOutConverted.totalExper;

            schedule.Tasks = JsonConvert.SerializeObject(algOutConverted.tasks);

            var scheduleSolution = (await db.Schedules.AddAsync(schedule)).Entity;
            var scheduleSolutionDTO = mapper.Map<ScheduleResultSolutionDTO>(scheduleSolution);

            //foreach (var task in algOutConverted.tasks)
            //{
            //    var scheduleTask = new ScheduleTask();
            //    scheduleTask.Startdate = task.startDate;
            //    scheduleTask.Enddate = task.endDate;
            //    scheduleTask.ScheduleId = scheduleSolution.Id;
            //    scheduleTask.TaskId = task.id;

            //    scheduleTask = (await db.ScheduleTasks.AddAsync(scheduleTask)).Entity;
            //    var taskResource = new ScheduleTaskResource();
            //    taskResource.ScheduleTaskId = scheduleTask.Id;
            //    taskResource.ResourceId = task.workerId;
            //    taskResource.Type = Const.RESOURCE_TYPE.WORKFORCE;

            //    // Pending Equipment
            //    await db.ScheduleTaskResources.AddAsync(taskResource);
            //}
            scheduleResultDTOs.Add(scheduleSolutionDTO);
        }
    }
}
