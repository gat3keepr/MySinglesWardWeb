using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using System.Web.UI.WebControls;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Models
{
    public class StakeListModel
    {
        public double StakeID { get; set; }
        public List<Ward> StakeList { get; set; }
        
        public StakeListModel(double StakeID){
            this.StakeID = StakeID;
			using (var db = new DBmsw())
			{
				//Get WardStakeIDs for wards in the stake
				var wards = db.tWardStakes.Where(x => x.StakeID == StakeID);

				//Create list of members for each ward
				StakeList = new List<Ward>();
				foreach (var ward in wards)
				{
					if (ward.Approved)
						StakeList.Add(Ward.get(ward.WardStakeID));
				}
				StakeList = StakeList.OrderBy(x => x.Location).ThenBy(x => x.Stake).ThenBy(x => x.ward).ToList();
			}
        }        
    }

    public class WardList
    {
        public double WardID { get; set; }
        public string wardName { get; set; }
        public List<BishopricModel> BishopricData { get; set; }
        public List<MemberModel> MembersData { get; set; }
        public List<ListItem> MemberNames { get; set; }
        
        public WardList(double WardStakeID)
        {
            this.WardID = WardStakeID;

            //Get Name of Ward
			Ward ward = Ward.get(WardStakeID);
            wardName = ward.ward + " Ward";

			Repository r = Repository.getInstance();
            //Get Bishoprics
			BishopricData = Cache.GetList(r.BishopricMembersID(WardStakeID), x => Cache.getCacheKey<BishopricModel>(x), y => BishopricModel.get(y));

            //Orders By Calling
            BishopricData = BishopricData.OrderBy(x => x.data.BishopricCalling).ToList();


            //Get Members in the ward
			MembersData = Cache.GetList(r.WardMembersID(WardStakeID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
			MemberNames = r.WardMemberNames(WardStakeID);
            MembersData = MembersData.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
        }
    }

}