using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;
using Newtonsoft.Json.Linq;

namespace JiraSchedulingConnectAppService.Services
{
    public class WorkforcesService : IWorkforcesService
    {
        private readonly JiraDemoContext db;
        private readonly IMapper mapper;
        private readonly HttpContext? httpContext;

        public WorkforcesService(JiraDemoContext dbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            db = dbContext;
            this.mapper = mapper;
            httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<List<WorkforceDTO.Response>> GetAllWorkforces()
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();
                var query = await db.Workforces.ToListAsync();
                var queryDTOResponse = mapper.Map<List<WorkforceDTO.Response>>(query);
                return queryDTOResponse;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }

        }

        public async Task<WorkforceDTO.Response> CreateWorkforce(WorkforceDTO.Request? workforceRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();
                var workforce = mapper.Map<Workforce>(workforceRequest);
                workforce.CloudId = cloudId;

                await db.Workforces.AddAsync(workforce);
                await db.SaveChangesAsync();
                var workforceDTOResponse = mapper.Map<WorkforceDTO.Response>(workforce);
                return workforceDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTO.Request> GetWorkforceById(string workforce_id)
        {
            Workforce workforce = new Workforce();
            try
            {
                workforce = await db.Workforces.SingleOrDefaultAsync(x => x.Id.ToString() == workforce_id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            var workforceDTORequest = mapper.Map<WorkforceDTO.Request>(workforce);
            return workforceDTORequest;
        }

        public async System.Threading.Tasks.Task DeleteWorkforce(WorkforceDTO.Request workforceRequest)
        {
            try
            {
                var workforce = await db.Workforces.SingleOrDefaultAsync(x => x.Id == workforceRequest.Id);
                db.Workforces.Remove(workforce);
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTO.Response> UpdateWorkforce(WorkforceDTO.Request workforceRequest)
        {
            try
            {
                var workforce = mapper.Map<Workforce>(workforceRequest);
                db.Update(workforce);
                await db.SaveChangesAsync();
                var workforceDTOResponse = mapper.Map<WorkforceDTO.Response>(workforce);
                return workforceDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}

