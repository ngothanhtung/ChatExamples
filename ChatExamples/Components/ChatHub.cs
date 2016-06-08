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
    [Authorize]
    public class ChatHub : Hub
    {
        public override Task OnConnected()
        {
            using (var db = new ChatDbContext())
            {
                // Retrieve user.
                var user = db.Users
                    .Include(u => u.Groups)
                    .SingleOrDefault(u => u.UserName == Context.User.Identity.Name);

                // If user does not exist in database, must add.
                if (user == null)
                {
                    user = new User()
                    {
                        UserName = Context.User.Identity.Name
                    };
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                else
                {
                    // Add to each assigned group.
                    foreach (var item in user.Groups)
                    {
                        Groups.Add(Context.ConnectionId, item.GroupName);
                    }
                }
            }
            return base.OnConnected();
        }

        public void Connect(string name)
        {
            // Hàm này chủ yếu để khai báo cho client.
            Clients.All.BroadcastMessage("Thông báo: [" + name + "] connected to chat group at " + DateTime.Now);
            //Clients.Others.BroadcastMessage("Thông báo: [" + name + "] connected to chat group at " + DateTime.Now);
        }

        [HubMethodName("JoinGroup")]
        public async Task JoinGroup(string groupName)
        {
            using (var db = new ChatDbContext())
            {
                // Retrieve group.
                var group = db.Groups.Find(groupName);

                if (group != null)
                {
                    var user = new User() { UserName = Context.User.Identity.Name };
                    db.Users.Attach(user);

                    group.Users.Add(user);
                    db.SaveChanges();
                    await Groups.Add(Context.ConnectionId, groupName);
                    // Hàm này chủ yếu để khai báo cho client.
                    Clients.Group(groupName).JoinGroup($"{user.UserName} joined to group [{groupName}]");
                }
            }

            //await Groups.Add(Context.ConnectionId, groupName);
            // Hàm này chủ yếu để khai báo cho client.
            //Clients.Group(groupName).JoinGroup($"{Context.ConnectionId} added to group [{groupName}]");
        }


        // By default, JavaScript clients refer to Hub methods by using a camel-cased version of the method name.
        // SignalR automatically makes this change so that JavaScript code can conform to JavaScript conventions.
        [HubMethodName("SendMessageInGroup")]
        public async Task SendMessageInGroup(string name, string message, string groupName)
        {
            // Hàm này chủ yếu để khai báo cho client.
            await Clients.Group(groupName).AddMessageToPage(new Message { UserName = name, MessageContent = message });
            using (var db = new ChatDbContext())
            {
                db.Messages.Add(new Message()
                {
                    Id = Guid.NewGuid(),
                    MessageContent = message,
                    UserName = name,
                    CreatedDate = DateTime.Now
                });
                db.SaveChanges();
            }
            //Clients.Caller.notifyMessageSent();
        }

        public async Task LeaveGroup(string groupName)
        {
            using (var db = new ChatDbContext())
            {
                // Retrieve group.
                var group = db.Groups.Find(groupName);
                if (group != null)
                {
                    var user = new User() { UserName = Context.User.Identity.Name };
                    db.Users.Attach(user);

                    group.Users.Remove(user);
                    db.SaveChanges();

                    
                    // Hàm này chủ yếu để khai báo cho client.
                    Clients.Group(groupName).LeaveGroup($"{Context.ConnectionId} leave from group [{groupName}]");

                    await Groups.Remove(Context.ConnectionId, groupName);
                }
            }

            // Hàm này chủ yếu để khai báo cho client.
            //Clients.Group(groupName).LeaveGroup($"{Context.ConnectionId} leave from group [{groupName}]");
            //return Groups.Remove(Context.ConnectionId, groupName);
        }

        // By default, JavaScript clients refer to Hub methods by using a camel-cased version of the method name.
        // SignalR automatically makes this change so that JavaScript code can conform to JavaScript conventions.
        [HubMethodName("SendMessage")]
        public async Task SendMessage(string name, string message)
        {
            // Hàm này chủ yếu để khai báo cho client.
            await Clients.Others.AddMessageToPage(new Message { UserName = name, MessageContent = message });
            //Clients.Caller.notifyMessageSent();
            await Clients.Caller.AddMessageToPage(new Message { UserName = "Me", MessageContent = message });
        }

        [HubMethodName("SendNotificationSqlDependency")]
        public static void SendNotificationSqlDependency()
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            // Hàm này chủ yếu để khai báo cho client.
            context.Clients.All.UpdateNotificationSqlDependency();
        }
    }
}