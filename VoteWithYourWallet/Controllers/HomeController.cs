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

        // Create new petition
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Start([Bind(Include = "Title, Text")] Petition petition) {
            petition.Signatures = 0;

            // inform user that both fields must be filled
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
                ModelState.AddModelError("", "Unable to save to database.");
            }

            return View(petition);
        }

        // View cause controller Home/ViewCause/petition.id
        public ActionResult ViewCause(int id) {
            // instantiate petition for database with id taken from method call
            Petition petitionToView = _db.Petitions.Find(id);
            ViewBag.Message = petitionToView.Title;
            
            // return the view passing the petition to it
            return View(petitionToView);
        }

        // Increment signatures when the button is pressed
        [HttpPost, ActionName("ViewCause")]
        [ValidateAntiForgeryToken]
        public ActionResult ViewCauseIncremented(int id) {
            // instantiate petition from database
            Petition petition = _db.Petitions.Find(id);
            // increase the signatures number by one
            petition.Signatures += 1;

            try {
                if (ModelState.IsValid) {
                    // update the entry in the database        
                    _db.Entry(petition).State = EntityState.Modified;
                    _db.SaveChanges();
                    //Debug.WriteLine("petition id: " + petition.Id);
                    // return the ViewCuase page
                    return RedirectToAction("ViewCause/" + petition.Id);
                }
            }
            catch (DataException) {
                ModelState.AddModelError("", "Unable to save changes to the database.");
            }

            return View(petition);
        }


        public ActionResult Browse() {
            ViewBag.Message = "Browse causes started by others";

            // pass the database using entity framework as list
            ViewData.Model = _db.Petitions.ToList();

            return View();
        }


        // /Home/Delete/petition.Id
        public ActionResult Delete(int id) {
            // instantiate and return petition with passed id number
            Petition petitionToBeDeleted = _db.Petitions.Find(id);
            return View(petitionToBeDeleted);
        }

        // Delete confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            // find petition with passed id and remove it from the database
            Petition petition = _db.Petitions.Find(id);
            _db.Petitions.Remove(petition);
            _db.SaveChanges();
            return RedirectToAction("Browse");
        }
    }
}