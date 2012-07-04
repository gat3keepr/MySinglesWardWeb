using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Models.Groups;

namespace MSW.Models
{
    public class StakeWardModel
    {
        public double StakeID { get; set; }
        public List<Ward> StakeList { get; set; }
        
        public StakeWardModel(double StakeID){
            this.StakeID = StakeID;
			using (var db = new DBmsw())
			{
				//Get WardStakeIDs for wards in the stake
				var wards = (from p in db.tWardStakes
							 join q in db.tSupportedWards on p.WardStakeID equals q.WardStakeID
							 where p.StakeID == StakeID
							 select new { p.Approved, p.StakeID, q.Ward, q.WardStakeID });

				//Create list of members for each ward
				StakeList = new List<Ward>();
				foreach (var ward in wards)
				{
					if (ward.Approved)
					{
						//Get Name of Ward
						string wardName = ward.Ward + " Ward";

						StakeList.Add(new Ward(wardName, ward.WardStakeID));
					}
				}

				StakeList = StakeList.OrderBy(x => x.WardName).ToList();
			}
        }
    }
}