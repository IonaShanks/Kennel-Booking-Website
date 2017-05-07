using Kennels.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace Kennels.Controllers
{
    public class HomeController : Controller
    {
        private KennelsContext db = new KennelsContext();

        public ActionResult About()
        {
            ViewBag.Message = "Helping you find the ideal kennel.";

            return View();
        }

        //Function to send email 
        public void ContactEmail(string Message, string Email, String Name)
        {
            //Making email            
            var message = new MailMessage();
            message.To.Add(new MailAddress("IonaKennel@gmail.com"));
            message.From = new MailAddress(Email);
            message.Subject = "Contact Form";
            message.Body = string.Format("<p>From: " + Name + "</p><p>Email: " + Email + "</p><p>Message: " + Message + "</p>");
            message.IsBodyHtml = true;

            //Sending email
            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                var credential = new NetworkCredential
                {
                    UserName = "IonaKennel@gmail.com",
                    Password = "Tallaght1"
                };
                smtp.Credentials = credential;
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact IS Kennel Finder";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel vm)
        {
            ViewBag.Message = "Contact IS Kennel Finder";
            if (ModelState.IsValid)
            {
                try
                {
                    //Fills the function paramaters based on what the use inputs in the form
                    String Message = vm.Message;
                    String Email = vm.EmailFrom;
                    String Name = vm.Name;
                    //Calls the function to send the email 
                    ContactEmail(Message, Email, Name);
                }
                catch (Exception ex)
                {
                    ModelState.Clear();
                    TempData["Error"] = $" Sorry we are facing Problem here {ex.Message}";
                }
            }
            else
            {
                ModelState.AddModelError("", "ModelState not valid");
                return View(vm);
            }
            ViewBag.Success = "Thank you for getting in contact, your message has been sent!";
            return View();
        }
    }
}