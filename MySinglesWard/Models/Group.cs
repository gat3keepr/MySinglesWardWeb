using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Utilities;

namespace MSW.Models
{
	/// <summary>
	/// Holds information from database used in groups. GROUPS AND NOTIFICATIONS HAVE NOT BEEN OPTIMIZED WITH CACHEING
	/// </summary>
    public class Group
    {
        public int GroupID { get; set; }
        public string Name { get; set; }
        public int TypeID { get; set; }
        public string Type { get; set; }
        public string Leader { get; set; }
        public int? LeaderID { get; set; }
        public string CoLeader { get; set; }
        public int? CoLeaderID { get; set; }
        public List<MemberModel> MemberList { get; set; }
        public bool joined { get; set; }
        public List<string> access { get; set; }
		public double WardID { get; set; }

		public static Group get(int groupID)
		{
			Group group = Cache.Get(Cache.getCacheKey<Group>(groupID)) as Group;

			if (group == null)
			{
                using (var db = new DBmsw())
                {
                    var dboGroup = db.tGroups.SingleOrDefault(x => x.GroupID == groupID);
                    group = new Group(dboGroup);

                    Cache.Set(Cache.getCacheKey<Group>(groupID), group);
                }
			}

			return group;
		}

        private Group(tGroup group)
        {
			using (var db = new DBmsw())
			{
				access = new List<string>();

				GroupID = group.GroupID;
				WardID = group.WardStakeID;
				TypeID = group.Type;
				LeaderID = group.LeaderID;
				CoLeaderID = group.CoLeaderID;
				switch (group.Type)
				{
					case 0:
						Type = "Stake";
						access.Add("Stake");
						access.Add("StakePres");
						access.Add("Stake?");
						break;
					case 1:
						Type = "Ward";
						access.Add("Bishopric");
						access.Add("Bishopric?");
						break;
					case 2:
						Type = "Elders Quorum";
						access.Add("Bishopric");
						access.Add("Elders Quorum");
						access.Add("Elders Quorum?");
						break;
					case 3:
						Type = "Relief Society";
						access.Add("Bishopric");
						access.Add("Relief Society");
						access.Add("Relief Society?");
						break;
					case 4:
						Type = "Activities";
						access.Add("Bishopric");
						access.Add("Activities");
						access.Add("Activities?");
						break;
					case 5:
						Type = "FHE";
						access.Add("Bishopric");
						access.Add("FHE");
						access.Add("FHE?");
						break;
				}

				Name = group.Name;

				if (group.LeaderID != null)
				{
					var leaderID = group.LeaderID;
					var leader = (from user in db.tUsers
								  join survey in db.tSurveyDatas on user.MemberID equals survey.SurveyID
								  where user.MemberID == leaderID
								  select new { user.LastName, survey.PrefName }).SingleOrDefault();
					Leader = Utilities.Cryptography.DecryptString(leader.LastName) + ", " + Utilities.Cryptography.DecryptString(leader.PrefName);
				}
				else
					Leader = "";

				if (group.CoLeaderID != null)
				{
					var coleaderID = group.CoLeaderID;
					var coleader = (from user in db.tUsers
									join survey in db.tSurveyDatas on user.MemberID equals survey.SurveyID
									where user.MemberID == coleaderID
									select new { user.LastName, survey.PrefName }).SingleOrDefault();
					CoLeader = Utilities.Cryptography.DecryptString(coleader.LastName) + ", " + Utilities.Cryptography.DecryptString(coleader.PrefName);
				}
				else
					CoLeader = "";
			}
        }

        public Group(tGroup group, int MemberID)
        {
			using (var db = new DBmsw())
			{
				access = new List<string>();

				GroupID = group.GroupID;
				WardID = group.WardStakeID;
				TypeID = group.Type;
				LeaderID = group.LeaderID;
				CoLeaderID = group.CoLeaderID;
				switch (group.Type)
				{
					case 0:
						Type = "Stake";
						access.Add("Stake");
						access.Add("StakePres");
						access.Add("Stake?");
						break;
					case 1:
						Type = "Ward";
						access.Add("Bishopric");
						access.Add("Bishopric?");
						break;
					case 2:
						Type = "Elders Quorum";
						access.Add("Bishopric");
						access.Add("Elders Quorum");
						access.Add("Elders Quorum?");
						break;
					case 3:
						Type = "Relief Society";
						access.Add("Bishopric");
						access.Add("Relief Society");
						access.Add("Relief Society?");
						break;
					case 4:
						Type = "Activities";
						access.Add("Bishopric");
						access.Add("Activities");
						access.Add("Activities?");
						break;
					case 5:
						Type = "FHE";
						access.Add("Bishopric");
						access.Add("FHE");
						access.Add("FHE?");
						break;
				}

				Name = group.Name;

				if (group.LeaderID != null)
				{
					var leaderID = group.LeaderID;
					var leader = (from user in db.tUsers
								  join survey in db.tSurveyDatas on user.MemberID equals survey.SurveyID
								  where user.MemberID == leaderID
								  select new { user.LastName, survey.PrefName }).SingleOrDefault();
					Leader = Utilities.Cryptography.DecryptString(leader.LastName) + ", " + Utilities.Cryptography.DecryptString(leader.PrefName);
				}
				else
					Leader = "";

				if (group.CoLeaderID != null)
				{
					var coleaderID = group.CoLeaderID;
					var coleader = (from user in db.tUsers
									join survey in db.tSurveyDatas on user.MemberID equals survey.SurveyID
									where user.MemberID == coleaderID
									select new { user.LastName, survey.PrefName }).SingleOrDefault();
					CoLeader = Utilities.Cryptography.DecryptString(coleader.LastName) + ", " + Utilities.Cryptography.DecryptString(coleader.PrefName);
				}
				else
					CoLeader = "";

				//Checks to make sure the user is in the group
				var groupUser = db.tGroupUsers.SingleOrDefault(x => x.MemberID == MemberID && x.GroupID == group.GroupID);

				if (groupUser != null)
					joined = true;
				else
					joined = false;
			}
        }

        public void GenerateMemberList()
        {
            MemberList = new List<MemberModel>();

			//Group may have no members. That is ok
			try
			{
				Repository r = Repository.getInstance();
				MemberList = Cache.GetList(r.GroupMemberIDs(GroupID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
				MemberList = MemberList.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
			}
			catch
			{

			}
        }
    }
}