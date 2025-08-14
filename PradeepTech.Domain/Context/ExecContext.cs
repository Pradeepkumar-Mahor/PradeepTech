// Ignore Spelling: Pradeep

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.Domain.Context
{
    public partial class ExecContext : DbContext
    {
        public bool DisableCache { get; set; }

        public ExecContext()
        {
        }

        public ExecContext(DbContextOptions<ExecContext> options)
            : base(options)
        {
            DisableCache = false;
        }

        public ExecContext(DbContextOptions<ExecContext> options, ExecContextOption execContextOption)
            : base(options)
        {
            if (execContextOption != null)
            {
                DisableCache = execContextOption.DisableCache;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.HasAnnotation("ProductVersion", "2.1.1");
        }
    }
}