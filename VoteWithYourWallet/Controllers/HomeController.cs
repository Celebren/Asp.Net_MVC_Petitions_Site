using System;
using System.Collections.Generic;
using System.Data;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Start([Bind(Include = "Title, Text")] Petition petition) {
            petition.Signatures = 0;
   
            // validation; inform user that both fields must be filled
            if (petition.Title == null) {
                ModelState.AddModelError("",
                    "Please provide a title for your cause");
            } else if (petition.Text == null) {
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


        public ActionResult Browse() {
            ViewBag.Message = "Browse causes started by others";

            // pass the database using entity framework
            ViewData.Model = _db.Petitions.ToList();

            return View();
        }

        public ActionResult SampleCause() {
            return View();
        }
    }
}