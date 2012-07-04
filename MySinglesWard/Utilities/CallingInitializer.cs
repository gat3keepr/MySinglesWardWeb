using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using System.Web.Security;
using MSW.Models.dbo;

namespace MSW.Utilities
{
	/// <summary>
	/// Initial Sort Order for a cookie-cutter ward
	/// </summary>
    public enum SortID { BISHOPRIC, CLERK, ELDERS_QUORUM, RELIEF_SOCIETY, SUNDAY_SCHOOL, HOME_EVENING, ACTIVITIES, WARD_MISSION, TEMPLE_FAMHIST, MUSIC, PUBLICITY, INSTITUTE, EMPLOYMENT, EMERGENCY, CUSTOM }

	/// <summary>
	/// Used when a new ward is created. Creates the ward with the defualt callings and organizations
	/// </summary>
    public class CallingInitializer
    {        

        public CallingInitializer()
        {

        }

		/// <summary>
		/// Called by ward creation process to create cookie-cutter callings and organizations
		/// </summary>
		public void InitializeWard(Ward ward)
        {
            //Reset Ward
            ResetAuthentications(ward);
            RemoveOldOrganizations_Callings(ward);

            //Add New Callins
            AddBishopric(ward);
            AddClerks(ward);
            AddEldersQuorum(ward);
            AddReliefSociety(ward);
            AddSundaySchool(ward);
            AddActivities(ward);
            AddEmergency(ward);
            AddEmployment(ward);
            AddHomeEvening(ward);
            AddInstitute(ward);
            AddMusic(ward);
            AddPublicity(ward);
            AddTemple_FamHist(ward);
            AddWardMission(ward);
        }

		/// <summary>
		/// used to ensure ward members all have basic rights. Important only to initial calling rollout
		/// </summary>
		private void ResetAuthentications(Ward ward)
        {
			using (var db = new DBmsw())
			{
				var users = db.tUsers.Where(x => x.WardStakeID == ward.WardStakeID).Where(x => x.IsBishopric == false);
				foreach (var user in users)
				{
					List<string> roles = Roles.GetRolesForUser(user.UserName).ToList();
					List<string> removeRoles = new List<string>();

					//Makes sure they are not removed from Notification subroles
					foreach (string role in roles)
					{
						if (role.Contains("?") || role.Contains("Member") || role.Contains("Global"))
						{
							removeRoles.Add(role);
						}
					}
					foreach (var role in removeRoles)
					{
						roles.Remove(role);
					}

					try
					{
						if (roles.Count != 0)
						{
							Roles.RemoveUserFromRoles(user.UserName, roles.ToArray());
							roles = Roles.GetRolesForUser(user.UserName).ToList();

							if (!roles.Contains("Member?") && !roles.Contains("Member"))
								Roles.AddUserToRole(user.UserName, "Member");
						}
					}
					catch
					{
					}
				}
			}
        }

		/// <summary>
		/// used to ensure ward members all have basic rights. Important only to initial calling rollout
		/// </summary>
		private void RemoveOldOrganizations_Callings(Ward ward)
        {
			using (var db = new DBmsw())
			{
				var orgs = db.tOrganizations.Where(x => x.WardID == ward.WardStakeID);
				List<tCalling> callingList = new List<tCalling>();

				foreach (var org in orgs)
				{
					var callings = db.tCallings.Where(x => x.OrgID == org.OrgID);
					foreach (var calling in callings)
					{
						callingList.Add(calling);
					}
				}

				db.tOrganizations.DeleteAllOnSubmit(orgs);
				db.tCallings.DeleteAllOnSubmit(callingList);
				db.SubmitChanges();
			}
        }

        private int _AddCalling(int OrgID, string Title, bool ITstake, int SortID)
        {
			using (var db = new DBmsw())
			{
				tCalling calling = new tCalling();
				calling.OrgID = OrgID;
				calling.Title = Title;
				calling.ITStake = ITstake;
				calling.SortID = SortID;

				db.tCallings.InsertOnSubmit(calling);
				db.SubmitChanges();

				return calling.CallingID;
			}
        }

		public tOrganization _AddOrganization(Ward ward, string Title, string ReportID, int SortID)
        {
			using (var db = new DBmsw())
			{
				tOrganization org = new tOrganization();
				org.Title = Title;
				org.ReportID = ReportID;
				org.WardID = ward.WardStakeID;
				org.SortID = SortID;

				db.tOrganizations.InsertOnSubmit(org);
				db.SubmitChanges();

				return org;
			}
        }

