using Kennels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kennels.Controllers
{

    [Authorize]
    public class RatingsController : Controller
    {
        public static bool Customer = false;
        public static string kenID; 

        private KennelsContext db = new KennelsContext();
        private UserManager<ApplicationUser> manager;
        public RatingsController()
        {
            db = new KennelsContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }


        public ApplicationUser getUser()
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
            return currentUser;
        }

        public void isCustomer()
        {
            var currentUser = getUser();
            if (currentUser != null)
            {
                if (currentUser.UserType == UserType.Customer)
                {
                    Customer = true;
                }
                else
                {
                    Customer = false;
                }
            }
            else
            {
                Customer = false;
            }
        }

        // GET: Ratings (user specific)       
        public ActionResult Index()
        {
            isCustomer();
            var currentUser = getUser();
            IQueryable<Rating> ratings = db.Rating.Include(k => k.Kennel).Where(b => b.User.Id == currentUser.Id);

            if (Customer == true)
            {                
                //If the Current user has ratings it shows them a list of their ratings
                if (ratings.Count() > 0)
                {
                    return View(ratings);
                }
                else
                {
                    //if no ratings displays blank list with message
                    ViewBag.NoRate = "No ratings to show.";
                    return View(ratings);
                }
            }
            else
            {
                //only customers can see their own ratings
                TempData["Unauth"] = "You are not authorised to do that, must be logged in as a customer";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        //Shows kennel ratings for specific kennels
        [AllowAnonymous]
        public ActionResult KennelRatings(string id)
        {
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var currentUser = getUser();
            IQueryable<Rating> ratings = db.Rating.Include(r => r.Kennel).Where(b => b.KennelID == id);

            isCustomer();

            kenID = id; 
            //If the selected kennel has ratings it shows them a list of the ratings 
            if (ratings.Count() > 0)
            {                
                return View(ratings);
            }
            else
            {
                //if no ratings it shows an empty list with a message
                ViewBag.NoRate = "No ratings to show.";
                return View(ratings);
            }            
        }

        // GET: Ratings/Details/{RatingID}
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            //var currentUser = manager.FindById(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = await db.Rating.FindAsync(id);
            if (rating == null)
            {
                return HttpNotFound();
            }            
            return View(rating);
            
        }

        // GET: Ratings/Create
        public ActionResult Create(string id)
        {
            isCustomer();
            var currentUser = getUser();
            
            bool ratingExists = db.Rating.Where(r => r.KennelID == id && r.User.Id == currentUser.Id).Count() > 0;
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //If user has already rated the kennel 
            if(ratingExists == true)
            {
                //Redirects to the already created rating to edit, with a message        
                TempData["alreadyRated"] = "You have already rated this kennel you may edit your rating.";                
                var rateID = db.Rating.Where(r => r.KennelID == id && r.User.Id == currentUser.Id).First();
                return RedirectToAction("Edit", new { id = rateID.RatingID });
            }

            //Only user type customer may make ratings
            if (Customer == true)
            {       
                var kID = db.Kennel.Where(k => k.KennelID == id).First();
                ViewBag.KennelName = kID.Name;
                return View();
            }
            else
            {
                TempData["Unauth"] = "You are not authorised to do that, you must be logged in as a customer";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }            
        }

        // POST: Ratings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string id, [Bind(Include = "RatingID,Ratings,KennelID,Comment,RatingDate")] Rating rating)
        {
            var currentUser = getUser();

            if (ModelState.IsValid)
            {
                bool totalRatingExists = db.TotalRating.Where(k => k.KennelID == id).Count() > 0;

                //if the kennel has never been rated it will add an initial rating                      
                if (totalRatingExists == false)
                {
                    var newTRate = new TotalRating()
                    {
                        KennelID = id,
                        TotalRatings = rating.Ratings,
                        TotalRaters = 1,
                        AverageRating = rating.Ratings
                    };

                    db.TotalRating.Add(newTRate);
                    await db.SaveChangesAsync();
                }

                //if the kennel has been rated previously (by any user) it will update the existing entry.
                else
                {
                    //Adds 1 to total Raters on the TotalRatings table where the KennelId match those of the rating.
                    //Adds the rating to the TotalRatings.
                    //Updates AverageRating
                    var tra = db.TotalRating.Where(r => r.KennelID == id).First();
                    tra.TotalRaters += 1;
                    tra.TotalRatings += rating.Ratings;
                    tra.AverageRating = tra.calcAvgRating(tra.TotalRatings, tra.TotalRaters);
                    db.Entry(tra).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                rating.RatingDate = DateTime.Now;
                rating.User = currentUser;
                rating.KennelID = id;
                db.Rating.Add(rating);
                await db.SaveChangesAsync();

                TempData["Thank"] = "Thank you for your rating";
                return RedirectToAction("Index");
            }

            return View(rating);
        }


        // GET: Ratings/Edit/{RatingID}
        public async Task<ActionResult> Edit(int? id)
        {
            var currentUser = getUser();
            isCustomer();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = await db.Rating.FindAsync(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            //only owner of the rating may edit it
            if (rating.User != currentUser)
            {
                TempData["Unauth"] = "You are not authorised to do that, log in as a different user";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            ViewBag.KennelID = new SelectList(db.Kennel, "KennelID", "Name", rating.KennelID);
            return View(rating);
        }

        // POST: Ratings/Edit/{RatingID}
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<ActionResult> Edit([Bind(Include = "RatingID,Ratings,KennelID,Comment,RatingDate")] Rating rating)
        {
            
            if (ModelState.IsValid)
            {
                //rating above only takes RatingID and the changed Rating so it's queried to find the rest.
                var ra = db.Rating.Where(r => r.RatingID == rating.RatingID).First();
                var tra = db.TotalRating.Where(r => r.KennelID == ra.KennelID).First();

                //Update total ratings by minusing the old rating and adding the new, then calculating the new average.                
                tra.TotalRatings = (tra.TotalRatings - ra.Ratings) + rating.Ratings;
                tra.AverageRating = tra.calcAvgRating(tra.TotalRatings, tra.TotalRaters);
                db.Entry(tra).State = EntityState.Modified;
                

                //Adds the new rating to the queried variable and updates the database with it.
                ra.Ratings = rating.Ratings;
                ra.Comment = rating.Comment;
                ra.RatingDate = DateTime.Now;
                db.Entry(ra).State = EntityState.Modified;
                await db.SaveChangesAsync();
                
                TempData["Thank"] = "Rating Updated";
                return RedirectToAction("Index");
            }
            
            return View(rating);
        }

        // GET: Ratings/Delete/{RatingID}
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = getUser();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = await db.Rating.FindAsync(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            //only owner of the rating may delete it
            if (rating.User != currentUser)
            {
                TempData["Unauth"] = "You are not authorised to do that, log in as a different user";
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
            return View(rating);
        }

        // POST: Ratings/Delete/{RatingID}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Rating rating = await db.Rating.FindAsync(id);
            var tra = db.TotalRating.Where(r => r.KennelID == rating.KennelID).First();

            // Minuses 1 from the total raters, Minuses the rating from the total rating
            // Updates total rating table and removes the entry from the rating table
            if (tra.TotalRaters > 1)
            {
                tra.TotalRaters -= 1;
                tra.TotalRatings = (tra.TotalRatings - rating.Ratings);
                tra.AverageRating = (tra.TotalRatings / tra.TotalRaters);
                db.Entry(tra).State = EntityState.Modified;
            }
            else
            {
                db.TotalRating.Remove(tra);
            }
            
            db.Rating.Remove(rating);
            await db.SaveChangesAsync();
            TempData["Thank"] = "Rating Removed"; 
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
