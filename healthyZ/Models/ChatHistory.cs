using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace healthyZ.Models
{
    [Table("chat_history")]
    public class chat_history : BaseModel
    {
        [PrimaryKey("id")]
        public long Id { get; set; }

        [Column("account_id")]
        public string accountId { get; set; }

        [Column("role")]
        public string role { get; set; }

        [Column("message")]
        public string message { get; set; }

        [Column("created_at")]
        public DateTime createdAt { get; set; }
    }
}
