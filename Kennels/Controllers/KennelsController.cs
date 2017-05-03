﻿using Kennels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kennels.Controllers
{

    [Authorize]
    public class KennelsController : Controller
    {
        public static bool KennelOwner = false;
        public static IQueryable<TotalRating> rate;
        public static TextInfo myTI = new CultureInfo("en-IE", false).TextInfo;

        public static string TitleCase(string q)
        {
            q = myTI.ToTitleCase(q);
            return q;
        }



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

            IQueryable<Kennel> kennels = db.Kennel.Include(k => k.TotalRating);


            //Search by date
            if (searchStart.HasValue && searchEnd.HasValue)
            {

                var rateSearchList = new List<Kennel>();
                //So that rating doesn't override date it is included
                if (!string.IsNullOrEmpty(searchRate))
                {
                    //Loops through every kennel in the kennel database
                    foreach (Kennel ken in db.Kennel)
                    {
                        var rateqry = db.TotalRating.Where(r => r.KennelID == ken.KennelID).FirstOrDefault();
                        int search = Convert.ToInt32(searchRate);

                        //Adds to list is kennel not yet rated
                        if (rateqry == null)
                        {
                            rateSearchList.Add(ken);
                        }
                        if (rateqry != null)
                        {
                            //Adds to list if kennel rating >= search rating
                            if (rateqry.AverageRating >= search)
                            {
                                rateSearchList.Add(ken);
                            }
                        }
                    }
                }
                else
                {
                    //Loops through every kennel in the kennel database
                    foreach (Kennel k in db.Kennel)
                    {
                        rateSearchList.Add(k);
                    }
                }

                //Checks the start date is before the end date
                if (searchEnd > searchStart)
                {

                    dateSearch = true;
                    //DateTime dt = DateTime.ParseExact(searchStart.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);

                    //Converts the variables from DateTime? to DateTime 
                    DateTime ss = Convert.ToDateTime(searchStart);
                    DateTime se = Convert.ToDateTime(searchEnd);

                    searchSt = ss;
                    searchEn = se;

                    //Creates a list of the kennels to be displayed
                    var avList = new List<Kennel>();

                    //Loops through every kennel in the kennel database
                    foreach (Kennel kennel in rateSearchList)
                    {
                        //Bool to keep track if a kennel is available or not
                        bool kenAvail = false;
                        //If the search exceeds the maximum days
                        if ((se - ss).Days >= kennel.MaxDays)
                        {
                            kenAvail = true;
                        }
                        //If the search doesn't exceed maximum days
                        else
                        {
                            for (DateTime date = ss; date <= se; date = date.AddDays(1))
                            {
                                var ka = db.KennelAvailability.Where(k => k.KennelID == kennel.KennelID && k.BookingDate == date).FirstOrDefault();
                                //Could be null if the kennel hasn't been booked at all on the day
                                if (ka != null)
                                {
                                    //Triggers true if the kennel is full for part of or all of the search dates
                                    if (ka.Full == true)
                                    {
                                        kenAvail = true;
                                    }
                                }
                            }

                        }
                        //If the kennel is not full for any of the days
                        if (kenAvail != true)
                        {
                            //Populates list
                            avList.Add(kennel);
                        }

                    }

                    BookingViewModel sd = new BookingViewModel
                    {
                        StartDate = ss,
                        EndDate = se
                    };

                    TempData["searchDates"] = sd;
                    kennels = avList.AsQueryable();
                }
                else
                {
                    kennels = rateSearchList.AsQueryable();
                    ViewBag.Invalid = "Invalid dates, start date must be before end date!";
                }
            }


            //Search by rating if search dates are not entered (overrides if entered)
            if (!string.IsNullOrEmpty(searchRate) && !searchStart.HasValue && !searchEnd.HasValue)
            {
                var rateList = new List<Kennel>();
                foreach (Kennel kennel in db.Kennel)
                {
                    var rateqry = db.TotalRating.Where(r => r.KennelID == kennel.KennelID).FirstOrDefault();
                    int search = Convert.ToInt32(searchRate);
                    //If null means it hasn't been rated so shows at all star ratings
                    if (rateqry == null)
                    {
                        rateList.Add(kennel);
                    }
                    if (rateqry != null)
                    {
                        //Adds the kennels with a rating >= the rating input in the search paramaters
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
                else if (sort == "Name (Z-A)")
                {
                    kennels = kennels.OrderByDescending(k => k.Name);
                }
                else if (sort == "Price per Night (H-L)")
                {
                    kennels = kennels.OrderByDescending(k => k.PricePerNight);
                }
                else if (sort == "Price per Night (L-H)")
                {
                    kennels = kennels.OrderBy(k => k.PricePerNight);
                }
                else if (sort == "Price per Week (H-L)")
                {
                    kennels = kennels.OrderByDescending(k => k.PricePerWeek);
                }
                else /*if (sort == "Price per Week (L-H)")*/
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


            rate = db.TotalRating;
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
                //If they have added a kennel
                if (kennels.Count() > 0)
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
                    //Displays the empty list with a message
                    ViewBag.noKennel = "You have not added any kennels";
                    return View(kennels);
                }
            }
            else
            {
                //Redirects to the all kennel index if not a kennel owner
                return Redirect("Index");
            }
        }

        [AllowAnonymous]
        // GET: Kennels/Details/{KennelID}
        public async Task<ActionResult> Details(string id)
        {            
            Booking sd = TempData["searchDates"] as Booking;
            TempData["searchDates"] = sd;

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
                string st = " Stars";
                if (RateQry.TotalRaters == 1)
                {
                    tr = " Review";
                    st = "Star";
                }
                //Shorten the string if it's longer than 3 characters long (e.g 2.5466 = 2.5)
                string avRate = RateQry.AverageRating.ToString();
                if (avRate.Length > 3)
                {
                    avRate = avRate.Substring(0, 3);
                }
                ViewBag.avgRating = avRate + " " + st + " (" + RateQry.TotalRaters + tr + ")";
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
            var currentUser = manager.FindById(User.Identity.GetUserId());
            //Only type kennel owner is authorized to add kennels
            if (currentUser.UserType != UserType.KennelOwner)
            {
                TempData["Unauth"] = "You are not authorised to do that, log in as a different user";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View();
        }

        // POST: Kennels/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "KennelID,Name,Address,County,Town,PhoneNumber,Email,Capacity,PricePerNight,PricePerWeek,MaxDays,LgeDog,MedDog,SmlDog,CancellationPeriod,Description,Grooming,Training")] Kennel kennels)
        {

            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (ModelState.IsValid)
            {
                kennels.User = currentUser;

                //Capitalises in title case when added to the database
                kennels.KennelID = kennels.KennelID.ToUpper();
                kennels.Name = TitleCase(kennels.Name);
                kennels.Address = TitleCase(kennels.Address);
                kennels.Town = TitleCase(kennels.Town);

                //Adds the kennel to the database
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

            //Only the kennel owner is authorized to edit the kennel
            if (kennels.User != currentUser)
            {
                TempData["Unauth"] = "You are not authorised to do that, you do not own this kennel";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(kennels);
        }

        // POST: Kennels/Edit/{KennelID}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "KennelID,Name,Address,County,Town,PhoneNumber,Email,Capacity,PricePerNight,PricePerWeek,MaxDays,LgeDog,MedDog,SmlDog,CancellationPeriod,Description,Grooming,Training")] Kennel kennels)
        {
            if (ModelState.IsValid)
            {
                //Capitalises to title case when updated
                kennels.Name = TitleCase(kennels.Name);
                kennels.Address = TitleCase(kennels.Address);
                kennels.Town = TitleCase(kennels.Town);

                //Updates the database entry
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

            //Only the kennel owner is authorized to delete the kennel
            if (kennels.User != currentUser)
            {
                TempData["Unauth"] = "You are not authorised to do that, log in as a different user";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
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
