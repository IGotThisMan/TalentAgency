using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentAgency.Areas.Identity.Data;

namespace TalentAgency.Models
{
    public class Apply
    {
        [Key]
        public string Apply_id { get; set; }

        public string email { get; set; }

        public string Event_name { get; set; }
              
        public string Status { get; set; }

        [Required] 
        [Display(Name = "Self Introduction")]
        [DataType(DataType.MultilineText)]
        public string Introduction { get; set; }

        public DateTime Date_created { get; set; }

    }
}
