using MVCVDO.Models;
using MVCVDO.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCVDO.Controllers
{
    public class EventController : Controller
    {
        // GET: Event/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Event/Register
        [HttpPost]
        public ActionResult Register(Users user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    EventRepository EvnRepo = new EventRepository();
                    if (EvnRepo.Register(user))
                    {
                        ViewBag.Message = "Register Successfully";
                        ModelState.Clear();
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        // GET: Event/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Event/Login
        [HttpPost]
        public ActionResult Login(Users user)
        {
            try
            {
                EventRepository EvnRepo = new EventRepository();

                DataTable dt= EvnRepo.Login(user);


                if (dt.Rows.Count>0) 
                {
                    string id = dt.Rows[0]["id"].ToString();
                    Session["user_id"] = id;

                    string mail = dt.Rows[0]["email"].ToString();
                    Session["email"] = mail;

                    string type = dt.Rows[0]["usertype"].ToString();
                  
                    if (type=="admin")
                    {
                        return View("../Event/AdminHome");
                    }
                    else if (type == "user")
                    {
                        return View("../Event/HomePage");
                    }
                    else
                    {
                        TempData["NotSuccess"] = "Username or password not exist";
                        return View();
                    }
                } else
                {
                    TempData["NotSuccess"]="Username or password not exist";
                    return View();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Event/AdminHome
        public ActionResult AdminHome()
        {
            return View();
        }

        // GET: Event/GetAllEmail
        public ActionResult GetAllEmail()
        {
            EventRepository EvnRepo = new EventRepository();
            ModelState.Clear();
            return View(EvnRepo.GetAllEmail());
        }


        // GET: Event/HomePage
        public ActionResult HomePage()
        {
            return View();
        }

        //// GET: Event
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //// GET: Event/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        //// GET: Event/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Event/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Event/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //// POST: Event/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //// GET: Event/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: Event/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
