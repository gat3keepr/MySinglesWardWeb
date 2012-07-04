using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW;
using System.Drawing;
using System.Net.Mail;
using MSW.Models;
using MSW.Models.dbo;
using MSW.Utilities;
using Newtonsoft.Json;

namespace MSW.Model
{
    /// <summary>
    /// Model used to handle all member information
    /// </summary>
    public class MemberModel
    {
		public MSWUser user;
		public MemberSurvey memberSurvey;
		public Photo photo;
		public MSW.Models.dbo.Ward ward;
		public NotificationPreference notificationPreference;
        public List<Group> GroupList { get; private set; }
        public bool updatedName { get; set; }
		public string CurrentWard { get; set; }
		public MemberTalk lastSpoke { get; set; }
        public MemberTeachingModel teaching { get; set; }

        /*-------------------------------------*
         *          Calling Information        *
         *-------------------------------------*/

        public List<MemberCalling> Callings { get; set; }
        public int sortOrgID  { get; set; }
        public int sortCallingID { get; set; }

		public static MemberModel get(string UserName)
		{
            string MemberID = null;
            try
            {
                //Get ID out of cache, users are not cached by Username but my ID
                MemberID = Cache.Get(Cache.getCacheKey<MSWUser>(UserName)) as string;
            }
            catch
            {
                using (var db = new DBmsw())
                {
                    MemberID = db.tUsers.SingleOrDefault(x => x.UserName == UserName).MemberID.ToString();
                }
            }

			if (MemberID == null)
			{
				MSWUser user = MSWUser.getUser(UserName); 
				MemberID = user.MemberID.ToString();

				Cache.Set(Cache.getCacheKey<MSWUser>(UserName), user.MemberID.ToString());
				Cache.Set(Cache.getCacheKey<MSWUser>(user.MemberID), user);
			}

			return get(int.Parse(MemberID));
		}

		public static MemberModel get(int MemberID)
		{
			return new MemberModel(MemberID);
		}


        private MemberModel(int MemberID)
        {
			user = MSWUser.getUser(MemberID);
            updatedName = user.LastName != null;

			try
			{
				photo = Photo.getPhoto(MemberID);
				ward = MSW.Models.dbo.Ward.get(user.WardStakeID);
				memberSurvey = MemberSurvey.getMemberSurvey(MemberID);
				notificationPreference = NotificationPreference.get(MemberID);

				/* Calling Information */
                sortOrgID = 100; //Value to ensure person at the bottom of the list if the member is sorted by calling
                sortCallingID = 100; //Value to ensure person at the bottom of the list if the member is sorted by calling

				List<Calling> callings = Cache.GetList(Repository.getInstance().MemberCallings(MemberID), x => Cache.getCacheKey<Calling>(x), y => Calling.get(y));
				Callings = new List<MemberCalling>();

				foreach (var calling in callings)
				{
                    Organization org = Organization.get(calling.OrgID);
					Callings.Add(new MemberCalling(calling, org));

                    if (Math.Min(sortOrgID, org.SortID) < sortOrgID)
                    {
                        sortOrgID = Math.Min(sortOrgID, org.SortID);
                        if (Math.Min(sortCallingID, calling.SortID) < sortCallingID)
                        {
                            sortCallingID = Math.Min(sortOrgID, calling.SortID);
                        }
                    }
				}

				if (ward.WardStakeID == 0.0)
					CurrentWard = ward.Location;
				else
					CurrentWard = ward.Location + " " + ward.Stake + " Stake " + ward.ward + " Ward";

				/* Last Spoke in Sacrament Meeting */
				lastSpoke = MemberTalk.get(MemberID);

			}
			catch
			{
				List<Calling> callings = new List<Calling>();
			}

            GroupList = new List<Group>();
        }

		/// <summary>
		/// Used to generate the information needed to get group information
		/// </summary>
        public void generateGroupList()
        {
            using (var db = new DBmsw())
            {
                var groups = (from g in db.tGroups
                              join gu in db.tGroupUsers on g.GroupID equals gu.GroupID
                              where gu.MemberID == user.MemberID
                              select g);
                foreach (var group in groups)
                {
                    GroupList.Add(Group.get(group.GroupID));
                }

                if (GroupList.Count > 0)
                    GroupList = GroupList.OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
            }
        }

		/// <summary>
		/// Generates the information used to get teaching information
		/// </summary>
		public void generateTeachingInfo()
		{
			/* Home/Visiting Teaching */
			teaching = new MemberTeachingModel(this.user.MemberID);
		}

        /// <summary>
		/// Generates the json used for leadership information
		/// </summary>
        public string leadershipJSON()
        {
            return "{ \"user\": " + JsonConvert.SerializeObject(user) +
                ", \"memberSurvey\":" + memberSurvey.toLeadershipJSON(ward.WardStakeID) + 
                ", \"photo\":" + JsonConvert.SerializeObject(photo) + "}";
        }

        /// <summary>
        /// Generates the json used for basic member information
        /// </summary>
        public string membershipJSON()
        {
            return "{ \"user\": " + JsonConvert.SerializeObject(user) +
                ", \"memberSurvey\":" + memberSurvey.toMembershipJSON(ward.WardStakeID) +
                ", \"photo\":" + JsonConvert.SerializeObject(photo) + "}";
        }
    }

	public class MemberCalling
	{
		public Calling calling { get; set; }
		public Organization organization { get; set; }

		public MemberCalling(Calling calling, Organization org)
		{
			this.calling = calling;
			this.organization = org;
		}
	}

    /// <summary>
    /// Model used to handle member home/visiting teaching
    /// </summary>
    public class MemberTeachingModel
    {
        public int MemberID { get; set; }
        public TeachingAssignment assignment { get; set; }
		public List<MemberModel> HTers { get; set; }
		public List<MemberModel> VTers { get; set; }

        public MemberTeachingModel(int MemberID)
        {
            this.MemberID = MemberID;

            assignment = TeachingAssignment.get(MemberID);
			OrganizationMember membership = OrganizationMember.get(MemberID);
			HTers = new List<MemberModel>();
			VTers = new List<MemberModel>();

			if (assignment != null)
			{
				//Get Home Teachers
				if (assignment.HTID != null)
				{
					HTers.AddRange(Cache.GetList(Repository.getInstance().getTeachers((int)assignment.HTID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
													.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList());
				}

				//Get Visiting Teachers
				if (assignment.VTID != null)
				{
					VTers.AddRange(Cache.GetList(Repository.getInstance().getTeachers((int)assignment.VTID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
													.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList());
				}
			}
        }
    }
}