using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using MSW.Model;
using MSW;
using MSW.Models;
using System.IO;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using MSW.Utilities;
using MSW.Models.dbo;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace MSW.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
			if (User.Identity.IsAuthenticated)
			{
				try
				{
					if (Session["Username"] == null)
						_NewSession();
					return RedirectToAction("Profile", "Home");
				}
				catch
				{
					return RedirectToAction("Index", "Stake", null);
				}
			}
			
			return View();
        }

        [Authorize]
        public ActionResult Profile()
        {
            if (User.IsInRole("StakePres") || User.IsInRole("Stake"))
                return RedirectToAction("Index", "Stake");
			try
			{
				if (Session["Username"] == null)
					_NewSession();
			}
			catch
			{
				return RedirectToAction("Index", "Stake", null);
			}

            MemberModel currentUser = MemberModel.get(Session["Username"] as string);
            
            if(!currentUser.updatedName)
                return RedirectToAction("UpdateName", "Global", null);
           
            currentUser.generateGroupList();

            //Check to see if the user's photo is waiting approval
            ViewData["Image"] = currentUser.photo.Status == (int)PhotoStatus.CROPPED ? "profile-approval.jpg" : currentUser.photo.FileName;
            
            //Figure out current users notification preference
            if (currentUser.notificationPreference.email && currentUser.notificationPreference.txt)
            {
                ViewData["notificationPref"] = "Email & Text"; 
            }
            else if (currentUser.notificationPreference.email && !currentUser.notificationPreference.txt)
            {
                ViewData["notificationPref"] = "Email only";
            }
            else if (!currentUser.notificationPreference.email && currentUser.notificationPreference.txt)
            {
                ViewData["notificationPref"] = "Text only";
            }
            else if (!currentUser.notificationPreference.email && !currentUser.notificationPreference.txt)
            {
                ViewData["notificationPref"] = "None";
            }

            if (User.IsInRole("Bishopric") && bool.Parse(Session["IsBishopric"] as string))
                return RedirectToAction("Index", "Bishopric");

			/*
			 * Survey Navigation
			 */
            int profileComplete = 0;
            if (currentUser.memberSurvey == null)
                return RedirectToAction("IntroPage", "Home");
            else
            {
                profileComplete += currentUser.memberSurvey.status;
                ViewData["SurveyDone"] = currentUser.memberSurvey.status;
            }
			
			/*
			 * Checks if the user has a picture uploaded 
			 */
			if (currentUser.photo.NewPhotoFileName == null && currentUser.photo.Status < (int)PhotoStatus.CROPPED)
                ViewData["PictureDone"] = false;
            else
            {
                ViewData["PictureDone"] = true;
                profileComplete++;
            }

			/*
			 * Checks if the user has notifications set up
			 */
            if (currentUser.notificationPreference == null)
                ViewData["NotificationsDone"] = false;
            else
            {
                ViewData["NotificationsDone"] = true;
                profileComplete++;
            }

            profileComplete = (int)(((double)profileComplete / 5.0) * 100);
            ViewData["profileComplete"] = profileComplete;

            if (currentUser.CurrentWard != null)
            {
                ViewData["ward"] = currentUser.CurrentWard;
            }

			//Gets the information for teaching
			currentUser.generateTeachingInfo();

            return View(currentUser);
        }

        [Authorize]
        public ActionResult IntroPage()
        {
            if (Session["Username"] == null)
                _NewSession();
            ViewData["User"] = User.Identity.Name;

            return View();
        }

        #region WardList
        [Authorize]
        public ActionResult WardList()
        {
            if (Session["Username"] == null)
                _NewSession();
            if (User.IsInRole("Member?"))
                return RedirectToAction("NotApproved", "Home");

            double WardID = double.Parse(Session["WardStakeID"] as string);

            //Create Sort options
            SelectListItem[] printList = new SelectListItem[4];
            printList[0] = new SelectListItem { Text = "By Last Name", Value = "lastName" };
            printList[2] = new SelectListItem { Text = "By Apartment", Value = "apartment" };
            printList[1] = new SelectListItem { Text = "By First Name & M/F", Value = "name" };
            printList[3] = new SelectListItem { Text = "By Calling", Value = "calling" };
            ViewData["DropDown"] = printList;

			if (WardID == 0)
                return RedirectToAction("NotInWard", "Home");

            return View(new WardListModel(WardID));
        }

        [Authorize]
        [HttpPost]
        public ActionResult WardList(string SortSelect)
        {
            if (Session["Username"] == null)
                _NewSession();
            if (User.IsInRole("Member?"))
                return RedirectToAction("NotApproved", "Home");
            double WardStakeID = double.Parse(Session["WardStakeID"] as string);

			if (WardStakeID == 0)
                return RedirectToAction("NotInWard", "Home");

            //Create Sort options
            SelectListItem[] printList = new SelectListItem[4];
            printList[0] = new SelectListItem { Text = "By Last Name", Value = "lastName" };
            printList[2] = new SelectListItem { Text = "By Apartment", Value = "apartment" };
            printList[1] = new SelectListItem { Text = "By First Name & M/F", Value = "name" };
            printList[3] = new SelectListItem { Text = "By Calling", Value = "calling" };
            ViewData["DropDown"] = printList;		

			WardListModel ward = new WardListModel(WardStakeID);

            try
            {
                if (SortSelect.Equals(""))
                    ward.members = ward.members.OrderBy(x => x.memberSurvey.gender).ThenBy(x => x.memberSurvey.prefName).ThenBy(x => x.user.LastName).ToList();
                else if (SortSelect.Equals("lastName"))
                    ward.members = ward.members.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
                else if (SortSelect.Equals("apartment"))
                    ward.members = ward.members.OrderBy(x => x.memberSurvey.residence).ThenBy(x => x.memberSurvey.prefName).ThenBy(x => x.user.LastName).ToList();
                else if (SortSelect.Equals("name"))
                    ward.members = ward.members.OrderBy(x => x.memberSurvey.gender).ThenBy(x => x.memberSurvey.prefName).ThenBy(x => x.user.LastName).ToList();
                else if (SortSelect.Equals("calling"))
                    ward.members = ward.members.OrderBy(x => x.sortOrgID).ThenBy(x => x.sortCallingID).ThenBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
            }
            catch
            {
                //Do nothing. SortSelect was null
            }


            return View(ward);
        }

        [Authorize]
        public ActionResult GetMember(int memberID)
        {

            if (Session["Username"] == null)
                _NewSession();

			MemberModel member = MemberModel.get(memberID);
            if (member.user.WardStakeID == double.Parse(Session["WardStakeID"] as string))
            {
                 ViewData["Image"] = member.photo.FileName;
            }
            else
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            return View(member);
        }

        [Authorize]
        public ActionResult GetBishopric(int memberID)
        {

            if (Session["Username"] == null)
                _NewSession();

			BishopricModel user = BishopricModel.get(memberID);

            if (user.ward.WardStakeID == double.Parse(Session["WardStakeID"] as string))
            {
                if (User.IsInRole("Member?"))
                    ViewData["Image"] = "profile-1.jpg";
                else
					ViewData["Image"] = user.photo.FileName;
            }
            else
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            return View(user);
        }

        #endregion

        #region Select & Change Ward
        [Authorize]
        public ActionResult SelectWardStake()
        {
            if (Session["Username"] == null)
                _NewSession();

            ViewData["ChosenStake"] = new List<SelectListItem>();
            ViewData["ChosenWard"] = new List<SelectListItem>();

            return View();
        }

        [Authorize]
        public ActionResult ChangeWard()
        {
            if (Session["Username"] == null)
                _NewSession();
            if (bool.Parse(Session["IsBishopric"] as string))
                return RedirectToAction("SelectWard", "Bishopric");

            ViewData["ChosenStake"] = new List<SelectListItem>();
            ViewData["ChosenWard"] = new List<SelectListItem>();

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangeWard(string ChosenWard)
        {
            if (Session["Username"] == null)
                _NewSession();

            if (ChosenWard == "")
                return RedirectToAction("ChangeWard", "Home");

			MemberModel member = MemberModel.get(User.Identity.Name);

            if (bool.Parse(Session["IsBishopric"] as string))
                return RedirectToAction("SelectWard", "Bishopric");

            MSWtools._removeMemberFromWard(member, ChosenWard);

            Session["WardStakeID"] = Double.Parse(ChosenWard).ToString();

            if (member.memberSurvey == null)
                return RedirectToAction("TakeSurvey", "Home");

            return RedirectToAction("Profile", "Home");
        }

        #endregion

        #region Survey
        [Authorize]
        public ActionResult TakeSurvey()
        {
            if (Session["Username"] == null)
                _NewSession();

            DropDowns dropdowns = new DropDowns();
            dropdowns.generateApartmentList(Session["WardStakeID"] as string);
            ViewData["ApartmentList"] = dropdowns.getApartmentList();

            return View();
        }

        [Authorize]
        [HttpPost]
        public bool PersonalInformation()
        {
            MemberModel member = MemberModel.get(User.Identity.Name);

            try
            {                
                NameValueCollection form = HttpUtility.ParseQueryString(Request.Form["memberSurvey"]);                
                MemberSurvey.savePersonal(form, member.user.MemberID);
                _resetWardCache(member.ward.WardStakeID);
                _NewSession();
            }
            catch (Exception e)
            {
                MSWtools._sendException(e, member.user.UserName);
                //Throw to javascript AJAX error handler
                throw e;
            }
            
            return true;
        }

        [Authorize]
        [HttpPost]
        public bool ChurchInformation()
        {
            MemberModel member = MemberModel.get(User.Identity.Name);

            try
            {
                NameValueCollection form = HttpUtility.ParseQueryString(Request.Form["memberSurvey"]);
                MemberSurvey.saveChurch(form, member.user.MemberID);
                _resetWardCache(member.ward.WardStakeID);
            }
            catch (Exception e)
            {
                MSWtools._sendException(e, member.user.UserName);
                //Throw to javascript AJAX error handler
                throw e;
            }

            return true;
        }

        [Authorize]
        [HttpPost]
        public bool OtherInformation()
        {
            MemberModel member = MemberModel.get(User.Identity.Name);
         
            try
            {
                NameValueCollection form = HttpUtility.ParseQueryString(Request.Form["memberSurvey"]);
                MemberSurvey.saveOther(form, member.user.MemberID);
                _resetWardCache(member.ward.WardStakeID);
            }
            catch (Exception e)
            {
                MSWtools._sendException(e, member.user.UserName);
                //Throw to javascript AJAX error handler
                throw e;
            }

            return true;
        }

        [Authorize]
        [HttpPost]
        public ActionResult FinishSurvey()
        {
            MemberModel member = MemberModel.get(User.Identity.Name);

            try
            {
                NameValueCollection form = Request.Form;
                MemberSurvey.saveOther(form, member.user.MemberID);
                _resetWardCache(member.ward.WardStakeID);
            }
            catch (Exception e)
            {
                MSWtools._sendException(e, member.user.UserName);
            }

            return RedirectToAction("Profile", "Home", null);
        }

       /* [Authorize]
        [HttpPost]
        public ActionResult TakeSurvey(MemberSurvey memberSurvey)
        {
            if (_checkForm(memberSurvey))
            {
                try
                {
					MemberView member = MemberView.getMemberView(User.Identity.Name);

                    memberSurvey.MemberID = member.user.MemberID;
					MemberSurvey.saveMemberSurvey(memberSurvey);
                }
                catch (Exception e)
                {
                    DropDowns dropdowns = new DropDowns();
                    dropdowns.generateApartmentList(Session["WardStakeID"] as string);
                    MSWtools._sendException(e);
                    ViewData["ApartmentList"] = dropdowns.getApartmentList();
                    ViewData["Error"] = "*Problem adding to database, Email support@mysinglesward.com";
                    return View(memberSurvey);
                }

            }
            else
            {
                DropDowns dropdowns = new DropDowns();
                dropdowns.generateApartmentList(Session["WardStakeID"] as string);

                ViewData["ApartmentList"] = dropdowns.getApartmentList();
                ViewData["Error"] = "Please review survey. Some information was entered wrong.";
                return View(memberSurvey);
            }

            _resetWardCache();

            return RedirectToAction("UploadPicture", "Photo");
        }*/

        [Authorize]
        public ActionResult UpdateSurvey()
        {

            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                DropDowns dropdowns = new DropDowns();
                dropdowns.generateApartmentList(Session["WardStakeID"] as string);
                ViewData["ApartmentList"] = dropdowns.getApartmentList();

                MemberSurvey survey = MemberSurvey.getMemberSurvey(int.Parse(Session["MemberID"] as string));
                return View(survey);
            }
        }

        /*[Authorize]
        [HttpPost]
		public ActionResult UpdateSurvey(MemberSurvey memberSurvey, FormCollection collection)
        {
            if (_checkForm(memberSurvey))
            {
                try
                {
					MemberView member = MemberView.getMemberView(User.Identity.Name);
                    UpdateModel(memberSurvey, collection.ToValueProvider());

                    //Checks Values for updated model
					_checkForm(memberSurvey);
					MemberSurvey.saveMemberSurvey(memberSurvey);
                    

                }
                catch (Exception e)
                {
                    DropDowns dropdowns = new DropDowns();
                    dropdowns.generateApartmentList(Session["WardStakeID"] as string);
                    MSWtools._sendException(e);
                    ViewData["ApartmentList"] = dropdowns.getApartmentList();
                    ViewData["Error"] = "*Problem adding to database.";
                    return View(memberSurvey);
                }

            }
            else
            {
                DropDowns dropdowns = new DropDowns();
                dropdowns.generateApartmentList(Session["WardStakeID"] as string);
                ViewData["ApartmentList"] = dropdowns.getApartmentList();
                ViewData["Error"] = "Please review survey. Some information was entered wrong.";
                return View(memberSurvey);
            }

            MSWtools.NukeStewardshipReports(Session["WardStakeID"] as string);

            return RedirectToAction("Profile", "Home", "");
        }*/

        
        #endregion

        #region SelectWardJSON

        [Authorize]
        public JsonResult getMemberNames(string name_startsWith)
        {
            if (Session["Username"] == null)
                _NewSession();

			name_startsWith = name_startsWith.ToLower().Replace(" ", "_");

            List<ListItem> memberNames = Repository.getInstance().WardMemberNames(double.Parse(Session["WardStakeID"] as string));
            memberNames = memberNames.Where(x => x.Value.Contains(name_startsWith)).OrderBy(x => x.Text).ToList<ListItem>();

            return Json(memberNames, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult getStakes(string location)
        {
            return Json(Repository.getInstance().getSelectedStakeList(location), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult getWards(string location, string stake)
        {
            return Json(Repository.getInstance().getSelectedWardList(location, stake), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ErrorViews
		//USED IN DROPDOWN ONLY
        [Authorize]
        public ActionResult NoWard()
        {
			MemberModel user = MemberModel.get(User.Identity.Name);

			MSWtools._removeMemberFromWard(user);

            Session["WardStakeID"] = "0";

            if (bool.Parse(Session["IsBishopric"] as string))
                return RedirectToAction("Index", "Bishopric");
            if (user.memberSurvey == null)
                return RedirectToAction("TakeSurvey", "Home");

            return RedirectToAction("Profile", "Home");
        }

        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        [Authorize]
        public ActionResult NotInWard()
        {
            return View();
        }

        [Authorize]
        public ActionResult NotApproved()
        {
            return View();
        }
        #endregion        

        private void _resetWardCache(double WardStakeID)
        {
            //This allows a member to be part of a ward
            if (WardStakeID == 0)
                return;

            Ward ward = Ward.get(WardStakeID);
            Cache.Remove("Ward:" + ward.WardStakeID);
            if (ward.StakeID != null)
                Cache.Remove("Stake:" + ward.StakeID);

            MSWtools.NukeStewardshipReports(WardStakeID.ToString());
            
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

                //Get gender information for menu
                try
                {
                    Session["isPriesthood"] = MemberSurvey.getMemberSurvey(user.MemberID).gender.ToString();
                }
                catch
                {
                    //Member doesnt have survey information
                }
			}
			catch(Exception e)
			{
				//Sometimes this would trigger an exception if a stake member was signed on and came back later
				throw e;
			}			
        }
    }
}
