using System.Collections.Generic;
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

namespace Kennels.Controllers
{
    
    [Authorize]
    public class KennelsController : Controller
    {
        public static bool KennelOwner = false;
        

        private KennelsContext db = new KennelsContext();
        private UserManager<ApplicationUser> manager;
        public KennelsController()
        {
            db = new KennelsContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        //For use in view
        public static DateTime searchSt;
        public static DateTime searchEn;
        public static bool dateSearch = false;  

        // GET: Kennels
        [AllowAnonymous]
        public ActionResult Index(string county, string searchName, DateTime? searchStart, DateTime? searchEnd, string searchRate, string sort)
        {
            dateSearch = false;
            //To only show specific things to specific users in the view
            KennelOwner = false;
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (currentUser != null)
            {
                if (currentUser.UserType == UserType.KennelOwner)
                {
                    KennelOwner = true;
                }
            }

            //Creates a list of distinct countys that have kennels in them.
            var CountyList = new List<string>();
            var CountyQry = db.Kennel.Select(c => c.County.ToString());            
            CountyList.AddRange(CountyQry.Distinct());
            ViewBag.county = new SelectList(CountyList);

            //Creates a list of ratings 1 - 5
            var RatingList = new List<double>();
            for (int i = 1; i <= 5; i++)
            {
                RatingList.Add(i);
            }
            ViewBag.rate = new SelectList(RatingList);

            //Select list to sort the table
            var SortList = new List<string>();
            SortList.Add("Name (A-Z)");
            SortList.Add("Name (Z-A)");
            SortList.Add("Price per Night (H-L)");
            SortList.Add("Price per Night (L-H)");
            SortList.Add("Price per Week (H-L)");            
            SortList.Add("Price per Week (L-H)");
            ViewBag.sort = new SelectList(SortList);
                        
            IQueryable<Kennel> kennels = db.Kennel;
                        
            
            //Search by date
            if (searchStart.HasValue && searchEnd.HasValue)
            {
                dateSearch = true;
                //Converts the variables from DateTime? to DateTime 
                DateTime ss = Convert.ToDateTime(searchStart);
                DateTime se = Convert.ToDateTime(searchEnd);

                searchSt = ss;
                searchEn = se;

                //Creates a list of the kennels to be displayed
                var avList = new List<Kennel>();
                     
                
                //Loops through every kennel in the kennel database
                foreach (Kennel kennel in db.Kennel)
                {
                    //Bool to keep track if a kennel is full or not
                    bool kenFull = false;
                    for (DateTime date = ss; date <= se; date = date.AddDays(1))
                    {
                        var ka = db.KennelAvailability.Where(k => k.KennelID == kennel.KennelID && k.BookingDate == date).FirstOrDefault();
                        //Could be null if the kennel hasn't been booked at all on the day
                        if (ka != null)
                        {                            
                            //Triggers true if the kennel is full for part of or all of the search dates
                            if (ka.Full == true)
                            {
                                kenFull = true;
                            }
                        }
                    }
                    //If the kennel is not full for any of the days it is added to the list
                    if (kenFull != true)
                    {     
                        avList.Add(kennel);
                    }                    
                }
                kennels = avList.AsQueryable();                    
            }

            
            //Search by rating
            if (!string.IsNullOrEmpty(searchRate))
            {
                var rateList = new List<Kennel>();
                //Adds kennels to the list depending on whether they are higher than or equal to the searched rating.
                foreach (Kennel kennel in db.Kennel)
                {
                    var rateqry = db.TotalRating.Where(r => r.KennelID == kennel.KennelID).FirstOrDefault();
                    int search = Convert.ToInt32(searchRate);
                    if (rateqry == null)
                    {
                        rateList.Add(kennel);
                    }
                    if (rateqry != null)
                    {
                        if (rateqry.AverageRating >= search)
                        {
                            rateList.Add(kennel);
                        }
                    }
                }
                kennels = rateList.AsQueryable();
            }

            //Sort results
            if (!string.IsNullOrEmpty(sort))
            {
                //Orders the kennel list based on the sort selected from the list.         
                if (sort == "Name (A-Z)")
                {
                    kennels = kennels.OrderBy(k => k.Name);
                }
                if (sort == "Name (Z-A)")
                {
                    kennels = kennels.OrderByDescending(k => k.Name);
                }
                if (sort == "Price per Night (H-L)")
                {
                    kennels = kennels.OrderByDescending(k => k.PricePerNight);
                }
                if (sort == "Price per Night (L-H)")
                {
                    kennels = kennels.OrderBy(k => k.PricePerNight);
                }
                if (sort == "Price per Week (H-L)")
                {
                    kennels = kennels.OrderByDescending(k => k.PricePerWeek);
                }
                if (sort == "Price per Week (L-H)")
                {
                    kennels = kennels.OrderBy(k => k.PricePerWeek);
                }
            }

            //Search by name
            if (!string.IsNullOrEmpty(searchName))
            {
                //Adds the kennels to the list that include the name searched
                kennels = kennels.Where(n => n.Name.Contains(searchName));              
            }

            //Search by county
            if (!string.IsNullOrEmpty(county))
            {
                //Adds the kennels to the list that are in the selected county
                kennels = kennels.Where(c => c.County.ToString() == county);              
            }
            
            //Displays the view from all the queries
            return View(kennels);
        }

        public ActionResult MyKennels()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            IQueryable<Kennel> kennels = db.Kennel.Where(b => b.User.Id == currentUser.Id);

            //If the user type is Kennel Owner
            if (currentUser.UserType == UserType.KennelOwner)
            {                
                if (kennels != null)
                {   
                    //Makes a list of all the kennels belonging to the owner and displays them in the view                 
                    var kenList = new List<Kennel>();
                    foreach (Kennel k in kennels)
                    {
                        kenList.Add(k);
                    }
                    return View(kenList);
                }
                else
                {
                    ViewBag.noKennel = "You have not added any kennels";
                    return View(kennels);
                }
            }
            else
            {
                return Redirect("Index");
            }
        }

