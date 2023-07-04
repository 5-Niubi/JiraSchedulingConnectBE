using System;
using AutoMapper;
using JiraSchedulingConnectAppService.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.DBModels;
using ModelLibrary.DTOs;
using ModelLibrary.DTOs.Projects;

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

        public async Task<WorkforceDTO> CreateWorkforce(WorkforceDTO? workforce)
        {
            try
            {
                var jwt = new JWTManagerService(httpContext);
                var cloudId = jwt.GetCurrentCloudId();

                var w1 = mapper.Map<Workforce>(workforce);
                w1.CloudId = cloudId;
                var projectCreatedEntity = await db.Workforces.AddAsync(w1);
                await db.SaveChangesAsync();
                return workforce;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async System.Threading.Tasks.Task DeleteWorkforce(Workforce w)
        {
            try
            {
                var w1 = await db.Workforces.SingleOrDefaultAsync(x => x.Id == w.Id);
                db.Workforces.Remove(w1);
                await db.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<Workforce>> GetAllWorkforces()
        {
            var jwt = new JWTManagerService(httpContext);
            var cloudId = jwt.GetCurrentCloudId();

            var query = await db.Workforces.ToListAsync();

            return query;
        }

        public async Task<Workforce> GetWorkforceById(string workforce_id)
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
            return workforce;
        }

        public async Task<WorkforceDTO> UpdateWorkforce(WorkforceDTO w)
        {
            try
            {
                var w1 = mapper.Map<Workforce>(w);
                db.Update(w1);
                await db.SaveChangesAsync();
                return w;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }

}

