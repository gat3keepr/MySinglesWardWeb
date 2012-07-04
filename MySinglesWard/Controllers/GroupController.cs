using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW.Model;
using MSW.Models;
using System.Configuration.Provider;
using MSW.Utilities;
using MSW.Models.dbo;
using MSW.Exceptions;

namespace MSW.Controllers
{
	[HandleError]
	public class GroupController : Controller
	{
		//
		// GET: /Group/

		[Authorize]
		public ActionResult Index()
		{
			if (Session["Username"] == null)
				_NewSession();

			if (Session["IsBishopric"] == null)
				ViewData["IsBishopric"] = "false";
			else
				ViewData["IsBishopric"] = Session["IsBishopric"];

			//Redirects if the user can only join groups
			if (!User.IsInRole("StakePres") && !User.IsInRole("Stake") && !User.IsInRole("Stake?") && !User.IsInRole("Bishopric") && !User.IsInRole("Bishopric?")
			&& !User.IsInRole("Elders Quorum") && !User.IsInRole("Elders Quorum?") && !User.IsInRole("Relief Society") &&
			!User.IsInRole("Relief Society?") && !User.IsInRole("Activities") && !User.IsInRole("Activities?") && !User.IsInRole("FHE")
			&& !User.IsInRole("FHE?"))
			{
				return RedirectToAction("Join", "Group");
			}

			return View();
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE," +
									"Stake?, Bishopric?, Elders Quorum?, Relief Society?, Activities?, FHE?")]
		public ActionResult List(bool? error)
		{
			if (Session["Username"] == null)
				_NewSession();

			if (!_checkValidUnit())
				return RedirectToAction("NotInWard", "Home");

			if (!User.IsInRole("StakePres") && !User.IsInRole("Stake") && !User.IsInRole("Bishopric") &&
				!User.IsInRole("Elders Quorum") && !User.IsInRole("Relief Society") &&
				!User.IsInRole("Activities") && !User.IsInRole("FHE"))
			{
				return RedirectToAction("SendNotification", "Group");
			}

			using (var db = new DBmsw())
			{

				//Name List
				List<SelectListItem> nameData = _getNameList();
				ViewData["NameList"] = nameData;

				//Type           
				List<SelectListItem> GroupList = _getGroupTypeList();
				ViewData["GroupList"] = GroupList;

				int myType = _getGroupType();

				//Groups
				IQueryable<tGroup> groups;

				if (User.IsInRole("StakePres") || User.IsInRole("Stake"))
				{
					groups = (from w in db.tGroups
							  where w.WardStakeID == double.Parse(Session["StakeID"] as string)
							  where w.Type == 0
							  select w);
				}
				else
				{
					if (myType == -1)
						groups = (from w in db.tGroups
								  where w.WardStakeID == double.Parse(Session["WardStakeID"] as string)
								  where w.Type != 0
								  select w);
					else
						groups = (from w in db.tGroups
								  where w.WardStakeID == double.Parse(Session["WardStakeID"] as string)
								  where w.Type == myType
								  select w);
				}

				List<Group> groupList = new List<Group>();
				foreach (var group in groups)
				{
					groupList.Add(Group.get(group.GroupID));
				}

				groupList = groupList.OrderBy(x => x.TypeID).ThenBy(x => x.Name).ToList();
				//Used to get default values to show in the list
				var dummyGroup = new tGroup();
				dummyGroup.Type = -1;
				ViewData["tGroup"] = dummyGroup;
				if (error == true)
					ViewData["Error"] = "Please try creating the group again.";
				return View(groupList);
			}
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE")]
		[HttpPost]
		public ActionResult Create(tGroup newGroup, FormCollection collection)
		{
			if (Session["Username"] == null)
				_NewSession();
			if (!_checkValidUnit())
				return RedirectToAction("NotInWard", "Home");

			try
			{
				using (var db = new DBmsw())
				{
					if (ModelState.IsValid)
					{
						if (newGroup.Type == (int)GroupType.STAKE)
							newGroup.WardStakeID = double.Parse(Session["StakeID"] as string);
						else
							newGroup.WardStakeID = double.Parse(Session["WardStakeID"] as string);

						db.tGroups.InsertOnSubmit(newGroup);
						db.SubmitChanges();

						_addLeaderRole(newGroup.LeaderID, newGroup.GroupID, newGroup.Type);
						_addLeaderRole(newGroup.CoLeaderID, newGroup.GroupID, newGroup.Type);
					}
				}
			}
			catch
			{
				return RedirectToAction("List", "Group", new { error = true });
			}

			return RedirectToAction("List", "Group");
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE," +
									"Stake?, Bishopric?, Elders Quorum?, Relief Society?, Activities?, FHE?")]
		public ActionResult SendNotification()
		{
			if (Session["Username"] == null)
				_NewSession();
			NotificationModel info = new NotificationModel(int.Parse(Session["MemberID"] as string), Session["Username"] as string);
			if (info.count == 0)
				return RedirectToAction("NoGroups", "Group");
			return View(info);
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE," +
									"Stake?, Bishopric?, Elders Quorum?, Relief Society?, Activities?, FHE?")]
		[HttpPost]
		public ActionResult SendNotification(string[] Stake, string[] Ward, string[] EQ, string[] RS, string[] Activities,
										string[] FHE, string text)
		{
			if (Session["Username"] == null)
				_NewSession();

			if (Stake == null && Ward == null && EQ == null && RS == null && Activities == null && FHE == null)
			{
				ViewData["Error"] = "Please choose a group.";
				NotificationModel info = new NotificationModel(int.Parse(Session["MemberID"] as string), Session["Username"] as string);
				return View(info);
			}

			using (var db = new DBmsw())
			{
				HashSet<int> groups = new HashSet<int>();
				if (Stake != null)
				{
					if (Stake[0].Contains("STAKE"))
					{
						string stakeID = Stake[0].Replace("STAKE", "");
						var moreGroups = db.tGroups.Where(x => x.WardStakeID == double.Parse(stakeID)).Where(x => x.Type == (int)GroupType.STAKE);
						foreach (var group in moreGroups)
						{
							groups.Add(group.GroupID);
						}
					}
					else
					{
						foreach (string groupID in Stake)
						{
							groups.Add(int.Parse(groupID));
						}
					}
				}

				if (Ward != null)
				{
					if (Ward[0].Contains("WARD"))
					{
						string wardID = Ward[0].Replace("WARD", "");
						var moreGroups = db.tGroups.Where(x => x.WardStakeID == double.Parse(wardID)).Where(x => x.Type == (int)GroupType.WARD);
						foreach (var group in moreGroups)
						{
							groups.Add(group.GroupID);
						}


					}
					else
					{
						foreach (string groupID in Ward)
						{
							groups.Add(int.Parse(groupID));
						}
					}
				}

				if (EQ != null)
				{
					if (EQ[0].Contains("EQ"))
					{
						string wardID = EQ[0].Replace("EQ", "");
						var moreGroups = db.tGroups.Where(x => x.WardStakeID == double.Parse(wardID)).Where(x => x.Type == (int)GroupType.ELDERS_QUORUM);
						foreach (var group in moreGroups)
						{
							groups.Add(group.GroupID);
						}
					}
					else
					{
						foreach (string groupID in EQ)
						{
							groups.Add(int.Parse(groupID));
						}
					}
				}

				if (RS != null)
				{
					if (RS[0].Contains("RS"))
					{
						string wardID = RS[0].Replace("RS", "");
						var moreGroups = db.tGroups.Where(x => x.WardStakeID == double.Parse(wardID)).Where(x => x.Type == (int)GroupType.RELIEF_SOCIETY);
						foreach (var group in moreGroups)
						{
							groups.Add(group.GroupID);
						}
					}
					else
					{
						foreach (string groupID in RS)
						{
							groups.Add(int.Parse(groupID));
						}
					}
				}

				if (Activities != null)
				{
					if (Activities[0].Contains("ACTIVITIES"))
					{
						string wardID = Activities[0].Replace("ACTIVITIES", "");
						var moreGroups = db.tGroups.Where(x => x.WardStakeID == double.Parse(wardID)).Where(x => x.Type == (int)GroupType.ACTIVITIES);
						foreach (var group in moreGroups)
						{
							groups.Add(group.GroupID);
						}
					}
					else
					{
						foreach (string groupID in Activities)
						{
							groups.Add(int.Parse(groupID));
						}
					}
				}

				if (FHE != null)
				{
					if (FHE[0].Contains("FHE"))
					{
						string wardID = FHE[0].Replace("FHE", "");
						var moreGroups = db.tGroups.Where(x => x.WardStakeID == double.Parse(wardID)).Where(x => x.Type == (int)GroupType.FHE);
						foreach (var group in moreGroups)
						{
							groups.Add(group.GroupID);
						}
					}
					else
					{
						foreach (string groupID in FHE)
						{
							groups.Add(int.Parse(groupID));
						}
					}
				}

				try
				{
					MSWtools.SendNotificationByGroupID(groups, text);
				}
				catch (InvalidOperationException e)
				{
					return RedirectToAction("SentFail", "Group", new { noneGroup = true });
				}
				catch (Exception e)
				{
					MSWtools._sendException(e);
					return RedirectToAction("SentFail", "Group");
				}
				return RedirectToAction("Sent", "Group");
			}
		}

		[Authorize(Roles = "Bishopric")]
		public bool SendToEntireWard(bool includeBishopric, string text)
		{
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			//Get all the memberIDs from the ward
			Repository r = Repository.getInstance();
			List<int> wardMembers = r.WardMembersID(user.WardStakeID);

			List<int> bishopric = new List<int>();

			bishopric = r.BishopricMembersID(user.WardStakeID);

			try
			{
				MSWtools.SendNotificationByMemberID(text, wardMembers, bishopric);
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
				throw e;
			}

			return true;
		}

		[Authorize(Roles = "Bishopric, Elders Quorum, Relief Society")]
		public bool SendToOrg(List<int> orgids, string text)
		{
			MSWUser user = MSWUser.getUser(User.Identity.Name);
			//Get all the memberIDs from the ward
			Repository r = Repository.getInstance();

			List<int> members = new List<int>();
			foreach (int orgID in orgids)
			{
				//Check to make sure organizations are in the ward
				Organization org = Organization.get(orgID);
				if (user.WardStakeID == org.WardID)
					members.AddRange(r.OrganizationMembership(orgID));
			}

			try
			{
				MSWtools.SendNotificationByMemberID(text, members, new List<int>());
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
				throw e;
			}

			return true;
		}

		[Authorize]
		public ActionResult Details(int id)
		{
			if (Session["Username"] == null)
				_NewSession();

			if (!_checkValidUnit())
				return RedirectToAction("NotInWard", "Home");

			try
			{
				using (var db = new DBmsw())
				{
					tGroup group = db.tGroups.SingleOrDefault(x => x.GroupID.Equals(id));

					//Check to make sure the group is in the ward
					_checkAuthorizedUser(group, MSWUser.getUser(User.Identity.Name));

					Group groupViewData = Group.get(group.GroupID);
					groupViewData.GenerateMemberList();
					return View(groupViewData);
				}
			}
			catch (AuthorizationException e) // User not authorized to view this group
			{
				return RedirectToAction("Unauthorized", "Home");
			}
		}

		[Authorize]
		public ActionResult Join()
		{
			if (Session["Username"] == null)
				_NewSession();

			if (!_checkValidUnit())
				return RedirectToAction("NotInWard", "Home");

			try
			{
				using (var db = new DBmsw())
				{
					var stake = db.tWardStakes.SingleOrDefault(x => x.WardStakeID == double.Parse(Session["WardStakeID"] as string));
					//Ward Groups
					var groups = db.tGroups.Where(x => x.WardStakeID == double.Parse(Session["WardStakeID"] as string)).ToList();

					var stakeGroups = new List<tGroup>();
					if (stake != null)
					{
						stakeGroups = db.tGroups.Where(x => x.WardStakeID == stake.StakeID).ToList();
						foreach (var group in stakeGroups)
						{
							groups.Add(group);
						}
					}

					return View(new GroupListModel(groups, int.Parse(Session["MemberID"] as string)));
				}
			}
			catch
			{
				return RedirectToAction("NoGroups");
			}
		}

		[Authorize]
		public bool JoinRequest(string button, string groupid, string memberid)
		{
			int groupID = int.Parse(groupid);
			int memberID = int.Parse(memberid);
			try
			{
				using (var db = new DBmsw())
				{
					//Check to make sure the user sending this request is in the correct ward
					_checkAuthorizedUser(db.tGroups.SingleOrDefault(x => x.GroupID == groupID), MSWUser.getUser(memberID));

					if (button.Equals("join"))
					{
						var checkGroupMembership = db.tGroupUsers.SingleOrDefault(x => x.GroupID == groupID && x.MemberID == memberID) != null;

						if (!checkGroupMembership)
						{
							var groupAssign = new tGroupUser();
							groupAssign.GroupID = groupID;
							groupAssign.MemberID = memberID;
							groupAssign.Leader = false;
							db.tGroupUsers.InsertOnSubmit(groupAssign);
						}
					}
					else
					{
						var groupRemove = db.tGroupUsers.Where(x => x.GroupID == groupID && x.MemberID == memberID && x.Leader == false);
						db.tGroupUsers.DeleteAllOnSubmit(groupRemove);
					}
					db.SubmitChanges();

					Repository.getInstance().removeGroupMemberIDs(groupID);
				}
			}
			catch (AuthorizationException ex)
			{
				throw ex;
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
				throw e;
			}

			return true;
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE," +
									"Stake?, Bishopric?, Elders Quorum?, Relief Society?, Activities?, FHE?")]
		public ActionResult Edit(int? id)
		{
			if (Session["Username"] == null)
				_NewSession();

			if (!_checkValidUnit())
				return RedirectToAction("NotInWard", "Home");

			try
			{
				using (var db = new DBmsw())
				{
					tGroup group = db.tGroups.SingleOrDefault(x => x.GroupID.Equals(id));
					if (!_checkGroup(group))
						return RedirectToAction("Unauthorized", "Home");

					//Name List
					List<SelectListItem> nameData = _getNameList();
					ViewData["NameList"] = nameData;

					//Type           
					List<SelectListItem> GroupTypeList = _getGroupTypeList();
					ViewData["GroupList"] = GroupTypeList;

					Group groupViewData = Group.get(group.GroupID);
					groupViewData.GenerateMemberList();
					return View(groupViewData);
				}
			}
			catch (AuthorizationException e)
			{
				return RedirectToAction("Unauthorized", "Home");
			}
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE")]
		[HttpPost]
		public ActionResult Edit(int? id, FormCollection collection)
		{
			if (Session["Username"] == null)
				_NewSession();

			if (!_checkValidUnit())
				return RedirectToAction("NotInWard", "Home");

			using (var db = new DBmsw())
			{
				tGroup group = db.tGroups.SingleOrDefault(x => x.GroupID.Equals(id));
				int oldType = group.Type;
				int? oldLeaderID = group.LeaderID;
				int? oldCoLeaderID = group.CoLeaderID;

				try
				{

					if (!_checkGroup(group))
						return RedirectToAction("Unauthorized", "Home");

					UpdateModel(group, collection);
					group.Type = int.Parse(collection.Get(1));

					//Make sure name isn't empty
					if (group.Name == null)
					{
						//Name List
						List<SelectListItem> nameData = _getNameList();
						ViewData["NameList"] = nameData;

						//Type           
						List<SelectListItem> GroupTypeList = _getGroupTypeList();
						ViewData["GroupList"] = GroupTypeList;

						ViewData["Error"] = "Please enter a name.";

						Group groupViewData = Group.get(group.GroupID);
						groupViewData.GenerateMemberList();
						return View(groupViewData);
					}

					//Check if types changed and change roles if they did & Change Leader Roles
					if (oldType != group.Type || group.LeaderID != oldLeaderID || group.CoLeaderID != oldCoLeaderID)
					{
						if (group.LeaderID != null)
						{
							_removeLeaderRole(oldLeaderID, group.GroupID, oldType);
							_addLeaderRole(group.LeaderID, group.GroupID, group.Type);
						}
						else
							_removeLeaderRole(oldLeaderID, group.GroupID, oldType);

						if (group.CoLeaderID != null)
						{
							_removeLeaderRole(oldCoLeaderID, group.GroupID, oldType);
							_addLeaderRole(group.CoLeaderID, group.GroupID, group.Type);
						}
						else
							_removeLeaderRole(oldCoLeaderID, group.GroupID, oldType);
					}

					db.SubmitChanges();
				}
				catch (Exception e)
				{
					//Name List
					List<SelectListItem> nameData = _getNameList();
					ViewData["NameList"] = nameData;

					//Type           
					List<SelectListItem> GroupTypeList = _getGroupTypeList();
					ViewData["GroupList"] = GroupTypeList;

					if (e is FormatException)
						ViewData["Error"] = "Please select a type.";
					else
						ViewData["Error"] = "Please fill in all fields and try again.";

					Group groupViewData = Group.get(group.GroupID);
					groupViewData.GenerateMemberList();
					return View(groupViewData);
				}

				return RedirectToAction("List", "Group");
			}
		}

		private void _addLeaderRole(int? LeaderID, int? GroupID, int type)
		{
			if (LeaderID == null)
				return;

			try
			{
				using (var db = new DBmsw())
				{
					tUser user = db.tUsers.SingleOrDefault(x => x.MemberID == LeaderID);

					string role = _getRole(type);

					if (System.Web.Security.Roles.IsUserInRole(user.UserName, role))
					{
						var roles = db.tMemberGroupRoles.SingleOrDefault(x => x.MemberID == LeaderID);

						bool newEntry = false;
						if (roles == null)
						{
							roles = new tMemberGroupRole();
							roles.MemberID = (int)LeaderID;
							newEntry = true;
						}

						switch (type)
						{
							case 0:
								roles.Stake++;
								break;
							case 1:
								roles.Bishopric++;
								break;
							case 2:
								roles.EldersQuorum++;
								break;
							case 3:
								roles.ReliefSociety++;
								break;
							case 4:
								roles.Activities++;
								break;
							case 5:
								roles.FHE++;
								break;
						}

						if (newEntry)
							db.tMemberGroupRoles.InsertOnSubmit(roles);

						var newLeader = new tGroupUser();
						newLeader.MemberID = (int)LeaderID;
						newLeader.GroupID = (int)GroupID;
						newLeader.Leader = true;
						db.tGroupUsers.InsertOnSubmit(newLeader);
					}
					else
					{
						System.Web.Security.Roles.AddUserToRole(user.UserName, role);

						var newLeader = new tGroupUser();
						newLeader.MemberID = (int)LeaderID;
						newLeader.GroupID = (int)GroupID;
						newLeader.Leader = true;
						db.tGroupUsers.InsertOnSubmit(newLeader);


					}

					var regularMembershipRecords = db.tGroupUsers.Where(x => x.Leader == false && x.MemberID == LeaderID && x.GroupID == GroupID);
					if (regularMembershipRecords != null)
						db.tGroupUsers.DeleteAllOnSubmit(regularMembershipRecords);

					db.SubmitChanges();
				}
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
			}
		}

		private void _removeLeaderRole(int? oldLeaderID, int? groupID, int oldType)
		{
			if (oldLeaderID == null)
				return;

			try
			{
				using (var db = new DBmsw())
				{
					tUser user = db.tUsers.SingleOrDefault(x => x.MemberID == oldLeaderID);

					string role = _getRole(oldType);
					bool remove = true;

					var roles = db.tMemberGroupRoles.SingleOrDefault(x => x.MemberID == oldLeaderID);

					//Returns if the user never had multiple roles for groups
					if (roles == null)
					{
						System.Web.Security.Roles.RemoveUserFromRole(user.UserName, role);
						return;
					}

					switch (oldType)
					{
						case 0:
							if (roles.Stake > 0)
							{
								roles.Stake--;
								remove = false;
							}
							break;
						case 1:
							if (roles.Bishopric > 0)
							{
								roles.Bishopric--;
								remove = false;
							}
							break;
						case 2:
							if (roles.EldersQuorum > 0)
							{
								roles.EldersQuorum--;
								remove = false;
							}
							break;
						case 3:
							if (roles.ReliefSociety > 0)
							{
								roles.ReliefSociety--;
								remove = false;
							}
							break;
						case 4:
							if (roles.Activities > 0)
							{
								roles.Activities--;
								remove = false;
							}
							break;
						case 5:
							if (roles.FHE > 0)
							{
								roles.FHE--;
								remove = false;
							}
							break;
					}

					if (remove)
						System.Web.Security.Roles.RemoveUserFromRole(user.UserName, role);

					//Deletes Leader from Group
					var groupLeader = db.tGroupUsers.Where(x => x.GroupID == groupID).FirstOrDefault(x => x.MemberID == user.MemberID);
					db.tGroupUsers.DeleteOnSubmit(groupLeader);

					db.SubmitChanges();
				}
			}
			catch (Exception e)
			{
				MSWtools._sendException(e);
			}
		}

		private string _getRole(int oldType)
		{
			switch (oldType)
			{
				case 0:
					return "Stake?";
				case 1:
					return "Bishopric?";
				case 2:
					return "Elders Quorum?";
				case 3:
					return "Relief Society?";
				case 4:
					return "Activities?";
				case 5:
					return "FHE?";
			}

			throw new InvalidOperationException();
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE")]
		public bool RemoveGroup(int id)
		{
			using (var db = new DBmsw())
			{
				tGroup group = db.tGroups.SingleOrDefault(x => x.GroupID.Equals(id));

				if (!_checkGroup(group))
					return false;

				try
				{
					var users = db.tGroupUsers.Where(x => x.GroupID == group.GroupID);
					db.tGroups.DeleteOnSubmit(group);

					//Remove Leaders from Roles;
					_removeLeaderRole(group.LeaderID, group.GroupID, group.Type);
					_removeLeaderRole(group.CoLeaderID, group.GroupID, group.Type);

					db.tGroupUsers.DeleteAllOnSubmit(users);
					db.SubmitChanges();
				}
				catch (Exception e)
				{
					MSWtools._sendException(e);
					return false;
				}
				return true;
			}
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE," +
									 "Stake?, Bishopric?, Elders Quorum?, Relief Society?, Activities?, FHE?")]
		public bool RemoveMember(int groupid, int memberid)
		{
			using (var db = new DBmsw())
			{
				tUser user = db.tUsers.SingleOrDefault(x => x.MemberID == int.Parse(Session["MemberID"] as string));
				tGroup group = db.tGroups.SingleOrDefault(x => x.GroupID == groupid);

				//Person is trying to remove people from group that they should not be able to remove from
				if (group.LeaderID != user.MemberID && group.CoLeaderID != user.MemberID && (_getGroupType() != group.Type && _getGroupType() != -1))
					return false;

				try
				{
					tGroupUser groupUser = db.tGroupUsers.Where(x => x.MemberID == memberid).Where(x => x.GroupID == groupid).FirstOrDefault();
					db.tGroupUsers.DeleteOnSubmit(groupUser);
					db.SubmitChanges();

					Repository.getInstance().removeGroupMemberIDs(groupid);
				}
				catch (Exception e)
				{
					MSWtools._sendException(e);
					return false;
				}
				return true;
			}
		}

		[Authorize(Roles = "StakePres,Stake,Bishopric,Elders Quorum, Relief Society, Activities, FHE," +
									"Stake?, Bishopric?, Elders Quorum?, Relief Society?, Activities?, FHE?")]
		public ActionResult Sent()
		{
			return View();
		}

		public ActionResult NoGroups()
		{
			return View();
		}

		public ActionResult SentFail(bool? noneGroup)
		{
			if (noneGroup == true)
				ViewData["Error"] = "There are no members in any of those groups that recieve notifications";
			else
				ViewData["Error"] = "Please try again. Email support@mysinglesward.com if this problem keeps occuring.";
			return View();
		}

		private List<SelectListItem> _getNameList()
		{
			List<SelectListItem> nameData = new List<SelectListItem>();
			if (User.IsInRole("StakePres") || User.IsInRole("Stake"))
			{

				nameData = Repository.getInstance().SelectListMembers(double.Parse(Session["StakeID"] as string), true);
			}
			else
			{
				nameData = Repository.getInstance().SelectListMembers(double.Parse(Session["WardStakeID"] as string), false);
			}

			nameData = nameData.OrderBy(x => x.Text).ToList();

			return nameData;
		}

		private List<SelectListItem> _getGroupTypeList()
		{
			//Allowed Creation Types
			List<string> GroupText = new List<string>();//{ "Stake", "Ward", "Elders Quorum", "Relief Society", "Activities", "FHE" };
			List<int> GroupValue = new List<int>();

			if (User.IsInRole("StakePres") || User.IsInRole("Stake"))
			{
				GroupText.Add("Stake");
				GroupValue.Add((int)GroupType.STAKE);
			}
			else if (User.IsInRole("Bishopric"))
			{
				GroupText.Add("Ward");
				GroupText.Add("Elders Quorum");
				GroupText.Add("Relief Society");
				GroupText.Add("Activities");
				GroupText.Add("FHE");
				GroupValue.Add((int)GroupType.WARD);
				GroupValue.Add((int)GroupType.ELDERS_QUORUM);
				GroupValue.Add((int)GroupType.RELIEF_SOCIETY);
				GroupValue.Add((int)GroupType.ACTIVITIES);
				GroupValue.Add((int)GroupType.FHE);
			}
			else if (User.IsInRole("Elders Quorum"))
			{
				GroupText.Add("Elders Quorum");
				GroupValue.Add((int)GroupType.ELDERS_QUORUM);
			}
			else if (User.IsInRole("Relief Society"))
			{
				GroupText.Add("Relief Society");
				GroupValue.Add((int)GroupType.RELIEF_SOCIETY);
			}
			else if (User.IsInRole("Activities"))
			{
				GroupText.Add("Activities");
				GroupValue.Add((int)GroupType.ACTIVITIES);
			}
			else if (User.IsInRole("FHE"))
			{
				GroupText.Add("FHE");
				GroupValue.Add((int)GroupType.FHE);
			}


			List<SelectListItem> GroupList = new List<SelectListItem>();

			for (int i = 0; i < GroupText.Count; i++)
			{
				GroupList.Add(new SelectListItem { Text = GroupText[i], Value = GroupValue[i].ToString() });
			}

			return GroupList;
		}

		private int _getGroupType()
		{
			int myType = 0;

			if (User.IsInRole("StakePres") || User.IsInRole("Stake"))
			{
				myType = (int)GroupType.STAKE;
			}
			else if (User.IsInRole("Bishopric"))
			{
				myType = -1;
			}
			else if (User.IsInRole("Elders Quorum"))
			{
				myType = (int)GroupType.ELDERS_QUORUM;
			}
			else if (User.IsInRole("Relief Society"))
			{
				myType = (int)GroupType.RELIEF_SOCIETY;
			}
			else if (User.IsInRole("Activities"))
			{
				myType = (int)GroupType.ACTIVITIES;
			}
			else if (User.IsInRole("FHE"))
			{
				myType = (int)GroupType.FHE;
			}

			return myType;
		}


		private bool _checkGroup(tGroup group)
		{
			if (User.IsInRole("StakePres") || User.IsInRole("Stake") || User.IsInRole("Stake?"))
			{
				if (group.Type != 0 || group.WardStakeID != double.Parse(Session["StakeID"] as string))
					return false;
			}
			else if (User.IsInRole("Bishopric") || User.IsInRole("Bishopric?"))
			{
				if (group.Type == 0 || group.WardStakeID != double.Parse(Session["WardStakeID"] as string))
					return false;
			}
			else if (User.IsInRole("Elders Quorum") || User.IsInRole("Elders Quorum?"))
			{
				if (group.Type != 2 || group.WardStakeID != double.Parse(Session["WardStakeID"] as string))
					return false;
			}
			else if (User.IsInRole("Relief Society") || User.IsInRole("Relief Society?"))
			{
				if (group.Type != 3 || group.WardStakeID != double.Parse(Session["WardStakeID"] as string))
					return false;
			}
			else if (User.IsInRole("Activities") || User.IsInRole("Activities?"))
			{
				if (group.Type != 4 || group.WardStakeID != double.Parse(Session["WardStakeID"] as string))
					return false;
			}
			else if (User.IsInRole("FHE") || User.IsInRole("FHE?"))
			{
				if (group.Type != 5 || group.WardStakeID != double.Parse(Session["WardStakeID"] as string))
					return false;
			}

			return true;
		}


		private void _NewSession()
		{
			try
			{
				MSWUser user = MSWUser.getUser(User.Identity.Name);

				//Checks the WardID and changes the ID to zero if the stake is missing
				//user = MSWtools._checkWardID(user);

				Session["Username"] = user.UserName;
				Session["WardStakeID"] = user.WardStakeID.ToString();
				Session["MemberID"] = user.MemberID.ToString();
				Session["IsBishopric"] = user.IsBishopric.ToString();
			}
			catch
			{
				StakeUser stakeUser = StakeUser.getStakeUser(User.Identity.Name);

				//Checks the WardID and changes the ID to zero if the stake is missing
				stakeUser = MSWtools._checkStakeID(stakeUser);

				Session["Username"] = stakeUser.UserName.ToString();
				Session["StakeID"] = stakeUser.StakeID.ToString();
				Session["MemberID"] = stakeUser.MemberID.ToString();
				Session["IsPresidency"] = stakeUser.IsPresidency.ToString();
				Session["HasPic"] = stakeUser.HasPic.ToString();
			}
		}

		private bool _checkValidUnit()
		{
			if (Session["WardStakeID"] == null)
			{
				if (int.Parse(Session["StakeID"] as string) == 0)
					return false;
			}
			else if (int.Parse(Session["WardStakeID"] as string) == 0)
				return false;

			return true;
		}

		private void _checkAuthorizedUser(tGroup group, MSWUser user)
		{
			//If the group is a stake group than anyone can join
			if (group.Type == (int)GroupType.STAKE)
				return;

			//If the group is any other group then the wardIDs need to match
			if (group.WardStakeID != user.WardStakeID)
				throw new AuthorizationException();
		}

	}
}
