using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MSW.Models.dbo;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models
{
    public class NotificationModel
    {
        public List<CheckBoxListInfo> L_Stake { get; private set; }
        public List<CheckBoxListInfo> L_Ward { get; private set; }
        public List<CheckBoxListInfo> L_EldersQuorum { get; private set; }
        public List<CheckBoxListInfo> L_ReliefSociety { get; private set; }
        public List<CheckBoxListInfo> L_Activities { get; private set; }
        public List<CheckBoxListInfo> L_FHE { get; private set; }
        public List<Organization> organizations;
        public int count { get; set; }

        public NotificationModel(int memberID, string Username)
        {
            L_Stake = new List<CheckBoxListInfo>();
            L_Ward = new List<CheckBoxListInfo>();
            L_EldersQuorum = new List<CheckBoxListInfo>();
            L_ReliefSociety = new List<CheckBoxListInfo>();
            L_Activities = new List<CheckBoxListInfo>();
            L_FHE = new List<CheckBoxListInfo>();

			using (var db = new DBmsw())
			{
				string[] roles = System.Web.Security.Roles.GetRolesForUser(Username);

				if (roles.Contains("StakePres") || roles.Contains("Stake") || roles.Contains("Bishopric") || roles.Contains("Elders Quorum")
					|| roles.Contains("Relief Society") || roles.Contains("Activities") || roles.Contains("FHE"))
				{
					if (roles.Contains("StakePres") || roles.Contains("Stake"))
					{
						var user = db.tStakeUsers.SingleOrDefault(x => x.MemberID == memberID);

						var groups = db.tGroups.Where(x => x.WardStakeID == user.StakeID);

						bool firstGroup = true;
						foreach (var group in groups)
						{
							if (firstGroup)
								L_Stake.Add(new CheckBoxListInfo("STAKE" + user.StakeID.ToString(), "All Stake Groups", false));
							L_Stake.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							firstGroup = false;
						}

					}

					if (roles.Contains("Bishopric"))
					{
						var user = db.tUsers.SingleOrDefault(x => x.MemberID == memberID);

						var groups = db.tGroups.Where(x => x.WardStakeID == user.WardStakeID).Where(x => x.Type == (int)GroupType.WARD);

						bool firstGroup = true;
						foreach (var group in groups)
						{
							if (firstGroup)
								L_Ward.Add(new CheckBoxListInfo("WARD" + user.WardStakeID.ToString(), "All Ward Groups", false));
							L_Ward.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							firstGroup = false;
						}
					}

					if (roles.Contains("Elders Quorum") || roles.Contains("Bishopric"))
					{
						var user = db.tUsers.SingleOrDefault(x => x.MemberID == memberID);

						var groups = db.tGroups.Where(x => x.WardStakeID == user.WardStakeID).Where(x => x.Type == (int)GroupType.ELDERS_QUORUM);

						bool firstGroup = true;
						foreach (var group in groups)
						{
							if (firstGroup)
								L_EldersQuorum.Add(new CheckBoxListInfo("EQ" + user.WardStakeID.ToString(), "All Elders Quorum Groups", false));

							L_EldersQuorum.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							firstGroup = false;
						}
					}

					if (roles.Contains("Relief Society") || roles.Contains("Bishopric"))
					{
						var user = db.tUsers.SingleOrDefault(x => x.MemberID == memberID);

						var groups = db.tGroups.Where(x => x.WardStakeID == user.WardStakeID).Where(x => x.Type == (int)GroupType.RELIEF_SOCIETY);

						bool firstGroup = true;
						foreach (var group in groups)
						{
							if (firstGroup)
								L_ReliefSociety.Add(new CheckBoxListInfo("RS" + user.WardStakeID.ToString(), "All Relief Society Groups", false));
							L_ReliefSociety.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							firstGroup = false;
						}
					}

					if (roles.Contains("Activities") || roles.Contains("Bishopric"))
					{
						var user = db.tUsers.SingleOrDefault(x => x.MemberID == memberID);

						var groups = db.tGroups.Where(x => x.WardStakeID == user.WardStakeID).Where(x => x.Type == (int)GroupType.ACTIVITIES);

						bool firstGroup = true;
						foreach (var group in groups)
						{
							if (firstGroup)
								L_Activities.Add(new CheckBoxListInfo("ACTIVITIES" + user.WardStakeID.ToString(), "All Activities Groups", false));
							L_Activities.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							firstGroup = false;
						}
					}

					if (roles.Contains("FHE") || roles.Contains("Bishopric"))
					{
						var user = db.tUsers.SingleOrDefault(x => x.MemberID == memberID);

						var groups = db.tGroups.Where(x => x.WardStakeID == user.WardStakeID).Where(x => x.Type == (int)GroupType.FHE);

						bool firstGroup = true;
						foreach (var group in groups)
						{
							if (firstGroup)
								L_FHE.Add(new CheckBoxListInfo("FHE" + user.WardStakeID.ToString(), "All FHE Groups", false));
							L_FHE.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							firstGroup = false;
						}
					}
				}

				var groupsLeader = db.tGroups.Where(x => x.LeaderID == memberID);
				var groupsCoLeader = db.tGroups.Where(x => x.CoLeaderID == memberID);
				var allGroups = groupsLeader.Union(groupsCoLeader);

				foreach (var group in allGroups)
				{
					switch (group.Type)
					{
						case 0:
							L_Stake.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							break;
						case 1:
							L_Ward.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							break;
						case 2:
							L_EldersQuorum.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							break;
						case 3:
							L_ReliefSociety.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							break;
						case 4:
							L_Activities.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							break;
						case 5:
							L_FHE.Add(new CheckBoxListInfo(group.GroupID.ToString(), group.Name, false));
							break;

					}
				}


				count = L_Activities.Count + L_EldersQuorum.Count + L_FHE.Count + L_ReliefSociety.Count + L_Stake.Count + L_Ward.Count;

				//If the User is a stake user then the notification creation is done
				if (roles.Contains("StakePres") || roles.Contains("Stake"))
				{
					return;
				}

				//Generate List of Organizations that the current user can send notifications to
				organizations = new List<Organization>();

				Repository r = Repository.getInstance();
				MSWUser member = MSWUser.getUser(Username);

				List<Organization> orgs = Cache.GetList(r.OrganizationIDs(member.WardStakeID), x => Cache.getCacheKey<Organization>(x), y => Organization.get(y));
				if (roles.Contains("Bishopric"))
					orgs = orgs.Where(x => x.ReportID == "Relief Society" || x.ReportID == "Elders Quorum").ToList();
				else if (roles.Contains("EldersQuorum"))
					orgs = orgs.Where(x => x.ReportID == "Elders Quorum").ToList();
				else if (roles.Contains("ReliefSociety"))
					orgs = orgs.Where(x => x.ReportID == "Relief Society").ToList();

				//If the user is a bishopric user, they can send a notifcation to any of the organizations
				if (roles.Contains("Bishopric"))
				{
					organizations = orgs;
				}
				else
				{
					//Check if user is the leader of a calling
					foreach (Organization org in orgs)
					{
						List<int> callingIDs = r.MemberCallings(member.MemberID);
						try
						{
							if (callingIDs.Contains((int)org.LeaderCallingID))
								organizations.Add(org);
							else
							{
								List<int> coleaderIDs = r.CoLeaderIDs(org.OrgID);
								foreach (int id in coleaderIDs)
									if (callingIDs.Contains(id))
										organizations.Add(org);
							}
						}
						catch
						{
							//Do nothing, the leadership calling was null and there is no leader for this organization
						}
					}
				}

				organizations = organizations.OrderBy(x => x.Title).ToList();
			}
        }
    }
}