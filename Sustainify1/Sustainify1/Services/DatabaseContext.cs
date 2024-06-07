using Sustainify1.Model;
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Sustainify1.Services
{
    
        public class DatabaseContext:DbContext
        {
            public DbSet<SustainifyModel> Sustainifies { get; set; }

            public DatabaseContext()
            {
                this.Database.EnsureCreated();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Sustainify.db");
                optionsBuilder.UseSqlite($"Filename={dbPath}");
            }
        }
}
