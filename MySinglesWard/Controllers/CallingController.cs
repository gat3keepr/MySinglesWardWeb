using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW.Models;
using MSW.Model;
using System.Web.UI.WebControls;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Controllers
{
    [HandleError]
    public class CallingController : Controller
    {
        //
        // GET: /Calling/
        #region Calling Assignment
        [Authorize(Roles = "Bishopric")]
        public ActionResult Index()
        {
            if (Session["Username"] == null)
                _NewSession();

            double WardID = double.Parse(Session["WardStakeID"] as string);
            if (WardID == 0)
                return RedirectToAction("NotInWard", "Home");

            //Name List
            List<SelectListItem> nameData = _getNameList();
            ViewData["NameList"] = nameData;

            return View(new CallingsModel(WardID));
        }

        [Authorize(Roles = "Bishopric")]
        public int AddCalling(int orgID)
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                int largest = 0;
                try
                {
                    largest = db.tCallings.Where(x => x.OrgID == orgID).Max(x => x.SortID);
                }
                catch { }

                tCalling calling = new tCalling();
                calling.OrgID = orgID;
                calling.Title = "";
                calling.SortID = largest + 1;
                calling.ITStake = false;
                db.tCallings.InsertOnSubmit(calling);

                db.SubmitChanges();
                Cache.Remove("Callings:" + orgID);
                Cache.Remove("WardCallingsIDs:" + double.Parse(Session["WardStakeID"] as string));

                Repository r = Repository.getInstance();
                r.NukeReportKeys(calling.CallingID);
                return calling.CallingID;
            }
        }

        [Authorize(Roles = "Bishopric")]
        public bool ReleaseMember(int callingID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Calling calling = Calling.get(callingID);
            Organization org = Organization.get(calling.OrgID);

            if (calling.Called != null)
            {
                //Create Pending Release
                PendingRelease release = PendingRelease.get(callingID);
                if (release != null)
                {
                    release.MemberID = calling.MemberID;
                    release.CalledDate = (DateTime)calling.Called;
                    release.SustainedDate = calling.Sustained;
                    release.SetApartDate = calling.SetApart;
                    PendingRelease.save(release);
                }
                else
                {
                    var newRelease = new tPendingRelease();
                    newRelease.WardID = org.WardID;
                    newRelease.OrgID = org.OrgID;
                    newRelease.CallingID = calling.CallingID;
                    newRelease.MemberID = calling.MemberID;
                    newRelease.CalledDate = (DateTime)calling.Called;
                    newRelease.SustainedDate = calling.Sustained;
                    newRelease.SetApartDate = calling.SetApart;
                    PendingRelease.create(newRelease);
                }

                //Remove member out of current calling
                try
                {
                    calling.MemberID = 0;
                    calling.Approved = null;
                    calling.Called = null;
                    calling.Sustained = null;
                    calling.SetApart = null;
                    Calling.save(calling);
                }
                catch
                {
                    //calling has been deleted
                }
            }
            else
                return false;

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool RemoveCalling(int callingID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Calling calling = Calling.get(callingID);
            Organization org = Organization.get(calling.OrgID);
            OrganizationCoLeader co_leaderCalling = OrganizationCoLeader.get(callingID);

            //Check to see if the calling is a district leader calling. If it is, remove the districtLeaderID
            District district = District.getByDistrictLeaderID(calling.CallingID);
            if (district != null)
            {
                if (district.LeaderAssigned)
                {
                    district.LeaderAssigned = District.removeDistrictLeader(district);
                }
                district.DistrictLeaderID = null;
                District.save(district);
            }

            //Removes Leader from calling role
            if (calling.CallingID == org.LeaderCallingID)
            {
                org.LeaderCallingID = null;
                Organization.save(org);
                if (calling.Sustained != null)
                {
                    MSWUser member = MSWUser.getUser(calling.MemberID);
                    Cache.Remove("MemberCallings:" + calling.MemberID);
                    if (org.ReportID != null)
                        _RemoveUserRole(member, org.ReportID);
                }
            }
            else if (co_leaderCalling != null) //Removes Co-Leader from calling role
            {
                if (calling.Sustained != null)
                {
                    MSWUser member = MSWUser.getUser(calling.MemberID);
                    Cache.Remove("MemberCallings:" + calling.MemberID);
                    if (org.ReportID != null)
                        _RemoveUserRole(member, org.ReportID);
                }
                OrganizationCoLeader.remove(co_leaderCalling);
            }
            Calling.remove(calling);

            //Make sure any pending releases are deleted if the calling is removed
            try
            {
                PendingRelease.remove(PendingRelease.get(calling.CallingID));
            }
            catch
            {
            }

            Cache.Remove("Callings:" + org.OrgID);
            Cache.Remove("WardCallingsIDs:" + double.Parse(Session["WardStakeID"] as string));
            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool UpdateCalling(int id, string title, int? assignment, int status, bool its, string description)
        {
            if (Session["Username"] == null)
                _NewSession();

            Calling calling = Calling.get(id);
            Organization org = Organization.get(calling.OrgID);
            bool co_leaderCalling = OrganizationCoLeader.get(id) != null;

            //Checks if the calling is a leader calling
            bool leaderCalling = (org.LeaderCallingID == id) || co_leaderCalling;


            if (assignment != null)
            {
                //MAKE SURE MEMBER IS NOT AUTHENTICATED FOR REPORT IF NOT SUSTAINED
                if (status < (int)Calling.Status.SUSTAINED && leaderCalling && org.ReportID != null && calling.Sustained != null)
                {
                    //Removes member from calling authentications of member switched and was sustained
                    if (assignment != calling.MemberID)
                    {
                        MSWUser oldMember = MSWUser.getUser(calling.MemberID);
                        Cache.Remove("MemberCallings:" + calling.MemberID);
                        _RemoveUserRole(oldMember, org.ReportID);
                    }
                    else
                    {
                        MSWUser newMember = MSWUser.getUser((int)assignment);
                        Cache.Remove("MemberCallings:" + newMember.MemberID);
                        _RemoveUserRole(newMember, org.ReportID);
                    }
                }

                if (status == (int)Calling.Status.SET_APART)
                {
                    calling.SetApart = DateTime.Today;
                }

                if (status >= (int)Calling.Status.SUSTAINED)
                {
                    if (leaderCalling && org.ReportID != null)
                    {
                        //Removes member from calling authentications of member switched and was sustained
                        if (assignment != calling.MemberID && calling.Sustained != null)
                        {
                            MSWUser oldMember = MSWUser.getUser(calling.MemberID);
                            Cache.Remove("MemberCallings:" + calling.MemberID);
                            _RemoveUserRole(oldMember, org.ReportID);
                        }

                        //Only adds the role if the member was previously not sustained
                        if (calling.Sustained == null)
                        {
                            MSWUser newMember = MSWUser.getUser((int)assignment);
                            Cache.Remove("MemberCallings:" + newMember.MemberID);
                            _AddUserRole(newMember, org.ReportID);
                        }
                    }                    

                    //Check to see if the calling status has changed
                    if (calling.Sustained == null)
                        calling.Sustained = DateTime.Today;
                    if (status == (int)Calling.Status.SUSTAINED)
                    {
                        calling.SetApart = null;
                    }
                }
                if (status >= (int)Calling.Status.CALLED)
                {
                    if (calling.Called == null)
                        calling.Called = DateTime.Today;
                    if (status == (int)Calling.Status.CALLED)
                    {
                        calling.SetApart = null;
                        calling.Sustained = null;
                    }
                }
                if (status >= (int)Calling.Status.APPROVED)
                {
                    if (calling.Approved == null)
                        calling.Approved = DateTime.Today;
                    if (status == (int)Calling.Status.APPROVED)
                    {
                        calling.SetApart = null;
                        calling.Sustained = null;
                        calling.Called = null;

                    }
                }
                if (status == (int)Calling.Status.NONE)
                {
                    calling.SetApart = null;
                    calling.Sustained = null;
                    calling.Called = null;
                    calling.Approved = null;
                }
            }
            else
            {
                calling.SetApart = null;
                calling.Sustained = null;
                calling.Called = null;
                calling.Approved = null;
            }

            //Removes memberCalling list if a member is ever given a new calling
            Cache.Remove("MemberCallings:" + calling.MemberID);
            Cache.Remove("MemberCallings:" + assignment);

            calling.Title = title;
            calling.MemberID = (assignment != null) ? (int)assignment : 0;
            calling.ITStake = its;
            calling.Description = description;
            Calling.save(calling);

            //Check if calling is a district leader calling. If so, assign them to the district leader role
            District district = District.getByDistrictLeaderID(calling.CallingID);
            if (assignment != null && status >= (int)Calling.Status.SUSTAINED && district != null)
            {
                District.assignDistrictLeader(district);
            }

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool UpdateCallingLeader(int orgID, int? callingLeaderID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Organization org = Organization.get(orgID);
            bool co_leaderCalling = false;
            if (callingLeaderID != null)
                co_leaderCalling = OrganizationCoLeader.get((int)callingLeaderID) != null;

            //Return false when a co-leader calling is assigned to be a leader
            if (co_leaderCalling)
                return false;

            //Return true if form got screwed up and someone selected the calling that was assigned as a leader
            if (org.LeaderCallingID == callingLeaderID)
                return true;

            if (callingLeaderID == null)
            {
                if (org.ReportID != null)
                {
                    //Remove leader from roles  
                    var calling = Calling.get((int)org.LeaderCallingID);
                    if (calling.Sustained != null)
                    {
                        var member = MSWUser.getUser(calling.MemberID);
                        _RemoveUserRole(member, org.ReportID);
                    }

                }
                org.LeaderCallingID = callingLeaderID;
                Organization.save(org);

                return true;
            }

            if (org.ReportID != null)
            {
                //Remove old leader from roles
                if (org.LeaderCallingID != null)
                {
                    var oldCalling = Calling.get((int)org.LeaderCallingID);
                    if (oldCalling.Sustained != null)
                    {
                        var member = MSWUser.getUser(oldCalling.MemberID);
                        _RemoveUserRole(member, org.ReportID);
                    }
                }

                //Adding the new calling and assigning roles
                var calling = Calling.get((int)callingLeaderID);
                if (calling.Sustained != null)
                {
                    var member = MSWUser.getUser(calling.MemberID);
                    _AddUserRole(member, org.ReportID);
                }
            }

            org.LeaderCallingID = callingLeaderID;
            Organization.save(org);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool AddCoLeader(int id, int callingLeaderID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Organization org = Organization.get(id);
            OrganizationCoLeader coLeaderTest = OrganizationCoLeader.get(callingLeaderID);

            if (coLeaderTest != null || org.LeaderCallingID == callingLeaderID)
            {
                return false;
            }

            var coLeader = new tOrganizationCoLeader();

            if (org.ReportID != null)
            {
                //Adding the new calling and assigning roles
                var calling = Calling.get(callingLeaderID);
                if (calling.Sustained != null)
                {
                    var member = MSWUser.getUser(calling.MemberID);
                    _AddUserRole(member, org.ReportID);
                }
            }

            coLeader.OrgID = org.OrgID;
            coLeader.CoLeaderID = callingLeaderID;
            OrganizationCoLeader.create(coLeader);
            Cache.Remove("CoLeaders:" + org.OrgID);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool RemoveCoLeader(int callingLeaderID)
        {
            if (Session["Username"] == null)
                _NewSession();

            OrganizationCoLeader coLeader = OrganizationCoLeader.get(callingLeaderID);
            Calling calling = Calling.get(callingLeaderID);
            Organization org = Organization.get(coLeader.OrgID);

            //Remove member from co-leader role when it is deleted
            if (calling.Sustained != null && org.ReportID != null)
            {
                var member = MSWUser.getUser(calling.MemberID);
                _RemoveUserRole(member, org.ReportID);
            }
            OrganizationCoLeader.remove(coLeader);
            Cache.Remove("CoLeaders:" + org.OrgID);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool UpdateReportID(int orgID, string ReportID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Organization org = Organization.get(orgID);

			//If the organization was a Elder's Quorum or a Relief Society and the user switches that, then the organization
			//information needs to be removed.
			if ((org.ReportID == "Elders Quorum" || org.ReportID == "Relief Society") && ReportID != org.ReportID)
			{
				Repository.getInstance().resetOrganization(org.OrgID);
			}

            //Sets ReportID to NULL and removes everyone out of roles if user so desires
            if (ReportID == null)
            {
                //Calling Leader
                if (org.LeaderCallingID != null)
                {
                    var calling = Calling.get((int)org.LeaderCallingID);
                    var member = MSWUser.getUser(calling.MemberID);

                    if (calling.Sustained != null)
                    {
                        _RemoveUserRole(member, org.ReportID);
                    }
                }

                //Calling co-leaders
                var co_leaders = Cache.GetList(Repository.getInstance().CoLeaderIDs(orgID), x => Cache.getCacheKey<OrganizationCoLeader>(x), y => OrganizationCoLeader.get(y));
                try
                {
                    foreach (var leader in co_leaders)
                    {
                        var calling = Calling.get(leader.CoLeaderID);
                        var member = MSWUser.getUser(calling.MemberID);

                        if (calling.Sustained != null)
                        {
                            _RemoveUserRole(member, org.ReportID);
                        }
                    }
                }
                catch (Exception e)
                {
                    MSWtools._sendException(e);
                }

                org.ReportID = ReportID;
                Organization.save(org);

                return true;
            }


            if (org.LeaderCallingID != null)
            {
                var calling = Calling.get((int)org.LeaderCallingID);
                var member = MSWUser.getUser(calling.MemberID);
                var co_leaders = Cache.GetList(Repository.getInstance().CoLeaderIDs(orgID), x => Cache.getCacheKey<OrganizationCoLeader>(x), y => OrganizationCoLeader.get(y));
                var Calling_Members = new Dictionary<int, int>();

                //Remove Leaders out of old roles
                if (org.ReportID != null && calling.Sustained != null)
                    _RemoveUserRole(member, org.ReportID);

                //Remove Calling co-leaders                
                try
                {
                    foreach (var leader in co_leaders)
                    {
                        try
                        {
                            var calling_coLeader = Calling.get(leader.CoLeaderID);
                            var member_coLeader = MSWUser.getUser(calling_coLeader.MemberID);

                            //Adds calling and user to dictionary to be used later to add to roles
                            if (member_coLeader != null)
                                Calling_Members.Add(calling_coLeader.CallingID, member_coLeader.MemberID);

                            if (calling_coLeader.Sustained != null)
                            {
                                _RemoveUserRole(member_coLeader, org.ReportID);
                            }
                        }
                        catch (Exception e)
                        {
                            MSWtools._sendException(e);
                        }
                    }
                }
                catch (Exception e)
                {
                    MSWtools._sendException(e);
                }

                //Add User to roles
                if (calling.Sustained != null)
                {
                    _AddUserRole(member, ReportID);
                }

                //Add Co-leaders to roles
                try
                {
                    foreach (var leader in Calling_Members)
                    {
                        if (Calling.get(leader.Key).Sustained != null)
                        {
                            _AddUserRole(MSWUser.getUser(leader.Value), ReportID);
                        }
                    }
                }
                catch (Exception e)
                {
                    MSWtools._sendException(e);
                }
            }

            org.ReportID = ReportID;
            Organization.save(org);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool RemoveOrganization(int orgID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Organization org = Organization.get(orgID);

            //Remove Membership in the organization. This only happens for elders quorums and relief societies
            Repository r = Repository.getInstance();
            List<OrganizationMember> orgMembers = Cache.GetList(r.OrganizationMembership(org.OrgID), x => Cache.getCacheKey<OrganizationMember>(x), y => OrganizationMember.get(y));

            foreach (var member in orgMembers)
            {
                OrganizationMember.remove(member);
                
                //Remove the member from their teaching assignemnts
                MSWtools.removeMemberFromTeachingAssignment(member.MemberID);
            }

            //Remove Organization Leader
            try
            {
                var orgLeader = Calling.get((int)org.LeaderCallingID);
                if (orgLeader.Sustained != null)
                    _RemoveUserRole(MSWUser.getUser(orgLeader.MemberID), org.ReportID);
            }
            catch { }

            //Remove Co-Leaders
            var co_leaderCallings = Cache.GetList(Repository.getInstance().CoLeaderIDs(orgID), x => Cache.getCacheKey<OrganizationCoLeader>(x), y => OrganizationCoLeader.get(y));
            try
            {
                foreach (var callingID in co_leaderCallings)
                {
                    var calling = Calling.get(callingID.CoLeaderID);
                    if (calling.Sustained != null)
                        _RemoveUserRole(MSWUser.getUser(calling.MemberID), org.ReportID);
                }
            }
            catch { }

            //Removes Organization, all callings, all coleaders, and any members joined to the organization
            foreach (var calling in co_leaderCallings)
            {
                OrganizationCoLeader.remove(calling);
            }

            foreach (var calling in Cache.GetList(Repository.getInstance().CallingIDs(orgID), x => Cache.getCacheKey<Calling>(x), y => Calling.get(y)))
            {
                Cache.Remove("MemberCallings:" + calling.MemberID);
                Calling.remove(calling);
            }

            Cache.Remove("Organizations:" + org.WardID);
            Cache.Remove("WardCallingsIDs:" + org.WardID);

            Organization.remove(org);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool AddOrganization(string orgName, int orgPreset)
        {
            if (Session["Username"] == null)
                _NewSession();

            double WardID = double.Parse(Session["WardStakeID"] as string);
            Ward ward = Ward.get(WardID);
            CallingInitializer callingInitializer = new CallingInitializer();

            switch (orgPreset)
            {
                case (int)SortID.BISHOPRIC:
                    callingInitializer.AddBishopric(ward, orgName);
                    break;
                case (int)SortID.CLERK:
                    callingInitializer.AddClerks(ward, orgName);
                    break;
                case (int)SortID.ELDERS_QUORUM:
                    callingInitializer.AddEldersQuorum(ward, orgName);
                    break;
                case (int)SortID.RELIEF_SOCIETY:
                    callingInitializer.AddReliefSociety(ward, orgName);
                    break;
                case (int)SortID.SUNDAY_SCHOOL:
                    callingInitializer.AddSundaySchool(ward, orgName);
                    break;
                case (int)SortID.HOME_EVENING:
                    callingInitializer.AddHomeEvening(ward, orgName);
                    break;
                case (int)SortID.ACTIVITIES:
                    callingInitializer.AddActivities(ward, orgName);
                    break;
                case (int)SortID.WARD_MISSION:
                    callingInitializer.AddWardMission(ward, orgName);
                    break;
                case (int)SortID.TEMPLE_FAMHIST:
                    callingInitializer.AddTemple_FamHist(ward, orgName);
                    break;
                case (int)SortID.MUSIC:
                    callingInitializer.AddMusic(ward, orgName);
                    break;
                case (int)SortID.PUBLICITY:
                    callingInitializer.AddPublicity(ward, orgName);
                    break;
                case (int)SortID.INSTITUTE:
                    callingInitializer.AddInstitute(ward, orgName);
                    break;
                case (int)SortID.EMERGENCY:
                    callingInitializer.AddEmergency(ward, orgName);
                    break;
                case (int)SortID.EMPLOYMENT:
                    callingInitializer.AddEmployment(ward, orgName);
                    break;
                default:
                    callingInitializer._AddOrganization(ward, orgName, null, (int)SortID.CUSTOM);
                    break;
            }

            Cache.Remove("Organizations:" + WardID);
            Cache.Remove("WardCallingsIDs:" + WardID);
            Repository.getInstance().NukeReportKeys(WardID);
            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool UpdateCallingDescription(int id, string description)
        {
            if (Session["Username"] == null)
                _NewSession();

            Calling calling = Calling.get(id);
            calling.Description = description;
            Calling.save(calling);
            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool SortCallings(int orgID, List<int> result)
        {
            if (Session["Username"] == null)
                _NewSession();

            for (int i = 0; i < result.Count; i++)
            {
                try
                {
                    var calling = Calling.get(result[i]);
                    calling.SortID = i;
                    Calling.save(calling);
                }
                catch
                {
                    //Will catch if calling has been deleted, Application just needs to continue
                }
            }

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool SortOrganizations(List<int> result)
        {
            if (Session["Username"] == null)
                _NewSession();

            for (int i = 0; i < result.Count; i++)
            {
                try
                {
                    var org = Organization.get(result[i]);
                    org.SortID = i;
                    Organization.save(org);
                }
                catch
                {
                    //Will catch if organization has been deleted, Application just needs to continue
                }
            }

            return true;
        }

        #endregion

        #region Reports

        [Authorize(Roles = "Bishopric")]
        public ActionResult Reports()
        {
            if (Session["Username"] == null)
                _NewSession();

            CallingReports.Report report = new CallingReports.Report(double.Parse(Session["WardStakeID"] as string));

            return View(report);
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult Dashboard()
        {
            if (Session["Username"] == null)
                _NewSession();

            CallingReports.Report report = new CallingReports.Report(double.Parse(Session["WardStakeID"] as string));

            return View(report);
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult CallingOverview()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                List<Organization> orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                                                            y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().CallingIDs(org.OrgID);

                    foreach (var callingID in callings)
                    {
                        newOrg.Callings.Add(new CallingModel(callingID));
                    }

					newOrg.Callings = newOrg.Callings.OrderBy(x => x.SortID).ToList();

                    if (newOrg.Callings.Count != 0)
                        orgList.Add(newOrg);
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult WithoutCallings()
        {
            if (Session["Username"] == null)
                _NewSession();

            var membersWithoutCallings = Repository.getInstance().WithoutCallings(double.Parse(Session["WardStakeID"] as string));
            var membersList = new List<ListItem>();
            foreach (var memberID in membersWithoutCallings)
            {
                MemberModel member = MemberModel.get(memberID);
                string name = member.user.LastName + ", " + member.memberSurvey.prefName;
                membersList.Add(new ListItem { Text = name, Value = member.user.MemberID.ToString() });
            }
            membersList = membersList.OrderBy(x => x.Text).ToList();
            ViewData["MembersList"] = membersList;
            return View();
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult Recommended()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                var orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                    y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().Recommended(org.OrgID);

                    if (callings != null)
                    {
                        foreach (var callingID in callings)
                        {
                            newOrg.Callings.Add(new CallingModel(callingID));
                        }

						newOrg.Callings = newOrg.Callings.OrderBy(x => x.SortID).ToList();

                        if (newOrg.Callings.Count != 0)
                            orgList.Add(newOrg);
                    }
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult Approved()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                var orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                    y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().Approved(org.OrgID);

                    foreach (var callingID in callings)
                    {
                        newOrg.Callings.Add(new CallingModel(callingID));
                    }

					newOrg.Callings = newOrg.Callings.OrderBy(x => x.SortID).ToList();

                    if (newOrg.Callings.Count != 0)
                        orgList.Add(newOrg);
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult Called()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                var orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                    y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().Called(org.OrgID);

                    foreach (var callingID in callings)
                    {
                        newOrg.Callings.Add(new CallingModel(callingID));
                    }

					newOrg.Callings = newOrg.Callings.OrderBy(x => x.SortID).ToList();

                    if (newOrg.Callings.Count != 0)
                        orgList.Add(newOrg);
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult Sustained()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                var orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                    y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().Sustained(org.OrgID);

                    foreach (var callingID in callings)
                    {
                        newOrg.Callings.Add(new CallingModel(callingID));
                    }

					newOrg.Callings = newOrg.Callings.OrderBy(x => x.SortID).ToList();

                    if (newOrg.Callings.Count != 0)
                        orgList.Add(newOrg);
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult ITS()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                var orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                    y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().ITS(org.OrgID);

                    foreach (var callingID in callings)
                    {
                        newOrg.Callings.Add(new CallingModel(callingID));
                    }

                    if (newOrg.Callings.Count != 0)
                        orgList.Add(newOrg);
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult Releases()
        {
            if (Session["Username"] == null)
                _NewSession();

            using (var db = new DBmsw())
            {
                var orgs = Cache.GetList(Repository.getInstance().OrganizationIDs(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<Organization>(x),
                    y => Organization.get(y)).OrderBy(x => x.SortID).ToList();

                var orgList = new List<CallingReports.Organization>();

                foreach (var org in orgs)
                {
                    CallingReports.Organization newOrg = new CallingReports.Organization(org.Title);
                    var callings = Repository.getInstance().Releases(org.OrgID);

                    foreach (var callingID in callings)
                    {
                        newOrg.Releases.Add(new ReleaseModel(callingID));
                    }

					newOrg.Releases = newOrg.Releases.OrderBy(x => x.calling.SortID).ToList();

                    if (newOrg.Releases.Count != 0)
                        orgList.Add(newOrg);
                }

                return View(orgList);
            }
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult NoSurvey()
        {
            if (Session["Username"] == null)
                _NewSession();

            var membersWithoutSurvey = Repository.getInstance().NoSurvey(double.Parse(Session["WardStakeID"] as string));
            var membersList = new List<ListItem>();
            foreach (var memberID in membersWithoutSurvey)
            {
                MemberModel member = MemberModel.get(memberID);
                try
                {
                    string name = member.user.LastName + ", " + member.user.FirstName;
                    membersList.Add(new ListItem { Text = name, Value = member.user.MemberID.ToString() });
                }
                catch
                {
                    membersList.Add(new ListItem { Text = member.user.Email, Value = member.user.MemberID.ToString() });
                }
            }
            membersList = membersList.OrderBy(x => x.Text).ToList();
            ViewData["MembersList"] = membersList;
            return View();
        }

        [Authorize(Roles = "Bishopric")]
        public bool ReleaseFinal(int callingID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Calling calling = Calling.get(callingID);
            PendingRelease release = PendingRelease.get(callingID);
            Organization org = Organization.get(calling.OrgID);

            OrganizationCoLeader co_leaderCalling = OrganizationCoLeader.get(callingID);

            //Check to see if the calling is a district leader calling and remove from the District Leader Role
            District district = District.getByDistrictLeaderID(calling.CallingID);
            if (district != null)
            {
                if (district.LeaderAssigned)
                {
                    district.LeaderAssigned = District.releaseDistrictLeader(district);
                    District.save(district);
                }                
            }

            //Removes memberCallingList no matter what happens when a member is released
            Cache.Remove("MemberCallings:" + calling.MemberID);

            try
            {
                MSWUser member = MSWUser.getUser(release.MemberID);

                //Remove from roles if they were sustained and the calling was a leadership calling                
                if (release.SustainedDate != null && (org.LeaderCallingID == callingID || co_leaderCalling != null) && org.ReportID != null)
                {
					try
					{
						_RemoveUserRole(member, org.ReportID);
					}
					catch(Exception e)
					{
						MSWtools._sendException(e, member.UserName);
					}
                }

                //Append Calling to survey data
                MemberSurvey surveyData = MemberSurvey.getMemberSurvey(release.MemberID);
                surveyData.pastCallings = org.Title + " - " + calling.Title + ", " + surveyData.pastCallings;
                MemberSurvey.saveMemberSurvey(surveyData);

                //Delete PendingRelease
                PendingRelease.remove(release);

                MSWtools.NukeStewardshipReports(Session["WardStakeID"] as string);
            }
            catch
            {
                throw new Exception();
            }

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public bool ReleaseRestore(int callingID)
        {
            if (Session["Username"] == null)
                _NewSession();

            Calling calling = Calling.get(callingID);
            PendingRelease release = PendingRelease.get(callingID);
            Organization org = Organization.get(calling.OrgID);

            //Removes memberCallingList no matter what happens when a member is released
            Cache.Remove("MemberCallings:" + calling.MemberID);

            //Returns false if a new member has already been sustained to new calling
            if (calling.Sustained != null)
                return false;

            calling.MemberID = release.MemberID;
            calling.Approved = release.CalledDate;
            calling.Called = release.CalledDate;
            calling.Sustained = release.SustainedDate;
            calling.SetApart = release.SetApartDate;
            Calling.save(calling);

            //Delete PendingRelease
            PendingRelease.remove(release);

            return true;
        }

        [Authorize(Roles = "Bishopric")]
        public ActionResult SpeakingAssignments()
        {
            if (Session["Username"] == null)
                _NewSession();

            //Members
            List<MemberModel> members = Cache.GetList(Repository.getInstance().WardMembersID(double.Parse(Session["WardStakeID"] as string)), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));
            members = members.OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

            return View(members);
        }

        [Authorize(Roles = "Bishopric")]
        public string GaveTalk(int memberID)
        {
            if (Session["Username"] == null)
                _NewSession();

            MemberModel member = MemberModel.get(memberID);

            //Get the last Sunday
            if (member.lastSpoke == null)
            {
                member.lastSpoke = MemberTalk.create(member.user.MemberID, MSWtools.GetFirstDayInWeek(DateTime.Today, DayOfWeek.Sunday));
            }
            else
            {
                member.lastSpoke.LastSpoke = MSWtools.GetFirstDayInWeek(DateTime.Today, DayOfWeek.Sunday);
                MemberTalk.save(member.lastSpoke);
            }

            return member.lastSpoke.LastSpoke.ToShortDateString();
        }

        #endregion

        private List<SelectListItem> _getNameList()
        {
            List<SelectListItem> nameData = new List<SelectListItem>();
            if (User.IsInRole("StakePres") || User.IsInRole("Stake"))
            {

                nameData = Repository.getInstance().SelectListMembers(double.Parse(Session["StakeID"] as string), true);
            }
            else
            {
                nameData = Repository.getInstance().SelectListMembers(double.Parse(Session["WardStakeID"] as string), false);
            }

            nameData = nameData.OrderBy(x => x.Text).ToList();

            return nameData;
        }

        private void _AddUserRole(MSWUser user, string role)
        {
            if (user != null && role != null)
            {
                using (var db = new DBmsw())
                {
                    var roles = db.tMemberRoles.Where(x => x.MemberID == user.MemberID).SingleOrDefault();

                    if (roles == null)
                    {
                        roles = new tMemberRole();
                        roles.MemberID = user.MemberID;
                        switch (role)
                        {
                            case "Activities":
                                roles.Activities++;
                                break;
                            case "Bishopric":
                                roles.Bishopric++;
                                break;
                            case "Clerk":
                                roles.Clerk++;
                                break;
                            case "Elders Quorum":
                                roles.Elders_Quorum++;
                                break;
                            case "Emergency":
                                roles.Emergency++;
                                break;
                            case "Employment":
                                roles.Employment++;
                                break;
                            case "FHE":
                                roles.FHE++;
                                break;
                            case "Institute":
                                roles.Institute++;
                                break;
                            case "Mission":
                                roles.Mission++;
                                break;
                            case "Music":
                                roles.Music++;
                                break;
                            case "Relief Society":
                                roles.Relief_Society++;
                                break;
                            case "Teaching":
                                roles.Teaching++;
                                break;
                            case "Temple/FamHist":
                                roles.Temple++;
                                break;
                        }
                        System.Web.Security.Roles.AddUserToRole(user.UserName, role);
                        db.tMemberRoles.InsertOnSubmit(roles);
                    }
                    else
                    {
                        int roleCount = 0;
                        switch (role)
                        {
                            case "Activities":
                                roles.Activities++;
                                roleCount = roles.Activities;
                                break;
                            case "Bishopric":
                                roles.Bishopric++;
                                roleCount = roles.Bishopric;
                                break;
                            case "Clerk":
                                roles.Clerk++;
                                roleCount = roles.Clerk;
                                break;
                            case "Elders Quorum":
                                roles.Elders_Quorum++;
                                roleCount = roles.Elders_Quorum;
                                break;
                            case "Emergency":
                                roles.Emergency++;
                                roleCount = roles.Emergency;
                                break;
                            case "Employment":
                                roles.Employment++;
                                roleCount = roles.Employment;
                                break;
                            case "FHE":
                                roles.FHE++;
                                roleCount = roles.FHE;
                                break;
                            case "Institute":
                                roles.Institute++;
                                roleCount = roles.Institute;
                                break;
                            case "Mission":
                                roles.Mission++;
                                roleCount = roles.Mission;
                                break;
                            case "Music":
                                roles.Music++;
                                roleCount = roles.Music;
                                break;
                            case "Relief Society":
                                roles.Relief_Society++;
                                roleCount = roles.Relief_Society;
                                break;
                            case "Teaching":
                                roles.Teaching++;
                                roleCount = roles.Teaching;
                                break;
                            case "Temple/FamHist":
                                roles.Temple++;
                                roleCount = roles.Temple;
                                break;
                        }

                        if (roleCount == 1)
                            System.Web.Security.Roles.AddUserToRole(user.UserName, role);
                    }
                    if (System.Web.Security.Roles.IsUserInRole(user.UserName, "Member"))
                        System.Web.Security.Roles.RemoveUserFromRole(user.UserName, "Member");
                    if (System.Web.Security.Roles.IsUserInRole(user.UserName, "Member?"))
                        System.Web.Security.Roles.RemoveUserFromRole(user.UserName, "Member?");
                    db.SubmitChanges();
                }
            }
        }

        private void _RemoveUserRole(MSWUser user, string role)
        {
            if (user != null && role != null)
            {
                using (var db = new DBmsw())
                {
                    var roles = db.tMemberRoles.Where(x => x.MemberID == user.MemberID).SingleOrDefault();

                    int roleCount = 0;
                    switch (role)
                    {
                        case "Activities":
                            roles.Activities--;
                            roleCount = roles.Activities;
                            break;
                        case "Bishopric":
                            roles.Bishopric--;
                            roleCount = roles.Bishopric;
                            break;
                        case "Clerk":
                            roles.Clerk--;
                            roleCount = roles.Clerk;
                            break;
                        case "Elders Quorum":
                            roles.Elders_Quorum--;
                            roleCount = roles.Elders_Quorum;
                            break;
                        case "Emergency":
                            roles.Emergency--;
                            roleCount = roles.Emergency;
                            break;
                        case "Employment":
                            roles.Employment--;
                            roleCount = roles.Employment;
                            break;
                        case "FHE":
                            roles.FHE--;
                            roleCount = roles.FHE;
                            break;
                        case "Institute":
                            roles.Institute--;
                            roleCount = roles.Institute;
                            break;
                        case "Mission":
                            roles.Mission--;
                            roleCount = roles.Mission;
                            break;
                        case "Music":
                            roles.Music--;
                            roleCount = roles.Music;
                            break;
                        case "Relief Society":
                            roles.Relief_Society--;
                            roleCount = roles.Relief_Society;
                            break;
                        case "Teaching":
                            roles.Teaching--;
                            roleCount = roles.Teaching;
                            break;
                        case "Temple/FamHist":
                            roles.Temple--;
                            roleCount = roles.Temple;
                            break;
                        default:
                            return;
                    }

                    if (roleCount == 0)
                        System.Web.Security.Roles.RemoveUserFromRole(user.UserName, role);

                    //Checks to see if all roles are empty
                    if (roles.Activities == 0 && roles.Bishopric == 0 && roles.Clerk == 0 && roles.Elders_Quorum == 0 && roles.Emergency == 0
                        && roles.Employment == 0 && roles.FHE == 0 && roles.FHE == 0 && roles.Institute == 0 && roles.Mission == 0 && roles.Music == 0
                         && roles.Relief_Society == 0 && roles.Teaching == 0 && roles.Temple == 0)
                        System.Web.Security.Roles.AddUserToRole(user.UserName, "Member");

                    db.SubmitChanges();
                }
            }
        }

        private void _NewSession()
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            if (user != null)
            {
                //Checks the WardID and changes the ID to zero if the stake is missing
                //user = MSWtools._checkWardID(user);

                Session["Username"] = user.UserName;
                Session["WardStakeID"] = user.WardStakeID.ToString();
                Session["MemberID"] = user.MemberID.ToString();
                Session["IsBishopric"] = user.IsBishopric.ToString();

				//Get gender information for menu
				try
				{
					Session["isPriesthood"] = MemberSurvey.getMemberSurvey(user.MemberID).gender.ToString();
				}
				catch
				{

				}
            }
            else
            {
                StakeUser stakeUser = StakeUser.getStakeUser(User.Identity.Name);

                //Checks the WardID and changes the ID to zero if the stake is missing
                stakeUser = MSWtools._checkStakeID(stakeUser);

                Session["Username"] = stakeUser.UserName.ToString();
                Session["StakeID"] = stakeUser.StakeID.ToString();
                Session["MemberID"] = stakeUser.MemberID.ToString();
                Session["IsPresidency"] = stakeUser.IsPresidency.ToString();
                Session["HasPic"] = stakeUser.HasPic.ToString();
            }
        }
    }
}
