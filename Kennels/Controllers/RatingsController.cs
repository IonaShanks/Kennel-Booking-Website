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
    public class RatingsController : Controller
    {
        public static bool Customer = false;

        private KennelsContext db = new KennelsContext();
        private UserManager<ApplicationUser> manager;
        public RatingsController()
        {
            db = new KennelsContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }
        // GET: Ratings (user specific)       
        public ActionResult Index()
        {
            Customer = false;
            var currentUser = manager.FindById(User.Identity.GetUserId());
            IQueryable<Rating> ratings = db.Rating.Include(k => k.Kennel).Where(b => b.User.Id == currentUser.Id);

            if (currentUser.UserType == UserType.Customer)
            {
                Customer = true;
                //If the Current user has ratings it shows them a list of their ratings, if not it redirects the the create action. 
                if (ratings != null)
                { return View(db.Rating.ToList().Where(r => r.User == currentUser)); }
                else
                {
                    ViewBag.NoRate = "No ratings to show.";
                    return View(ratings);
                }
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        //Shows kennel ratings for specific kennels
        [AllowAnonymous]
        public ActionResult KennelRatings(string id)
        {
            
            var currentUser = manager.FindById(User.Identity.GetUserId());
            IQueryable<Rating> ratings = db.Rating.Include(r => r.Kennel).Where(b => b.KennelID == id);

            Customer = false;
            if (currentUser != null)
            {
                if (currentUser.UserType == UserType.Customer)
                {
                    Customer = true;
                }
            }
            //If the Current user has ratings it shows them a list of their ratings, if not it shows an empty list. 
            if (ratings != null)
            {                
                return View(db.Rating.ToList().Where(r => r.KennelID == id));
            }
            else
            {
                ViewBag.NoRate = "No ratings to show.";
                return View(ratings);
            }            
        }

        // GET: Ratings/Details/{RatingID}
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            var currentUser = manager.FindById(User.Identity.GetUserId());
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
        public ActionResult Create()
        {
            Customer = false;
            var currentUser = manager.FindById(User.Identity.GetUserId());

            if (currentUser.UserType == UserType.Customer)
            {             
                Customer = true;                 
                ViewBag.KennelID = new SelectList(db.Kennel, "KennelID", "Name");
                return View();
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }            
        }

        // POST: Ratings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "RatingID,Ratings,KennelID,Comment,RatingDate")] Rating rating)
        {       
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            
            bool ratingExists = db.Rating.Where(r => r.KennelID == rating.KennelID && r.User.Id == currentUser.Id).Count() > 0;

            if (ratingExists == false)
            {
                if (ModelState.IsValid)
                {
                    bool totalRatingExists = db.TotalRating.Where(k => k.KennelID == rating.KennelID).Count() > 0;

                    //if the kennel has never been rated it will add an initial rating                      
                    if (totalRatingExists == false)
                    {
                        var newTRate = new TotalRating()
                        {
                            KennelID = rating.KennelID,
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
                        var tra = db.TotalRating.Where(r => r.KennelID == rating.KennelID).First();
                        tra.TotalRaters += 1;
                        tra.TotalRatings += rating.Ratings;
                        tra.AverageRating = tra.calcAvgRating(tra.TotalRatings, tra.TotalRaters);
                        db.Entry(tra).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                    }

                    rating.RatingDate = DateTime.Now;
                    rating.User = currentUser;
                    db.Rating.Add(rating);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }

                return View(rating);
            }
            else
            {
                ViewBag.AlreadyRated = "You have already rated this kennel you may edit your rating.";
                //Redirects to the already created rating to edit so each user may only add one rating per kennel
                var rateID = db.Rating.Where(r => r.KennelID == rating.KennelID && r.User.Id == currentUser.Id).First();
                return RedirectToAction("Edit", new { id = rateID.RatingID });
            }


        }

        // GET: Ratings/Edit/{RatingID}
        public async Task<ActionResult> Edit(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = await db.Rating.FindAsync(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            if (rating.User != currentUser)
            {
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
                return RedirectToAction("Index");
            }

            ViewBag.KennelID = new SelectList(db.Kennel, "KennelID", "Name", rating.KennelID);
            return View(rating);
        }

        // GET: Ratings/Delete/{RatingID}
        public async Task<ActionResult> Delete(int? id)
        {
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = await db.Rating.FindAsync(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            if (rating.User != currentUser)
            {
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
            tra.TotalRaters -= 1;
            tra.TotalRatings = (tra.TotalRatings - rating.Ratings);
            db.Entry(tra).State = EntityState.Modified;
            db.Rating.Remove(rating);
            await db.SaveChangesAsync();
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
