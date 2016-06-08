using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ChatExamples.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace ChatExamples.Components
{
    public class RejoingGroupPipelineModule : HubPipelineModule
    {
        public override Func<HubDescriptor, IRequest, IList<string>, IList<string>> BuildRejoiningGroups(Func<HubDescriptor, IRequest, IList<string>, IList<string>> rejoiningGroups)
        {
            if (rejoiningGroups == null) throw new ArgumentNullException(nameof(rejoiningGroups));
            rejoiningGroups = (hb, r, l) =>
            {
                var assignedGroups = new List<string>();
                using (var db = new ChatDbContext())
                {
                    var user = db.Users.Include(u => u.Groups).FirstOrDefault(u => u.UserName == r.User.Identity.Name);
                    if (user != null)
                    {
                        foreach (var item in user.Groups)
                        {
                            assignedGroups.Add(item.GroupName);
                        }
                    }
                }
                return assignedGroups;
            };

            return rejoiningGroups;
        }
    }
}