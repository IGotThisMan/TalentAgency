using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentAgency.Areas.Identity.Data;

namespace TalentAgency.Models
{
    public class Event
    {
        [Key]
        public string Event_id { get; set; }
        [Required]     
        public string Event_name { get; set; }

        public string email { get; set; }

        [Required]
        public string description { get; set; }

        public DateTime date_created { get; set; }


    }
}
