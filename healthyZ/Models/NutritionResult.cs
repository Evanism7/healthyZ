using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace healthyZ.Models
{
    [Table("food_record")]
    public class NutritionResult : BaseModel
    {
        

        [PrimaryKey("id")]

        public int Id { get; set; }
        [Column("account_id")]
        public string account_id { get; set; }
        [Column("food_name")]
        public string food_name { get; set; }
        [Column("Weight")]
        public int Weight { get; set; }
        [Column("carbohydrates")]
        public int carbohydrates { get; set; }
        [Column("fat")]
        public int fat { get; set; }
        [Column("protein")]
        public int protein { get; set; }
        [Column("calories")]
        public int calories { get; set; }
        [Column("day")]
        public string day { get; set; }

    }
}
