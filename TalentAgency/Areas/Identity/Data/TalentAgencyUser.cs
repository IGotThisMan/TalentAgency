using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TalentAgency.Models;

namespace TalentAgency.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the TalentAgencyUser class
    public class TalentAgencyUser : IdentityUser
    {
        [PersonalData]
        public string talent_fname { get; set; }
        [PersonalData]
        public string talent_lname { get; set; }
        [PersonalData]
        public string gender { get; set; }
        [PersonalData]
        public int age { get; set; }
        [PersonalData]
        public string user_role { get; set; }

        public IList<Event> events { get; set; }

    }
}
