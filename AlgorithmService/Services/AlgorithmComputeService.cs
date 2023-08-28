using AlgorithmLibrary;
using AlgorithmLibrary.GA;
using AlgorithmLibrary.Solver;
using AlgorithmServiceServer.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs.Algorithm;
using ModelLibrary.DTOs.Algorithm.ScheduleResult;
using System.Text.Json;
using UtilsLibrary;
using UtilsLibrary.Exceptions;

namespace AlgorithmServiceServer.Services
{
    public class AlgorithmComputeService : IAlgorithmComputeService
    {
        private readonly WoTaasContext db;
        private readonly HttpContext? http;
        private readonly IMapper mapper;
        public AlgorithmComputeService(WoTaasContext db, IHttpContextAccessor httpAccessor, IMapper mapper)
        {
            this.db = db;
            http = httpAccessor.HttpContext;
            this.mapper = mapper;
        }

        public async Task<List<ScheduleResultSolutionDTO>> GetDataToCompute(int parameterId)
        {
            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var inputTo = new InputToORDTO();

            var parameterEntity = await db.Parameters.Where(p => p.Id == parameterId)
                .Include(p => p.Project).FirstOrDefaultAsync() ??
                    throw new NotFoundException($"Can not find parameter with id: {parameterId}");

            var projectFromDB = parameterEntity.Project;
            var parameterResources = await db.ParameterResources.Where(prs => prs.ParameterId == parameterId
                                    && prs.Type == Const.RESOURCE_TYPE.WORKFORCE)
                                    .Include(pr => pr.Resource).ThenInclude(w => w.WorkforceSkills).ToListAsync();

            var workerFromDB = new List<Workforce>();
            parameterResources.ForEach(e => workerFromDB.Add(e.Resource));

            var taskFromDB = await db.Tasks.Where(t => t.ProjectId == parameterEntity.ProjectId && t.IsDelete == false)
               .Include(t => t.TasksSkillsRequireds).Include(t => t.TaskPrecedenceTasks)
               .Include(t => t.Milestone).ToListAsync();

            var skillFromDB = await db.Skills.Where(s => s.CloudId == cloudId && s.IsDelete == false).ToListAsync();

            inputTo.StartDate = (DateTime)parameterEntity.StartDate;
            var deadline = (int)Utils.GetDaysBeetween2Dates
                (parameterEntity.StartDate, parameterEntity.Deadline);
            inputTo.Deadline = (deadline == 0) ? 1 : deadline;

            inputTo.Budget = parameterEntity.Budget;
            inputTo.WorkerList = workerFromDB;

            inputTo.TaskList = taskFromDB.ToList();
            inputTo.SkillList = skillFromDB;

            inputTo.FunctionList = new List<Function>();
            inputTo.EquipmentList = new List<Equipment>();

            inputTo.ObjectiveTime = parameterEntity.ObjectiveTime;
            inputTo.ObjectiveCost = parameterEntity.ObjectiveCost;
            inputTo.ObjectiveQuality = parameterEntity.ObjectiveQuality;
            inputTo.BaseWorkingHours = projectFromDB?.BaseWorkingHour ?? Const.DEFAULT_BASE_WORKING_HOUR;

            // Execute converter
            var converter = new AlgorithmConverter(inputTo, mapper);
            var outputToAlgorithm = converter.ToOR();

            List<AlgorithmRawOutput> algorithmOutputRaws = new();

            switch (parameterEntity.Optimizer)
            {
                case Const.OPTIMIZER.GA:
                    // Running with GA
                    var ga = new GAExecution();
                    ga.SetParam(outputToAlgorithm);
                    algorithmOutputRaws = ga.Run();

                    break;
                case Const.OPTIMIZER.SOLVER:
                    // Running with Solver
                    algorithmOutputRaws = CPSAT.Schedule(outputToAlgorithm);
                    break;
            }

            var algorithmOutputConverted = new List<OutputFromORDTO>();
            var scheduleResultDTOs = new List<ScheduleResultSolutionDTO>();
            await db.Database.BeginTransactionAsync();
            try
            {
                var schedules = new List<Schedule>();
                foreach (var algOutRaw in algorithmOutputRaws)
                {
                    var algOutConverted = converter.FromOR(algOutRaw.Genes,
                        new int[0], algOutRaw.TaskBegin, algOutRaw.TaskFinish);

                    algOutConverted.timeFinish = algOutRaw.TimeFinish;
                    algOutConverted.totalExper = algOutRaw.TotalExper;

                    var totalSalary = CalculateTotalSalary(projectFromDB, algOutConverted);
                    algOutConverted.totalSalary = totalSalary;

                    algorithmOutputConverted.Add(algOutConverted);

                    var schedule = await InsertScheduleIntoDB(parameterEntity, algOutConverted);
                    schedules.Add(schedule);
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();

                scheduleResultDTOs = mapper.Map<List<ScheduleResultSolutionDTO>>(schedules);
            }
            catch (Exception)
            {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await db.Database.CloseConnectionAsync();
            }
            return scheduleResultDTOs;
        }

        private async Task<Schedule> InsertScheduleIntoDB(
                Parameter parameter, OutputFromORDTO algOutConverted
            )
        {
            // Insert result into Schedules table
            var schedule = new Schedule();
            schedule.ParameterId = parameter.Id;
            schedule.Since = parameter.StartDate;
            schedule.Duration = algOutConverted.timeFinish;
            schedule.Cost = algOutConverted.totalSalary;
            schedule.Quality = algOutConverted.totalExper;
            schedule.Tasks = JsonSerializer.Serialize(algOutConverted.tasks);
            schedule.Title = Const.SCHEDULE.DEFAULT_TITLE;
            var scheduleSolution = await db.Schedules.AddAsync(schedule);
            return scheduleSolution.Entity;
        }

        private long CalculateTotalSalary(Project project, OutputFromORDTO algOutConverted)
        {
            var baseWorkingHour = project.BaseWorkingHour;

            var workerSalaryDict = new Dictionary<WorkforceScheduleResultDTO, long>();
            var tasks = algOutConverted.tasks;

            // Filter all worker from tasks
            foreach (var task in tasks)
            {
                if (workerSalaryDict.Keys.Where(t => task.workforce.id == t.id).Count() == 0)
                {
                    workerSalaryDict.Add(task.workforce, 0);
                }
            }

            foreach (var wKey in workerSalaryDict.Keys)
            {
                var totalDurationOfWker = tasks.Where(t => t.workforce.id == wKey.id).Sum(t => t.duration);
                var totalCostOfWker = totalDurationOfWker * (int)baseWorkingHour * wKey.unitSalary;
                workerSalaryDict[wKey] = totalCostOfWker ?? 0;
            }

            //var totalCost = workerSalaryDict.Values.Sum();
            var totalCost = algOutConverted.totalSalary;
            return totalCost;
        }
    }
}
