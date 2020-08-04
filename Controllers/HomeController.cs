using Blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure.Interception;
using System.EnterpriseServices;
using System.Linq;
using System.Net.Mail;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace Blog.Controllers
{
      [RequireHttps]
      public class HomeController : Controller
      {
            private ApplicationDbContext db = new ApplicationDbContext();
            
           
            public ActionResult Index()
            {
                  var allPublishedBlogPosts = db.Posts.Where(b => b.Published).OrderByDescending(b => b.Created).ToList();
                  return View(allPublishedBlogPosts);
            }

            public ActionResult About()
            {
                  //ViewBag.Message = "Your application description page.";

                  return View();
            }
            
            public ActionResult Contact()
            {
                  //ViewBag.Message = "Contact Us";
                  EmailModel model = new EmailModel();
                  return View(model);
            }  
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> Contact(EmailModel model)
            {
                  if (ModelState.IsValid)
                  {
                        try
                        {
                              var body = $"<p>Email From: <bold>{model.FromName}</bold> ({model.FromEmail})</p><p>Message:</p><p>{model.Body}</p>";
                              var from = $"{model.FromEmail}";
                              model.Body = "This is a message from your portfolio site.  The name and the email of the contacting person is above.";
                              var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                              {
                                    Subject =$"Portfolio Email: {model.FromEmail}.",
                                    Body = string.Format(body, model.FromName, model.FromEmail, model.Body),
                                    IsBodyHtml = true
                              };
                              var svc = new EmailService();
                              await svc.SendAsync(email);
                              return View(new EmailModel());
                        }
                        catch (Exception ex)
                        {
                              Console.WriteLine(ex.Message);
                              await Task.FromResult(0);
                        }
                  }
                  return View(model);
            }
           
      }
     
}