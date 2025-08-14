using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.Domain.Context
{
    public partial class ViewContext : DbContext
    {
        public bool DisableCache { get; set; }

        public ViewContext()
        {
        }

        public ViewContext(DbContextOptions<ViewContext> options)
            : base(options)
        {
            DisableCache = false;
        }

        public ViewContext(DbContextOptions<ViewContext> options, ViewContextOption viewContextOption)
            : base(options)
        {
            if (viewContextOption != null)
            {
                DisableCache = viewContextOption.DisableCache;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.HasAnnotation("ProductVersion", "2.1.1");
        }
    }
}