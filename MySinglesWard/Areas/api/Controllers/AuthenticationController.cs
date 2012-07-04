using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MSW.Models;
using Newtonsoft.Json;
using MSW.Model;
using MSW.Models.dbo;
using MSW.Controllers;
using MSW.Utilities;

namespace MSW.Areas.api.Controllers
{
    [HandleError]
    public class AuthenticationController : Controller
    {
        public IFormsAuthenticationService FormsService { get; set; }
        public IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }
        //
        // GET: /api/Account/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Login(string email, string password)
        {
            if (MembershipService.ValidateUser(Membership.GetUserNameByEmail(email), password))
            {
                FormsService.SignIn(Membership.GetUserNameByEmail(email), true);

                string NEEDS_NOTHING = "NEEDS_NOTHING";
                string NEEDS_PHOTO = "NEEDS_PHOTO";
                string NEEDS_SURVEY = "NEEDS_SURVEY";
                string UNAUTHORIZED = "UNAUTHORIZED";

                MemberModel currentUser = MemberModel.get(Membership.GetUserNameByEmail(email));
                List<string> status = new List<string>();

                if (User.IsInRole("Member?"))
                    status.Add(UNAUTHORIZED);

                if (currentUser.memberSurvey == null && !currentUser.user.IsBishopric)
                    status.Add(NEEDS_SURVEY);
                else if (currentUser.photo.NewPhotoFileName == null && currentUser.photo.Status < (int)PhotoStatus.CROPPED)
                {
                    status.Add(NEEDS_PHOTO);
                }
                else
                {
                    status.Add(NEEDS_NOTHING);
                }

                return JsonConvert.SerializeObject(new { authentication = status });
            }
            else
            {
                return JsonConvert.SerializeObject(new { authentication = new List<string>() });
            }
        }

        [HttpPost]
        public string Register(RegisterModel model)
        {
            //The users email will be the username 
            model.UserName = model.Email;

            // Attempt to register the user
            MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

            if (createStatus == MembershipCreateStatus.Success)
            {
                FormsService.SignIn(model.UserName, true /* createPersistentCookie */);
                new AccountController()._CreateUser(model, false /*This user in not a Bishopric User*/);

                //Email New User
                MSWtools._EmailNewMember(model.Email, model.UserName);
                return JsonConvert.SerializeObject(new { memberID = MSWUser.getUser(model.UserName).MemberID });
            }
            else
                return JsonConvert.SerializeObject(new { memberID = 0 });
        }

        [HttpPost]
        public string BishopricRegister(RegisterModel model, string bishopricCode)
        {
            if (bishopricCode.Equals("!NewBishopric"))
            {
                //The users email will be the username 
                model.UserName = model.Email;

                MembershipCreateStatus createStatus = MembershipService.CreateUser(model.UserName, model.Password, model.Email);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsService.SignIn(model.UserName, true /* createPersistentCookie */);
                    new AccountController()._CreateUser(model, true /* This user is a Bishopric */);
                    System.Web.Security.Roles.AddUserToRole(model.UserName, "Bishopric");
                    return JsonConvert.SerializeObject(new { memberID = MSWUser.getUser(model.UserName).MemberID });
                }
                else
                    return JsonConvert.SerializeObject(new { memberID = 0 });
            }

            throw new Exception();
        }

    }
}
