using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Models.dbo;
using MSW.Utilities;
using MSW.Exceptions;

namespace MSW.Models
{
    /// <summary>
    /// Model used to populate the Reporting View
    /// </summary>
    public class ReportTeachingModel
    {
        public bool assigned { get; set; }
        public Organization org { get; set; }
        public List<MemberModel> companions { get; set; }
        public Dictionary<MemberModel, Dictionary<TeachingMonth, TeachingVisit>> teachees { get; set; }
        public List<TeachingMonth> months { get; set; }

        private const int REPORTABLEMONTHS = 2;

        /// <summary>
        /// Get the report based on the member requesting the page
        /// </summary>
        public ReportTeachingModel(int MemberID)
        {
            Repository r = Repository.getInstance();
            companions = new List<MemberModel>();
            teachees = new Dictionary<MemberModel, Dictionary<TeachingMonth, TeachingVisit>>();

            //Get current months for teaching
            months = Cache.GetList(r.getTeachingMonths(REPORTABLEMONTHS), x => Cache.getCacheKey<TeachingMonth>(x), y => TeachingMonth.get(y)).ToList();

            //Get the companions and the teachees for the Member
            try
            {
                org = Organization.get(OrganizationMember.get(MemberID).OrgID);
            }
            catch (NullReferenceException e)
            {
                //This should have been set. If orgID is null then the user needs to be sent to SelectOrganization
                throw new OrganizationNotSetException();
            }
            
            TeachingAssignment ta = TeachingAssignment.get(MemberID);

            //If the member is not assigned to be a teacher than return a model with no information
            if (ta == null)
            {
                assigned = false;
                return;
            }
            else
            {
                assigned = true;

                //Companionship
                if (ta.CompanionshipID != null)
                {
                    companions = Cache.GetList(r.getTeachers((int)ta.CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                                                .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

                    //teachees
                    foreach(int teacheeID in r.getTeachees((int)ta.CompanionshipID))
                    {
                        teachees.Add(MemberModel.get(teacheeID), generateTeachee(teacheeID, (int)ta.CompanionshipID, months));
                    }
                    teachees = teachees.OrderBy(x => x.Key.memberSurvey.gender).ThenBy(x => x.Key.user.LastName)
                        .ThenBy(x => x.Key.memberSurvey.prefName).ToDictionary(x => x.Key, x => x.Value);
                }
            }
        }

        /// <summary>
        /// Get the report based on a companionshipID
        /// </summary>
        public ReportTeachingModel(string CompanionshipID)
        {
            Companionship comp = Companionship.get(int.Parse(CompanionshipID));
            Repository r = Repository.getInstance();
            companions = new List<MemberModel>();
            teachees = new Dictionary<MemberModel, Dictionary<TeachingMonth, TeachingVisit>>();

            //Get current months for teaching
            months = Cache.GetList(r.getTeachingMonths(REPORTABLEMONTHS), x => Cache.getCacheKey<TeachingMonth>(x), y => TeachingMonth.get(y)).ToList();

            District d = District.get(comp.DistrictID);
            org = Organization.get(d.OrgID);

            companions = Cache.GetList(r.getTeachers(comp.CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                                                .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

            //teachees
            foreach (int teacheeID in r.getTeachees(comp.CompanionshipID))
            {
                teachees.Add(MemberModel.get(teacheeID), generateTeachee(teacheeID, comp.CompanionshipID, months));
            }
            teachees = teachees.OrderBy(x => x.Key.memberSurvey.gender).ThenBy(x => x.Key.user.LastName)
                .ThenBy(x => x.Key.memberSurvey.prefName).ToDictionary(x => x.Key, x => x.Value);

            //assigned is true by default because this is based on a companionship, not member
            assigned = true;
        }

        /// <summary>
        /// Generates the teaching information for a teachee
        /// </summary>
        public static Dictionary<TeachingMonth, TeachingVisit> generateTeachee(int teacheeID, int companionshipID, IEnumerable<TeachingMonth> months)
        {
            Dictionary<TeachingMonth, TeachingVisit> record = new Dictionary<TeachingMonth, TeachingVisit>();

            foreach (var month in months)
            {
                record.Add(month, TeachingVisit.get(month.TeachingMonthID, teacheeID, companionshipID));
            }

            return record;
        }
    }
}