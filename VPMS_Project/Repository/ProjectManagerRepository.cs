using Microsoft.EntityFrameworkCore;
using VPMS_Project.Data;
using VPMS_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VPMS_Project.Repository
{
    public class ProjectManagerRepository
    {
        private readonly EmpStoreContext _context = null;

        public ProjectManagerRepository(EmpStoreContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectManagerModel>> GetProjectManager()
        {
            return await _context.PreSalesProjectManagers.Select(x => new ProjectManagerModel()
            {
                Id = x.Id,
                Name = x.Name,
                Descrption = x.Descrption
            }).ToListAsync();
        }
    }


}
