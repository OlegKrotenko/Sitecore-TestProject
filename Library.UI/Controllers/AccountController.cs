using Library.Domain.Models;
using Library.Services;
using Library.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Library.UI.Controllers
{
    public class AccountController : Controller
    {
        readonly private IUserService service;

        public AccountController(IUserService service)
        {
            this.service = service;
        }
        // GET: Account
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
              return  RedirectToAction("Index", "Library");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (service.RegistrationNewUser(user))
                    {                       
                        FormsAuthentication.SetAuthCookie(user.Email, false);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Library", new { message = ex.Message } );
            }
         
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User model, string returnUrl)
        {
            // Lets first check if the Model is valid or not
            if (ModelState.IsValid)
            {

                string userMail = model.Email;
                bool userValid = service.Login(model);
                if (userValid)
                {

                    FormsAuthentication.SetAuthCookie(userMail, false);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {

                        return RedirectToAction("Index", "Library");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user email is incorrect.");
                }

            }
            return View(model);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Library");
        }
    }
}