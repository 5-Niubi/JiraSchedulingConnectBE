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
    public class AlgorithmComputeService : IAlgorithmComputeService
    {
        private readonly JiraDemoContext db;
        private readonly HttpContext? http;
        private readonly IMapper mapper;
        public AlgorithmComputeService(JiraDemoContext db, IHttpContextAccessor httpAccessor, IMapper mapper)
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

            var taskFromDB = await db.Tasks.Where(t => t.ProjectId == parameterEntity.ProjectId)
               .Include(t => t.TasksSkillsRequireds).Include(t => t.TaskPrecedenceTasks)
               .Include(t => t.Milestone).ToListAsync();

            var skillFromDB = await db.Skills.Where(s => s.CloudId == cloudId).ToListAsync();

            inputTo.StartDate = (DateTime)parameterEntity.StartDate;
            inputTo.Deadline = (int)Utils.GetDaysBeetween2Dates
                (parameterEntity.StartDate, parameterEntity.Deadline);

            inputTo.Budget = (int)parameterEntity.Budget;
            inputTo.WorkerList = workerFromDB;

            inputTo.TaskList = taskFromDB.ToList();
            inputTo.SkillList = skillFromDB;

            inputTo.FunctionList = new List<Function>();
            inputTo.EquipmentList = new List<Equipment>();

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
                var schedules = new List<Schedule>();
                foreach (var algOutRaw in algorithmOutputRaws)
                {
                    var algOutConverted = converter.FromOR(algOutRaw.Genes,
                        new int[0], algOutRaw.TaskBegin, algOutRaw.TaskFinish);

                    algOutConverted.timeFinish = algOutRaw.TimeFinish;
                    algOutConverted.totalExper = algOutRaw.TotalExper;
                    algOutConverted.totalSalary = algOutRaw.TotalSalary;

                    algorithmOutputConverted.Add(algOutConverted);

                    var schedule = await InsertScheduleIntoDB(parameterId, algOutConverted);
                    schedules.Add(schedule);
                }

                await db.SaveChangesAsync();
                await db.Database.CommitTransactionAsync();

                scheduleResultDTOs = mapper.Map<List<ScheduleResultSolutionDTO>>(schedules);
            }
            catch (Exception ex)
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
                int parameterId, OutputFromORDTO algOutConverted
            )
        {
            // Insert result into Schedules table
            var schedule = new Schedule();
            schedule.ParameterId = parameterId;
            schedule.Duration = algOutConverted.timeFinish;
            schedule.Cost = algOutConverted.totalSalary;
            schedule.Quality = algOutConverted.totalExper;
            schedule.Tasks = JsonConvert.SerializeObject(algOutConverted.tasks);

            var scheduleSolution = await db.Schedules.AddAsync(schedule);
            return scheduleSolution.Entity;
        }
    }
}
