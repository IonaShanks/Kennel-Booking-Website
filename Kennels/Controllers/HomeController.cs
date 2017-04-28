using System.Data;
using System.Linq;
using System.Web.Mvc;
using Kennels.Models;
using System;
using System.Collections.Generic;

namespace Kennels.Controllers
{
    public class HomeController : Controller
    {
        private KennelsContext db = new KennelsContext();
       
        //public ActionResult Index(string county, string searchName, DateTime? searchStart, DateTime? searchEnd)
        //{
        //    //Creates a list of distinct countys that have kennels in them.
        //    var CountyList = new List<string>();
        //    var CountyQry = db.Kennel.Select(c => c.County.ToString());
        //    CountyList.AddRange(CountyQry.Distinct());
        //    ViewBag.county = new SelectList(CountyList);

        //    IQueryable<Kennel> kennels = db.Kennel;

        //    if (searchStart.HasValue && searchEnd.HasValue)
        //    {
        //        //Converts the variables from DateTime? to DateTime 
        //        DateTime ss = Convert.ToDateTime(searchStart);
        //        DateTime se = Convert.ToDateTime(searchEnd);

        //        //Creates a list of the kennels to be displayed
        //        var avList = new List<Kennel>();
        //        //Bool to keep track if a kennel is full or not
        //        bool kenFull = false;
        //        //Loops through every kennel in the kennel database
        //        foreach (Kennel kennel in db.Kennel)
        //        {
        //            for (DateTime date = ss; date <= se; date = date.AddDays(1))
        //            {
        //                var ka = db.KennelAvailability.Where(k => k.KennelID == kennel.KennelID && k.BookingDate == date).FirstOrDefault();
        //                //Could be null if the kennel hasn't been booked at all on the day
        //                if (ka != null)
        //                {
        //                    //Triggers true if the kennel is full for part of or all of the search dates
        //                    if (ka.Full == true)
        //                    {
        //                        kenFull = true;
        //                    }
        //                }
        //            }
        //            //If the kennel is not full for any of the days it is added to the list
        //            if (kenFull != true)
        //            {
        //                avList.Add(kennel);
        //            }
        //        }
        //        kennels = avList.AsQueryable();
        //    }

        //    if (!string.IsNullOrEmpty(searchName))
        //    {
        //        //Adds the kennels to the list that include the name searched
        //        kennels = kennels.Where(n => n.Name.Contains(searchName));
        //    }

        //    if (!string.IsNullOrEmpty(county))
        //    {
        //        //Adds the kennels to the list that are in the selected county
        //        kennels = kennels.Where(c => c.County.ToString() == county);
        //    }

        //    //Displays the view from all the queries
        //    return View(kennels);
        //}

        public ActionResult About()
        {
            ViewBag.Message = "Helping you find the ideal kennel.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}