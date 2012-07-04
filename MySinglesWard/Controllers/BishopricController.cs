using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW;
using MSW.Model;
using MSW.Models;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Controllers
{
    [HandleError]
    public class BishopricController : Controller
    {
        //
        // GET: /Bishopric/
        [Authorize(Roles = "Bishopric")]
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!bool.Parse(Session["IsBishopric"] as string))
            {
                ViewData["InfoDone"] = "yellow-circle-icon.png";
            }
            else
            {
				BishopricModel bishopricData = BishopricModel.get(int.Parse(Session["MemberID"] as string));

                if (bishopricData.data.BishopricName == "")
                    ViewData["InfoDone"] = "red-x-icon.png";
                else
                    ViewData["InfoDone"] = "green_check.gif";

				if (bishopricData.photo == null)
					ViewData["PictureDone"] = "red-x-icon.png";
				else if (bishopricData.photo.NewPhotoFileName == null && bishopricData.photo.Status < (int)PhotoStatus.CROPPED)
					ViewData["PictureDone"] = "red-x-icon.png";
				else
					ViewData["PictureDone"] = "green_check.gif";
            }            

            ViewData["Image"] = -1;

            ViewData["User"] = User.Identity.Name;

            if (Session["WardStakeID"].Equals(""))
                return RedirectToAction("SelectWard", "Bishopric");
            return View(new WardModel(Session["WardStakeID"] as string));
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult SelectWard()
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!bool.Parse(Session["IsBishopric"] as string))
                return RedirectToAction("Unauthorized", "Home");

            ViewData["ChosenStake"] = new List<SelectListItem>();
            ViewData["ChosenWard"] = new List<SelectListItem>();

            return View();
        }

        [Authorize(Roles = "Bishopric")]
        [HttpPost]
        public ActionResult SelectWard(string ChosenWard, string password)
        {
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			if (!user.IsBishopric)
				return RedirectToAction("Unauthorized", "Home");

            try
            {
                Session["WardStakeID"] = MSWtools.removeBishopricMember(user, ChosenWard, password);
            }
            catch
            {
                ViewData["Error"] = "Incorrect Ward Password";
                ViewData["ChosenStake"] = new List<SelectListItem>();
                ViewData["ChosenWard"] = new List<SelectListItem>();
                return View();
            }
            
            return RedirectToAction("Index", "Bishopric");
        }

        [Authorize(Roles = "Bishopric")]
        [HttpPost]
        public ActionResult CreateNewWard(string Area, string Stake, string Ward, string Password)
        {
            if (Area.Equals("") || Stake.Equals("") || Ward.Equals("") || Password.Equals(""))
            {
                return RedirectToAction("SelectWard", "Bishopric");
            }
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			tSupportedWard newWard = new tSupportedWard();

            newWard.Location = Area;
            newWard.Stake = Stake;
            newWard.Ward = Ward;
            newWard.Password = Password;
            newWard.WardStakeID = Area.ToLower().GetHashCode() +
                Stake.ToLower().GetHashCode() + Ward.ToLower().GetHashCode();
            try
            {
				Ward ward = MSW.Models.dbo.Ward.create(newWard);
                user.WardStakeID = newWard.WardStakeID;
				MSWUser.saveUser(user);

                //Create Callings for new ward
                CallingInitializer ci = new CallingInitializer();
				ci.InitializeWard(ward);

                Session["WardStakeID"] = newWard.WardStakeID.ToString();
            }
            catch
            {
                return Redirect("WardExists");
            }
            return RedirectToAction("Index", "Bishopric");
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public ActionResult UpdateResidence(int? id)
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!_checkValidWard())
                return RedirectToAction("NotInWard", "Home");

            if (id == 1)
                ViewData["Error"] = "That Residence was not in your ward.";
            using (var db = new DBmsw())
            {
				Repository r = Repository.getInstance();
                List<Residence> residences = Cache.GetList(r.ResidenceIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Residence>(x),
                                                                        y => Residence.get(y));
                ViewData["ResidenceList"] = residences.OrderBy(x => x.SortID).ToList();
            }
            return View();
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public bool RemoveResidence(int id)
        {
            try
            {
				Residence.remove(Residence.get(id));
            }
            catch
            {
                return false;
            }
            return true;
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public int AddResidence(string residence)
        {
            tResidence newResidence = new tResidence();
            newResidence.WardStakeID = double.Parse(Session["WardStakeID"] as string);
            newResidence.Residence = residence;
			
            try
            {
				int id = Residence.create(newResidence);
                return id;
            }
            catch
            {
				throw new Exception();
            }     
        }

        [Authorize(Roles = "Bishopric")]
        public bool ApproveNewMember(string button, int MemberID)
        {
			MSWUser user = MSWUser.getUser(MemberID);
            try
            {
                if (button.Equals("approve"))
                {
                    System.Web.Security.Roles.AddUserToRole(user.UserName, "Member");
                    System.Web.Security.Roles.RemoveUserFromRole(user.UserName, "Member?");
                }
                else
                {
                    MSWtools._removeMemberFromWard(MemberModel.get(user.MemberID));
                }
            }
            catch
            {
				throw new Exception();
            }
			Cache.Remove("Ward:" + Session["WardStakeID"] as string);
            return true;
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public bool RemoveMember(int id)
        {
			MSWUser user = MSWUser.getUser(id);

            if (user.IsBishopric)
            {
                user.WardStakeID = 0;

				MSWUser.saveUser(user);
                Cache.Remove("Bishopric:" + Session["WardStakeID"] as string);
                MSWtools.NukeStewardshipReports(Session["WardStakeID"] as string);
                if (User.Identity.Name.Equals(user.UserName))
                {
                    _NewSession();
                }

            }
            else
            {
                MSWtools._removeMemberFromWard(MemberModel.get(id));
            }

            return true;
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public bool UpdateMember(int id, string prefName, string lastName, string apartment)
        {
			MemberSurvey survey = MemberSurvey.getMemberSurvey(id);
			MSWUser user = MSWUser.getUser(id);
            try
            {
                survey.prefName = prefName;
                user.LastName = lastName;
                survey.residence = apartment;
				MemberSurvey.saveMemberSurvey(survey);
				MSWUser.saveUser(user);
            }
            catch
            {
                throw new Exception();
            }

			MSWtools.NukeStewardshipReports(Session["WardStakeID"] as string);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        [HttpPost]
        public ActionResult ChangeWardPassword(string currentPassword, string newPassword)
        {
            if (!_checkValidWard())
                return RedirectToAction("NotInWard", "Home");

            Ward ward = Ward.get(double.Parse(Session["WardStakeID"] as string));

            if (ward.WardStakeID == 0)
                return RedirectToAction("NotInWard", "Home");

            if (currentPassword.Equals("") || newPassword.Equals(""))
                return RedirectToAction("PasswordMatchError");
            else if (currentPassword.Equals(ward.Password))
            {
                ward.Password = newPassword;
				Ward.save(ward);
                return RedirectToAction("Index", "Bishopric");
            }
            else
                return RedirectToAction("PasswordMatchError", "Bishopric");

        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult BishopricData()
        {
            if (Session["Username"] == null)
                _NewSession();

            BishopricData user = MSW.Models.dbo.BishopricData.get(int.Parse(Session["MemberID"] as string));

            //Create Calling options
            SelectListItem[] CallingList = new SelectListItem[5];
            CallingList[0] = new SelectListItem { Text = "Bishop", Value = "1" };
            CallingList[1] = new SelectListItem { Text = "First Counselor", Value = "2" };
            CallingList[2] = new SelectListItem { Text = "Second Counselor", Value = "3" };
            CallingList[3] = new SelectListItem { Text = "Ward Clerk", Value = "4" };
            CallingList[4] = new SelectListItem { Text = "High Councilman", Value = "5" };
            ViewData["DropDown"] = CallingList;

            if (user != null)
            {                
                return View(user);
            }
            return View();
        }

        [Authorize(Roles = "Bishopric")]
        [HttpPost]
        public ActionResult BishopricData(BishopricData bishopric, FormCollection collection)
        {
            bishopric.MemberID = int.Parse(Session["MemberID"] as string);

            if (ModelState.IsValid && bishopric.BishopricName != null && bishopric.BishopricPhone != null
                 && bishopric.BishopricCalling != null)
            {
                if (bishopric.BishopricAddress == null)
                    bishopric.BishopricAddress = " ";
                if (bishopric.WifeName == null)
                    bishopric.WifeName = " ";
                if (bishopric.WifePhone == null)
                    bishopric.WifePhone = " ";
                
                if (MSWtools._isPhone(bishopric.BishopricPhone))
                {
                    try
                    {
						MSW.Models.dbo.BishopricData.save(bishopric);
                    }
                    catch(Exception e)
                    {
						MSWtools._sendException(e);
                    }
                    return RedirectToAction("Index", "Bishopric");
                }
                else
                {
                    //Create Calling options
                    SelectListItem[] CallingList = new SelectListItem[5];
                    CallingList[0] = new SelectListItem { Text = "Bishop", Value = "1" };
                    CallingList[1] = new SelectListItem { Text = "First Counselor", Value = "2" };
                    CallingList[2] = new SelectListItem { Text = "Second Counselor", Value = "3" };
                    CallingList[3] = new SelectListItem { Text = "Ward Clerk", Value = "4" };
                    CallingList[4] = new SelectListItem { Text = "High Councilman", Value = "5" };
                    ViewData["DropDown"] = CallingList;

                    ViewData["Error"] = "Phone Number entered incorrectly.";
                    return View(bishopric);
                }

            }
            else
            {
                //Create Calling options
                SelectListItem[] CallingList = new SelectListItem[5];
                CallingList[0] = new SelectListItem { Text = "Bishop", Value = "1" };
                CallingList[1] = new SelectListItem { Text = "First Counselor", Value = "2" };
                CallingList[2] = new SelectListItem { Text = "Second Counselor", Value = "3" };
                CallingList[3] = new SelectListItem { Text = "Ward Clerk", Value = "4" };
                CallingList[4] = new SelectListItem { Text = "High Councilman", Value = "5" };
                ViewData["DropDown"] = CallingList;

                ViewData["Error"] = "Please make sure all fields are filled out.";
                return View(bishopric);
            }

        }

        [Authorize(Roles = "Bishopric")]
        [HttpPost]
        public ActionResult RecoverWardPassword()
        {
            if (!_checkValidWard())
                return RedirectToAction("NotInWard", "Home");

            //if (!bool.Parse(Session["IsBishopric"] as string))
            //    return RedirectToAction("Index", "Bishopric");

			MSWUser bishopricMember = MSWUser.getUser(Session["Username"] as string);
			Ward ward = Ward.get(bishopricMember.WardStakeID);
    
            MSWtools._WardPasswordRecover(ward.Password, bishopricMember.Email);

            return RedirectToAction("SuccessRecover", "Bishopric");
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult JoinStake()
        {
            if (Session["Username"] == null)
                _NewSession();

            if (!_checkValidWard())
                return RedirectToAction("NotInWard", "Home");

            ViewData["ChosenStake"] = new List<SelectListItem>();
            return View();
        }

        [Authorize(Roles = "Bishopric")]
        [HttpPost]
        public ActionResult JoinStake(string ChosenStake)
        {
            try
            {
                if (double.Parse(ChosenStake) == 0)
                {
                    return RedirectToAction("Index", "Bishopric");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Bishopric");
            }

            try
            {
				WardStake wardStake = WardStake.get(double.Parse(Session["WardStakeID"] as string));
                wardStake.StakeID = double.Parse(ChosenStake);
                wardStake.Approved = false;
				WardStake.save(wardStake);
            }
            catch
            {
                var newStake = new tWardStake();
				newStake.StakeID = double.Parse(ChosenStake);
				newStake.WardStakeID = double.Parse(Session["WardStakeID"] as string);
				newStake.Approved = false;
				WardStake.create(newStake);
            }
			Cache.Remove("StakeWards:" + double.Parse(ChosenStake));

            return RedirectToAction("Index", "Bishopric");
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public ActionResult ManageWardList()
        {
            if (Session["Username"] == null)
                _NewSession();

            double WardStakeID = double.Parse(Session["WardStakeID"] as string);

            if (WardStakeID == 0)
                return RedirectToAction("NotInWard", "Home");

			//Members
            List<MemberModel> membersData = Cache.GetList(Repository.getInstance().WardMembersID(WardStakeID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
			membersData = membersData.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

            //Apartment Dropdown
            DropDowns dropdowns = new DropDowns();
            dropdowns.generateApartmentList(Session["WardStakeID"] as string);
            ViewData["ApartmentList"] = dropdowns.getApartmentList();
            
            return View(membersData);
        }

        [Authorize(Roles = "Bishopric, Clerk")]
        public bool RequestRecord(int id, bool records)
        {
            try
            {
				MSWUser user = MSWUser.getUser(id);
                user.RecordsRequested = records;
				MSWUser.saveUser(user);

                return true;
            }
            catch
            {
                throw new Exception();
            }
        }

        [Authorize(Roles = "Bishopric,Clerk")]
        public bool SortResidences(List<int> result)
        {
            if (Session["Username"] == null)
                _NewSession();

            for (int i = 0; i < result.Count; i++)
            {
                try
                {
                    var residence = Residence.get(result[i]);
                    residence.SortID = i;
                    Residence.save(residence);
                }
                catch
                {
                    //Will catch if residence has been deleted, Application just needs to continue
                }
            }

            return true;
        }

        [Authorize(Roles = "Bishopric,Clerk")]
        public bool StreetAddress(int residenceID, string address)
        {
            if (Session["Username"] == null)
                _NewSession();

            Residence residence = Residence.get(residenceID);
            residence.streetAddress = address;
            Residence.save(residence);            

            return true;
        }


        [Authorize]
        public ActionResult PasswordMatchError()
        {
            return View();
        }

        [Authorize]
        public ActionResult WardExists()
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
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            //Checks the WardID and changes the ID to zero if the stake is missing
            //user = MSWtools._checkWardID(user);

            Session["Username"] = user.UserName.ToString();
            Session["WardStakeID"] = user.WardStakeID.ToString();
            Session["MemberID"] = user.MemberID.ToString();
            Session["IsBishopric"] = user.IsBishopric.ToString();
        }

        private bool _checkValidWard()
        {
            if (int.Parse(Session["WardStakeID"] as string) == 0)
                return false;
            return true;
        }

    }
}
