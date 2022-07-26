using TalentAgency.Areas.Identity.Data;
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace TalentAgency.Models
{
    public class Files
    {
        [Key]
        public string File_id { get; set; }

        public string profile_picture { get; set; }

        public string cv_file { get; set; }

        public string email { get; set; }

        public Files[] getsessionemail(string email)
        {
            Files[] records = null;
            SqlConnection cnn = new SqlConnection("Server=(localdb)\\mssqllocaldb;Database=TalentAgency;Trusted_Connection=True;MultipleActiveResultSets=true");
            string sql = "select * from Files where email = 'test1@mail.com'";
            using (var command = new SqlCommand(sql, cnn))
            {
                cnn.Open();
                using (var reader = command.ExecuteReader())
                {
                    var list = new List<Files>();
                    while (reader.Read())
                        list.Add(new Files
                        {
                            File_id = reader.GetString(0),
                            profile_picture = reader.GetString(1),
                            cv_file = reader.GetString(2),
                            email = reader.GetString(3)
                        });
                    records = list.ToArray();
                    return records;
                }
            }
        }


    }
}
