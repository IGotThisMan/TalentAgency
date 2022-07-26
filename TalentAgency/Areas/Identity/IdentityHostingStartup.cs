using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentAgency.Areas.Identity.Data;
using TalentAgency.Data;

[assembly: HostingStartup(typeof(TalentAgency.Areas.Identity.IdentityHostingStartup))]
namespace TalentAgency.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TalentAgencyContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TalentAgencyContextConnection")));

                services.AddDefaultIdentity<TalentAgencyUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<TalentAgencyContext>();
            });
        }
    }
}