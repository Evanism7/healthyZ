using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace healthyZ.Models
{
    [Table("login")]
    public class login : BaseModel
    {
        [PrimaryKey("id")]
        public int id { get; set; }
        [Column("account_id")]
        public string account_id { get; set; }
        [Column("username")]
        public string username { get; set; }
        [Column("password")]
        public string password { get; set; }
        [Column("birthday")]
        public DateTime? Birthday { get; set; }

        [Column("age")]
        public int? Age { get; set; }

        [Column("height")]
        public decimal? Height { get; set; }  // 單位：cm

        [Column("weight")]
        public decimal? Weight { get; set; }  // 單位：kg

        [Column("bmi")]
        public decimal? BMI { get; set; }
    }
}
