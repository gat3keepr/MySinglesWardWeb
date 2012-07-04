using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MSW.Model;
using System.Web.Mvc;
using MSW.Models.dbo;

namespace MSW.Utilities
{
    /// <summary>
    /// Gets objects other than DBOs out of cache. Caching IDs for lists is a common 
    /// use of the repository.
    /// </summary>
    public class Repository
    {
        public static Repository r = null;

        //Follows singleton pattern
        private Repository()
        {
        }

        public static Repository getInstance()
        {
            if (r == null)
            {
                r = new Repository();
            }

            return r;
        }

        #region Users

        //Returns the keys for all members in a ward
        public List<int> WardMembersID(double WardStakeID)
        {
            List<int> wardMembers = Cache.Get("Ward:" + WardStakeID) as List<int>;

            if (wardMembers == null)
            {
                using (var db = new DBmsw())
                {
                    wardMembers = (from user in db.tUsers
                                   join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                   where user.WardStakeID == WardStakeID
                                   where user.IsBishopric == false
                                   select user).Select(x => x.MemberID).ToList();

                    Cache.Set("Ward:" + WardStakeID, wardMembers);
                }
            }

            return wardMembers;
        }

        //Returns the keys for all members in a stake
        public List<int> StakeMembersID(double StakeID)
        {
            List<int> stakeMembers = Cache.Get("Stake:" + StakeID) as List<int>;

            if (stakeMembers == null)
            {
                using (var db = new DBmsw())
                {
                    stakeMembers = (from user in db.tUsers
                                    join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                    join ward in db.tWardStakes on user.WardStakeID equals ward.WardStakeID
                                    where ward.StakeID == StakeID
                                    where user.IsBishopric == false
                                    select user).Select(x => x.MemberID).ToList();

                    Cache.Set("Stake:" + StakeID, stakeMembers);
                }
            }

            return stakeMembers;
        }

        //Returns the keys for all members in a ward
        public List<int> BishopricMembersID(double WardStakeID)
        {
            List<int> bishopricMembers = Cache.Get("Bishopric:" + WardStakeID) as List<int>;

            if (bishopricMembers == null)
            {
                using (var db = new DBmsw())
                {
                    var users = db.tUsers.Where(x => x.WardStakeID == WardStakeID && x.IsBishopric == true);
                    bishopricMembers = new List<int>();
                    foreach (var user in users)
                    {
                        bishopricMembers.Add(user.MemberID);
                    }

                    Cache.Set("Bishopric:" + WardStakeID, bishopricMembers);
                }
            }

            return bishopricMembers;
        }

