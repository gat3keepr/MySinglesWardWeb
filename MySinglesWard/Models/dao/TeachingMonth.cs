using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    public class TeachingMonth
    {
        public int TeachingMonthID { get; set; }
        public DateTime teachingMonth { get; set; }
        public static string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

		public static TeachingMonth get(int TeachingMonthID)
		{
			TeachingMonth month = Cache.Get(Cache.getCacheKey<TeachingMonth>(TeachingMonthID)) as TeachingMonth;

			if (month == null)
			{
				month = new TeachingMonth(TeachingMonthID);
				Cache.Set(Cache.getCacheKey<TeachingMonth>(TeachingMonthID), month);
			}

			return month;
		}

		public static void save(TeachingMonth month)
		{
			Cache.Remove(Cache.getCacheKey<TeachingMonth>(month.TeachingMonthID));

            _NukeCache(month);

			using (var db = new DBmsw())
			{
				var targetMonth = db.tTeachingMonths.SingleOrDefault(x => x.TeachingMonthID == month.TeachingMonthID);
                
                targetMonth.TeachingMonthID = month.TeachingMonthID;
				targetMonth.teachingMonth = month.teachingMonth;                   

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<TeachingMonth>(month.TeachingMonthID), month);
			}
		}

        public static TeachingMonth create(tTeachingMonth month)
        {
            using (var db = new DBmsw())
            {
                db.tTeachingMonths.InsertOnSubmit(month);

                db.SubmitChanges();

                TeachingMonth newMonth = new TeachingMonth(month.TeachingMonthID);
                Cache.Set(Cache.getCacheKey<TeachingMonth>(newMonth.TeachingMonthID), newMonth);

                _NukeCache(newMonth);

                return newMonth;
            }
        }

		public static void remove(TeachingMonth month)
		{
			Cache.Remove(Cache.getCacheKey<TeachingMonth>(month.TeachingMonthID));

            _NukeCache(month);

			using (var db = new DBmsw())
			{
				db.tTeachingMonths.DeleteOnSubmit(db.tTeachingMonths.SingleOrDefault(x => x.TeachingMonthID == month.TeachingMonthID));
				db.SubmitChanges();
			}
		}

        private TeachingMonth(int TeachingMonthID)
		{
			using (var db = new DBmsw())
			{
                var month = db.tTeachingMonths.SingleOrDefault(x => x.TeachingMonthID == TeachingMonthID);

                this.TeachingMonthID = month.TeachingMonthID;
                teachingMonth = month.teachingMonth;
			}
		}

        private static void _NukeCache(TeachingMonth CompanionshipID)
        {
            Repository r = Repository.getInstance();
            r.removeTeachingMonths();
        }
    }
}