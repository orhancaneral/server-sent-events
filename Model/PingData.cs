using System;

namespace Server_Sent_Events.Server.Model
{
    public class PingData
    {
        public int UserID { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
