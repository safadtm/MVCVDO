using MVCVDO.Models;
using MVCVDO.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
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


        // GET:Event/EmailAndPasswordSend
        public ActionResult EmailAndPasswordSend(int id)
        {
            EventRepository EvnRepo = new EventRepository();
            
            return View(EvnRepo.GetAllEmail().Find(users=>users.id==id));
        }

        // POST:Event/EmailAndPasswordSend
        [HttpPost]
        public ActionResult EmailAndPasswordSend(Users user)
        {

            SendEmailToUser(user.email, user.usertype);
            ViewBag.Message = "Password send to email";
            ModelState.Clear(); 
            return View();
        }

        // auto generate password
        public string GeneratePassword()
        {
            string PasswordLength = "8";
            string NewPassword = "";
            string allowedChars = "";
            allowedChars = "1,2,3,4,5,6,7,8,9,0";
            allowedChars += "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,Z";
            allowedChars += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            char[] sep =
            {
                 ','
            };

            
            string[] arr = allowedChars.Split(sep);
            string IDString = "";
            string temp = "";

            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(PasswordLength); i++)
            {
                temp = arr[rand.Next(arr.Length)];
                IDString += temp;
                NewPassword = IDString;
            }
            return NewPassword;
        }

        // Send Email To User
        public void SendEmailToUser(string emailId, string usertype)
        {
            string strNewPassword = GeneratePassword().ToString();
            EventRepository EmpRepo = new EventRepository();

            // Save username and password in the database
            EmpRepo.SaveDetails(emailId, strNewPassword, usertype);

            var fromMail = new MailAddress("theboicoder@gmail.com", "ADMIN");
            var fromEmailpassword = "adgggcaqsgwveebg"; // Use the generated app password here
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword)
            };

            var message = new System.Net.Mail.MailMessage(fromMail, toEmail)
            {
                Subject = "Registration Completed-MVC APP",
                Body = $"<p>Your registration is complete. Your user ID is <strong>{toEmail.Address}</strong> and your password is <strong>{strNewPassword}</strong>.</p>",
                IsBodyHtml = true
            };

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

        }

        // GET: Event/AddDepartment

        public ActionResult AddDepartment()
        {
            return View();
        }

        // POST: Event/AddDepartment
        [HttpPost]
        public ActionResult AddDepartment(Department dept)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    EventRepository EvnRepo = new EventRepository();
                    if (EvnRepo.AddDepartment(dept))
                    {
                        ViewBag.Message = "Department added Successfully";
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

        // GET: Event/ViewAllApplication
        public ActionResult ViewAllApplication()
        {
            EventRepository EvnRepo = new EventRepository();
            ModelState.Clear();
            return View(EvnRepo.GetMyApplicationDetails());
        }


        // GET:Event/ViewApplicationDetails

        public ActionResult ViewApplicationDetails(int aid)
        {
            EventRepository EvnRepo = new EventRepository();

            return View(EvnRepo.GetMyApplicationDetails().Find(emp => emp.A_Id == aid));

        }

        // POST:Event/Reject
        [HttpPost]
        public ActionResult ViewApplicationDetails(Application app, int aid, int rid)
        {
            try
            {
                EventRepository EvnRepo = new EventRepository();
                Users user = EvnRepo.GetUserById(rid);

                string mail = user.email;
                EvnRepo.AcceptApplication(aid);

                string status = "Accepted";
                ViewBag.Status = status;

                // email sending
                AcceptDeleteMailSendToUser(mail, status);

                ModelState.Clear();
                return View("../Event/AdminHome");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // GET:Event/ViewApplicationDetails

        public ActionResult RejectApplication(int aid, int rid)
        {
            try
            {
                EventRepository EvnRepo = new EventRepository();
                Users user = EvnRepo.GetUserById(rid);

                string mail = user.email;
                EvnRepo.RejectApplication(aid);

                string status = "Rejected";
                ViewBag.Status = status;

                // email sending
                AcceptDeleteMailSendToUser(mail, status);

                ModelState.Clear();
                return View("../Event/AdminHome");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // Accept or Delete Mail Send To User
        public void AcceptDeleteMailSendToUser(string emailId, string status)
        {


            var fromMail = new MailAddress("your@gmail.com", "your name");
            var fromEmailpassword = "your app password"; // Use the generated app password here
            var toEmail = new MailAddress(emailId);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword)
            };

            var message = new System.Net.Mail.MailMessage(fromMail, toEmail)
            {
                Subject = "Application Status",
                Body = $"<p>Application <strong>{status}</strong>.</p>",
                IsBodyHtml = true
            };

            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                Console.WriteLine($"Error sending email: {ex.Message}");
            }

        }

        // GET: Event/HomePage
        public ActionResult HomePage()
        {
            return View();
        }

        // GET:Event/ResetPassword
        public ActionResult ResetPassword()
        {
            int id = Session["user_id"] != null ? Convert.ToInt32(Session["user_id"]) : 0;
           // ViewBag.UserId = id;

            // Fetch the current password from the database
            EventRepository EvnRepo = new EventRepository();
            var user = EvnRepo.GetUserById(id); // Assuming this fetches user data by ID

            if (user != null)
            {
                var model = new Users
                {
                    currentPassword = user.password // Load current password into the model
                };
                return View(model);
            }

            ViewBag.Message = "User not found.";
            return View();
        }

        // POST:Event/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(Users user)
        {
            int id = Session["user_id"] != null ? Convert.ToInt32(Session["user_id"]) : 0;
            ViewBag.UserId = id;

            try
            {
                // Ignore validation for the email field
                ModelState.Remove("email");

                if (ModelState.IsValid)
                {
                    EventRepository EvnRepo = new EventRepository();
                    if (EvnRepo.ResetPassword(id, user))
                    {
                        ViewBag.Message = "Password reset successfully";
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


        // GET:Event/MyApplication
        public ActionResult MyApplication()
        {
            try
            {
                EventRepository EvnRepo = new EventRepository();

                int id = Convert.ToInt32(Session["user_id"]);

                DataTable dt = EvnRepo.GetApplicationStatus(id);

                if (dt.Rows.Count > 0)
                {
                    string type = dt.Rows[0]["ApplicationStatus"].ToString();
                    ViewBag.Flag = 1;
                }
                else
                {
                    ViewBag.Flag = 0;
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET:Event/NewApplication
        public ActionResult NewApplication()
        {
            try
            {
                EventRepository EvnRepo = new EventRepository();
                DataTable dt = EvnRepo.SelectDepartment(new Department());
                ViewBag.DepartmentList = EvnRepo.ToSelectList(dt, "D_Id", "DepartmentName");
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST:Event/NewApplication
        [HttpPost]
        public ActionResult NewApplication(Application app)
        {
            int id = Session["user_id"] != null ? Convert.ToInt32(Session["user_id"]) : 0;
            try
            {
                if (ModelState.IsValid)
                {
                    EventRepository EvnRepo = new EventRepository();
                    if (EvnRepo.NewApplication(id, app))
                    {
                        ViewBag.Message = "Application submitted successfully";
                        ModelState.Clear();
                    }
                }

                return View("../Event/HomePage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET: Event/MyApplicationDetails
        public ActionResult MyApplicationDetails()
        {
            int id = Session["user_id"] != null ? Convert.ToInt32(Session["user_id"]) : 0;
            EventRepository EvnRepo = new EventRepository();

            DataTable dt = EvnRepo.SelectDepartment(new Department());
            ViewBag.DepartmentList = EvnRepo.ToSelectList(dt, "D_Id", "DepartmentName");
            ModelState.Clear();
            return View(EvnRepo.GetMyApplicationDetails().Find(evn => evn.R_Id == id));
        }

        // POST: Event/MyApplicationDetails
        [HttpPost]
        public ActionResult MyApplicationDetails(string buttonType, Application app)
        {
            ModelState.Remove("ApplicationStatus");
            EventRepository EvnRepo = new EventRepository();

            DataTable dt = EvnRepo.SelectDepartment(new Department());
            ViewBag.DepartmentList = EvnRepo.ToSelectList(dt, "D_Id", "DepartmentName");

            // Handle Update
            if (buttonType == "Update")
            {
                if (app.A_Id > 0)
                {
                    bool isUpdated = EvnRepo.UpdateMyApplication(app);
                    ViewBag.Message = isUpdated ? "Updated Successfully" : "Update Failed";
                }
                else
                {
                    ViewBag.Message = "Application ID is missing or invalid.";
                }
            }

            // Handle Delete
            if (buttonType == "Delete")
            {
                if (app.A_Id > 0)
                {
                    bool isDeleted = EvnRepo.RejectApplication(app.A_Id);
                    ViewBag.Message = isDeleted ? "Deleted Successfully" : "Delete Failed";
                }
                else
                {
                    ViewBag.Message = "Application ID is missing or invalid.";
                }
            }

            ModelState.Clear();
            return View("../Event/HomePage");
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
