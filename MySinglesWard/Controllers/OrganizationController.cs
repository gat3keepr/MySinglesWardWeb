using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW.Models;
using MSW.Utilities;
using MSW.Model;
using MSW.Models.dbo;
using MSW.Exceptions;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;
using MSW.Models.Reports;
using MSW.Models.dao;

namespace MSW.Controllers
{
	[HandleError]
	public class OrganizationController : Controller
	{
		private const int NOTINORG = 0;
		//
		// GET: /Organization/
		[Authorize]
		public ActionResult Index()
		{
			try
			{
				if (Session["Username"] == null)
					_NewSession();

				int orgID = _getUserOrganizationID();
				MSWOrganizationModel model = MSWOrganizationModel.get(orgID);

				return View(model);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		#region Membership

		//
		// GET: /Organization/Membership
		[Authorize]
		public ActionResult Membership()
		{
			try
			{
				int orgID = _getUserOrganizationID();

				if (Session["Username"] == null)
					_NewSession();

				MSWOrganizationModel model = MSWOrganizationModel.get(orgID);

				//Get list of members
				List<MemberModel> members = Cache.GetList(model.orgMembers.Select(x => x.MemberID).ToList(),
					x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));

				members = members.OrderBy(x => x.memberSurvey.prefName).ThenBy(x => x.user.LastName).ToList();

				//Set header variable
				ViewBag.Header = model.org.Title;
				ViewBag.orgID = model.org.OrgID;
				ViewBag.ReportID = model.org.ReportID;

				//Create Sort options
				SelectListItem[] printList = new SelectListItem[3];
				printList[0] = new SelectListItem { Text = "By Last Name", Value = "lastName" };
				printList[2] = new SelectListItem { Text = "By Apartment", Value = "apartment" };
				printList[1] = new SelectListItem { Text = "By First Name & M/F", Value = "name" };
				ViewData["DropDown"] = printList;

				return View(members);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		//
		// GET: /Organization/ManageMembership
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult ManageMembership()
		{
			try
			{
				if (Session["Username"] == null)
					_NewSession();

				int orgID = _getUserOrganizationID();
				MSWOrganizationModel model = MSWOrganizationModel.get(orgID);
				model.generateMembershipLists();

				return View(model);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		//
		// GET: /Organization/ManageMembership
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		[HttpPost]
		public bool ManageMembership(int? id)
		{
			try
			{
				int orgID = _getUserOrganizationID();
				_checkUserWard(orgID);

				NameValueCollection form = HttpUtility.ParseQueryString(Request.Form["MemberIDList"]);

				foreach (string MemberID in form.AllKeys)
				{
					string addToOrg = form.Get(MemberID);
					if (addToOrg != "false")
						_AddMemberToOrganization(MemberID, orgID);
				}
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				throw e;
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				throw e;
			}
			return true;
		}

		//Adds a member to an organization
		private void _AddMemberToOrganization(string id, int orgID)
		{
			int MemberID = int.Parse(id);

			MSWUser member = MSWUser.getUser(MemberID);
			Organization org = Organization.get(orgID);

			if (member.WardStakeID != org.WardID)
				throw new UserNotInOrganizationException();

			OrganizationMember orgMember = OrganizationMember.get(MemberID);

			if (orgMember == null)
			{
				tOrganizationMember newMember = new tOrganizationMember();
				newMember.MemberID = MemberID;
				newMember.OrgID = orgID;
				OrganizationMember.create(newMember);
			}
			else
			{
				orgMember.OrgID = orgID;
				orgMember.PendingOrgID = null;
				orgMember.status = (int)OrganizationMember.Status.APPROVED;
				OrganizationMember.save(orgMember);
			}
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool ApproveNewMembership(string button, int MemberID)
		{
			OrganizationMember orgMember = OrganizationMember.get(MemberID);

			try
			{
				_checkUserWard((int)orgMember.PendingOrgID);

				// Member Approved
				if (button.Equals("approve"))
				{
					orgMember.status = (int)OrganizationMember.Status.APPROVED;
					orgMember.OrgID = (int)orgMember.PendingOrgID;
				}
				else // Not Approved
				{
					orgMember.status = orgMember.OrgID != NOTINORG ?
											(int)OrganizationMember.Status.APPROVED : (int)OrganizationMember.Status.NONE;
					orgMember.PendingOrgID = null;
				}

				OrganizationMember.save(orgMember);
			}
			catch
			{
				throw new Exception();
			}

			//Remove the organization membership from cache
			Repository r = Repository.getInstance();
			r.removeOrganizationMembership(orgMember.OrgID);

			return true;
		}

		//Removes a member from an organization
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool RemoveMember(int id)
		{
			OrganizationMember orgMember = OrganizationMember.get(id);
			_checkUserWard(orgMember.OrgID);

			//Remove member from current teaching assignment in the organization
			MSWtools.removeMemberFromTeachingAssignment(orgMember.MemberID);

			//Remove the Member from the organization
			OrganizationMember.remove(orgMember);

			return true;
		}

		//Gets the organization member names for the member search feature on the Organization List
		[Authorize]
		public bool getMemberNames(string name_startsWith, int orgID)
		{
			//OrganizationMember orgMember = OrganizationMember.getByMemberID();


			return true;
		}

		//
		// GET: /Organization/SelectOrganization
		[Authorize(Roles = "Bishopric")]
		public ActionResult SelectOrganization()
		{
			if (Session["Username"] == null)
				_NewSession();

			//Get member information
			MemberModel member = MemberModel.get(User.Identity.Name);

			//Get all the elders quorums and relief societies in the ward to select which org they want to manage
			Repository r = Repository.getInstance();
			List<Organization> orgs = Cache.GetList(r.OrganizationIDs(member.user.WardStakeID), x => Cache.getCacheKey<Organization>(x), y => Organization.get(y));

			//Filter out orgs based on authentication type
			if (User.IsInRole("Bishopric"))
				orgs = orgs.Where(x => x.ReportID == "Relief Society" || x.ReportID == "Elders Quorum").ToList();
			else if (User.IsInRole("EldersQuorum"))
				orgs = orgs.Where(x => x.ReportID == "Elders Quorum").ToList();
			else if (User.IsInRole("ReliefSociety"))
				orgs = orgs.Where(x => x.ReportID == "Relief Society").ToList();

			List<MSWOrganizationModel> orgModels = new List<MSWOrganizationModel>();

			foreach (Organization org in orgs)
				orgModels.Add(MSWOrganizationModel.get(org.OrgID));

			orgModels = orgModels.OrderBy(x => x.org.ReportID).ThenBy(x => x.org.Title).ToList();
			return View(orgModels);
		}

		//Used to select which organization will be managed. This is set in a session variable
		[Authorize(Roles = "Bishopric")]
		[HttpPost]
		public ActionResult SelectOrganization(int orgID)
		{
			//Get user and make sure the organization is in the ward of the user
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			Organization org = Organization.get(orgID);

			try
			{

				if (user.WardStakeID != org.WardID)
					return RedirectToAction("Error");

				//Assign the selected org to the user
				Session["orgID"] = orgID.ToString();
				return RedirectToAction("Index");
			}
			catch (NullReferenceException e)
			{
				return RedirectToAction("Error");
			}
		}

		//
		// GET: /Organization/RequestOrganization
		[Authorize]
		public ActionResult RequestOrganization()
		{
			if (Session["Username"] == null)
				_NewSession();

			//Get member information
			MemberModel member = MemberModel.get(User.Identity.Name);

			//Check if member has made a request
			OrganizationMember orgMember = OrganizationMember.get(member.user.MemberID);
			if (orgMember != null)
				if (orgMember.status == (int)OrganizationMember.Status.PENDING)
					return RedirectToAction("OrganizationPending");

			//Get all the elders quorums and relief societies in the ward to select which org they want to manage
			Repository r = Repository.getInstance();
			List<Organization> orgs = Cache.GetList(r.OrganizationIDs(member.user.WardStakeID), x => Cache.getCacheKey<Organization>(x), y => Organization.get(y));

			//Filter out orgs based on authentication type
			if (member.memberSurvey.gender)
				orgs = orgs.Where(x => x.ReportID == "Elders Quorum").ToList();
			else
				orgs = orgs.Where(x => x.ReportID == "Relief Society").ToList();

			List<MSWOrganizationModel> orgModels = new List<MSWOrganizationModel>();

			foreach (Organization org in orgs)
				orgModels.Add(MSWOrganizationModel.get(org.OrgID));

			orgModels = orgModels.OrderBy(x => x.org.Title).ThenBy(x => x.org.Title).ToList();
			return View(orgModels);
		}

		//Used to send a request for membership to a certain organization by a new member in the ward
		[Authorize]
		[HttpPost]
		public ActionResult RequestOrganization(int orgID)
		{
			//Get user and make sure the organization is in the ward of the user
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			Organization org = Organization.get(orgID);

			try
			{
				if (user.WardStakeID != org.WardID)
					return RedirectToAction("Error");

				//Assign the selected org to the user - pending
				OrganizationMember orgMember = OrganizationMember.get(user.MemberID);

				if (orgMember == null)
				{
					tOrganizationMember newMember = new tOrganizationMember();
					newMember.MemberID = user.MemberID;
					newMember.OrgID = NOTINORG;
					newMember.PendingOrgID = org.OrgID;
					newMember.Status = (int)OrganizationMember.Status.PENDING;
					OrganizationMember.create(newMember);
				}
				else
				{
					orgMember.PendingOrgID = org.OrgID;
					orgMember.status = (int)OrganizationMember.Status.PENDING;
					OrganizationMember.save(orgMember);
				}

				return RedirectToAction("Profile", "Home", null);
			}
			catch (NullReferenceException e)
			{
				return RedirectToAction("Error");
			}
		}

		#endregion

		#region Home/Visiting Teaching Management

		//
		// GET: /Organization/ManageTeaching
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult ManageTeaching()
		{
			try
			{
				if (Session["Username"] == null)
					_NewSession();

				int orgID = _getUserOrganizationID();
				ManageTeachingModel model = new ManageTeachingModel(orgID);
				ViewData["orgCallings"] = model.orgCallings;
				return View(model);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		//
		// GET: /Organization/AddTeacher
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult AddTeacherList(int CompanionshipID)
		{
			try
			{
				if (Session["Username"] == null)
					_NewSession();

				int orgID = _getUserOrganizationID();
				Companionship comp = Companionship.get(CompanionshipID);
				District d = District.get(comp.DistrictID);

				//Check that the companionship is in the members organization
				if (orgID != d.OrgID)
					throw new AuthorizationException();

				//Get a list of all the members that still need a teaching assignment
				Repository r = Repository.getInstance();
				List<int> orgMembership = r.OrganizationMembership(orgID);
				List<TeachingAssignment> assignments = Cache.GetList(orgMembership, x => Cache.getCacheKey<TeachingAssignment>(x),
											y => TeachingAssignment.get(y));

				List<int> membersWithoutAssignment = new List<int>();
				int i = 0;
				foreach (TeachingAssignment ta in assignments)
				{
					if (ta == null)
						membersWithoutAssignment.Add(orgMembership[i]);
					else if (ta.CompanionshipID == null)
						membersWithoutAssignment.Add(orgMembership[i]);

					i++;
				}

				List<MemberModel> members = Cache.GetList(membersWithoutAssignment, x => Cache.getCacheKey<MemberModel>(x),
											y => MemberModel.get(y)).OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

				ViewBag.CompanionshipID = CompanionshipID;
				return View(members);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		//
		// GET: /Organization/AddTeacher
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public JsonResult AddTeacher(int memberID, int CompanionshipID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			Companionship comp = Companionship.get(CompanionshipID);
			District d = District.get(comp.DistrictID);
			Organization org = Organization.get(orgID);
			MSWUser user = MSWUser.getUser(memberID);

			//Check that the companionship is in the members organization
			if (orgID != d.OrgID)
				throw new AuthorizationException();

			//Check that member is in the ward
			if (org.WardID != user.WardStakeID)
				throw new AuthorizationException();

			//Add member to companionship
			TeachingAssignment assignment = TeachingAssignment.get(memberID);

			if (assignment == null)
			{
				tTeachingAssignment newAssignment = new tTeachingAssignment();
				newAssignment.MemberID = memberID;
				newAssignment.CompanionshipID = CompanionshipID;
				assignment = TeachingAssignment.create(newAssignment);
			}
			else
			{
				//Check to make sure member isn't already in the companionship
				if (assignment.CompanionshipID == CompanionshipID)
					throw new Exception();

				assignment.CompanionshipID = CompanionshipID;
				TeachingAssignment.save(assignment);
			}

			MemberModel member = MemberModel.get(memberID);

			return Json(new
			{
				@name = member.user.LastName + ", " + member.memberSurvey.prefName
			}, JsonRequestBehavior.AllowGet);

		}

		//
		// GET: /Organization/RemoveTeacher
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool RemoveTeacher(int memberID, int CompanionshipID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			Companionship comp = Companionship.get(CompanionshipID);
			District d = District.get(comp.DistrictID);
			Organization org = Organization.get(orgID);
			MSWUser user = MSWUser.getUser(memberID);

			//Check that the companionship is in the members organization
			if (orgID != d.OrgID)
				throw new AuthorizationException();

			//Check that member is in the ward
			if (org.WardID != user.WardStakeID)
				throw new AuthorizationException();

			//Remove member from companionship
			TeachingAssignment assignment = TeachingAssignment.get(memberID);

			assignment.CompanionshipID = null;
			TeachingAssignment.save(assignment);

			return true;

		}

		//
		// GET: /Organization/AddTeacheeList
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult AddTeacheeList(int CompanionshipID)
		{
			try
			{
				if (Session["Username"] == null)
					_NewSession();

				int orgID = _getUserOrganizationID();
				Companionship comp = Companionship.get(CompanionshipID);
				District d = District.get(comp.DistrictID);
				Organization org = Organization.get(orgID);

				//Check that the companionship is in the members organization
				if (orgID != d.OrgID)
					throw new AuthorizationException();
				Repository r = Repository.getInstance();

				//Get all the people who have not be assigned a teacher to this type of organization
				List<int> orgsToTeach = new List<int>();
				orgsToTeach.Add(org.OrgID);
				orgsToTeach.AddRange(r.getOrganizationsToTeach(orgID));

				List<TeachingAssignment> assignments = new List<TeachingAssignment>();
				List<int> orgMembership = new List<int>(); ;
				foreach (int TeachingOrgID in orgsToTeach)
				{
					List<int> membership = r.OrganizationMembership(TeachingOrgID);
					orgMembership.AddRange(membership);
					assignments.AddRange(Cache.GetList(membership, x => Cache.getCacheKey<TeachingAssignment>(x),
											y => TeachingAssignment.get(y)));
				}

				List<int> membersWithoutTeacher = new List<int>();
				int i = 0;
				foreach (TeachingAssignment ta in assignments)
				{
					if (ta == null)
						membersWithoutTeacher.Add(orgMembership[i]);
					else if (ta.HTID == null && org.ReportID == "Elders Quorum")
						membersWithoutTeacher.Add(orgMembership[i]);
					else if (ta.VTID == null && org.ReportID == "Relief Society")
						membersWithoutTeacher.Add(orgMembership[i]);

					i++;
				}

				List<MemberModel> members = Cache.GetList(membersWithoutTeacher, x => Cache.getCacheKey<MemberModel>(x),
											y => MemberModel.get(y)).OrderBy(x => x.memberSurvey.gender)
												.ThenBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

				ViewBag.CompanionshipID = CompanionshipID;
				return View(members);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		//
		// GET: /Organization/AddTeachee
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public JsonResult AddTeachee(int memberID, int CompanionshipID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			Companionship comp = Companionship.get(CompanionshipID);
			District d = District.get(comp.DistrictID);
			Organization org = Organization.get(orgID);
			MSWUser user = MSWUser.getUser(memberID);

			//Check that the companionship is in the members organization
			if (orgID != d.OrgID)
				throw new AuthorizationException();

			//Check that member is in the ward
			if (org.WardID != user.WardStakeID)
				throw new AuthorizationException();

			//Add member to companionship
			TeachingAssignment assignment = TeachingAssignment.get(memberID);

			if (assignment == null)
			{
				tTeachingAssignment newAssignment = new tTeachingAssignment();
				newAssignment.MemberID = memberID;
				if (org.ReportID == "Elders Quorum")
					newAssignment.HTID = CompanionshipID;
				else
					newAssignment.VTID = CompanionshipID;
				assignment = TeachingAssignment.create(newAssignment);
			}
			else
			{
				if (org.ReportID == "Elders Quorum")
					assignment.HTID = CompanionshipID;
				else
					assignment.VTID = CompanionshipID;
				TeachingAssignment.save(assignment);
			}

			MemberModel member = MemberModel.get(memberID);

			return Json(new
			{
				@name = member.user.LastName + ", " + member.memberSurvey.prefName
			}, JsonRequestBehavior.AllowGet);

		}

		//
		// GET: /Organization/RemoveTeachee
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool RemoveTeachee(int memberID, int CompanionshipID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			Companionship comp = Companionship.get(CompanionshipID);
			District d = District.get(comp.DistrictID);
			Organization org = Organization.get(orgID);
			MSWUser user = MSWUser.getUser(memberID);

			//Check that the companionship is in the members organization
			if (orgID != d.OrgID)
				throw new AuthorizationException();

			//Check that member is in the ward
			if (org.WardID != user.WardStakeID)
				throw new AuthorizationException();

			//Remove member from companionship
			TeachingAssignment assignment = TeachingAssignment.get(memberID);

			if (org.ReportID == "Elders Quorum")
				assignment.HTID = null;
			else
				assignment.VTID = null;

			TeachingAssignment.save(assignment);

			return true;

		}

		//
		// GET: /Organization/NewDistrict
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult NewDistrict(string NewDistrictName)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();

			//Create New District
			tDistrict newDistrict = new tDistrict();
			newDistrict.OrgID = orgID;
			newDistrict.Title = NewDistrictName;

			District.create(newDistrict);

			return RedirectToAction("ManageTeaching");

		}

		//
		// GET: /Organization/RemoveDistrict
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool RemoveDistrict(int DistrictID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			District district = District.get(DistrictID);

			//Remove the user from the District Leader role if assigned
			if (district.LeaderAssigned)
			{
				District.removeDistrictLeader(district);
			}

			//Remove companionships and all the members who are assigned as teachers and teachees
			Repository r = Repository.getInstance();
			List<Companionship> comps = Cache.GetList(r.getCompanionships(district.DistrictID), x => Cache.getCacheKey<Companionship>(x), y => Companionship.get(y));

			foreach (var comp in comps)
			{
				_removeCompanionship(comp);
			}

			//Remove District
			District.remove(district);

			return true;
		}

		//
		// GET: /Organization/AddCompanionship
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public int AddCompanionship(int DistrictID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			District district = District.get(DistrictID);

			//Add new Companionship
			tCompanionship newComp = new tCompanionship();
			newComp.DistrictID = district.DistrictID;

			Companionship comp = Companionship.create(newComp);

			return comp.CompanionshipID;
		}

		//
		// GET: /Organization/RemoveCompanionship
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool RemoveCompanionship(int CompanionshipID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			Companionship comp = Companionship.get(CompanionshipID);
			District d = District.get(comp.DistrictID);
			Organization org = Organization.get(orgID);

			//Check that the companionship is in the members organization
			if (orgID != d.OrgID)
				throw new AuthorizationException();

			_removeCompanionship(comp);

			return true;
		}

		//
		// GET: /Organization/OrgToTeachList
		[Authorize(Roles = "Bishopric, Elders Quorum")]
		public ActionResult OrgToTeachList()
		{
			try
			{
				if (Session["Username"] == null)
					_NewSession();

				int orgID = _getUserOrganizationID();
				Organization org = Organization.get(orgID);
				Repository r = Repository.getInstance();

				IEnumerable<Organization> orgs = Cache.GetList(r.OrganizationIDs(org.WardID), x => Cache.getCacheKey<Organization>(x), y => Organization.get(y));

				//Only the relief Society is taught by elders quorum
				orgs = orgs.Where(x => x.ReportID == "Relief Society").ToList();

				ViewBag.OrgID = orgID;
				return View(Cache.GetList(orgs.Select(x => x.OrgID), x => Cache.getCacheKey<MSWOrganizationModel>(x), y => MSWOrganizationModel.get(y)));
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		//Changes who teaches the organization
		// GET: /Organization/OrgToTeach
		[Authorize(Roles = "Bishopric, Elders Quorum")]
		public bool OrgToTeach(int TeachingOrgID, string button)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			Organization org = Organization.get(TeachingOrgID);
			MSWUser user = MSWUser.getUser(User.Identity.Name);

			//Check that member is in the ward
			if (org.WardID != user.WardStakeID)
				throw new AuthorizationException();

			TeachingOrganization teachingOrg = TeachingOrganization.get(TeachingOrgID);
			if (button == "Teaching")
			{
				if (teachingOrg == null)
				{
					tTeachingOrganization t = new tTeachingOrganization();
					t.TeachingOrganizationID = TeachingOrgID;
					t.TeacherID = orgID;
					TeachingOrganization.create(t);
				}
				else
				{
					teachingOrg.TeacherID = orgID;
					TeachingOrganization.save(teachingOrg);
				}
			}
			else
			{
				//Remove all teachees that belong to this organization
				Repository r = Repository.getInstance();
				List<TeachingAssignment> assignments = Cache.GetList(r.OrganizationMembership(TeachingOrgID), x => Cache.getCacheKey<TeachingAssignment>(x),
											y => TeachingAssignment.get(y));

				foreach (var member in assignments)
				{
					//If the member is being home taught by this organization then that assignment needs to be removed.
					if (member == null)
						continue;
					if (member.HTID == null)
						continue;

					Companionship comp = Companionship.get((int)member.HTID);
					District dist = District.get(comp.DistrictID);

					if (dist.OrgID == orgID)
					{
						member.HTID = null;
						TeachingAssignment.save(member);
					}
				}

				TeachingOrganization.remove(teachingOrg);
			}

			return true;

		}

		//Changes who teaches the organization
		// GET: /Organization/ChangeDistrictLeader
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public JsonResult ChangeDistrictLeader(int? newLeaderCallingID, int DistrictID)
		{
			if (Session["Username"] == null)
				_NewSession();

			int orgID = _getUserOrganizationID();
			District district = District.get(DistrictID);
			Organization org = Organization.get(district.OrgID);
			MSWUser user = MSWUser.getUser(User.Identity.Name);

			//Check that the User is in the assigned district
			if (org.WardID != user.WardStakeID)
				throw new AuthorizationException();

			//Check to make sure the new leader ID is already a district leader
			Repository r = Repository.getInstance();
			List<District> districts = Cache.GetList(r.getDistricts(district.OrgID), x => Cache.getCacheKey<District>(x),
											y => District.get(y));

			if (districts.Select(x => x.DistrictLeaderID).Contains(newLeaderCallingID) && newLeaderCallingID != null)
				return Json(new
				{
					@callingID = district.DistrictLeaderID,
					@error = "alreadyDistrictLeader"
				}, JsonRequestBehavior.AllowGet);

			//Remove the old district leader
			if (district.DistrictLeaderID != null && newLeaderCallingID != district.DistrictLeaderID)
			{
				district.LeaderAssigned = District.removeDistrictLeader(district);
			}

			//Assign the new calling to be the district leader
			if (newLeaderCallingID != null)
			{
				district.DistrictLeaderID = newLeaderCallingID;
				district.LeaderAssigned = District.assignDistrictLeader(district);
			}
			else //Remove any district leader from the district
			{
				district.DistrictLeaderID = null;
				district.LeaderAssigned = false;
			}

			District.save(district);

			return Json(new
			{
				@callingID = newLeaderCallingID,
				@error = ""
			}, JsonRequestBehavior.AllowGet);

		}

		private void _removeCompanionship(Companionship comp)
		{
			//Reset Teachers
			Repository r = Repository.getInstance();
			List<TeachingAssignment> teachers = Cache.GetList(r.getTeachers(comp.CompanionshipID), x => Cache.getCacheKey<TeachingAssignment>(x), y => TeachingAssignment.get(y));

			foreach (var assignment in teachers)
			{
				if (assignment.CompanionshipID == comp.CompanionshipID)
				{
					assignment.CompanionshipID = null;
					TeachingAssignment.save(assignment);
				}
			}

			//Reset Teachees
			List<TeachingAssignment> teachees = Cache.GetList(r.getTeachees(comp.CompanionshipID), x => Cache.getCacheKey<TeachingAssignment>(x), y => TeachingAssignment.get(y));

			foreach (var assignment in teachees)
			{
				if (assignment.HTID == comp.CompanionshipID)
				{
					assignment.HTID = null;
					TeachingAssignment.save(assignment);
				}
				if (assignment.VTID == comp.CompanionshipID)
				{
					assignment.VTID = null;
					TeachingAssignment.save(assignment);
				}
			}

			//TODO: Set TeachingVisits CompanionshipIDs to NULL

			//Remove Companionships
			Companionship.remove(comp);
		}
		#endregion

		#region Teaching

		//Main page for Teaching - both home and visiting
		// GET: /Organization/Teaching
		[Authorize]
		public ActionResult Teaching()
		{
			if (Session["Username"] == null)
				_NewSession();

			MSWUser user = MSWUser.getUser(User.Identity.Name);
			try
			{
				ReportTeachingModel rtModel = new ReportTeachingModel(user.MemberID);
				return View(rtModel);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("RequestOrganization");
			}

		}

		//Action to submit teaching reports
		// GET: /Organization/Reports
		[Authorize]
		public int ReportVisit(int visitID, bool wasVisited, bool needsAttention)
		{
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			TeachingAssignment teachAssign = TeachingAssignment.get(user.MemberID);

			TeachingVisit tV = TeachingVisit.get(visitID);

			//Remove the percentage out of the cache
			Repository.getInstance().removeTeachingPercentage(Organization.get(
				District.get(Companionship.get(tV.CompanionshipID).DistrictID).OrgID).OrgID, tV.TeachingMonthID);

			//make sure the user is allowed to submit this report
			//It is either one of the companions of the companionship or the district/org Leader
			try
			{
				_authorizeVisitReport(user, teachAssign, tV);
			}
			catch (Exception e) //User not authorized
			{
				throw e; //AJAX handler handle error
			}

			//Save the visit
			tV.wasVisited = wasVisited;
			tV.needsAttention = needsAttention;
			tV.reported = true;
			TeachingVisit.save(tV);

			//Create teaching records for each member in the companionship
			Repository r = Repository.getInstance();

			//tA may be null if the user is a bishopric user
			int? companionshipID = tV.CompanionshipID;

			foreach (int MemberID in r.getTeachers((int)companionshipID))
			{
				TeachingRecord.create(MemberID, visitID);
			}

			//Create Teaching record for person being taught            
			TaughtRecord.create(tV.MemberID, visitID);

			return tV.MemberID;
		}

		//
		// GET: /Organization/ManageTeaching
		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society, District Leader")]
		public ActionResult ReportTeaching()
		{
			try
			{
				int orgID = _getUserOrganizationID();
				ViewBag.org = Organization.get(orgID);
				Dictionary<District, List<ReportTeachingModel>> models = new Dictionary<District, List<ReportTeachingModel>>();

                if (User.IsInRole("Elders Quorum") || User.IsInRole("Relief Society") || User.IsInRole("Bishopric"))
				{
					IEnumerable<int> districtIDs = Repository.getInstance().getDistricts(orgID);
					foreach (int id in districtIDs)
					{
						District district = District.get(id);

						//Add companionships to list
						List<int> compIDs = new List<int>();
						compIDs.AddRange(Repository.getInstance().getCompanionships(id));

						List<ReportTeachingModel> reports = new List<ReportTeachingModel>();
						foreach (int cID in compIDs)
						{
							reports.Add(new ReportTeachingModel(cID.ToString()));
						}

						models.Add(district, reports);
					}
				}
				else
				{
					//Find the districtID for the district leader requesting the page
					MSWUser user = MSWUser.getUser(User.Identity.Name);

					IEnumerable<int> districtIDs = Repository.getInstance().getDistricts(orgID);
					foreach (int id in districtIDs)
					{
						District district = District.get(id);
						if (district.DistrictLeaderID != null)
						{
							Calling calling = Calling.get((int)district.DistrictLeaderID);
							if (calling.MemberID == user.MemberID)
							{
								List<int> compIDs = new List<int>();
								compIDs.AddRange(Repository.getInstance().getCompanionships(id));

								List<ReportTeachingModel> reports = new List<ReportTeachingModel>();
								foreach (int cID in compIDs)
								{
									reports.Add(new ReportTeachingModel(cID.ToString()));
								}

								models.Add(district, reports);
								break;
							}
						}
					}
				}

				return View(models);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		/// <summary>
		/// Checks the authorization of the user for the visit report submission
		/// </summary>
		private void _authorizeVisitReport(MSWUser user, TeachingAssignment teachAssign, TeachingVisit tV)
		{
			Companionship comp = Companionship.get(tV.CompanionshipID);

			District district = District.get(comp.DistrictID);
			Organization org = Organization.get(district.OrgID);

			//User is in the bishopric
			if (org.WardID == user.WardStakeID && User.IsInRole("Bishopric"))
				return;

			//User is in the companionship for the current visit
			if (teachAssign.CompanionshipID == comp.CompanionshipID)
				return;

			//Get District Leader
			Calling districtLeader = null;
			if (district.LeaderAssigned)
				districtLeader = Calling.get((int)district.DistrictLeaderID);

			//District Leader has been called and sustained and is the current user
			if (districtLeader != null)
				if (districtLeader.MemberID == user.MemberID && districtLeader.CallingStatus >= (int)Calling.Status.SUSTAINED)
					return;

			//The user is called and sustained the Organization Leader
			if (org.LeaderCallingID != null)
			{
				Calling orgLeader = Calling.get((int)org.LeaderCallingID);
				if (orgLeader.MemberID == user.MemberID && orgLeader.CallingStatus >= (int)Calling.Status.SUSTAINED)
					return;
			}

			//The user is called and sustained as a co-leader of the organization
			List<int> coleaders = Repository.getInstance().CoLeaderIDs(org.OrgID);
			foreach (int callingID in coleaders)
			{
				Calling coleader = Calling.get(callingID);
				if (coleader.MemberID == user.MemberID && coleader.CallingStatus >= (int)Calling.Status.SUSTAINED)
					return;
			}

			//User did not passed the authorization tests
			throw new AuthorizationException();
		}



		#endregion

		#region Teaching Reports

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult Reports()
		{
			try
			{
				int orgID = _getUserOrganizationID();
				MSWUser user = MSWUser.getUser(User.Identity.Name);

				OrganizationReport report = new OrganizationReport(orgID);

				return View(report);
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		[HttpGet]
		public ActionResult TeachingCSV(int MonthID)
		{
			try
			{
				int orgID = _getUserOrganizationID();
				Organization org = Organization.get(orgID);
				TeachingMonth month = TeachingMonth.get(MonthID);

				String file = GenerateCSV.MakeMonthlyTeachingReport(orgID, MonthID);
				byte[] byteArray = Encoding.ASCII.GetBytes(file);
				MemoryStream stream = new MemoryStream(byteArray);

				FileStreamResult fileStream = new FileStreamResult(stream, "application/csv");
				fileStream.FileDownloadName = "MonthlyReport - " + org.Title + " - " + TeachingMonth.monthNames[month.teachingMonth.Month - 1] + ".csv";
				return fileStream;
			}
			catch (OrganizationNotSetException e)
			{
				//Bishopric user needs to go select an organization before they can continue
				return RedirectToAction("SelectOrganization");
			}
			catch (UserNotInOrganizationException e)
			{
				//User not in an organization yet
				return RedirectToAction("RequestOrganization");
			}
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult CallingOverview()
		{
			int orgID = _getUserOrganizationID();
			Organization org = Organization.get(orgID);

			CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
			var callings = Repository.getInstance().CallingIDs(org.OrgID);

			foreach (var callingID in callings)
			{
				newOrg.Callings.Add(new CallingModel(callingID));
			}

			return View(newOrg);
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult WithoutCallings()
		{
			int orgID = _getUserOrganizationID();
			MSWUser user = MSWUser.getUser(User.Identity.Name);

			var membersWithoutCallings = Repository.getInstance().WithoutCallings(user.WardStakeID);
			var membersList = new List<ListItem>();
			foreach (var memberID in membersWithoutCallings)
			{
				MemberModel member = MemberModel.get(memberID);
				string name = member.user.LastName + ", " + member.memberSurvey.prefName;

				//Check to see if the member is in the organization
				OrganizationMember orgMember = OrganizationMember.get(member.user.MemberID);
				if (orgMember == null)
					continue;
				if (orgMember.OrgID == orgID)
					membersList.Add(new ListItem { Text = name, Value = member.user.MemberID.ToString() });
			}
			membersList = membersList.OrderBy(x => x.Text).ToList();
			return View(membersList);
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult TeachingReport()
		{
			int orgID = _getUserOrganizationID();

			return View(new TeachingReport(orgID));
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult TeachingReportByMember()
		{
			int orgID = _getUserOrganizationID();

			return View(new MemberTeachingReport(orgID));
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public ActionResult MembersNotTaught()
		{
			int orgID = _getUserOrganizationID();

			return View();
		}

		#endregion

		#region Helper Methods

		/* Returns the Organization ID for a user */
		private int _getUserOrganizationID()
		{
			int? orgID = null;
			if (User.IsInRole("Bishopric"))
			{
				try
				{
					orgID = int.Parse(Session["orgID"] as string);
				}
				catch (ArgumentNullException e)
				{
					//This should have been set. If orgID is null then the user needs to be sent to SelectOrganization
					throw new OrganizationNotSetException();
				}
			}
			else if (User.IsInRole("Elders Quorum") || User.IsInRole("Relief Society"))
			{
				//Leader user will only be assigned to one organization
				MSWUser user = MSWUser.getUser(User.Identity.Name);

				Repository r = Repository.getInstance();
				OrganizationMember orgMembership = OrganizationMember.get(user.MemberID);

				//First time a leader goes to view an organization, they need to be added to the organization they are the leader of
				//Algorithm may seem a little intense but it needs to be sure the calling is the leadership calling incase the member has multiple callings
				if (orgMembership == null)
				{
					//give membership to the leader of the organization
					var callingIDs = r.MemberCallings(user.MemberID);

					foreach (var callingID in callingIDs)
					{
						Calling calling = Calling.get(callingID);
						Organization org = Organization.get(calling.OrgID);

						if (org.LeaderCallingID == calling.CallingID && User.IsInRole(org.ReportID))
						{
							//Create Membership
							tOrganizationMember orgMember = new tOrganizationMember();
							orgMember.MemberID = user.MemberID;
							orgMember.OrgID = org.OrgID;
                            orgMember.Status = (int)OrganizationMember.Status.APPROVED;
							OrganizationMember.create(orgMember);

							//Assign return variable
							orgID = org.OrgID;
							break;
						}
						else if (User.IsInRole(org.ReportID)) //Checks the co-leaders
						{
							var coLeaderIDs = r.CoLeaderIDs(org.OrgID);

							foreach (var coleaderID in coLeaderIDs)
							{
								if (coleaderID == callingID)
								{
									//Create Membership
									tOrganizationMember orgMember = new tOrganizationMember();
									orgMember.MemberID = user.MemberID;
									orgMember.OrgID = org.OrgID;
                                    orgMember.Status = (int)OrganizationMember.Status.APPROVED;
									OrganizationMember.create(orgMember);

									//Assign return variable
									orgID = org.OrgID;
									break;
								}
							}
						}
					}

				}
				else
				{
					//Make sure organization leader is in their organization that they lead
					if (orgMembership.status < (int)OrganizationMember.Status.APPROVED)
					{
						orgMembership.status = (int)OrganizationMember.Status.APPROVED;
						OrganizationMember.save(orgMembership);
					}

					orgID = orgMembership.OrgID;
				}
			}
			else
			{
				//Regular user will only be assigned to one organization
				MSWUser user = MSWUser.getUser(User.Identity.Name);

				OrganizationMember orgMembership = OrganizationMember.get(user.MemberID);

				//Throws if the user is not in a organization
				if (orgMembership == null)
					throw new UserNotInOrganizationException();
				else if (orgMembership.OrgID == NOTINORG)
					throw new UserNotInOrganizationException();
				else
					orgID = orgMembership.OrgID;
			}
			return (int)orgID;
		}

		[Authorize]
		public ActionResult OrganizationPending()
		{
			return View();
		}

		[Authorize]
		public ActionResult NotInOrganization()
		{
			return View();
		}

		[Authorize]
		public ActionResult Error()
		{
			return View();
		}

		//Checks to see if the request to change an organization came from a user in the ward that the organization belongs too
		private void _checkUserWard(int orgID)
		{
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			Organization org = Organization.get(orgID);

			if (user.WardStakeID != org.WardID)
				throw new UserNotInOrganizationException();
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

				}
			}
			catch
			{
				//Sometimes this would trigger an exception if a stake member was signed on and came back later
				throw new Exception();
			}
		}
		#endregion
	}
}
