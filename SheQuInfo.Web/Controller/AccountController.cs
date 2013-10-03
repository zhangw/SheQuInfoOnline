using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SheQuInfo.Models;
using SheQuInfo.Models.Logic;
using SheQuInfo.Models.Model;
using SheQuInfo.Web.Membership;
using SheQuInfo.Web.Models;

namespace SheQuInfo.Web.Controller
{
    public class AccountController : SheQuController
    {
        private UserService mUserService;

        public AccountController(IUnitWork unitWork)
            : base(unitWork)
        {
            mUserService = new UserService(unitWork);
        }

        public IFormsAuthenticationService FormsService { get; set; }

        public IMembershipService MembershipService { get; set; }

        [Authorize]
        public ActionResult ChangePassword()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************
        [Authorize]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                    {
                        return RedirectToAction("ChangePasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);

                    //Logger.LogError("Account", "ChangePassword", e, "", "");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult LogOff()
        {
            FormsService.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOn()
        {
            return View();
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************
        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (MembershipService.ValidateUser(model.UserName, model.Password))
                    {
                        FormsService.SignIn(model.UserName, model.RememberMe);

                        //将角色加入
                        var user = mUserService.GetUserByName(model.UserName);
                        SetCookie(user);

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "The user name or password provided is incorrect.");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);

                    //TODO
                    //Logger.LogError("Account", "LogOn", e, "", "");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Register()
        {
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View();
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************
        // **************************************
        // URL: /Account/Register
        // **************************************
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Attempt to register the user
                    MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", ErrorCodeToString(createStatus));
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);

                    //Logger.LogError("Account", "Register", e, "", "");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.PasswordLength = MembershipService.MinPasswordLength;
            return View(model);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        private void SetCookie(User user)
        {
            string rolos = string.Join("|", user.Roles.Select(m => m.RoleName));
            string roleTypes = string.Join("|", user.Roles.Select(m => m.RoleType));
            var croleTypes = new HttpCookie("roleTypes", roleTypes);
            croleTypes.Expires = DateTime.MaxValue;
            Response.Cookies.Add(croleTypes);

            var croles = new HttpCookie("roleTypes", roleTypes);
            croles.Expires = DateTime.MaxValue;
            Response.Cookies.Add(croles);
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************
    }
}