        public void AddBishopric(Ward ward, string title = "Bishopric")
        {
            int sortID = 0;
            tOrganization BishopricOrg = _AddOrganization(ward, title, "Bishopric", (int)SortID.BISHOPRIC);
            
            //Bishopric Callings
            int leaderCallingID = _AddCalling(BishopricOrg.OrgID, "Executive Secretary", true, ++sortID);

            //Assign Leader
            BishopricOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(BishopricOrg.OrgID, "Assistant Executive Secretary", true, ++sortID);            
        }

		public void AddClerks(Ward ward, string title = "Clerk")
        {
            int sortID = 0;
            tOrganization clerkOrg = _AddOrganization(ward, title, "Clerk", (int)SortID.CLERK);

            //Bishopric Callings
            int leaderCallingID = _AddCalling(clerkOrg.OrgID, "Assistant Ward Membership Clerk", false, ++sortID);

            //Assign Leader
            clerkOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(clerkOrg.OrgID, "Assistant Ward Financial Clerk", true, ++sortID);
        }

		public void AddEldersQuorum(Ward ward, string title = "Elders Quorum")
        {
            //Elders Quorum Callings
            int sortID = 0;
            tOrganization EQOrg = _AddOrganization(ward, title, "Elders Quorum", (int)SortID.ELDERS_QUORUM);
            int leaderCallingID = _AddCalling(EQOrg.OrgID, "President", true, ++sortID);

            //Assign Leader
            EQOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(EQOrg.OrgID, "1st Counselor", true, ++sortID);
            _AddCoLeaderCalling(EQOrg.OrgID, "2nd Counselor", true, ++sortID);
            _AddCoLeaderCalling(EQOrg.OrgID, "Secretary", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "Activities Leader", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "Activities Committee", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "HT District Supervisor", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "HT District Supervisor", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "HT District Supervisor", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "Redeem The Dead Chair", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "Proclaim The Gospel Chair", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "Perfect The Saints Chair", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "Care for Poor & Needy Chair", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "Instructor", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "Instructor", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "Instructor", true, ++sortID);
            _AddCalling(EQOrg.OrgID, "Sacrament Prep. Coordinator", false, ++sortID);
            _AddCalling(EQOrg.OrgID, "Meeting Coordinator", false, ++sortID);

        }

		public void AddReliefSociety(Ward ward, string title = "Relief Society")
        {
            int sortID = 0;
            tOrganization RSOrg = _AddOrganization(ward, title, "Relief Society", (int)SortID.RELIEF_SOCIETY);
            int leaderCallingID = _AddCalling(RSOrg.OrgID, "President", true, ++sortID);

            //Assign Leader
            RSOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(RSOrg.OrgID, "1st Counselor", true, ++sortID);
            _AddCoLeaderCalling(RSOrg.OrgID, "2nd Counselor", true, ++sortID);
            _AddCoLeaderCalling(RSOrg.OrgID, "Secretary", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Press Secretary", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Activities Leader", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Activities Committee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Enrichment Leader", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Enrichment Committee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Instructor 1", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Instructor 2", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Instructor 3", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Visiting Teaching Coordinator", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "VT District Supervisor", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "VT District Supervisor", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "VT District Supervisor", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Compassionate Service Leader", true, ++sortID);
            _AddCalling(RSOrg.OrgID, "Compassionate Service Commitee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Compassionate Service Commitee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Compassionate Service Commitee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Compassionate Service Commitee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Compassionate Service Commitee", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Chorister", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Pianist", false, ++sortID);
            _AddCalling(RSOrg.OrgID, "Greeter", false, ++sortID);

        }

