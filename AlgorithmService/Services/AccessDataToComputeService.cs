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

        public async Task<List<ScheduleResultSolutionDTO>> GetDataToCompute(int projectId, int parameterId)
        {
            var cloudId = new JWTManagerService(http).GetCurrentCloudId();
            var inputTo = new InputToORDTO();

            var projectFromDB = await db.Projects
                .Where(p => p.CloudId == cloudId && p.Id == projectId)
                .Include(p => p.Tasks)
                .ThenInclude(t => t.TaskPrecedenceTasks)
                .FirstOrDefaultAsync();
            if (projectFromDB == null)
            {
                throw new NotFoundException($"Can not find project with id: {projectId}");
            }
            var taskFromDB = db.Tasks.Where(t => t.ProjectId == projectId)
                .Include(t => t.TasksSkillsRequireds).ToList();

            var parameterLatest = db.Parameters.Where(p => p.Id == parameterId).FirstOrDefault();
            if (parameterLatest == null)
            {
                throw new NotFoundException($"Can not find parameter with id: {parameterId}");
            }

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
            inputTo.TaskList = projectFromDB.Tasks.ToList();
            inputTo.WorkerList = workerFromDB;
            inputTo.SkillList = skillFromDB;
            inputTo.FunctionList = functionFromDB;
            inputTo.EquipmentList = equipmentsFromDB;

            var converter = new AlgorithmConverter(inputTo);

            var outputToAlgorithm = converter.ToOR();
            var ga = new GAExecution();
            ga.SetParam(outputToAlgorithm);
            var algorithmOutputRaws = ga.Run();
            var algorithmOutputConverted = new List<OutputFromORDTO>();
            var scheduleResultDTOs = new List<ScheduleResultSolutionDTO>();


            foreach (var algOutRaw in algorithmOutputRaws)
            {
                var algOutConverted = converter.FromOR(algOutRaw.Genes,
                    new int[0], algOutRaw.TaskBegin, algOutRaw.TaskFinish);
                algorithmOutputConverted.Add(algOutConverted);


                // Insert result into Schedules table
                var schedule = new Schedule();
                schedule.ParameterId = parameterId;
                schedule.Duration = algOutConverted.TimeFinish;
                schedule.Cost = algOutConverted.TotalSalary;
                schedule.Quality = algOutConverted.TotalExper;

                schedule.Tasks = JsonConvert.SerializeObject(algOutConverted.Tasks);

                var scheduleSolution = db.Schedules.Add(schedule).Entity;
                var scheduleSolutionDTO = mapper.Map<ScheduleResultSolutionDTO>(scheduleSolution);
                scheduleResultDTOs.Add(scheduleSolutionDTO);
            }
            db.SaveChanges();


            return scheduleResultDTOs;

        }
    }
}
