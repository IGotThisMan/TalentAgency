using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalentAgency.Areas.Identity.Data;

namespace TalentAgency.Data
{
    public class TalentAgencyContext : IdentityDbContext<TalentAgencyUser>
    {
        public TalentAgencyContext(DbContextOptions<TalentAgencyContext> options)
            : base(options)
        {
        }
        //public DbSet<TalentAgency.Models.Files> Files { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
