using AlgorithmServiceServer.DTOs.AlgorithmController;
using AlgorithmServiceServer.Services.Interfaces;
using AutoMapper;
using JiraSchedulingConnectAppService.Common;
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

            var parameterEntity = db.Parameters.Where(p => p.Id == parameterId)
                .Include(p => p.Project).FirstOrDefault() ??
                    throw new NotFoundException($"Can not find parameter with id: {parameterId}");

            var projectFromDB = parameterEntity.Project;
            var parameterResources = db.ParameterResources.Where(prs => prs.ParameterId == parameterId
                                    && prs.Type == Const.RESOURCE_TYPE.WORKFORCE)
                                    .Include(pr => pr.ResourceNavigation).ToList();

            var workerFromDB = new List<Workforce>();
            parameterResources.ForEach(e => workerFromDB.Add(e.ResourceNavigation));

            var taskFromDB = db.Tasks.Where(t => t.ProjectId == parameterEntity.ProjectId)
               .Include(t => t.TasksSkillsRequireds).Include(t => t.TaskPrecedenceTasks).ToList();
            var skillFromDB = db.Skills.Where(s => s.CloudId == cloudId).ToList();

            // Equipment
            //var functionFromDB = db.Functions.Where(f => f.CloudId == cloudId).ToList();
            //var equipmentsFromDB = db.Equipments.Where(e => e.CloudId == cloudId)
            //    .Include(eq => eq.EquipmentsFunctions)
            //    .ToList();
            // ---------

            inputTo.StartDate = (DateTime)projectFromDB.StartDate;
            inputTo.Deadline = (int)projectFromDB.Deadline.Value
                .Subtract(projectFromDB.StartDate.Value).TotalDays;

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

            scheduleResultDTOs.Add(scheduleSolutionDTO);
        }
    }
}
