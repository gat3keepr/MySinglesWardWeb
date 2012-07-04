using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Models.dbo;
using MSW.Utilities;

namespace MSW.Models.Reports
{
    public class OrganizationReport
    {
        public Organization org { get; set; }
        public List<TeachingMonth> months { get; set; }

        //Dictionary<MonthID, Double>
        public Dictionary<int, int> teachingPercentages { get; set; }

        public int callingsFilled { get; set; }
        public int callingsEmpty { get; set; }

        public OrganizationReport(int orgID)
        {
            this.org = Organization.get(orgID);

            //get the teaching Line Graph info
            months = Cache.GetList(Repository.getInstance().getTeachingMonths(3), x => Cache.getCacheKey<TeachingMonth>(x),
                y => TeachingMonth.get(y));

            teachingPercentages = new Dictionary<int, int>();

            foreach (var month in months)
            {
                teachingPercentages.Add(month.TeachingMonthID, 
                    (int)Repository.getInstance().getTeachingPercentage(orgID, month.TeachingMonthID));
            }

            //get the calling info
            List<Calling> callings = Cache.GetList(Repository.getInstance().CallingIDs(orgID), x => Cache.getCacheKey<Calling>(x),
                y => Calling.get(y));

            callingsFilled = callings.Where(x => x.MemberID != 0).Count();
            callingsEmpty = callings.Count - callingsFilled;

        }


    }
}