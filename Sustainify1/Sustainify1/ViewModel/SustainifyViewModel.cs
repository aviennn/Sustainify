using Sustainify1.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Sustainify1.ViewModel
{
    public class SustainifyViewModel
    {
        private Services.DatabaseContext getContext()
        {
            return new Services.DatabaseContext();
        }

        public int InsertAction(SustainifyModel obj)
        {
            var _dbContext = getContext();
            _dbContext.Sustainifies.Add(obj);
            int c = _dbContext.SaveChanges();
            return c;
        }

        public async Task<List<SustainifyModel>> GetAllSustanifies()
        {
            var _dbContext = getContext();
            var res = await _dbContext.Sustainifies.ToListAsync();
            return res;
        }

        public async Task<int> UpdateAction(SustainifyModel obj)
        {
            var _dbContext = getContext();
            _dbContext.Sustainifies.Update(obj);
            int c = await _dbContext.SaveChangesAsync();
            return c;
        }

        public int DeleteAction(SustainifyModel obj)
        {
            var _dbContext = getContext();
            _dbContext?.Sustainifies.Remove(obj);
            int c = _dbContext.SaveChanges();
            return c;
        }
    }
}
