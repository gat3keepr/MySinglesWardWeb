using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Models.dbo;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models
{
    public class MSWOrganizationModel
    {
        public MemberModel president { get; set; }
        public double currentTeachingPercentage { get; set; }
        public Organization org { get; set; }
        public List<MemberModel> members { get; set; }
        public List<OrganizationMember> orgMembers { get; set; }
        public List<MemberModel> MemberApprovals { get; set; }
        public List<MemberModel> MembersNotInOrganization { get; set; }
        public Organization teacherOrganization { get; set; }

        public static MSWOrganizationModel get(int orgID)
        {
            return new MSWOrganizationModel(orgID);
        }

        private MSWOrganizationModel(int orgID)
        {
            Repository r = Repository.getInstance();
            members = new List<MemberModel>();
            MemberApprovals = new List<MemberModel>();

            //Calculate stats on Organization
            orgMembers = Cache.GetList(r.OrganizationMembership(orgID), x => Cache.getCacheKey<OrganizationMember>(x), y => OrganizationMember.get(y));

            org = Organization.get(orgID);

            //Check Leader Calling is assigned
            if (org.LeaderCallingID != 0 && org.LeaderCallingID != null)
            {
                Calling leaderCalling = Calling.get((int)org.LeaderCallingID);
                if (leaderCalling != null)
                    if (leaderCalling.MemberID != 0 && leaderCalling.CallingStatus >= (int)Calling.Status.SUSTAINED)
                        president = MemberModel.get((int)leaderCalling.MemberID);
            }

            //Get Member Information for current Membership
            foreach (OrganizationMember member in orgMembers)
            {
                MemberModel model = MemberModel.get(member.MemberID);
                members.Add(model);

                //Add members to approval list if they are pending
                if (member.status == (int)OrganizationMember.Status.PENDING)
                    MemberApprovals.Add(model);
            }

            //Get information about what organization teaches this organization
            TeachingOrganization tO = TeachingOrganization.get(orgID);
            if (tO != null)
            {
                teacherOrganization = Organization.get(tO.TeacherID);
            }

            TeachingMonth month = TeachingMonth.get(Repository.getInstance().getCurrentTeachingMonthID());
            //Get Teaching Percentage
            currentTeachingPercentage = Repository.getInstance().getTeachingPercentage(orgID, month.TeachingMonthID); 
        }

        public void generateMembershipLists()
        {
            Repository r = Repository.getInstance();
            MembersNotInOrganization = new List<MemberModel>();

            //Get a list of members not in a organization in the ward
            List<Organization> orgs = Cache.GetList(r.OrganizationIDs(org.WardID), x => Cache.getCacheKey<Organization>(x), y => Organization.get(y));
            List<int> orgIDs = orgs.Where(x => x.ReportID == org.ReportID).Select(x => x.OrgID).ToList();

            HashSet<int> membersInOrg = new HashSet<int>();

            //Add members of each organization in the ward to the list of members
            foreach (int id in orgIDs)
            {
                List<OrganizationMember> currentOrgMembers = Cache.GetList(r.OrganizationMembership(id),
                                                         x => Cache.getCacheKey<OrganizationMember>(x), y => OrganizationMember.get(y));

                foreach (OrganizationMember m in currentOrgMembers)
                {
                    //Check to see if the user is pending for their first organization
                    if (m.OrgID != 0)
                        membersInOrg.Add(m.MemberID);
                }
            }

            //Members
            List<MemberSurvey> allWardMembers = Cache.GetList(r.WardMembersID(org.WardID), x => Cache.getCacheKey<MemberSurvey>(x), y => MemberSurvey.getMemberSurvey(y));
            List<int> allWardMembersID = allWardMembers.Where(x => x.gender == (org.ReportID == "Elders Quorum")).Select(x => x.memberID).ToList();

            MembersNotInOrganization = Cache.GetList(allWardMembersID.Except(membersInOrg).ToList(),
                                                         x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y)).OrderBy(x => x.user.LastName)
                                                         .ThenBy(x => x.memberSurvey.prefName).ThenBy(x => x.memberSurvey.residence).ToList();
        }
    }
}