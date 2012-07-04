using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class OrganizationCoLeader
	{
		public int id { get; set; }
		public int OrgID { get; set; }
		public int CoLeaderID { get; set; }

		public static OrganizationCoLeader get(int callingID)
		{
			OrganizationCoLeader leader = Cache.Get(Cache.getCacheKey<OrganizationCoLeader>(callingID)) as OrganizationCoLeader;

			if (leader == null)
			{
				using (var db = new DBmsw())
				{
					var dboLeader = db.tOrganizationCoLeaders.SingleOrDefault(x => x.CoLeaderID == callingID);
					if (dboLeader == null)
						return null;

					leader = new OrganizationCoLeader(dboLeader);

					Cache.Set(Cache.getCacheKey<OrganizationCoLeader>(callingID), leader);
				}
			}

			return leader;

		}

		public static void create(tOrganizationCoLeader leader)
		{
			Cache.Remove("CoLeaders:" + leader.OrgID);

            using (var db = new DBmsw())
            {
                db.tOrganizationCoLeaders.InsertOnSubmit(leader);
                db.SubmitChanges();
            }
		}

		public static void remove(OrganizationCoLeader leader)
		{
			Cache.Remove(Cache.getCacheKey<OrganizationCoLeader>(leader.CoLeaderID));
			Cache.Remove("CoLeaders:" + leader.OrgID);

            using (var db = new DBmsw())
            {
                db.tOrganizationCoLeaders.DeleteOnSubmit(db.tOrganizationCoLeaders.SingleOrDefault(x => x.id == leader.id));
                db.SubmitChanges();
            }
		}

		private OrganizationCoLeader(tOrganizationCoLeader leader)
		{
			this.id = leader.id;
			OrgID = leader.OrgID;
			CoLeaderID = leader.CoLeaderID;
		}
	}
}