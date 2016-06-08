using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChatExamples.Models;

namespace ChatExamples.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Index()
        {
            using (var db = new ChatDbContext())
            {
                var groups = db.Groups.OrderBy(x => x.GroupName).ToList();
                var user = db.Users.Include(u => u.Groups).SingleOrDefault(u => u.UserName == User.Identity.Name);
                if (user != null)
                {
                    var mygroups = user.Groups.OrderBy(x => x.GroupName).ToList();
                    ViewBag.MyGroupId = new SelectList(mygroups, "GroupName", "GroupName");
                }
                else
                {
                    ViewBag.MyGroupId = new SelectList(new[] { new Group() { GroupName = "Public" } }, "GroupName", "GroupName"); ;

                }

                ViewBag.GroupId = new SelectList(groups, "GroupName", "GroupName");
                return View();
            }
        }       
    }
}