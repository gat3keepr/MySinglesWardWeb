using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW;
using MSW.Model;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Models
{
	/// <summary>
	/// View of the Bishopric Home page with member and photo approval
	/// </summary>
    public class WardModel
    {
        public double WardStakeID { get { return _WardStakeID; } }
        private double _WardStakeID;
        public string ward { get { return _ward; } }
        private string _ward;
        public int totalMembers { get { return _totalMembers; } }
        private int _totalMembers = 0;
        public int brothers { get { return _brothers; } }
        private int _brothers = 0;
        public int sisters { get { return _sisters; } }
        private int _sisters = 0;
        public int residences { get { return _residences; } }
        private int _residences;
        public List<MemberModel> MemberApprovals { get { return _MemberApprovals; } }
		private List<MemberModel> _MemberApprovals;
        public List<MemberModel> PhotoApprovals { get { return _PhotoApprovals; } }
        private List<MemberModel> _PhotoApprovals;
        public List<Notification> NotificationApprovals { get { return _NotificationApprovals; } }
        private List<Notification> _NotificationApprovals;
		public string stake { get; set; }

        public WardModel(string WardStakeID)
        {
            _WardStakeID = double.Parse(WardStakeID);
            if (!WardStakeID.Equals("0"))
            {
                _MemberApprovals = new List<MemberModel>();
                _PhotoApprovals = new List<MemberModel>();
                _NotificationApprovals = new List<Notification>();
                try
                {
                    //Get Members and figure out what members need to be approved into the ward
					Repository r = Repository.getInstance();
					List<MemberModel> members = Cache.GetList(r.WardMembersID(double.Parse(WardStakeID)), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
					members = members.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
					dbo.Ward ward = dbo.Ward.get(double.Parse(WardStakeID));
                    _ward = ward.Location + " " + ward.Stake + " Stake " + ward.ward + " Ward";
                    _totalMembers = members.Count;
                    foreach (var member in members)
                    {
                        try
                        {
                            if (System.Web.Security.Roles.IsUserInRole(member.user.UserName, "Member?"))
								_MemberApprovals.Add(member);							
                        }
                        catch(Exception e)
                        {
							_MemberApprovals.Add(member);
							MSWUser newUser = MSWUser.getUser(member.user.MemberID);
                            newUser.LastName = " ";
							MSWUser.saveUser(newUser);
                        }

						if (member.memberSurvey.gender == true)
							_brothers++;
						else
							_sisters++;
                    }

                    //Get number of residences in the ward
					_residences = r.ResidenceIDs(double.Parse(WardStakeID)).Count();

                    //Find the stake information if the ward belongs to a stake
					WardStake stakeID = WardStake.get(_WardStakeID);
					if (stakeID != null)
					{
						Stake stake = Stake.get(stakeID.StakeID);
						this.stake = stake.Location + " " + stake.stake + " Stake"; 
					}

                    //Find the Photos that need to be Moderated
                    _PhotoApprovals.AddRange(members.Where(x => x.photo.Status == (int)PhotoStatus.CROPPED && x.photo.NewPhotoFileName != null)); 

                    //Find the Notifcations that need to be moderated
                    _NotificationApprovals = Cache.GetList(r.WardNotifications(double.Parse(WardStakeID)), x => Cache.getCacheKey<Notification>(x), y => Notification.get(y));
                }
                catch
                {
                    _ward = "No Ward Created";
                    _totalMembers = 0;
                    _brothers = 0; 
                    _sisters = 0;
                    _residences = 0;
                    _MemberApprovals = new List<MemberModel>();
                }
            }
            else
            {
                _ward = "No Ward Created";
                _totalMembers = 0;
                _brothers = 0;
                _sisters = 0;
                _residences = 0;
                _MemberApprovals = new List<MemberModel>();
				_PhotoApprovals = new List<MemberModel>();
				_NotificationApprovals = new List<Notification>();
            }
        }
    }
}