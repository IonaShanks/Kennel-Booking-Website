using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using Kennels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Kennels.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {        
        private KennelsContext db = new KennelsContext();
        private UserManager<ApplicationUser> manager;
        public BookingsController()
        {
            db = new KennelsContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));            
        }
        
        public void SendConfirmation(string Message, string Subject)
        {
            //Making email
            var currentUser = manager.FindById(User.Identity.GetUserId());
            var message = new MailMessage();
            message.To.Add(new MailAddress(currentUser.Email));
            message.From = new MailAddress("IonaKennel@gmail.com");
            message.Subject = Subject;
            message.Body = string.Format(Message);
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

        // GET: Bookings
        public ActionResult Index()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            IQueryable<Booking> bookings = db.Booking.Where(b => b.User.Id == currentUser.Id);
            
            //If the user type is Kennel Owner
            if (currentUser.UserType == UserType.KennelOwner)
            {               
                var ken = db.Kennel.Where(k => k.User.Id == currentUser.Id);
                if (ken.Count() > 0)
                {
                    var bookedList = new List<Booking>();                    
                    foreach (Kennel k in ken)
                    {     
                        foreach (Booking booked in db.Booking.Where(b => b.KennelID == k.KennelID))
                        {
                            bookedList.Add(booked);
                        }
                    }

                    return View(bookedList);
                }
                else
                {
                    ViewBag.Empty = "Your kennels have no bookings.";
                    return View(bookings);
                }
            }
            //If the user type is Customer
            else if (currentUser.UserType == UserType.Customer)
            {              
                //If the Current user has bookings it shows them a list of their bookings, if not it redirects the the create action. 
                if (bookings.Count() > 0)
                {
                    return View(db.Booking.Include(k => k.Kennel).ToList().Where(b => b.User == currentUser));
                }
                else
                {
                    //Returns a blank table
                    ViewBag.Empty = "You currently have no bookings.";
                    return View(bookings);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        // GET: Bookings/Details/{BookingID}
        public async Task<ActionResult> Details(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId()); 
                        
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var booking = db.Booking.Include(k => k.Kennel).Where(b => b.BookingID == id).FirstOrDefault();
            if (booking == null)
            {
                return HttpNotFound();
            }

            bool kenOwn = false;
            if (currentUser.UserType == UserType.KennelOwner)
            {
                var kk = db.Kennel.Where(k => k.KennelID == booking.KennelID).FirstOrDefault();
                if (kk.User == currentUser)
                { kenOwn = true; }
            }
            //Only if logged in as the user who owns the booking or owner of kennel can you view details about it, otherwise you get an unauthorized error. 
            if (kenOwn == true || booking.User == currentUser)
            {
                return View(booking);
            }           
            
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        // GET: Bookings/Create
        public ActionResult Create(string id)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if(currentUser.UserType != UserType.Customer)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var kID = db.Kennel.Where(k => k.KennelID == id).First();
            ViewBag.KennelName = kID.Name;            
            return View();
        }
               
        
        //Declaring a boolean for use within the Create ActionResult
        private bool full;
        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> Create(string id, [Bind(Include = "BookingID,StartDate,EndDate,TotalNights,Price,KennelID")] Booking booking)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            
            var kennel = db.Kennel.Where(k => k.KennelID.ToUpper() == id.ToUpper()).First();

            //Loop through each date between start and end to check if the kennel is full for that date or not. 
            for (DateTime date = booking.StartDate; date <= booking.EndDate; date = date.AddDays(1))
            {
                bool kennelFull = db.KennelAvailability.Where(k => k.KennelID.ToUpper() == id.ToUpper() && k.BookingDate == date).Any(a => a.Full.Equals(true));

                //If for any date in the loop kennelFull = true then a full boolean is triggered to be true. 
                if (kennelFull == true)
                {
                    full = true;
                }
            }
            
            //If any date in the booking is full then it will not add the booking instead skip to the else.
            if (full == false)
            {
                if (ModelState.IsValid)
                {
                    //Loops through the dates in the booking from start to end
                    for (DateTime date = booking.StartDate; date <= booking.EndDate; date = date.AddDays(1))
                    {
                        //for the date the loop is on in the iteration it will check to see if the date already exists
                        bool dateExists = db.KennelAvailability.Where(k => k.KennelID.ToUpper() == id.ToUpper() && k.BookingDate == date).Count() > 0;
                        
                        //if the date does not exist it will add it to the table                      
                        if (dateExists == false)
                        {                            
                            var newDate = new KennelAvailability()
                            {
                                KennelID = id,
                                BookingDate = date,
                                Availability = 1                            
                            };

                            db.KennelAvailability.Add(newDate);
                            await db.SaveChangesAsync();
                        }

                        //if the date does exists it will update the existing entry.
                        else
                        {
                            //Adds 1 to Availability on the KennelAvailability table where the KennelId and Date match those of the booking.
                            var ka = db.KennelAvailability.Where(r => r.KennelID.ToUpper() == id.ToUpper() && r.BookingDate == date).First();                            
                            ka.Availability += 1;
                            db.Entry(ka).State = EntityState.Modified;
                            await db.SaveChangesAsync();

                            //Updates the KennelAvailability to full if the kennel has reached capacity for that date. 
                            
                            if (ka.Availability == kennel.Capacity)
                            {                               
                                ka.Full = true;
                                db.Entry(ka).State = EntityState.Modified;
                                await db.SaveChangesAsync();
                            }                            
                        }
                    }
                    
                    //Calls the funtion to calculate the total nights based on the start and end date specified by the user
                    booking.TotalNights = booking.CalcTotalNights(booking.StartDate, booking.EndDate);

                    //Calls the function CalcTotalPrice which takes in (double pricePerNight, double pricePerWeek) to calculate the price based on the prices the kennel offers.
                    booking.Price = booking.CalcTotalPrice(kennel.PricePerNight, kennel.PricePerWeek, booking.TotalNights);
                    booking.KennelID = id;
                    booking.User = currentUser;
                    db.Booking.Add(booking);

                    await db.SaveChangesAsync();

                    //Body of the email
                    string bookingConf = "<p> Kennel Name: " + kennel.Name + "</p><p> Date from: " + booking.StartDate.ToString().Substring(0,10) + 
                        "</p><p> Date to: " + booking.EndDate.Date.ToString().Substring(0, 10) + "</p><p> Total price: " + booking.Price;
                    string Message = "<p>Hi " + currentUser.Fname + " " + currentUser.Lname + "</p>" +
                        "<p>Thank you for booking with IS Kennels. </p> <p>Your booking is as follows: </p>" + bookingConf;
                    string Subject = "Booking Conformation for " + kennel.Name;

                    //Sends the email
                    SendConfirmation(Message, Subject);
                    
                    
                    return RedirectToAction("Index");
                }

                //Returns the view of the booking if the model state is not valid.
                ViewBag.KennelID = new SelectList(db.Kennel, "KennelID", "Name", booking.KennelID);                              
                return View(booking);
            }
            else
            {
                //return a view where it says that one or more of the dates selected is full. 
                ViewBag.KennelID = new SelectList(db.Kennel, "KennelID", "Name", booking.KennelID);                
                ViewBag.Message = kennel.Name + " is not available for the selected dates.";
                return View(booking);
            }
        }

        

        // GET: Bookings/Delete/{BookingID}
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = await db.Booking.FindAsync(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            bool kenOwn = false;
            
            if (currentUser.UserType == UserType.KennelOwner)
            {
                var kk = db.Kennel.Where(k => k.KennelID == booking.KennelID).FirstOrDefault();
                if(kk.User == currentUser)
                { kenOwn = true; }
            }
            //Only if logged in as the user who owns the booking or owner of kennel can you delete it, otherwise you get an unauthorized error. 
            if (kenOwn == true || booking.User == currentUser)
            {
                var kID = db.Kennel.Where(k => k.KennelID == booking.KennelID).First();
                ViewBag.KennelName = kID.Name;
                return View(booking);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        // POST: Bookings/Delete/{BookingID}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Booking booking = await db.Booking.FindAsync(id);
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            var kennel = db.Kennel.Where(k => k.KennelID == booking.KennelID).First();

            for (DateTime date = booking.StartDate; date <= booking.EndDate; date = date.AddDays(1))
            {
                //Removes one from availability to show that there is one more free spot available in the kennel
                var ka = db.KennelAvailability.Where(r => r.KennelID.ToUpper() == booking.KennelID.ToUpper() && r.BookingDate == date).First();
                ka.Availability -= 1;
                db.Entry(ka).State = EntityState.Modified;
                await db.SaveChangesAsync();
               
                //If the kennel on the date is full before the booking is cancelled then it is updated to not be full once the booking is cancelled
                if (ka.Full == true)
                {
                    ka.Full = false;
                    db.Entry(ka).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }
            }
                        
            db.Booking.Remove(booking);
            await db.SaveChangesAsync();

            string bookingConf = "<p> Kennel Name: " + kennel.Name + "</p><p> Date from: " + booking.StartDate.ToString().Substring(0, 10) +
                        "</p><p> Date to: " + booking.EndDate.Date.ToString().Substring(0, 10) + "</p><p> Total price: " + booking.Price;
            string Message = "<p>Hi " + currentUser.Fname + " " + currentUser.Lname + "</p>" +
                        "<p>Confirmation of the cancellation of the following booking: </p>" + bookingConf;
            string Subject = "Booking Cancellation for " + kennel.Name;

            //Sends the email
            SendConfirmation(Message, Subject);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
