using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;

namespace MSW.Models.dbo
{
	[Serializable]
	public class Organization
	{
		public int OrgID { get; set; }
		public double WardID { get; set; }
		public int? LeaderCallingID { get; set; }
		public string Title { get; set; }
		public string ReportID { get; set; }
		public int SortID { get; set; }

		public static Organization get(int OrgID)
		{
			Organization org = Cache.Get(Cache.getCacheKey<Organization>(OrgID)) as Organization;

			if (org == null)
			{
				org = new Organization(OrgID);
				Cache.Set(Cache.getCacheKey<Organization>(OrgID), org);
			}

			return org;
		}

		public static void save(Organization org)
		{
			Cache.Remove(Cache.getCacheKey<Organization>(org.OrgID));

			Repository r = Repository.getInstance();
            r.NukeReportKeys(org);

			using (var db = new DBmsw())
			{
				var targetOrg = db.tOrganizations.SingleOrDefault(x => x.OrgID == org.OrgID);

				targetOrg.WardID = org.WardID;
				targetOrg.LeaderCallingID = org.LeaderCallingID;
				targetOrg.Title = org.Title;
				targetOrg.ReportID = org.ReportID;
				targetOrg.SortID = org.SortID;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<Organization>(org.OrgID), org);
			}
		}

		public static void remove(Organization org)
		{
			Cache.Remove(Cache.getCacheKey<Organization>(org.OrgID));
			Cache.Remove("Organizations:" + org.WardID);

			Repository r = Repository.getInstance();
            r.NukeReportKeys(org);

			using (var db = new DBmsw())
			{
				db.tOrganizations.DeleteOnSubmit(db.tOrganizations.SingleOrDefault(x => x.OrgID == org.OrgID));
				db.SubmitChanges();
			}
		}

		private Organization(int OrgID)
		{
			using (var db = new DBmsw())
			{
				var org = db.tOrganizations.SingleOrDefault(x => x.OrgID == OrgID);

				this.OrgID = org.OrgID;
				Title = org.Title;
				LeaderCallingID = org.LeaderCallingID;
				ReportID = org.ReportID;
				SortID = org.SortID;
				WardID = org.WardID;
			}
		}

	}
}