		public void AddSundaySchool(Ward ward, string title = "Sunday School")
        {
            int sortID = 0;
            tOrganization SSOrg = _AddOrganization(ward, title, "Teaching", (int)SortID.SUNDAY_SCHOOL);
            int leaderCallingID = _AddCalling(SSOrg.OrgID, "President", true, ++sortID);

            //Assign Leader
            SSOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(SSOrg.OrgID, "1st Counselor", true, ++sortID);
            _AddCoLeaderCalling(SSOrg.OrgID, "2nd Counselor", true, ++sortID);
            _AddCoLeaderCalling(SSOrg.OrgID, "Secretary", true, ++sortID);
            _AddCalling(SSOrg.OrgID, "Gospel Doctrine Teacher", true, ++sortID);
            _AddCalling(SSOrg.OrgID, "Gospel Doctrine Teacher", true, ++sortID);
            _AddCalling(SSOrg.OrgID, "Gospel Doctrine Teacher", true, ++sortID);
            _AddCalling(SSOrg.OrgID, "Gospel Doctrine Teacher", true, ++sortID);
            _AddCalling(SSOrg.OrgID, "Gospel Principles Teacher", true, ++sortID);
            _AddCalling(SSOrg.OrgID, "Gospel Principles Teacher", true, ++sortID);

        }

		public void AddHomeEvening(Ward ward, string title = "Home Evening")
        {
            int sortID = 0;
            tOrganization HEOrg = _AddOrganization(ward, title, "FHE", (int)SortID.HOME_EVENING);
            int leaderCallingID = _AddCalling(HEOrg.OrgID, "Coordinator", true, ++sortID);

            //Assign Leader
            HEOrg.LeaderCallingID = leaderCallingID;
            _AddCoLeaderCalling(HEOrg.OrgID, "Coordinator", true, ++sortID);

            _AddCalling(HEOrg.OrgID, "FHE Group Leader G1", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G1", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G2", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G2", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G3", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G3", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G4", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G4", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G5", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G5", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G6", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G6", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G7", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G7", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G8", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G8", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G9", false, ++sortID);
            _AddCalling(HEOrg.OrgID, "FHE Group Leader G9", false, ++sortID);

        }

