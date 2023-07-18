using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Parameters;
using ModelLibrary.DTOs.Projects;
using ModelLibrary.DTOs.Skills;
using Newtonsoft.Json.Linq;
using JiraSchedulingConnectAppService.Common;
using JiraSchedulingConnectAppService.Services.Interfaces;

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
        public async Task<List<WorkforceDTOResponse>> GetAllWorkforces(List<int>? Ids)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var query = (Ids == null) ?
                    await db.Workforces.ToListAsync() : await db.Workforces.Where(
                    W => Ids.Contains(W.Id) == true).ToListAsync();
                var queryDTOResponse = mapper.Map<List<WorkforceDTOResponse>>(query);
                return queryDTOResponse;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        



        public async Task<WorkforceDTOResponse> CreateWorkforce(WorkforceDTORequest? workforceRequest)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();
                var workforce = mapper.Map<Workforce>(workforceRequest);
                workforce.CloudId = cloudId;

                var workforceEntity = await db.Workforces.AddAsync(workforce);
                await db.SaveChangesAsync();
                var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(workforceEntity.Entity);
                return workforceDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTOResponse> GetWorkforceById(string workforce_id)
        {
            try
            {
                var workforce = await db.Workforces.Where(e => e.Id.ToString() == workforce_id).FirstOrDefaultAsync();
                var workforceResponse = mapper.Map<WorkforceDTOResponse>(workforce);
                return workforceResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTOResponse> DeleteWorkforce(string? workforce_id)
        {
            try
            {
                var workforce = await db.Workforces.SingleOrDefaultAsync(x => x.Id.ToString() == workforce_id);
                if(workforce == null) {
                    return null;
                }
                workforce.IsDelete = Const.DELETE_STATE.DELETE;
                workforce.DeleteDatetime = DateTime.Now;
                var workforceEntity = db.Workforces.Update(workforce);
                await db.SaveChangesAsync();
                var workforceResponse = mapper.Map<WorkforceDTOResponse>(workforceEntity.Entity);
                return workforceResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<WorkforceDTOResponse> UpdateWorkforce(WorkforceDTORequest workforceRequest)
        {
            try
            {
                var workforce = mapper.Map<Workforce>(workforceRequest);
                var workforceEntity = db.Update(workforce);
                await db.SaveChangesAsync();
                var workforceDTOResponse = mapper.Map<WorkforceDTOResponse>(workforceEntity.Entity);
                return workforceDTOResponse;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}

