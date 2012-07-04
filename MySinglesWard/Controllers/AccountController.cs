using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MSW.Models;
using MSW;
using MSW.Model;
using System.Text.RegularExpressions;
using System.Collections;
using Recaptcha;
using System.Web.UI.WebControls;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Controllers
{

    [HandleError]
    public class AccountController : Controller
    {

        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // **************************************
        // URL: /Account/LogOn
        // **************************************

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                string email = Membership.GetUserNameByEmail(model.UserName);
                if (email == null)
                {
                    ModelState.AddModelError("", "The email or password provided is incorrect. Make sure you are signing in with an email address.");
                    return View(model);
                }

                if (MembershipService.ValidateUser(email, model.Password))
                {
                    FormsService.SignIn(email, model.RememberMe);
                    if (!String.IsNullOrEmpty(returnUrl))
                    {
                        _NewSession(email);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Profile", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The email or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // **************************************
        // URL: /Account/LogOff
        // **************************************

        public ActionResult LogOff()
        {
            FormsService.SignOut();
            Session["Username"] = null;
            Session["WardStakeID"] = null;
            Session["MemberID"] = null;

            Session["IsBishopric"] = null;

            Session["HasPic"] = null;


            return RedirectToAction("Index", "Home");
        }

        // **************************************
        // URL: /Account/Register
        // **************************************

        public ActionResult Register()
        {
            TempData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View();
        }

        public ActionResult RegisterAgain(RegisterModel model)
        {
            TempData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }

        [HttpPost]
        public ActionResult MemberRegister(RegisterModel model)
        {
            bool emailSyntax = isEmail(model.Email);

            if (!emailSyntax)
            {
                TempData["Error"] = "-Email is entered wrong";

                return View(model);
            }
            bool recaptcha = PerformRecaptcha();
            if (ModelState.IsValid && recaptcha)
            {
                //The users email will be the username 
                model.UserName = model.Email;

                // Attempt to register the user
                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    _CreateUser(model, false /*This user in not a Bishopric User*/);

                    //Email New User
                    MSWtools._EmailNewMember(model.Email, model.UserName);
                    return RedirectToAction("Profile", "Home");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }


            // If we got this far, something failed, redisplay form
            TempData["PasswordLength"] = MembershipService.MinPasswordLength;
            if (!recaptcha)
                TempData["ReCaptcha"] = "ReCaptcha words were entered incorrectly.";

            return View(model);
        }

        // **************************************
        // Used to post Bishopric membership form
        // **************************************

        [HttpPost]
        public ActionResult BishopricRegister(RegisterModel model, string bishopricCode)
        {

            bool emailSyntax = isEmail(model.Email);

            if (!emailSyntax)
            {
                TempData["Error"] = "-Email is entered wrong";
                return View(model);
            }
            bool recaptcha = PerformRecaptcha();
            if (ModelState.IsValid && recaptcha)
            {
                if (bishopricCode.Equals("!NewBishopric"))
                {
                    //The users email will be the username 
                    model.UserName = model.Email;

                    MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                        _CreateUser(model, true /* This user is a Bishopric */);
                        System.Web.Security.Roles.AddUserToRole(model.UserName, "Bishopric");
                        return RedirectToAction("SelectWard", "Bishopric");
                    }
                    else
                    {
                        ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                    }

                }
                else
                {
                    TempData["Error"] = "Bishopric Code is incorrect.";
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            TempData["PasswordLength"] = MembershipService.MinPasswordLength;
            if (!recaptcha)
                TempData["ReCaptcha"] = "ReCaptcha words were entered incorrectly.";

            return View(model);
        }

        [HttpPost]
        public ActionResult StakeRegister(RegisterModel model, string stakeCode)
        {
            bool isPresidency;
            bool emailSyntax = isEmail(model.Email);

            if (!emailSyntax)
            {
                TempData["Error"] = "-Email is entered wrong";
                return View(model);
            }
            bool recaptcha = PerformRecaptcha();

            if (stakeCode.Equals("!NewStakePresidency"))
            {
                isPresidency = true;
            }
            else if (stakeCode.Equals("!NewStake"))
            {
                isPresidency = false;
            }
            else
            {
                TempData["Error"] = "Stake Code is incorrect.";

                return View(model);
            }

            if (ModelState.IsValid && recaptcha)
            {
                //The users email will be the username 
                model.UserName = model.Email;

                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, false /* createPersistentCookie */);
                    _CreateStakeUser(model, isPresidency);
                    return RedirectToAction("SelectStake", "Stake");
                }
                else
                {
                    ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                }
            }


            // If we got this far, something failed, redisplay form
            TempData["PasswordLength"] = MembershipService.MinPasswordLength;
            if (!recaptcha)
                TempData["ReCaptcha"] = "ReCaptcha words were entered incorrectly.";

            return View(model);
        }

        private static bool isEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            try
            {
                if (re.IsMatch(inputEmail))
                    return (true);
                else
                    return (false);
            }
            catch
            {
                return (false);
            }
        }

        internal void _CreateUser(RegisterModel model, bool isBishopric)
        {
            tUser newUser = new tUser();
            newUser.FirstName = Utilities.Cryptography.EncryptString(model.FirstName);
            newUser.LastName = Utilities.Cryptography.EncryptString(model.LastName);
            newUser.UserName = model.UserName;
            newUser.Email = Utilities.Cryptography.EncryptString(model.Email);
            newUser.IsBishopric = isBishopric;

            if (!isBishopric)
                System.Web.Security.Roles.AddUserToRole(model.UserName, "Member?");

            using (var db = new DBmsw())
            {
                db.tUsers.InsertOnSubmit(newUser);
                db.SubmitChanges();
            }

            //Setting user all notifiactions to true
            tNotificationPreference pref = new tNotificationPreference();
            pref.MemberID = newUser.MemberID;
            pref.txt = false;
            pref.email = true;
            pref.stake = true;
            pref.ward = true;
            pref.elders = true;
            pref.reliefsociety = true;
            pref.activities = true;
            pref.fhe = true;

            NotificationPreference.create(pref);
        }

        private void _CreateStakeUser(RegisterModel model, bool isPresidency)
        {
            tStakeUser newUser = new tStakeUser();
            newUser.FirstName = Utilities.Cryptography.EncryptString(model.FirstName);
            newUser.LastName = Utilities.Cryptography.EncryptString(model.LastName);
            newUser.UserName = model.UserName;
            newUser.Email = Utilities.Cryptography.EncryptString(model.Email);
            newUser.HasPic = false;
            newUser.isPresidency = isPresidency;
            if (!isPresidency)
                System.Web.Security.Roles.AddUserToRole(model.UserName, "Stake");
            else
                System.Web.Security.Roles.AddUserToRole(model.UserName, "StakePres");

            using (var db = new DBmsw())
            {
                db.tStakeUsers.InsertOnSubmit(newUser);
                db.SubmitChanges();
            }
        }

        // **************************************
        // URL: /Account/ChangePassword
        // **************************************

        [Authorize]
        public ActionResult ChangePref(bool? carrierError, bool? emailError, bool? success)
        {
            if (Session["Username"] == null)
                _NewSession();

            ViewData["TakenSurvey"] = MemberSurvey.getMemberSurvey(int.Parse(Session["MemberID"] as string)) != null ? "true" : "false";

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            if (carrierError == true)
                ViewData["Error"] = "Please choose a service provider";
            if (emailError == true)
                ViewData["Error"] = "Email is already in use";
            if (success == true)
                ViewData["Success"] = "Your Preferences have been saved.";

            if (Session["IsBishopric"] == null)
                ViewData["IsBishopric"] = true;
            else
                ViewData["IsBishopric"] = Session["IsBishopric"];

            NotificationPreference preference = NotificationPreference.get(int.Parse(Session["MemberID"] as string));
            ViewData["MemberID"] = int.Parse(Session["MemberID"] as string);
            if (preference == null)
            {
                //Notification Information
                ViewData["txt"] = 0;
                ViewData["carrier"] = "Cell Phone Provider";
                ViewData["EmailBox"] = 0;
                ViewData["stake"] = 0;
                ViewData["ward"] = 0;
                ViewData["elders"] = 0;
                ViewData["reliefsociety"] = 0;
                ViewData["activities"] = 0;
                ViewData["fhe"] = 0;
                ViewData["NotificationPref"] = preference;
            }
            else
            {
                ViewData["txt"] = preference.txt;
                ViewData["carrier"] = preference.carrier;
                ViewData["EmailBox"] = preference.email;
                ViewData["stake"] = preference.stake;
                ViewData["ward"] = preference.ward;
                ViewData["elders"] = preference.elders;
                ViewData["reliefsociety"] = preference.reliefsociety;
                ViewData["activities"] = preference.activities;
                ViewData["fhe"] = preference.fhe;
                ViewData["NotificationPref"] = preference;
            }

            return View();
        }

        private string _getCarrier(string carrier)
        {
            if (carrier == null)
                return "Cell Phone Provider";

            String[] CarrierText = new String[] {"ACS Wireless", 
                                       "Alltel", 
                                       "AT&T", 
                                       "Bell Mobility",
                                       "Blue Sky Frog", 
                                       "Bluegrass Cellular", 
                                       "Boost Mobile", 
                                       "Nextel", 
                                       "Qwest", 
                                       "Sprint", 
                                       "T-Mobile",
                                       "Tracfone", 
                                       "US Cellular", 
                                       "Verizon", 
                                       "Virgin Mobile"};
            String[] CarrierValue = new String[] {"@paging.acswireless.com", 
                                        "@message.alltel.com", 
                                        "@txt.att.net",
                                        "@txt.bellmobility.ca", 
                                        "@blueskyfrog.com",
                                        "@sms.bluecell.com",
                                        "@myboostmobile.com",
                                        "@messaging.nextel.com",
                                        "@qwestmp.com",
                                        "@messaging.sprintpcs.com", 
                                        "@tmomail.net", 
                                        "@txt.att.net",
                                        "@email.uscc.net", 
                                        "@vtext.com", 
                                        "@vmobl.com" };

            int position = 0;
            foreach (String carriertype in CarrierValue)
            {
                if (carriertype.Equals(carrier))
                {
                    return CarrierText[position];
                }
                position++;
            }

            return CarrierText[position];
        }

        [Authorize]
        [HttpPost]
        public ActionResult NotifcationPreferences(NotificationPreference pref, FormCollection collection)
        {
            if (Session["Username"] == null)
                _NewSession();

            try
            {
                string carrier = pref.carrier;
                if (pref.carrier == null)
                    pref.carrier = carrier;
                NotificationPreference.save(pref);
            }
            catch (Exception e)
            {
                MSWtools._sendException(e);
                return RedirectToAction("ChangePref", "Account", new { carrierError = true });
            }

            return RedirectToAction("ChangePref", "Account", new { success = true });
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangePref(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
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

            // If we got this far, something failed, redisplay form
            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;
            return View(model);
        }



        [Authorize]
        [HttpPost]
        public ActionResult ChangeEmail(string emailfield)
        {
            string checkEmail = null;
            try
            {
                checkEmail = Membership.GetUserNameByEmail(emailfield);

                if (checkEmail == null)
                {
                    ViewData["Error"] = "Email already in use";
                    return RedirectToAction("ChangePref", "Account", new { emailError = true });
                }
            }
            catch
            {
                checkEmail = "FAIL";
                return RedirectToAction("Error", "Home");
            }

            //Email Checks out. Change it
            try
            {
                MembershipUser thisUser;
                if (isEmail(emailfield))
                {
                    //Changes Email in Database
                    MSWUser user = MSWUser.getUser(User.Identity.Name);
                    if (user != null)
                    {
                        user.Email = emailfield;
                        MSWUser.saveUser(user);

                        //Changes Email in Security Database
                        thisUser = Membership.GetUser(user.UserName);
                    }
                    else
                    {
                        StakeUser stakeUser = StakeUser.getStakeUser(User.Identity.Name);
                        stakeUser.Email = emailfield;
                        StakeUser.saveUser(stakeUser);

                        //Changes Email in Security Database
                        thisUser = Membership.GetUser(stakeUser.UserName);
                    }
                    thisUser.Email = emailfield;
                    Membership.UpdateUser(thisUser);

                    return RedirectToAction("Profile", "Home");
                }
                ViewData["Error"] = "Invalid Email";
                return RedirectToAction("ChangePref", "Account");
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        // **************************************
        // URL: /Account/ChangePasswordSuccess
        // **************************************

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            if (!isEmail(email) || email == null)
            {
                ModelState.AddModelError("email", "Please enter a valid email");
                if (Request.Browser.IsMobileDevice)
                {
                    TempData["Route"] = "emailerror";
                    return View("_PasswordRecover");
                }
                return View();
            }

            if (!MembershipService.ResetPassword(email))
            {
                TempData["Route"] = "emailerror";
                return RedirectToAction("FailRecover", "Account");
            }
            else
            {
                TempData["Route"] = "success";
                return RedirectToAction("SuccessRecover", "Account");
            }

        }

        public ActionResult FailRecover()
        {
            return View();
        }
        public ActionResult SuccessRecover()
        {
            return View();
        }

        private bool PerformRecaptcha()
        {
            var validator = new RecaptchaValidator
            {
                PrivateKey = "6Lc3idESAAAAAE53j9CxMFi_P-X4B_N2vdMDO7qE",
                RemoteIP = Request.UserHostAddress,
                Response = Request.Form["recaptcha_response_field"],
                Challenge = Request.Form["recaptcha_challenge_field"]
            };

            try
            {
                var validationResult = validator.Validate();

                if (validationResult.ErrorMessage == "incorrect-captcha-sol")
                    ModelState.AddModelError("ReCaptcha", string.Format("Please retry the ReCaptcha portion again."));

                return validationResult.IsValid;
            }
            catch
            {
                ModelState.AddModelError("ReCaptcha", "an error occured with ReCaptcha please consult documentation.");
                return false;
            }
        }

        private void _NewSession()
        {
            try
            {
                MSWUser user = MSWUser.getUser(User.Identity.Name);

                //Checks the WardID and changes the ID to zero if the stake is missing
                //user = MSWtools._checkWardID(user);

                Session["Username"] = user.UserName.ToString();
                Session["WardStakeID"] = user.WardStakeID.ToString();
                Session["MemberID"] = user.MemberID.ToString();
                Session["IsBishopric"] = user.IsBishopric.ToString();
            }
            catch
            {
                StakeUser user = StakeUser.getStakeUser(User.Identity.Name);

                //Checks the StakeID and changes the ID to zero if the stake is missing
                user = MSWtools._checkStakeID(user);

                Session["Username"] = user.UserName.ToString();
                Session["StakeID"] = user.StakeID.ToString();
                Session["MemberID"] = user.MemberID.ToString();
                Session["IsPresidency"] = user.IsPresidency.ToString();
                Session["HasPic"] = user.HasPic.ToString();
            }
        }

        private void _NewSession(string UserName)
        {
            try
            {
                MSWUser user = MSWUser.getUser(UserName);

                //Checks the WardID and changes the ID to zero if the stake is missing
                //user = MSWtools._checkWardID(user);

                Session["Username"] = user.UserName.ToString();
                Session["WardStakeID"] = user.WardStakeID.ToString();
                Session["MemberID"] = user.MemberID.ToString();
                Session["IsBishopric"] = user.IsBishopric.ToString();

                //Get gender information for menu
                try
                {
                    Session["isPriesthood"] = MemberSurvey.getMemberSurvey(user.MemberID).gender.ToString();
                }
                catch
                {

                }
            }
            catch
            {
                StakeUser user = StakeUser.getStakeUser(UserName);

                //Checks the StakeID and changes the ID to zero if the stake is missing
                user = MSWtools._checkStakeID(user);

                Session["Username"] = user.UserName.ToString();
                Session["StakeID"] = user.StakeID.ToString();
                Session["MemberID"] = user.MemberID.ToString();
                Session["IsPresidency"] = user.IsPresidency.ToString();
                Session["HasPic"] = user.HasPic.ToString();
            }
        }

        // **************************************
        // DropDown Support
        // **************************************

        [Authorize]
        public ActionResult getStakes(string location)
        {
            return Json(Repository.getInstance().getSelectedStakeList(location), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult getWards(string location, string stake)
        {
            return Json(Repository.getInstance().getSelectedWardList(location, stake), JsonRequestBehavior.AllowGet);
        }
    }
}
