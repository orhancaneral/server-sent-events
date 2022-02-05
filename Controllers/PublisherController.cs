using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server_Sent_Events.Server.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Server_Sent_Events.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private static ConcurrentQueue<PingData> pings = new ConcurrentQueue<PingData>();

        private static bool flagUser = false;

        private static int userId = 1;

        [HttpGet]
        public bool StartStopUserCreation()
        {
            flagUser = !flagUser;

            return flagUser;
        }

        [HttpGet("generate-user")]
        public void CreateUser()
        {
            while (flagUser)
            {
                pings.Enqueue(new PingData { UserID = userId++ });

                Thread.Sleep(1000);
            }
        }

        [HttpGet("message")]
        public void Message()
        {
            Response.ContentType = "text/event-stream";
            Response.WriteAsync($":Hello {Request.Host}\n");

            const int intervalMs = 1000;
            do
            {
                if (pings.TryDequeue(out var nextPing))
                {
                    Response.WriteAsync($"event:Ping\n");
                    Response.WriteAsync($"retry:{intervalMs}\n");
                    Response.WriteAsync($"id:{DateTime.Now.Ticks}\n");
                    Response.WriteAsync($"data:{JsonSerializer.Serialize(nextPing)}\n\n");
                }

                Thread.Sleep(intervalMs);
            } while (Response.Body.CanWrite);
        }
    }
}