        //Returns a list of member names for auto complete
        public List<ListItem> WardMemberNames(double WardStakeID)
        {
            Dictionary<int, string> memberNames = Cache.Get("WardMemberNames:" + WardStakeID) as Dictionary<int, string>;

            if (memberNames == null)
            {
                memberNames = new Dictionary<int, string>();
                List<MemberModel> members = Cache.GetList(new Repository().WardMembersID(WardStakeID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
                foreach (var member in members)
                {
                    memberNames.Add(member.user.MemberID, member.memberSurvey.prefName + " " + member.user.LastName);
                }

                Cache.Set("WardMemberNames:" + WardStakeID, memberNames);
            }
            List<ListItem> names = new List<ListItem>();
            foreach (var member in memberNames)
            {
                names.Add(new ListItem() { Value = (member.Value.ToLower()).Replace(" ", "_"), Text = member.Value });
            }

            return names;
        }

        #endregion

        #region Ward

        public List<int> ResidenceIDs(double WardStakeID)
        {
            List<int> residences = Cache.Get("Residences:" + WardStakeID) as List<int>;

            if (residences == null)
            {
                using (var db = new DBmsw())
                {
                    residences = new List<int>();
                    residences.AddRange(db.tResidences.Where(x => x.WardStakeID == WardStakeID).Select(x => x.id));

                    Cache.Set("Residences:" + WardStakeID, residences);
                }
            }

            return residences;

        }

        #endregion

        #region Stake

        public List<double> getStakeWards(double StakeID)
        {
            List<double> stakeWards = Cache.Get("StakeWards:" + StakeID) as List<double>;

            if (stakeWards == null)
            {
                using (var db = new DBmsw())
                {
                    stakeWards = db.tWardStakes.Where(x => x.StakeID == StakeID).Select(x => x.WardStakeID).ToList();

                    Cache.Set("StakeWards:" + StakeID, stakeWards);
                }
            }

            return stakeWards;
        }

        public void removeStakeWards(double StakeID)
        {
            Cache.Remove("StakeWards:" + StakeID);
        }

        public List<int> getStakeUsers(double StakeID)
        {
            List<int> stakeUsers = Cache.Get("StakeUsers:" + StakeID) as List<int>;

            if (stakeUsers == null)
            {
                using (var db = new DBmsw())
                {
                    stakeUsers = (from u in db.tStakeUsers
                                  where u.StakeID == StakeID
                                  select u).Select(x => x.MemberID).ToList();

                    Cache.Set("StakeUsers:" + StakeID, stakeUsers);
                }
            }

            return stakeUsers;

        }

        public void removeStakeUsers(double StakeID)
        {
            Cache.Remove("StakeUsers:" + StakeID);
        }

        #endregion

        #region Callings

        public List<int> WardCallingsIDs(double WardID)
        {
            List<int> wardCallingsIDs = Cache.Get("WardCallingsIDs:" + WardID) as List<int>;

            if (wardCallingsIDs == null)
            {
                using (var db = new DBmsw())
                {
                    wardCallingsIDs = (from calling in db.tCallings
                                       join org in db.tOrganizations on calling.OrgID equals org.OrgID
                                       where org.WardID == WardID
                                       select calling.CallingID).ToList();

                    Cache.Set("WardCallingsIDs:" + WardID, wardCallingsIDs);
                }
            }

            return wardCallingsIDs;
        }

        public List<int> OrganizationIDs(double WardID)
        {
            List<int> orgIDs = Cache.Get("Organizations:" + WardID) as List<int>;

            if (orgIDs == null)
            {
                using (var db = new DBmsw())
                {
                    orgIDs = db.tOrganizations.Where(x => x.WardID == WardID).Select(x => x.OrgID).ToList();

                    Cache.Set("Organizations:" + WardID, orgIDs);
                }
            }

            return orgIDs;
        }

        public List<int> CallingIDs(int OrgID)
        {
            List<int> callingIDs = Cache.Get("Callings:" + OrgID) as List<int>;

            if (callingIDs == null)
            {
                using (var db = new DBmsw())
                {
                    callingIDs = db.tCallings.Where(x => x.OrgID == OrgID).Select(x => x.CallingID).ToList();

                    Cache.Set("Callings:" + OrgID, callingIDs);
                }
            }

            return callingIDs;
        }

        public List<int> CoLeaderIDs(int OrgID)
        {
            List<int> leaderIDs = Cache.Get("CoLeaders:" + OrgID) as List<int>;

            if (leaderIDs == null)
            {
                using (var db = new DBmsw())
                {
                    leaderIDs = db.tOrganizationCoLeaders.Where(x => x.OrgID == OrgID).Select(x => x.CoLeaderID).ToList();

                    Cache.Set("CoLeaders:" + OrgID, leaderIDs);
                }
            }

            return leaderIDs;
        }

        /// <summary>
        /// Returns a list of calling IDs for a member. These calling IDs are callings that are 
        /// publicly ok to display because the member has been sustained to the calling.
        /// </summary>
        public List<int> MemberCallings(int MemberID)
        {
            List<int> memberCallings = Cache.Get("MemberCallings:" + MemberID) as List<int>;

            if (memberCallings == null)
            {
                using (var db = new DBmsw())
                {
                    memberCallings = new List<int>();
                    memberCallings.AddRange(db.tCallings.Where(x => x.MemberID == MemberID && x.Sustained != null).Select(x => x.CallingID).ToList());
                    memberCallings.AddRange(db.tPendingReleases.Where(x => x.MemberID == MemberID).Select(x => x.CallingID).ToList());

                    Cache.Set("MemberCallings:" + MemberID, memberCallings);
                }
            }

            return memberCallings;
        }

        #endregion

        #region Calling Report

        public List<int> Recommended(int OrgID)
        {
            List<int> callings = Cache.Get("Recommended:" + OrgID) as List<int>;

            if (callings == null)
            {
                using (var db = new DBmsw())
                {
                    callings = (from calling in db.tCallings
                                where calling.OrgID == OrgID
                                where calling.Approved == null
                                where calling.MemberID != 0
                                orderby calling.SortID
                                select calling.CallingID).ToList();

                    Cache.Set("Recommended:" + OrgID, callings);
                }
            }

            return callings;
        }

        public List<int> Approved(int OrgID)
        {
            List<int> callings = Cache.Get("Approved:" + OrgID) as List<int>;

            if (callings == null)
            {
                using (var db = new DBmsw())
                {
                    callings = (from calling in db.tCallings
                                where calling.OrgID == OrgID
                                where calling.Approved != null
                                where calling.Called == null
                                where calling.MemberID != 0
                                orderby calling.SortID
                                select calling.CallingID).ToList();

                    Cache.Set("Approved:" + OrgID, callings);
                }
            }

            return callings;
        }

        public List<int> Called(int OrgID)
        {
            List<int> callings = Cache.Get("Called:" + OrgID) as List<int>;

            if (callings == null)
            {
                using (var db = new DBmsw())
                {
                    callings = (from calling in db.tCallings
                                where calling.OrgID == OrgID
                                where calling.Called != null
                                where calling.Sustained == null
                                where calling.MemberID != 0
                                orderby calling.SortID
                                select calling.CallingID).ToList();

                    Cache.Set("Called:" + OrgID, callings);
                }
            }

            return callings;
        }

        public List<int> Sustained(int OrgID)
        {
            List<int> callings = Cache.Get("Sustained:" + OrgID) as List<int>;

            if (callings == null)
            {
                using (var db = new DBmsw())
                {
                    callings = (from calling in db.tCallings
                                where calling.OrgID == OrgID
                                where calling.Sustained != null
                                where calling.SetApart == null
                                where calling.MemberID != 0
                                orderby calling.SortID
                                select calling.CallingID).ToList();

                    Cache.Set("Sustained:" + OrgID, callings);
                }
            }

            return callings;
        }

        public List<int> ITS(int OrgID)
        {
            List<int> callings = Cache.Get("ITS:" + OrgID) as List<int>;

            if (callings == null)
            {
                using (var db = new DBmsw())
                {
                    callings = (from calling in db.tCallings
                                where calling.OrgID == OrgID
                                where calling.ITStake == true
                                orderby calling.SortID
                                select calling.CallingID).ToList();

                    Cache.Set("ITS:" + OrgID, callings);
                }
            }

            return callings;
        }

        public List<int> Releases(int OrgID)
        {
            List<int> callings = Cache.Get("Releases:" + OrgID) as List<int>;

            if (callings == null)
            {
                using (var db = new DBmsw())
                {
                    callings = (from release in db.tPendingReleases
                                join info in db.tCallings on release.CallingID equals info.CallingID
                                where info.OrgID == OrgID
                                orderby info.SortID
                                select release.CallingID).ToList();

                    Cache.Set("Releases:" + OrgID, callings);
                }
            }

            return callings;
        }

        public List<int> WithoutCallings(double WardID)
        {
            List<int> membersList = Cache.Get("WithoutCallings:" + WardID) as List<int>;

            if (membersList == null)
            {
                using (var db = new DBmsw())
                {
                    var members = (from user in db.tUsers
                                   join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                   where user.WardStakeID == WardID
                                   select new { user, p.PrefName });

                    var membersWithCalling = (from user in db.tUsers
                                              join c in db.tCallings on user.MemberID equals c.MemberID
                                              join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                              where user.WardStakeID == WardID
                                              select new { user, p.PrefName });

                    membersList = members.Except(membersWithCalling).Select(x => x.user.MemberID).ToList();

                    Cache.Set("WithoutCallings:" + WardID, membersList);
                }
            }

            return membersList;
        }

        public List<int> NoSurvey(double WardID)
        {
            List<int> membersList = Cache.Get("NoSurvey:" + WardID) as List<int>;

            if (membersList == null)
            {
                using (var db = new DBmsw())
                {
                    var members = (from user in db.tUsers
                                   where user.WardStakeID == WardID
                                   where user.IsBishopric == false
                                   select user);

                    var membersWithSurvey = (from user in db.tUsers
                                             join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                             where user.WardStakeID == WardID
                                             select user);

                    membersList = members.Except(membersWithSurvey).Select(x => x.MemberID).ToList();

                    Cache.Set("NoSurvey:" + WardID, membersList);
                }
            }

            return membersList;
        }

        public void NukeReportKeys(int callingID)
        {
            try
            {
                Calling calling = Calling.get(callingID);
                Organization org = Organization.get(calling.OrgID);

                Cache.Remove("WithoutCallings:" + org.WardID);
                Cache.Remove("NoSurvey:" + org.WardID);

                //Calling Report Page
                Cache.Remove("NumberWithOutCalling:" + org.WardID);
                Cache.Remove("NumberWithCalling:" + org.WardID);
                Cache.Remove("CallingStatus:" + org.WardID);
                Cache.Remove("CallingFilled:" + org.WardID);
                Cache.Remove("SurveyStatus:" + org.WardID);

                //Individual Reports - From Repository
                Cache.Remove("Recommended:" + calling.OrgID);
                Cache.Remove("Approved:" + calling.OrgID);
                Cache.Remove("Called:" + calling.OrgID);
                Cache.Remove("Sustained:" + calling.OrgID);
                Cache.Remove("ITS:" + calling.OrgID);
                Cache.Remove("Releases:" + calling.OrgID);
            }
            catch
            {
                //The calling doesn't exist anymore
            }
        }

        public void NukeReportKeys(Organization org)
        {
            try
            {
                Cache.Remove("WithoutCallings:" + org.WardID);
                Cache.Remove("NoSurvey:" + org.WardID);

                //Calling Report Page
                Cache.Remove("NumberWithOutCalling:" + org.WardID);
                Cache.Remove("NumberWithCalling:" + org.WardID);
                Cache.Remove("CallingStatus:" + org.WardID);
                Cache.Remove("CallingFilled:" + org.WardID);
                Cache.Remove("SurveyStatus:" + org.WardID);

                //Individual Reports - From Repository
                Cache.Remove("Recommended:" + org.OrgID);
                Cache.Remove("Approved:" + org.OrgID);
                Cache.Remove("Called:" + org.OrgID);
                Cache.Remove("Sustained:" + org.OrgID);
                Cache.Remove("ITS:" + org.OrgID);
                Cache.Remove("Releases:" + org.OrgID);
            }
            catch
            {
                //The organization doesnt exist
            }
        }

        public void NukeReportKeys(double WardID)
        {
            Cache.Remove("WithoutCallings:" + WardID);
            Cache.Remove("NoSurvey:" + WardID);

            //Calling Report Page
            Cache.Remove("NumberWithOutCalling:" + WardID);
            Cache.Remove("NumberWithCalling:" + WardID);
            Cache.Remove("CallingStatus:" + WardID);
            Cache.Remove("CallingFilled:" + WardID);
            Cache.Remove("SurveyStatus:" + WardID);
        }

        #endregion

        #region PDFkeys

        public List<int> PrintKeys(double WardStakeID, string AuxType)
        {
            List<int> wardMembers = Cache.Get("Ward-" + AuxType + ":" + WardStakeID) as List<int>;

            if (wardMembers == null)
            {
                using (var db = new DBmsw())
                {
                    IQueryable<tUser> Users;
                    if (AuxType.Equals("sisters"))
                    {
                        Users = (from user in db.tUsers
                                 join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                 where user.WardStakeID == WardStakeID
                                 where user.IsBishopric == false
                                 where p.Gender == false
                                 select user);
                    }
                    else if (AuxType.Equals("elders"))
                    {
                        Users = (from user in db.tUsers
                                 join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                 where user.WardStakeID == WardStakeID
                                 where user.IsBishopric == false
                                 where p.Gender == true
                                 select user);
                    }
                    else
                    {
                        return new Repository().WardMembersID(WardStakeID);
                    }

                    wardMembers = new List<int>();
                    foreach (var user in Users)
                    {
                        wardMembers.Add(user.MemberID);
                    }

                    Cache.Set("Ward-" + AuxType + ":" + WardStakeID, wardMembers);
                }
            }

            return wardMembers;
        }

        #endregion

        #region Groups

        internal IEnumerable<int> GroupMemberIDs(int GroupID)
        {
            List<int> groupMembers = Cache.Get("groupMembers:" + GroupID) as List<int>;

            if (groupMembers == null)
            {
                using (var db = new DBmsw())
                {

                    var members = (from g in db.tGroupUsers
                                   join m in db.tUsers on g.MemberID equals m.MemberID
                                   where g.GroupID == GroupID
                                   where g.Leader == false
                                   select m);
					groupMembers = new List<int>();
                    foreach (tUser member in members)
                    {
                        groupMembers.Add(member.MemberID);
                    }

                    Cache.Set("groupMembers:" + GroupID, groupMembers);
                }
            }

            return groupMembers;
        }

        internal void removeGroupMemberIDs(int GroupID)
        {
            Cache.Remove("groupMembers:" + GroupID);
        }

        //Returns a list of member names for auto complete
        public List<SelectListItem> SelectListMembers(double WardStakeID, bool Stake)
        {
            Dictionary<int, string> memberNames = Cache.Get("SelectList-Stake-" + Stake + ":" + WardStakeID) as Dictionary<int, string>;

            if (memberNames == null)
            {
                memberNames = new Dictionary<int, string>();
                List<MemberModel> members = new List<MemberModel>();
                if (!Stake)
                {
                    members = Cache.GetList(new Repository().WardMembersID(WardStakeID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
                }
                else
                {
                    members = Cache.GetList(new Repository().StakeMembersID(WardStakeID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
                }
                foreach (var member in members)
                {
                    memberNames.Add(member.user.MemberID, member.user.LastName + ", " + member.memberSurvey.prefName);
                }
                Cache.Set("SelectList-Stake-" + Stake + ":" + WardStakeID, memberNames);
            }

            List<SelectListItem> membersList = new List<SelectListItem>();
            foreach (var member in memberNames)
            {
                membersList.Add(new SelectListItem() { Value = member.Key.ToString(), Text = member.Value });
            }
            return membersList;
        }

        #endregion

        #region DropDownSupport

        /****************************
		 *    Stake Selection tools
		 ****************************/

        public List<string> getSelectStakeList()
        {
            List<string> locationList = Cache.Get("SupportedStakes") as List<string>;

            if (locationList == null)
            {
                using (var db = new DBmsw())
                {
                    var locations = db.tSupportedStakes.Where(x => x.StakeID != 0).Select(x => x.Location);

                    locationList = new List<string>();
                    HashSet<string> locationHash = new HashSet<string>();

                    foreach (string location in locations)
                    {
                        if (locationHash.Add(location))
                            locationList.Add(location);
                    }

                    locationList.OrderBy(x => x);

                    Cache.Set("SupportedStakes", locationList);
                }
            }

            return locationList;
        }

        public List<ListItem> getStakesList(string location)
        {
            Dictionary<double, string> stakeList = Cache.Get("StakeSelect:" + location.Replace(" ", "_")) as Dictionary<double, string>;
            List<ListItem> Stakes = new List<ListItem>();

            if (stakeList == null)
            {
                using (var db = new DBmsw())
                {
                    var locations = db.tSupportedStakes.Where(x => x.Location.Equals(location)).OrderBy(x => x.Stake);

                    stakeList = new Dictionary<double, string>();
                    HashSet<string> checkList = new HashSet<string>();
                    foreach (var stake in locations)
                    {
                        if (checkList.Add(stake.Stake))
                        {
                            Stakes.Add(new ListItem() { Value = stake.StakeID.ToString(), Text = stake.Stake.ToString() });
                            stakeList.Add(stake.StakeID, stake.Stake);
                        }
                    }

                    Cache.Set("StakeSelect:" + location.Replace(" ", "_"), stakeList);
                }
            }
            else
            {
                foreach (var stake in stakeList)
                {
                    Stakes.Add(new ListItem() { Value = stake.Key.ToString(), Text = stake.Value.ToString() });
                }
            }

            return Stakes;
        }

        /****************************
         *    Ward Selection tools
         ****************************/

        public List<string> getWardSelectList()
        {
            List<string> locationList = Cache.Get("SupportedWards") as List<string>;

            if (locationList == null)
            {
                using (var db = new DBmsw())
                {
                    var locations = db.tSupportedWards.Where(x => x.WardStakeID != 0).Select(x => x.Location);

                    HashSet<string> locationHash = new HashSet<string>();
                    locationList = new List<string>();
                    foreach (string location in locations)
                    {
                        if (locationHash.Add(location))
                            locationList.Add(location);
                    }
                    locationList.OrderBy(x => x);

                    Cache.Set("SupportedWards", locationList);
                }
            }

            return locationList;
        }

        public List<string> getSelectedStakeList(string location)
        {
            List<string> stakeList = Cache.Get("SupportedStakes:" + location.Replace(" ", "_")) as List<string>;

            if (stakeList == null)
            {
                using (var db = new DBmsw())
                {
                    var stakes = db.tSupportedWards.Where(x => x.Location.Equals(location)).Select(x => x.Stake);

                    HashSet<string> hashStakes = new HashSet<string>();
                    foreach (string stake in stakes)
                    {
                        hashStakes.Add(stake);
                    }

                    stakeList = hashStakes.ToList();
					stakeList.Sort(delegate(string one, string two)
					{
						try
						{
							int a = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(one, "([\\d]+)").Groups[1].Value);
							int b = Convert.ToInt32(System.Text.RegularExpressions.Regex.Match(two, "([\\d]+)").Groups[1].Value);

							return a.CompareTo(b);
						}
						catch
						{
							return one.CompareTo(two);
						}
					});

                    Cache.Set("SupportedStakes:" + location.Replace(" ", "_"), stakeList);
                }
            }

            return stakeList;
        }

        public List<ListItem> getSelectedWardList(string location, string stake)
        {
            Dictionary<double, string> wardList = Cache.Get("SupportedWards:" + location.Replace(" ", "_") + stake.Replace(" ", "_")) as Dictionary<double, string>;
            List<ListItem> Wards = new List<ListItem>();

            if (wardList == null)
            {
                using (var db = new DBmsw())
                {
                    var wards = db.tSupportedWards.Where(x => x.Stake.Equals(stake) && x.Location.Equals(location)).OrderBy(x => x.Ward);

                    wardList = new Dictionary<double, string>();
                    HashSet<string> checkList = new HashSet<string>();

                    wards = wards.OrderBy(x => x.Ward);

                    foreach (var ward in wards)
                    {
                        if (checkList.Add(ward.Ward))
                        {
                            Wards.Add(new ListItem() { Value = ward.WardStakeID.ToString(), Text = ward.Ward.ToString() });
                            wardList.Add(ward.WardStakeID, ward.Ward);
                        }
                    }

                    Cache.Set("SupportedWards:" + location.Replace(" ", "_") + stake.Replace(" ", "_"), wardList);
                }
            }
            else
            {
                foreach (var ward in wardList)
                {
                    Wards.Add(new ListItem() { Value = ward.Key.ToString(), Text = ward.Value.ToString() });
                }
            }

            return Wards;
        }

        public void removeUnitSelectionCache(Ward ward)
        {
            Cache.Remove("SupportedWards");
            Cache.Remove("SupportedStakes:" + ward.Location.Replace(" ", "_"));
            Cache.Remove("SupportedWards:" + ward.Stake.Replace(" ", "_"));
        }

        public void removeUnitSelectionCache(Stake stake)
        {
            Cache.Remove("SupportedStakes");
            Cache.Remove("StakeSelect:" + stake.Location.Replace(" ", "_"));
        }
        #endregion

        #region Organizations

        //Returns a list of memberIDs for an organization
        public List<int> OrganizationMembership(int orgID)
        {
            List<int> orgMembership = Cache.Get("OrganizationMembership:" + orgID) as List<int>;

            if (orgMembership == null)
            {
                using (var db = new DBmsw())
                {
                    //Confirmed Members
					HashSet<int> members = new HashSet<int>();

                    foreach(int id in db.tOrganizationMembers.Where(x => x.OrgID == orgID).Select(x => x.MemberID))
					{
						members.Add(id);
					}

                    //Pending Members
					foreach (int id in db.tOrganizationMembers.Where(x => x.PendingOrgID == orgID).Select(x => x.MemberID))
					{
						members.Add(id);
					}

					orgMembership = members.ToList();
                    Cache.Set("OrganizationMembership:" + orgID, orgMembership);
                }
            }

            return orgMembership;
        }

        public void removeOrganizationMembership(int orgID)
        {
            Cache.Remove("OrganizationMembership:" + orgID);
        }

        #endregion

        #region Home Teaching

        public IEnumerable<int> getDistricts(int OrgID)
        {
            List<int> distrcits = Cache.Get("Districts:" + OrgID) as List<int>;

            if (distrcits == null)
            {
                using (var db = new DBmsw())
                {
                    distrcits = new List<int>();
                    distrcits.AddRange(db.tDistricts.Where(x => x.OrgID == OrgID).Select(x => x.DistrictID).ToList());

                    Cache.Set("Districts:" + OrgID, distrcits);
                }
            }

            return distrcits;
        }

        public void removeDistricts(int OrgID)
        {
            Cache.Remove("Districts:" + OrgID);
        }

        public IEnumerable<int> getCompanionships(int DistrictID)
        {
            List<int> companionships = Cache.Get("Companionships:" + DistrictID) as List<int>;

            if (companionships == null)
            {
                using (var db = new DBmsw())
                {
                    companionships = new List<int>();
                    companionships.AddRange(db.tCompanionships.Where(x => x.DistrictID == DistrictID).Select(x => x.CompanionshipID).ToList());

                    Cache.Set("Companionships:" + DistrictID, companionships);
                }
            }

            return companionships;
        }

        public void removeCompanionships(int DistrictID)
        {
            Cache.Remove("Companionships:" + DistrictID);
        }

        public IEnumerable<int> getTeachers(int CompanionshipID)
        {
            List<int> teachers = Cache.Get("Teachers:" + CompanionshipID) as List<int>;

            if (teachers == null)
            {
                using (var db = new DBmsw())
                {
                    teachers = new List<int>();
                    teachers.AddRange(db.tTeachingAssignments.Where(x => x.CompanionshipID == CompanionshipID).Select(x => x.MemberID).ToList());

                    Cache.Set("Teachers:" + CompanionshipID, teachers);
                }
            }

            return teachers;
        }

        public void removeTeachers(int CompanionshipID)
        {
            Cache.Remove("Teachers:" + CompanionshipID);
        }

        public IEnumerable<int> getTeachees(int CompanionshipID)
        {
            List<int> teachees = Cache.Get("Teachees:" + CompanionshipID) as List<int>;

            if (teachees == null)
            {
                using (var db = new DBmsw())
                {
                    teachees = new List<int>();
                    teachees.AddRange(db.tTeachingAssignments.Where(x => x.HTID == CompanionshipID || x.VTID == CompanionshipID)
                                                                        .Select(x => x.MemberID).ToList());

                    Cache.Set("Teachees:" + CompanionshipID, teachees);
                }
            }

            return teachees;
        }

        public void removeTeachees(int CompanionshipID)
        {
            Cache.Remove("Teachees:" + CompanionshipID);
        }

        internal IEnumerable<int> getOrganizationsToTeach(int orgID)
        {
            List<int> orgs = Cache.Get("OrganizationsToTeach:" + orgID) as List<int>;

            if (orgs == null)
            {
                using (var db = new DBmsw())
                {
                    orgs = new List<int>();
                    orgs.AddRange(db.tTeachingOrganizations.Where(x => x.TeacherID == orgID)
                                                                        .Select(x => x.TeachingOrganizationID).ToList());

                    Cache.Set("OrganizationsToTeach:" + orgID, orgs);
                }
            }

            return orgs;
        }

        public void removeOrganizationsToTeach(int orgID)
        {
            Cache.Remove("OrganizationsToTeach:" + orgID);
        }

        #endregion

        internal List<int> WardNotifications(double WardID)
        {
            List<int> wardNotifications = Cache.Get("WardNotifications:" + WardID) as List<int>;

            if (wardNotifications == null)
            {
                using (var db = new DBmsw())
                {
                    wardNotifications = new List<int>();
                    wardNotifications.AddRange(db.tNotifications.Where(x => x.WardID == WardID).Select(x => x.NotificationID).ToList());

                    Cache.Set("WardNotifications:" + WardID, wardNotifications);
                }
            }

            return wardNotifications;
        }

        /// <summary>
        /// Gets the number of teaching months determined by the program
        /// </summary>
        internal List<int> getTeachingMonths(int numMonths)
        {
            List<int> months = Cache.Get("TeachingMonths") as List<int>;

            //Gets all months out of the database
            if (months == null)
            {
                using (var db = new DBmsw())
                {
                    months = new List<int>();
                    months.AddRange(db.tTeachingMonths.Select(x => x.TeachingMonthID).ToList());

                    Cache.Set("TeachingMonths", months);
                }
            }

            //If number of months is greater than in the list assign that as the number
            if (months.Count() < numMonths)
                numMonths = months.Count();

            //Returns how many months were requested
            List<int> returnMonths = new List<int>();

            for (int i = months.Count() - 1; i > months.Count() - (numMonths + 1); i--)
            {
                returnMonths.Add(months[i]);
            }

            return returnMonths;
        }

        public void removeTeachingMonths()
        {
            Cache.Remove("TeachingMonths");
        }

        /// <summary>
        /// Gets the teaching percentage for a given month for an organization
        /// </summary>
        public double getTeachingPercentage(int orgID, int monthID)
        {
            double? percent = Cache.Get("Teaching-Percentage:" + orgID + "-" + monthID) as double?;

            if (percent == null)
            {
                using (var db = new DBmsw())
                {
                    int visits = 0;
                    int totalVisited = 0;

                    Organization org = Organization.get(orgID);
                    IEnumerable<int> districts = Repository.getInstance().getDistricts(orgID);
                    TeachingMonth month = TeachingMonth.get(monthID);

                    //Calculate percentage for all the districts in the organization
                    foreach (int districtID in districts)
                    {
                        //District Information
                        District district = District.get(districtID);

                        //Get all the companionships for a district
                        IEnumerable<int> companionships = Repository.getInstance().getCompanionships(districtID);

                        foreach (int CompanionshipID in companionships)
                        {
                            //Companionship Information
                            Companionship comp = Companionship.get(CompanionshipID);

                            List<MemberModel> teachees = Cache.GetList(Repository.getInstance().getTeachees(CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                                                        .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

                            foreach (MemberModel member in teachees)
                            {
                                try
                                {
                                    TeachingVisit visit = TeachingVisit.get(monthID, member.user.MemberID, CompanionshipID);

                                    //Increase the amount of members visited
                                    visits++;

                                    //Figure out if the person was visited
                                    if (visit.reported)
                                        if (visit.wasVisited)
                                            totalVisited++;
                                }
                                catch
                                {
                                    //Visit did not happen
                                }
                            }
                        }
                    }

                    if(visits == 0)
                        percent = 0;
                    else
                        percent = (double)totalVisited / (double)visits * 100;
                    Cache.Set("Teaching-Percentage:" + orgID + "-" + monthID, percent);
                }
            }

            return (double)percent;
        }

        public void removeTeachingPercentage(int orgID, int monthID)
        {
            Cache.Remove("Teaching-Percentage:" + orgID + "-" + monthID);
        }

        /// <summary>
        /// Gets the current monthID for TeachingMonth
        /// </summary>
        internal int getCurrentTeachingMonthID()
        {
            return Repository.getInstance().getTeachingMonths(1)[0];
        }

        public List<int> getTaughtRecords(int MemberID)
        {
            List<int> taughtRecords = Cache.Get("TaughtRecords:" + MemberID) as List<int>;

            if (taughtRecords == null)
            {
                using (var db = new DBmsw())
                {
                    taughtRecords = new List<int>();
                    taughtRecords.AddRange(db.tTaughtRecords.Where(x => x.MemberID == MemberID).Select(x => x.TeachingVisitID).ToList());

                    Cache.Set("TaughtRecords:" + MemberID, taughtRecords);
                }
            }

            return taughtRecords;
        }

        public void removeTaughtRecords(int MemberID)
        {
            Cache.Remove("TaughtRecords:" + MemberID);
        }

        public List<int> getTeachingRecords(int MemberID)
        {
            List<int> teachingRecords = Cache.Get("TeachingRecords:" + MemberID) as List<int>;

            if (teachingRecords == null)
            {
                using (var db = new DBmsw())
                {
                    teachingRecords = new List<int>();
                    teachingRecords.AddRange(db.tTeachingRecords.Where(x => x.MemberID == MemberID).Select(x => x.TeachingVisitID).ToList());

                    Cache.Set("TeachingRecords:" + MemberID, teachingRecords);
                }
            }

            return teachingRecords;
        }

        public void removeTeachingRecords(int MemberID)
        {
            Cache.Remove("TeachingRecords:" + MemberID);
        }

		internal void resetOrganization(int orgID)
		{
			//Remove organization districts and companionships and teaching assignments
			IEnumerable<int> districts = Repository.getInstance().getDistricts(orgID);

			foreach (int districtID in districts)
			{
				IEnumerable<int> companionships = Repository.getInstance().getCompanionships(districtID);

				foreach (int compID in companionships)
				{
					//remove Teachers
					List<TeachingAssignment> teachers = Cache.GetList(Repository.getInstance().getTeachers(compID), x => Cache.getCacheKey<TeachingAssignment>(x), y => TeachingAssignment.get(y));

					foreach (var teacher in teachers)
					{
						teacher.CompanionshipID = null;
						TeachingAssignment.save(teacher);
					}

					Repository.getInstance().removeTeachers(compID);

					//remove Teachees
					List<TeachingAssignment> teachees = Cache.GetList(Repository.getInstance().getTeachees(compID), x => Cache.getCacheKey<TeachingAssignment>(x), y => TeachingAssignment.get(y));
					foreach (var teachee in teachees)
					{
						if (teachee.HTID == compID)
							teachee.HTID = null;

						if (teachee.VTID == compID)
							teachee.VTID = null;

						TeachingAssignment.save(teachee);
					}

					Repository.getInstance().removeTeachees(compID);
				}

				//Remove the district leader and disaccociate the district with the organization
				District district = District.get(districtID);

				District.removeDistrictLeader(district);

				district.OrgID = 0;

				District.save(district);
			}

			Repository.getInstance().removeDistricts(orgID);

			//Reset Organization membership
			List<OrganizationMember> memberships = Cache.GetList(Repository.getInstance().OrganizationMembership(orgID), x => Cache.getCacheKey<OrganizationMember>(x), y => OrganizationMember.get(y));

			for (int i = 0; i < memberships.Count; i++)
			{
				OrganizationMember.remove(memberships[i]);
			}

			Repository.getInstance().removeOrganizationMembership(orgID);
		}
	}
}