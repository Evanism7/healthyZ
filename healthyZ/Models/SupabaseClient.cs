using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace healthyZ.Models
{
    public class SupabaseClient
    {
        // 連線登入 Supabase 共用函式
        public Supabase.Client GetClient()
        {
            // 初始化 Supabase 客戶端
            Supabase.Client _client;
            _client = new Supabase.Client(
                "https://zgajpjoewcbijoplqnti.supabase.co",
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InpnYWpwam9ld2NiaWpvcGxxbnRpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NTE0MjAwMTMsImV4cCI6MjA2Njk5NjAxM30.RIuoFbPdEKAYrn4lw7Ei-VkoKx44dnnk9pdP-G8GX3g",
                new Supabase.SupabaseOptions { AutoConnectRealtime = true });
            return _client;
        }
    }
}
