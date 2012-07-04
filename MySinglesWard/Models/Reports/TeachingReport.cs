using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Models.dbo;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.Reports
{
    /// <summary>
    /// Model used to give a month by month report organized by districts
    /// </summary>
    public class TeachingReport
    {
        public Organization org { get; set; }
        public List<TeachingMonth> months { get; set; }
        public List<DistrictModel> districts { get; set; }

        //Dictionary<MonthID, Double>
        public Dictionary<int, Double> teachingPercentages { get; set; }

        //Dictionary<CompanionshipModel, Dictionary<MemberID, Dictionary<TeachingMonthID, TeachingVisit>>>
        public Dictionary<CompanionshipModel, Dictionary<int, Dictionary<int, TeachingVisit>>> teachingVisits { get; set; }

        public TeachingReport(int orgID)
        {
            this.org = Organization.get(orgID);
            this.districts = new List<DistrictModel>();
            teachingVisits = new Dictionary<CompanionshipModel, Dictionary<int, Dictionary<int, TeachingVisit>>>();
            teachingPercentages = new Dictionary<int, double>();

            //Get the months for the home teaching report
            months = Cache.GetList(Repository.getInstance().getTeachingMonths(5), x => Cache.getCacheKey<TeachingMonth>(x),
                y => TeachingMonth.get(y));

            TeachingMonth currentMonth = TeachingMonth.get(Repository.getInstance().getCurrentTeachingMonthID());

            //Get districts that belong to this organization
            List<District> districts = Cache.GetList(Repository.getInstance().getDistricts(orgID), x => Cache.getCacheKey<District>(x), y => District.get(y))
                                            .OrderBy(x => x.Title).ToList();

            foreach (var d in districts)
            {
                DistrictModel district = new DistrictModel(d);
                this.districts.Add(district);

                //For Each compaionship in the model, get a list of Teaching Visits for the current month
                foreach (var companionship in district.companionships)
                {
                    Dictionary<int, Dictionary<int, TeachingVisit>> memberVisits = new Dictionary<int, Dictionary<int, TeachingVisit>>();
                    //teachees
                    foreach (int teacheeID in Repository.getInstance().getTeachees(companionship.comp.CompanionshipID))
                    {
                        memberVisits.Add(teacheeID, ReportTeachingModel.generateTeachee(teacheeID, companionship.comp.CompanionshipID, months).ToDictionary(x => x.Key.TeachingMonthID, x => x.Value));
                    }

                    teachingVisits.Add(companionship, memberVisits);

                }
            }

            //Get percentages for each of the teaching months
            foreach (var month in months)
            {
                teachingPercentages.Add(month.TeachingMonthID, Repository.getInstance().getTeachingPercentage(org.OrgID, month.TeachingMonthID));
            }
        }
    }

    /// <summary>
    /// Model used for a member-by-member view of the teaching data
    /// </summary>
    public class MemberTeachingReport
    {
        public Organization org { get; set; }
        public List<TeachingMonth> months { get; set; }
        public List<MemberModel> members { get; set; }

        //Dictionary<MemberID, Dictionary<TeachingMonthID, TeachingVisit>>
        public Dictionary<int, Dictionary<int, KeyValuePair<TeachingVisit, bool>>> memberVisits { get; set; }

        //Dictionary<MemberID, Number of Missed Visits>
        public Dictionary<int, int> consecutiveMissedVisits { get; set; }

        public MemberTeachingReport(int orgID)
        {
            //Get organization
            org = Organization.get(orgID);

            //Get a list of all the companionshipIDs in the organization to use to check member visits
            List<int> companionshipIDs = new List<int>();

            consecutiveMissedVisits = new Dictionary<int,int>();

            List<District> districts = Cache.GetList(Repository.getInstance().getDistricts(orgID), x => Cache.getCacheKey<District>(x), y => District.get(y))
                                            .ToList();
            foreach (var district in districts)
            {
                companionshipIDs.AddRange(Cache.GetList(Repository.getInstance().getCompanionships(district.DistrictID), x => Cache.getCacheKey<Companionship>(x),
                    y => Companionship.get(y)).Select(x => x.CompanionshipID));
            }

            memberVisits = new Dictionary<int, Dictionary<int, KeyValuePair<TeachingVisit, bool>>>();

            //Get all the members in an organization
            members = Cache.GetList(Repository.getInstance().OrganizationMembership(orgID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                                            .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

            //Get the months for the teaching report
            months = Cache.GetList(Repository.getInstance().getTeachingMonths(3), x => Cache.getCacheKey<TeachingMonth>(x),
                y => TeachingMonth.get(y));

            //Get the member visit information
            foreach (var member in members)
            {
                //Dictionary<TeachingMonthID, TeachingVisit>
                Dictionary<int, KeyValuePair<TeachingVisit, bool>> teachingMonths = new Dictionary<int, KeyValuePair<TeachingVisit, bool>>();

                //Get all the teaching visits for a member
                List<TeachingVisit> visits = Cache.GetList(Repository.getInstance().getTaughtRecords(member.user.MemberID), x => Cache.getCacheKey<TeachingVisit>(x),
                y => TeachingVisit.get(y));

                //get visit information for each month
                foreach (var month in months)
                {
                    List<TeachingVisit> monthVisits = visits.Where(x => x.TeachingMonthID == month.TeachingMonthID && x.MemberID == member.user.MemberID).ToList();

                    if (monthVisits.Count() != 0)
                    {
                        bool belongsToOrg = false;
                        TeachingVisit currentVisit = null;
                        foreach (var visit in monthVisits)
                        {
                            //Check to see if the visit belongs to the organization
                            if (companionshipIDs.Contains(visit.CompanionshipID))
                            {
                                belongsToOrg = true;
                                currentVisit = visit;
                            }
                        }

                        //if none of the visits were assigned to the organization so the visit will be assigned to the first visit
                        if (currentVisit == null)
                            currentVisit = monthVisits[0];

                        teachingMonths.Add(month.TeachingMonthID, new KeyValuePair<TeachingVisit, bool>(currentVisit, belongsToOrg));
                    }
                    else
                    {
                        //This member is not assigned a teacher
                        teachingMonths.Add(month.TeachingMonthID, new KeyValuePair<TeachingVisit, bool>(null, false));
                    }
                }

                memberVisits.Add(member.user.MemberID, teachingMonths);

                //Generate the consecutive missed visit information
                int cMissedVisits = 0;
                for (int i = 0; i < months.Count; i++)
                {
                    TeachingVisit visit = teachingMonths[months[i].TeachingMonthID].Key;
                    bool belongsToOrg = teachingMonths[months[i].TeachingMonthID].Value;

                    if (visit == null) //No visit data so the member was not visited
                    {
                        cMissedVisits++;
                    }
                    else if (!visit.wasVisited || !belongsToOrg) //The visit was recorded as not visited or not reported as visited
                    {
                        cMissedVisits++;
                    }
                    else //Member was visited
                        break;                        
                }

                consecutiveMissedVisits.Add(member.user.MemberID, cMissedVisits);
            }
        }
    }
}