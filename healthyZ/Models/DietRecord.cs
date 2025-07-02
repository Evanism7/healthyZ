using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;


namespace healthyZ.Models
{
    [Table("food_record")]
    public class DietRecord : BaseModel
    {
        [PrimaryKey("id")]
        public int id { get; set; }
        [Column("account_id")]
        public string account_id { get; set; }
        [Column("calories")]
        public int? calories { get; set; }
        [Column("food_name")]
        public string food_name { get; set; }
        [Column("day")]

        public DateTime? day { get; set; }
        [Column("protein")]
        public decimal? protein { get; set; }
        [Column("carbohydrates")]
        public decimal? carbohydrates { get; set; }
        [Column("fat")]
        public decimal? fat { get; set; }
        [Column("location")]
        public string location { get; set; }
        [Column("rating")]
        public int? rating { get; set; }
    }
}
