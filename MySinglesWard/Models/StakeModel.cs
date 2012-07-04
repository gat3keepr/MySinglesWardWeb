using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW;
using MSW.Models.dbo;
using MSW.Utilities;

namespace MSW.Models
{
    public class StakeModel
    {
        public double? StakeID { get { return _StakeID; } }
        private double? _StakeID;
        public string stake { get { return _stake; } }
        private string _stake;
        public int wards { get { return _wards; } }
        private int _wards = 0;
        public int totalMembers { get { return _totalMembers; } }
        private int _totalMembers = 0;
        public int StakeUsers { get { return _stakeUsers; } }
        private int _stakeUsers = 0;
        public List<Ward> NeedApprovals { get { return _NeedApprovals; } }
        private List<Ward> _NeedApprovals;

        public StakeModel(double StakeID)
        {
			Repository r = Repository.getInstance();
            _StakeID = StakeID;
            if (!StakeID.Equals("0"))
            {
                _NeedApprovals = new List<Ward>();
                try
                {
					Stake stake = Stake.get(StakeID);
                    _stake = stake.Location + " " + stake.stake + " Stake";

					List<WardStake> Wards = Cache.GetList(r.getStakeWards(StakeID), x => Cache.getCacheKey<WardStake>(x), y => WardStake.get(y));

					foreach (var ward in Wards)
					{
						_totalMembers += r.WardMembersID(ward.WardID).Count();

						if (!ward.Approved)						
						{
							try
							{
								_NeedApprovals.Add(Ward.get(ward.WardID));
								
							}
							catch
							{
								WardStake.remove(ward);
								_wards--;
							}
						}
						_wards++;
					}
					
                    _stakeUsers = r.getStakeUsers(StakeID).Count();
                }
                catch
                {
                    _stake = "No Stake Created";
                    _totalMembers = 0;
                    _stakeUsers = 0;
                    _wards = 0;
                    _NeedApprovals = new List<Ward>();
                }
            }
            else
            {
                _stake = "No Stake Created";
                _totalMembers = 0;
                _stakeUsers = 0;
                _wards = 0;
                _NeedApprovals = new List<Ward>();
            }
        }

    }
}