		public void AddActivities(Ward ward, string title = "Activities")
        {
            int sortID = 0;
            tOrganization ACTOrg = _AddOrganization(ward, title, "Activities", (int)SortID.ACTIVITIES);
            int leaderCallingID = _AddCalling(ACTOrg.OrgID, "Co-Chair", true, ++sortID);

            //Assign Leader
            ACTOrg.LeaderCallingID = leaderCallingID;
            
            _AddCoLeaderCalling(ACTOrg.OrgID, "Co-Chair", true, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Athletic Director", false, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Assistant Athletic Director", false, ++sortID);

            _AddCoLeaderCalling(ACTOrg.OrgID, "Service Coordinator", true, ++sortID);
            _AddCoLeaderCalling(ACTOrg.OrgID, "Service Coordinator", true, ++sortID);

            _AddCalling(ACTOrg.OrgID, "Service Committee", true, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Ward Prayer Coordinator", false, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Ward Prayer Coordinator", false, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Committee Member", false, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Committee Member", false, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Committee Member", false, ++sortID);
            _AddCalling(ACTOrg.OrgID, "Committee Member", false, ++sortID);

        }

		public void AddWardMission(Ward ward, string title = "Ward Mission")
        {
            int sortID = 0;
            tOrganization MissionOrg = _AddOrganization(ward, title, "Mission", (int)SortID.WARD_MISSION);
            int leaderCallingID = _AddCalling(MissionOrg.OrgID, "Leader", true, ++sortID);

            //Assign Leader
            MissionOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(MissionOrg.OrgID, "Assistant Ward Mission Leader", true, ++sortID);
            _AddCalling(MissionOrg.OrgID, "Ward Missionary", true, ++sortID);
            _AddCalling(MissionOrg.OrgID, "Ward Missionary", true, ++sortID);
            _AddCalling(MissionOrg.OrgID, "Ward Missionary", true, ++sortID);
            _AddCalling(MissionOrg.OrgID, "Ward Missionary", true, ++sortID);

        }

		public void AddTemple_FamHist(Ward ward, string title = "Temple & Family History")
        {
            int sortID = 0;
            tOrganization TempleOrg = _AddOrganization(ward, title, "Temple/FamHist", (int)SortID.TEMPLE_FAMHIST);
            int leaderCallingID = _AddCalling(TempleOrg.OrgID, "Family History Coordinator", true, ++sortID);

            //Assign Leader
            TempleOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(TempleOrg.OrgID, "Family History Coordinator", true, ++sortID);
            _AddCalling(TempleOrg.OrgID, "Consultant", true, ++sortID);
            _AddCalling(TempleOrg.OrgID, "Instructor", false, ++sortID);
            _AddCalling(TempleOrg.OrgID, "Indexing Worker", false, ++sortID);
            _AddCalling(TempleOrg.OrgID, "Indexing Worker", false, ++sortID);
            _AddCalling(TempleOrg.OrgID, "Indexing Worker", false, ++sortID);
            _AddCalling(TempleOrg.OrgID, "Indexing Worker", false, ++sortID);

        }

		public void AddMusic(Ward ward, string title = "Ward Music")
        {
            int sortID = 0;
            tOrganization MusicOrg = _AddOrganization(ward, title, "Music", (int)SortID.MUSIC);
            int leaderCallingID = _AddCalling(MusicOrg.OrgID, "Chair", true, ++sortID);

            //Assign Leader
            MusicOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(MusicOrg.OrgID, "Assistant Chair", false, ++sortID);
            _AddCoLeaderCalling(MusicOrg.OrgID, "Choir Director", false, ++sortID);

            _AddCalling(MusicOrg.OrgID, "Choir Accompanist", false, ++sortID);
            _AddCalling(MusicOrg.OrgID, "Sacrament Meeting Chorister", false, ++sortID);
            _AddCalling(MusicOrg.OrgID, "Sacrament Meeting Chorister", false, ++sortID);
            _AddCalling(MusicOrg.OrgID, "Ward Pianist", false, ++sortID);
            _AddCalling(MusicOrg.OrgID, "Ward Pianist", false, ++sortID);

        }

		public void AddPublicity(Ward ward, string title = "Publicity Committee")
        {
            int sortID = 0;
            tOrganization PubOrg = _AddOrganization(ward, title, null, (int)SortID.PUBLICITY);

            _AddCalling(PubOrg.OrgID, "Representative", false, ++sortID);

            _AddCalling(PubOrg.OrgID, "Sacrament Bulletin", false, ++sortID);
            _AddCalling(PubOrg.OrgID, "Ward Directory Specialist", false, ++sortID);
            _AddCalling(PubOrg.OrgID, "Sacrament Meeting Greeter", false, ++sortID);
            _AddCalling(PubOrg.OrgID, "Historian", false, ++sortID);
            _AddCalling(PubOrg.OrgID, "Ward Newsletter", false, ++sortID);

        }

		public void AddInstitute(Ward ward, string title = "Institute")
        {
            int sortID = 0;
            tOrganization IOrg = _AddOrganization(ward, title, "Institute", (int)SortID.INSTITUTE);
            int leaderCallingID = _AddCalling(IOrg.OrgID, "Representative", true, ++sortID);

            //Assign Leader
            IOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(IOrg.OrgID, "Representative", true, ++sortID);

        }

		public void AddEmployment(Ward ward, string title = "Employment")
        {
            int sortID = 0;
            tOrganization IOrg = _AddOrganization(ward, title, "Employment", (int)SortID.EMPLOYMENT);
            int leaderCallingID = _AddCalling(IOrg.OrgID, "Specialist", true, ++sortID);

            //Assign Leader
            IOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(IOrg.OrgID, "Specialist", true, ++sortID);

        }

		public void AddEmergency(Ward ward, string title = "Emergency Preparedness")
        {
            int sortID = 0;
            tOrganization IOrg = _AddOrganization(ward, title, "Emergency", (int)SortID.EMERGENCY);
            int leaderCallingID = _AddCalling(IOrg.OrgID, "Coordinator", true, ++sortID);

            //Assign Leader
            IOrg.LeaderCallingID = leaderCallingID;

            _AddCoLeaderCalling(IOrg.OrgID, "Coordinator", true, ++sortID);

        }

        private void _AddCoLeaderCalling(int OrgID, string Title, bool ITstake, int SortID)
        {
			using (var db = new DBmsw())
			{
				tCalling calling = new tCalling();
				calling.OrgID = OrgID;
				calling.Title = Title;
				calling.ITStake = ITstake;
				calling.SortID = SortID;

				db.tCallings.InsertOnSubmit(calling);
				db.SubmitChanges();

				tOrganizationCoLeader orgCo = new tOrganizationCoLeader();
				orgCo.OrgID = OrgID;
				orgCo.CoLeaderID = calling.CallingID;
				db.tOrganizationCoLeaders.InsertOnSubmit(orgCo);
				db.SubmitChanges();
			}
        }
    }
}