using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Models;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.CallingReports
{
	/// <summary>
	/// Helper class used to store basic information for Calling Reports
	/// </summary>
	public class Organization
	{
		public string Title { get; set; }
		public List<CallingModel> Callings { get; set; }
		public List<ReleaseModel> Releases { get; set; }

		public Organization(string Title)
		{
			this.Title = Title;
			Callings = new List<CallingModel>();
			Releases = new List<ReleaseModel>();
		}
	}

	/// <summary>
	/// Collects all the information needed to display dashboard calling charts
	/// </summary>
	public class Report
	{
		public int? membersCalling { get; set; }
		public int? membersWithoutCalling { get; set; }

		public int recommended { get; set; }
		public int approved { get; set; }
		public int called { get; set; }
		public int sustained { get; set; }
		public int setApart { get; set; }

		public int callingsFilled { get; set; }
		public int callingsEmpty { get; set; }

		public int surveyComplete { get; set; }
		public int surveyIncomplete { get; set; }

		public Report(double WardID)
		{
            using (var db = new DBmsw())
            {

                //Chart 1 - Total Members w/ Callings
                membersWithoutCalling = Cache.Get("NumberWithOutCalling:" + WardID) as int?;
                membersCalling = Cache.Get("NumberWithCalling:" + WardID) as int?;
                if (membersWithoutCalling == null || membersCalling == null)
                {
                    var _membersCalling = (from user in db.tUsers
                                           join calling in db.tCallings on user.MemberID equals calling.MemberID
                                           where user.WardStakeID == WardID
                                           select user).Distinct();

                    membersCalling = _membersCalling.Count();
                    membersWithoutCalling = db.tUsers.Where(x => x.WardStakeID == WardID && x.IsBishopric == false).Count() - membersCalling;

                    Cache.Set("NumberWithCalling:" + WardID, membersCalling);
                    Cache.Set("NumberWithOutCalling:" + WardID, membersWithoutCalling);
                }

                //Chart 2 - Calling Status	
				Repository r = Repository.getInstance();
                List<Calling> callings = Cache.GetList(r.WardCallingsIDs(WardID), x => Cache.getCacheKey<Calling>(x), y => Calling.get(y));
                recommended = callings.Where(x => x.Approved == null).Count();
                approved = callings.Where(x => x.Approved != null && x.Called == null).Count();
                called = callings.Where(x => x.Called != null && x.Sustained == null).Count();
                sustained = callings.Where(x => x.Sustained != null && x.SetApart == null).Count();
                setApart = callings.Where(x => x.SetApart != null).Count();


                //Chart 3 - Calling Filled Status
                callingsFilled = callings.Where(x => x.MemberID != 0).Count();
                callingsEmpty = callings.Where(x => x.MemberID == 0).Count();


                //Chart 4 - Survey Status
                Dictionary<string, int> SurveyStatus = Cache.Get("SurveyStatus:" + WardID) as Dictionary<string, int>;
                if (SurveyStatus != null)
                {
                    surveyComplete = SurveyStatus["complete"];
                    surveyIncomplete = SurveyStatus["incomplete"];
                }
                else
                {
                    var members = db.tUsers.Where(x => x.WardStakeID == WardID && x.IsBishopric == false);

                    var membersWithSurvey = (from user in db.tUsers
                                             join p in db.tSurveyDatas on user.MemberID equals p.SurveyID
                                             where user.WardStakeID == WardID
                                             select user);

                    SurveyStatus = new Dictionary<string, int>();
                    SurveyStatus["complete"] = surveyComplete = membersWithSurvey.Count();
                    SurveyStatus["incomplete"] = surveyIncomplete = members.Count() - membersWithSurvey.Count();

                    Cache.Set("SurveyStatus:" + WardID, SurveyStatus);
                }
            }
		}		
	}
}