using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TalentAgency.Models;

namespace TalentAgency.Data
{
    public class TalentAgencyNonIdentityContext : DbContext
    {
        public TalentAgencyNonIdentityContext (DbContextOptions<TalentAgencyNonIdentityContext> options)
            : base(options)
        {
        }

        public DbSet<TalentAgency.Models.Apply> Apply { get; set; }

        public DbSet<TalentAgency.Models.Event> Event { get; set; }
    }
}
