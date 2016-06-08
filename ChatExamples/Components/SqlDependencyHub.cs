using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ChatExamples.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ChatExamples.Components
{
    public class SqlDependencyHub : Hub
    {
        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        [HubMethodName("SendNotificationSqlDependency")]
        public static void SendNotificationSqlDependency()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<SqlDependencyHub>();
            // Hàm này chủ yếu để khai báo cho client.
            context.Clients.All.UpdateNotificationSqlDependency();
        }
    }
}