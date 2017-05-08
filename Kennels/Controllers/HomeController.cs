using Kennels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public void ContactEmail(string Message, string EmailFrom, String Name, string EmailTo)
        {
            //Making email            
            var message = new MailMessage();
            message.To.Add(new MailAddress(EmailTo));
            message.From = new MailAddress(EmailFrom);
            message.Subject = "Contact Form";
            message.Body = string.Format("<p>From: " + Name + "</p><p>Email: " + EmailFrom + "</p><p>Message: " + Message + "</p>");
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


        public List<string> GetEmailList()
        {
            var emailList = new List<string>();
            var emailQry = db.Kennel.Select(c => c.Name.ToString());
            emailList.AddRange(emailQry.Distinct());
            emailList.Add("IS Kennel Finder");
            return emailList;
        }

        [HttpGet]
        public ActionResult Contact(string emailTo)
        {
            ViewBag.emailTo = new SelectList(GetEmailList());
            ViewBag.Message = "Contact IS Kennel Finder";

            return View();
        }       

        [HttpPost]
        public ActionResult Contact(ContactViewModel vm, string emailTo)
        {
            ViewBag.Message = "Contact IS Kennel Finder";
            ViewBag.emailTo = new SelectList(GetEmailList());

            if (string.IsNullOrEmpty(emailTo))
            {
                ViewBag.SelectKennel = "Please Select a Kennel to Contact";
                ViewBag.emailTo = new SelectList(GetEmailList());
                return View(vm);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        //Fills the function paramaters based on what the use inputs in the form
                        String Message = vm.Message;
                        String EmailFrom = vm.EmailFrom;
                        String Name = vm.Name;
                        String EmailTo;
                        if (vm.EmailTo == "IS Kennel Finder")
                        {
                            EmailTo = "ionakennel@gmail.com";
                        }
                        else
                        {
                            var email = db.Kennel.Where(k => k.Name == vm.EmailTo).FirstOrDefault();
                            EmailTo = email.Email;
                        }
                        //Calls the function to send the email 
                        ContactEmail(Message, EmailFrom, Name, EmailTo);
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
                ViewBag.emailTo = new SelectList(GetEmailList());
                ViewBag.Success = "Thank you for getting in contact, your message has been sent!";
                return View();
            }
        }
    }
}