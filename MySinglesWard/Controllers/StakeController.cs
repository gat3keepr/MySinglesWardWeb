using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW.Models;
using MSW;
using System.Web.UI.WebControls;
using MSW.Model;
using System.Runtime.Caching;
using System.Data.SqlClient;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Controllers
{
    [HandleError]
    public class StakeController : Controller
    {
        //
        // GET: /Stake/

        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                _NewSession();

			var user = StakeUserModel.get(User.Identity.Name);

            if(user.data.StakeName == "")
                ViewData["InfoDone"] = "red-x-icon.png";
            else
                ViewData["InfoDone"] = "green_check.gif";

            if (!user.photo.Cropped)
                ViewData["PictureDone"] = "red-x-icon.png";
            else
                ViewData["PictureDone"] = "green_check.gif";

            if (Session["StakeID"].Equals(""))
                return RedirectToAction("SelectStake", "Stake");
            return View(new StakeModel(double.Parse(Session["StakeID"] as string)));
        }

        #region Select & Create Stake

        [Authorize(Roles = "StakePres, Stake")]
        public ActionResult SelectStake()
        {
            if (Session["Username"] == null)
                _NewSession();

            ViewData["ChosenStake"] = new List<SelectListItem>();

            return View();
        }

        [Authorize(Roles = "StakePres, Stake")]
        [HttpPost]
        public ActionResult SelectStake(string ChosenStake, string password)
        {
			StakeUser user = StakeUser.getStakeUser(User.Identity.Name);

			Repository r = Repository.getInstance();
			r.removeStakeUsers(user.StakeID);
			r.removeStakeUsers(double.Parse(ChosenStake));

			//Stake is not supported and user is being added to no stake
            try
            {
                if (double.Parse(ChosenStake) == 0)
                {
					
					user.StakeID = 0;
					StakeUser.saveUser(user);
                    Session["StakeID"] = "0";
                    return RedirectToAction("Index", "Stake");
                }
            }
            catch
            {
                user.StakeID = 0;
				StakeUser.saveUser(user);
                Session["StakeID"] = "0";
                return RedirectToAction("Index", "Stake");
            }

			double stake = 0;
            try
            {
				Stake newStake = Stake.get(double.Parse(ChosenStake));
                if (newStake.Password.Equals(password))
                {
                    user.StakeID = double.Parse(ChosenStake);
                    stake = double.Parse(ChosenStake);
                }
                else
                {
                    ViewData["Error"] = "Incorrect Stake Password";
                    ViewData["ChosenStake"] = new List<SelectListItem>();
                    return View();
                }
            }
            catch
            {
                user.StakeID = 0;
            }
            Session["StakeID"] = stake.ToString();
			StakeUser.saveUser(user);
			r.removeStakeUsers(double.Parse(ChosenStake));

            return RedirectToAction("Index", "Stake");
        }

        [Authorize(Roles = "StakePres")]
        [HttpPost]
        public ActionResult CreateNewStake(string Area, string Stake, string Password)
        {
			StakeUser user = StakeUser.getStakeUser(User.Identity.Name);
            tSupportedStake newStake = new tSupportedStake();

            if (Area.Equals("") || Stake.Equals("") || Password.Equals(""))
            {
                return RedirectToAction("SelectStake", "Stake");
            }

            newStake.Location = Area;
            newStake.Stake = Stake;
            newStake.Password = Password;
            newStake.StakeID = Area.ToLower().GetHashCode() + Stake.ToLower().GetHashCode();
            newStake.StakeID += 1000000;

            try
            {
				MSW.Models.dbo.Stake.create(newStake);
                user.StakeID = newStake.StakeID;
				StakeUser.saveUser(user);

                Session["StakeID"] = newStake.StakeID.ToString();
            }
            catch
            {
                return Redirect("StakeExists");
            }
            return RedirectToAction("Index", "Stake");
        }

        [Authorize(Roles = "StakePres")]
        [HttpPost]
        public ActionResult ChangeStakePassword(string currentPassword, string newPassword)
        {
            if (!_checkValidStake())
                return RedirectToAction("NotInStake", "Stake");
			Stake stake = Stake.get(double.Parse(Session["StakeID"] as string));

            if (stake.StakeID == 0)
                return RedirectToAction("NotInStake", "Stake");

            if (currentPassword.Equals("") || newPassword.Equals(""))
                return RedirectToAction("PasswordMatchError");
            else if (currentPassword.Equals(stake.Password))
            {
                stake.Password = newPassword;
				Stake.save(stake);
                return RedirectToAction("Index", "Stake");
            }
            else
                return RedirectToAction("PasswordMatchError");

        }

        [Authorize(Roles = "StakePres")]
        [HttpPost]
        public ActionResult RecoverStakePassword()
        {
            if (!_checkValidStake())
                return RedirectToAction("NotInStake", "Stake");
			StakeUser user = StakeUser.getStakeUser(Session["Username"] as string);
			Stake stake = Stake.get(user.StakeID);           

            MSWtools._StakePasswordRecover(stake.Password, user.Email);
            return RedirectToAction("SuccessRecover", "Stake");
        }
        
        #endregion

        #region Stake List
        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult StakeList()
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!_checkValidStake())
                return RedirectToAction("NotInStake");

            var user = StakeUserModel.get(User.Identity.Name);
            StakeListModel StakeList = new StakeListModel(user.user.StakeID);

            return View(StakeList);
        }

        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult GetMember(int memberID)
        {

            if (Session["Username"] == null)
                _NewSession();

			MemberModel user = MemberModel.get(memberID);
			List<double> wards = Repository.getInstance().getStakeWards(double.Parse(Session["StakeID"] as string));

            bool checkInStake = false;
            foreach (var ward in wards)
            {
                if (ward == user.user.WardStakeID)
                    checkInStake = true;
            }

            if (checkInStake)
            {
                ViewData["Image"] = user.photo.FileName;
            }
            else
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            return View(user);
        }

        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult GetBishopric(int memberID)
        {
            if (Session["Username"] == null)
                _NewSession();

			BishopricModel user = BishopricModel.get(memberID);
            List<double> wards = Repository.getInstance().getStakeWards(double.Parse(Session["StakeID"] as string));

            bool checkInStake = false;
            foreach (var ward in wards)
            {
                if (ward == user.user.WardStakeID)
                    checkInStake = true;
            }

            if (checkInStake)
            {
                ViewData["Image"] = user.photo.FileName;
            }
            else
            {
                return RedirectToAction("Unauthorized", "Home");
            }

            return View(user);
        }

        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult GetWard(double wardID)
        {
            WardList ward = new WardList(wardID);
            var user = StakeUserModel.get(User.Identity.Name);
            if (Repository.getInstance().getStakeWards(user.user.StakeID).Contains(wardID))
            {
                return View(ward);
            }

            throw new Exception();
        }
        #endregion

		[Authorize(Roles = "StakePres,Stake")]
		public ActionResult StakeUsers()
		{
			if (Session["Username"] == null)
				_NewSession();

            using (var db = new DBmsw())
            {
                double StakeID = double.Parse(Session["StakeID"] as string);

                if (StakeID == 0)
                    return RedirectToAction("NotInStake", "Stake");

                var users = (from user in db.tStakeUsers
                             where user.StakeID == StakeID
                             select user.MemberID);

                List<StakeUserModel> stakeUsers = new List<StakeUserModel>();

                foreach (var user in users)
                {
                    stakeUsers.Add(StakeUserModel.get(user));
                }

                stakeUsers = stakeUsers.OrderBy(x => x.data.StakeCalling).ThenBy(x => x.user.LastName).ThenBy(x => x.user.FirstName).ToList();

                //Stake Name
                Stake stake = Stake.get(StakeID);
                ViewData["StakeName"] = stake.Location + " " + stake.stake + " Stake";

                return View(stakeUsers);
            }
		}

		[Authorize(Roles = "StakePres")]
		public bool RemoveStakeUser(int id)
		{
			StakeUser user = StakeUser.getStakeUser(id);
			user.StakeID = 0;
			StakeUser.saveUser(user);

			return true;
		}

        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult StakeUserData()
        {
            if (Session["Username"] == null)
                _NewSession();

			StakeData user = StakeData.get(int.Parse(Session["MemberID"] as string));

            //Create Calling options
            List<SelectListItem> CallingList = new List<SelectListItem>();
            if (User.IsInRole("StakePres"))
            {
                CallingList.Add(new SelectListItem { Text = "Stake President", Value = "1" });
                CallingList.Add(new SelectListItem { Text = "First Counselor", Value = "2" });
                CallingList.Add(new SelectListItem { Text = "Second Counselor", Value = "3" });
                CallingList.Add(new SelectListItem { Text = "Executive Secretary", Value = "4" });
				CallingList.Add(new SelectListItem { Text = "High Counselman", Value = "5" });
            }
            else
            {
                CallingList.Add(new SelectListItem { Text = "Stake Relief Society", Value = "6" });
                CallingList.Add(new SelectListItem { Text = "Stake Clerk", Value = "7" });
                CallingList.Add(new SelectListItem { Text = "Stake Activities", Value = "8" });
                CallingList.Add(new SelectListItem { Text = "Other Stake Calling", Value = "9" });
            }
            ViewData["DropDown"] = CallingList;

            return View(user);
        }

        [Authorize(Roles = "StakePres,Stake")]
        [HttpPost]
        public ActionResult StakeUserData(StakeData stakeUser, FormCollection collection)
        {
            stakeUser.MemberID = int.Parse(Session["MemberID"] as string);

            if (ModelState.IsValid && stakeUser.StakeName != null && stakeUser.StakePhone != null
                 && stakeUser.StakeCalling != null)
            {
                if (MSWtools._isPhone(stakeUser.StakePhone))
                {
                    try
                    {
						StakeData.save(stakeUser);
                    }
                    catch(Exception e)
                    {
						MSWtools._sendException(e);
                    }
                    return RedirectToAction("Index", "Stake");
                }
                else
                {
                    //Create Calling options
					List<SelectListItem> CallingList = new List<SelectListItem>();
					if (User.IsInRole("StakePres"))
					{
						CallingList.Add(new SelectListItem { Text = "Stake President", Value = "1" });
						CallingList.Add(new SelectListItem { Text = "First Counselor", Value = "2" });
						CallingList.Add(new SelectListItem { Text = "Second Counselor", Value = "3" });
						CallingList.Add(new SelectListItem { Text = "Executive Secretary", Value = "4" });
						CallingList.Add(new SelectListItem { Text = "High Counselman", Value = "5" });
					}
					else
					{
						CallingList.Add(new SelectListItem { Text = "Stake Relief Society", Value = "6" });
						CallingList.Add(new SelectListItem { Text = "Stake Clerk", Value = "7" });
						CallingList.Add(new SelectListItem { Text = "Stake Activities", Value = "8" });
						CallingList.Add(new SelectListItem { Text = "Other Stake Calling", Value = "9" });
					}
                    ViewData["DropDown"] = CallingList;

                    ViewData["Error"] = "Phone Number entered incorrectly.";
                    return View(stakeUser);
                }
            }
            else
            {
                //Create Calling options
				List<SelectListItem> CallingList = new List<SelectListItem>();
				if (User.IsInRole("StakePres"))
				{
					CallingList.Add(new SelectListItem { Text = "Stake President", Value = "1" });
					CallingList.Add(new SelectListItem { Text = "First Counselor", Value = "2" });
					CallingList.Add(new SelectListItem { Text = "Second Counselor", Value = "3" });
					CallingList.Add(new SelectListItem { Text = "Executive Secretary", Value = "4" });
					CallingList.Add(new SelectListItem { Text = "High Counselman", Value = "5" });
				}
				else
				{
					CallingList.Add(new SelectListItem { Text = "Stake Relief Society", Value = "6" });
					CallingList.Add(new SelectListItem { Text = "Stake Clerk", Value = "7" });
					CallingList.Add(new SelectListItem { Text = "Stake Activities", Value = "8" });
					CallingList.Add(new SelectListItem { Text = "Other Stake Calling", Value = "9" });
				}
                ViewData["DropDown"] = CallingList;

                ViewData["Error"] = "Please make sure all fields are filled out.";
                return View(stakeUser);
            }

        }

        [Authorize(Roles = "StakePres,Stake")]
        public ActionResult NoStake()
        {
			StakeUser user = StakeUser.getStakeUser(User.Identity.Name);
            user.StakeID = 0;
			StakeUser.saveUser(user);
            Session["StakeID"] = "0";

            return RedirectToAction("Index", "Stake");
        }

        [Authorize(Roles = "StakePres,Stake,Bishopric")]
        [Authorize]
        public JsonResult getStakes(string location)
        {
            return Json(Repository.getInstance().getStakesList(location), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "StakePres")]
        public bool ApproveNewWard(string button, int WardStakeID)
        {
			WardStake ward = WardStake.get(WardStakeID);
            try
            {
                if (button.Equals("approve"))
                {
                    ward.Approved = true;
					WardStake.save(ward);
                }
                else
                {
					WardStake.remove(ward);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        [Authorize(Roles = "StakePres,Stake")]
        public JsonResult searchStakeList(string name_startsWith)
        {
            if (Session["Username"] == null)
                _NewSession();            

            name_startsWith = name_startsWith.ToLower();

            List<SelectListItem> memberNames = Repository.getInstance().SelectListMembers(double.Parse(Session["StakeID"] as string), true);
            memberNames = memberNames.Where(x => x.Value.Contains(name_startsWith)).OrderBy(x => x.Text).ToList<SelectListItem>();

            return Json(memberNames, JsonRequestBehavior.AllowGet);
        }

        /*[Authorize(Roles = "Stake,Stake?")]
        public JsonResult searchWardList(string name_startsWith, string WardID)
        {
            name_startsWith = name_startsWith.ToLower();
            ObjectCache cache = MemoryCache.Default;

            List<ListItem> memberNames = (List<ListItem>)cache.Get(WardID);
            memberNames = memberNames.Where(x => x.Value.Contains(name_startsWith)).OrderBy(x => x.Text).ToList<ListItem>();

            return Json(memberNames, JsonRequestBehavior.AllowGet);
        }*/
        [Authorize]
        public ActionResult PasswordMatchError()
        {
            return View();
        }

        [Authorize]
        public ActionResult StakeExists()
        {
            return View();
        }

        [Authorize]
        public ActionResult NotInStake()
        {
            return View();
        }

        [Authorize]
        public ActionResult SuccessRecover()
        {
            return View();
        }

        private void _NewSession()
        {
            StakeUser user = StakeUser.getStakeUser(User.Identity.Name);

            user = MSWtools._checkStakeID(user);

            Session["Username"] = user.UserName.ToString();
            Session["StakeID"] = user.StakeID.ToString();
            Session["MemberID"] = user.MemberID.ToString();
            Session["IsPresidency"] = user.IsPresidency.ToString();
            Session["HasPic"] = user.HasPic.ToString();
        }

        private bool _checkValidStake()
        {
            if (int.Parse(Session["StakeID"] as string) == 0)
                return false;
            return true;
        }

    }
}
