using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VoteWithYourWallet.Models;

namespace VoteWithYourWallet.Controllers {
    public class HomeController : Controller {
        private PetitionsDBEntities _db;

        public HomeController() {
            // instantiate _db when HomeController constructor is called
            _db = new PetitionsDBEntities();
        }

        public ActionResult Index() {
            return View();
        }

        public ActionResult Start() {
            ViewBag.Message = "Start a new cause";
            return View();
        }

        // Create method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Start([Bind(Include = "Title, Text")] Petition petition) {
            petition.Signatures = 0;

            // validation; inform user that both fields must be filled
            if (petition.Title == null) {
                ModelState.AddModelError("",
                    "Please provide a title for your cause");
            }
            else if (petition.Text == null) {
                ModelState.AddModelError("",
                    "Please provide a description for your cause");
            }

            try {
                if (ModelState.IsValid) {
                    // add object to database
                    _db.Petitions.Add(petition);
                    _db.SaveChanges();

                    return RedirectToAction("Browse");
                }
            }
            catch (DataException) {
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(petition);
        }

        // View cause controller
        public ActionResult ViewCause(int id) {
            Petition petitionToView = _db.Petitions.Find(id);
            ViewBag.Message = petitionToView.Title;

            return View(petitionToView);
        }

        // Increment signatures
        [HttpPost, ActionName("ViewCause")]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCauseIncremented(int id) {
            Petition petition = _db.Petitions.Find(id);
            petition.Signatures += 1;
            try {
                if (ModelState.IsValid) {
                    
                    _db.Entry(petition).State = EntityState.Modified;
                    _db.SaveChanges();
                    Debug.WriteLine("petition id: " + petition.Id);
                    return RedirectToAction("ViewCause/" + petition.Id);
                }
            }
            catch (DataException) {
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(petition);
        }


        public ActionResult Browse() {
            ViewBag.Message = "Browse causes started by others";

            // pass the database using entity framework
            ViewData.Model = _db.Petitions.ToList();

            return View();
        }


        // /Home/Delete
        public ActionResult Delete(int id) {
            Petition petitionToBeDeleted = _db.Petitions.Find(id);
            return View(petitionToBeDeleted);
        }

        // Delete method
        // POST: /Home/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Petition petition = _db.Petitions.Find(id);
            _db.Petitions.Remove(petition);
            _db.SaveChanges();
            return RedirectToAction("Browse");
        }

        public ActionResult SampleCause() {
            return View();
        }
    }
}