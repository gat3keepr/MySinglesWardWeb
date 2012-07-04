using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;
using System.Reflection;

namespace MSW.Models.dbo
{
	[Serializable]
	public class NotificationPreference
	{
		public int MemberID { get; set; }
		public bool txt { get; set; }
		public string carrier { get; set; }
		public bool email { get; set; }
		public bool stake { get; set; }
		public bool ward { get; set; }
		public bool elders { get; set; }
		public bool reliefsociety { get; set; }
		public bool activities { get; set; }
		public bool fhe { get; set; }

		public static NotificationPreference get(int MemberID)
		{
			NotificationPreference pref = Cache.Get(Cache.getCacheKey<NotificationPreference>(MemberID)) as NotificationPreference;

			if (pref == null)
			{
				using (var db = new DBmsw())
				{
					var dboPref = db.tNotificationPreferences.SingleOrDefault(x => x.MemberID == MemberID);

					if (dboPref == null)
						return null;
					pref = new NotificationPreference(dboPref);

					Cache.Set(Cache.getCacheKey<NotificationPreference>(pref.MemberID), pref);
				}				
			}

			return pref;
		}

		public static void save(NotificationPreference pref)
		{
			Cache.Remove(Cache.getCacheKey<NotificationPreference>(pref.MemberID));

			using (var db = new DBmsw())
			{
				var targetPref = db.tNotificationPreferences.SingleOrDefault(x => x.MemberID == pref.MemberID);

				if (targetPref == null)
				{
					targetPref = new tNotificationPreference();
					targetPref.MemberID = pref.MemberID;
					db.tNotificationPreferences.InsertOnSubmit(targetPref);
				}

				targetPref.txt = pref.txt;
				targetPref.carrier = pref.carrier;
				targetPref.email = pref.email;
				targetPref.stake = pref.stake;
				targetPref.ward = pref.ward;
				targetPref.elders = pref.elders;
				targetPref.reliefsociety = pref.reliefsociety;
				targetPref.activities = pref.activities;
				targetPref.fhe = pref.fhe;

				db.SubmitChanges();
				db.ClearCache();
				Cache.Set(Cache.getCacheKey<NotificationPreference>(pref.MemberID), pref);
			}
		}

        public static void create(tNotificationPreference pref)
        {
            using (var db = new DBmsw())
            {
                db.tNotificationPreferences.InsertOnSubmit(pref);
                db.SubmitChanges();

                NotificationPreference np = new NotificationPreference(pref);
                Cache.Set(Cache.getCacheKey<NotificationPreference>(pref.MemberID), np);
            }
        }

		private NotificationPreference(tNotificationPreference pref)
		{			
			this.MemberID = pref.MemberID;

			if (pref != null)
			{				
				txt = pref.txt;
				carrier = pref.carrier;
				email = pref.email;
				stake = pref.stake;
				ward = pref.ward;
				elders = pref.elders;
				reliefsociety = pref.reliefsociety;
				activities = pref.activities;
				fhe = pref.fhe;
			}
		}

		public NotificationPreference()
		{
		}
	}
}