        [AllowAnonymous]
        // GET: Kennels/Details/{KennelID}
        public async Task<ActionResult> Details(string id)
        {
            KennelOwner = false;
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (currentUser != null)
            {
                if (currentUser.UserType == UserType.KennelOwner)
                {
                    KennelOwner = true;
                }
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Kennel kennels = await db.Kennel.FindAsync(id);
            if (kennels == null)
            {
                return HttpNotFound();
            }

            //Adds rating to the details section
            var RateQry = db.TotalRating.Where(r => r.KennelID == id).FirstOrDefault();
            if (RateQry != null)
            {
                string tr = " Reviews";
                if (RateQry.TotalRaters == 1)
                {
                    tr = " Review";
                }
                //Shorten the string if it's longer than 3 characters long (e.g 2.5466 = 2.5)
                string avRate = RateQry.AverageRating.ToString();
                if (avRate.Length > 3)
                {
                    avRate = avRate.Substring(0, 3);
                }
                ViewBag.avgRating = avRate + " out of 5 (" + RateQry.TotalRaters + tr + ")";
            }
            else
            {
                ViewBag.avgRating = "No rating!";
            }

            return View(kennels);
        }
        

        // GET: Kennels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Kennels/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "KennelID,Name,Address,County,PhoneNumber,Email,Capacity,PricePerNight,PricePerWeek,LgeDog,MedDog,SmlDog,CancellationPeriod")] Kennel kennels)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                kennels.User = currentUser;
                db.Kennel.Add(kennels);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(kennels);
        }

        // GET: Kennels/Edit/{KennelID}
        public async Task<ActionResult> Edit(string id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kennel kennels = await db.Kennel.FindAsync(id);
            if (kennels == null)
            {
                return HttpNotFound();
            }

            if (kennels.User != currentUser)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(kennels);
        }

        // POST: Kennels/Edit/{KennelID}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "KennelID,Name,Address,County,PhoneNumber,Email,Capacity,PricePerNight,PricePerWeek,LgeDog,MedDog,SmlDog,CancellationPeriod")] Kennel kennels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kennels).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("MyKennels");
            }
            return View(kennels);
        }

        // GET: Kennels/Delete/{KennelID}
        public async Task<ActionResult> Delete(string id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kennel kennels = await db.Kennel.FindAsync(id);
            if (kennels == null)
            {
                return HttpNotFound();
            }
            if (kennels.User != currentUser)
            {                
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "You are not authorized to do that");
            }
            return View(kennels);
        }

        // POST: Kennels/Delete/{KennelID}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Kennel kennels = await db.Kennel.FindAsync(id);
            db.Kennel.Remove(kennels);
            await db.SaveChangesAsync();
            return RedirectToAction("MyKennels");
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
