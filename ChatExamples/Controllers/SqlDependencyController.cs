using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ChatExamples.Models;

namespace ChatExamples.Controllers
{
    public class SqlDependencyController : Controller
    {
        // GET: SqlDependency
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetJsonMessages()
        {
            /*
            var db = new ChatDbContext();
            var messages = db.Messages.OrderBy(x => x.CreatedDate).ToList().Select(x => new
            {
                x.Id,
                x.MessageContent,
                x.UserName,
                CreatedDate = x.CreatedDate.ToString(CultureInfo.InvariantCulture)
            }).ToList();
            */
            var repository = new MessageRepository();
            return Json(repository.GetAllMessages().Select(x => new
            {
                x.Id,
                x.MessageContent,
                x.UserName,
                CreatedDate = x.CreatedDate.ToString(CultureInfo.InvariantCulture)
            }), JsonRequestBehavior.AllowGet);
            //return Json(messages, JsonRequestBehavior.AllowGet);
        }

        public HttpResponseMessage DeleteMessage(string id)
        {
            var db = new ChatDbContext();

            var message = db.Messages.Find(new Guid(id));

            if (message != null)
            {
                db.Messages.Remove(message);
                db.SaveChanges();
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent("Update completed!", Encoding.UTF8, "text/html")
            };
        }
    }
}