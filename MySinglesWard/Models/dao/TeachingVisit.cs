using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class TeachingVisit
    {
        public int TeachingVisitID { get; set; }
        public int MemberID { get; set; }
        public int CompanionshipID { get; set; }
        public int TeachingMonthID { get; set; }
        public bool wasVisited { get; set; }
        public bool needsAttention { get; set; }
        public bool reported { get; set; }

		public static TeachingVisit get(int TeachingVisitID)
		{
            TeachingVisit tv = Cache.Get(Cache.getCacheKey<TeachingVisit>(TeachingVisitID)) as TeachingVisit;

			if (tv == null)
			{
                tv = new TeachingVisit(TeachingVisitID);
                Cache.Set(Cache.getCacheKey<TeachingVisit>(TeachingVisitID), tv);
			}

			return tv;
		}

        /// <summary>
        /// Gets a teaching visit or creates a new one if the result comes back null
        /// </summary>
        internal static TeachingVisit get(int monthID, int teacheeID, int companionshipID)
        {
            using (var db = new DBmsw())
            {
                try
                {
                    return TeachingVisit.get(db.tTeachingVisits.SingleOrDefault(x => x.MemberID == teacheeID
                    && x.TeachingMonthID == monthID && x.CompanionshipID == companionshipID).TeachingVisitID);
                }
                catch
                {
                    tTeachingVisit tV = new tTeachingVisit();
                    tV.CompanionshipID = companionshipID;
                    tV.MemberID = teacheeID;
                    tV.TeachingMonthID = monthID;

                    TeachingVisit teachingVisit = TeachingVisit.create(tV);
                    return teachingVisit;
                }
            }
        }

		public static void save(TeachingVisit tv)
		{
			Cache.Remove(Cache.getCacheKey<TeachingVisit>(tv.TeachingVisitID));

            _NukeCache(tv);

			using (var db = new DBmsw())
			{
				var targetMR = db.tTeachingVisits.SingleOrDefault(x => x.TeachingVisitID == tv.TeachingVisitID);

                targetMR.TeachingVisitID = tv.TeachingVisitID;
                targetMR.MemberID = tv.MemberID;
                targetMR.CompanionshipID = tv.CompanionshipID;
                targetMR.TeachingMonthID = tv.TeachingMonthID;
                targetMR.wasVisited = tv.wasVisited;
                targetMR.needsAttention = tv.needsAttention;
                targetMR.reported = tv.reported;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<TeachingVisit>(tv.TeachingVisitID), tv);
			}
		}

        public static TeachingVisit create(tTeachingVisit tv)
        {
            using (var db = new DBmsw())
            {
                db.tTeachingVisits.InsertOnSubmit(tv);

                db.SubmitChanges();

                TeachingVisit newTV = new TeachingVisit(tv.TeachingVisitID);
                Cache.Set(Cache.getCacheKey<TeachingVisit>(tv.TeachingVisitID), newTV);

                _NukeCache(newTV);

                return newTV;
            }
        }

		public static void remove(TeachingVisit tv)
		{
			Cache.Remove(Cache.getCacheKey<TeachingVisit>(tv.TeachingVisitID));

            _NukeCache(tv);

			using (var db = new DBmsw())
			{
				db.tTeachingVisits.DeleteOnSubmit(db.tTeachingVisits.SingleOrDefault(x => x.TeachingVisitID == tv.TeachingVisitID));
				db.SubmitChanges();
			}
		}

        private TeachingVisit(int TeachingVisitID)
		{
			using (var db = new DBmsw())
			{
                var tv = db.tTeachingVisits.SingleOrDefault(x => x.TeachingVisitID == TeachingVisitID);

                this.TeachingVisitID = tv.TeachingVisitID;
                MemberID = tv.MemberID;
				CompanionshipID = tv.CompanionshipID;
                TeachingMonthID = tv.TeachingMonthID;
                wasVisited = tv.wasVisited;
                needsAttention = tv.needsAttention;
                reported = tv.reported;
			}
		}

        private static void _NukeCache(TeachingVisit tv)
        {

        }
    }
}