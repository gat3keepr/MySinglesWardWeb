using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class PendingRelease
	{
		public int id { get; set; }
		public double WardID { get; set; }
		public int OrgID { get; set; }
		public int CallingID { get; set; }
		public int MemberID { get; set; }
		public DateTime CalledDate { get; set; }
		public DateTime? SustainedDate { get; set; }
		public DateTime? SetApartDate { get; set; }

		public static PendingRelease get(int callingID)
		{
			PendingRelease release = Cache.Get(Cache.getCacheKey<PendingRelease>(callingID)) as PendingRelease;

			if (release == null)
			{
                using (var db = new DBmsw())
                {
                    var dboRelease = db.tPendingReleases.SingleOrDefault(x => x.CallingID == callingID);
                    if (dboRelease == null)
                        return null;
                    release = new PendingRelease(dboRelease);

                    Cache.Set(Cache.getCacheKey<PendingRelease>(callingID), release);
                }
			}

			return release;
		}

		public static void save(PendingRelease release)
		{
			Cache.Remove(Cache.getCacheKey<PendingRelease>(release.CallingID));

			Repository r = Repository.getInstance();
            r.NukeReportKeys(release.CallingID);

            using (var db = new DBmsw())
            {
                var targetRelease = db.tPendingReleases.SingleOrDefault(x => x.CallingID == release.CallingID);

                targetRelease.WardID = release.WardID;
                targetRelease.OrgID = release.OrgID;
                targetRelease.CallingID = release.CallingID;
                targetRelease.MemberID = release.MemberID;
                targetRelease.CalledDate = release.CalledDate;
                targetRelease.SustainedDate = release.SustainedDate;
                targetRelease.SetApartDate = release.SetApartDate;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<PendingRelease>(release.CallingID), release);
            }
		}

		public static PendingRelease create(tPendingRelease newRelease)
		{
            using (var db = new DBmsw())
            {
                db.tPendingReleases.InsertOnSubmit(newRelease);
                db.SubmitChanges();

				Repository.getInstance().NukeReportKeys(newRelease.CallingID);

                PendingRelease release = new PendingRelease(newRelease);
                Cache.Set(Cache.getCacheKey<PendingRelease>(release.CallingID), release);
                return release;
            }
		}

        public static void remove(PendingRelease release)
        {
            Cache.Remove(Cache.getCacheKey<PendingRelease>(release.CallingID));
            Repository.getInstance().NukeReportKeys(release.CallingID);

            using (var db = new DBmsw())
            {
                db.tPendingReleases.DeleteOnSubmit(db.tPendingReleases.SingleOrDefault(x => x.id == release.id));
                db.SubmitChanges();
            }
        }

		private PendingRelease(tPendingRelease release)
		{
			this.id = release.id;
			WardID = release.WardID;
			OrgID = release.OrgID;
			CallingID = release.CallingID;
			MemberID = release.MemberID;
			CalledDate = release.CalledDate;
			SustainedDate = release.CalledDate;
			SetApartDate = release.SetApartDate;
		}
	}
}