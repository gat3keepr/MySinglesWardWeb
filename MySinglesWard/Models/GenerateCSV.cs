using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using System.Text;
using MSW.Models.dbo;
using MSW.Utilities;

namespace MSW.Models
{
    public class GenerateCSV
    {
		/// <summary>
		/// Arrays used for the Column Headers in the CSV files
		/// </summary>
        #region Headers
        private static String[] StakePresHeaders = {"Last Name", "First Name", "Pref Name", "Ward", "Cell Phone", "Email", "Current Address", "Birthday", 
                               "Gender","Priesthood", "Home Address", "Home Phone",  "Emergency Contact Name", "Emergency Contact Phone", "Prev Bishops", 
                               "Time in Ward", "Mission Information", "Patriarchal Blessing", 
                               "Endowed", "Temple Recommend", "Expiration Date", "Temple Worker",
                               "School", "Religion Class", "Employed","Employer", "Callings", "Music Skill", "Music Ability" , "Teaching Desire", 
                               "Teaching Skills", "Calling Pref", "Activities", "Interests", "Description"};

        private static String[] StakeHeaders = {"Last Name", "Pref Name", "Ward", "Cell Phone", "Email", "Current Address", "Birthday", 
                               "Gender", "Emergency Contact Name", "Emergency Contact Phone",
                               "Mission Information", "Temple Worker", "School", "Religion Class", "Employed","Employer", "Callings", "Music Skill", 
                               "Music Ability", "Activities", "Interests", "Description"};

        private static String[] BishopricHeaders = {"Last Name", "First Name", "Pref Name", "Cell Phone", "Email", "Current Address", "Birthday", 
                                             "Gender","Priesthood", "Home Address", "Home Phone", "Home Ward & Stake", 
                               "Home Bishop", "Emergency Contact Name", "Emergency Contact Phone", "Prev Bishops", 
                               "Time in Ward", "Mission Information", "Patriarchal Blessing", 
                               "Endowed", "Temple Recommend", "Expiration Date", "Temple Worker",
                               "School", "Religion Class", "Employed","Employer", "Callings", "Music Skill", "Music Ability" , "Teaching Desire", 
                               "Teaching Skills", "Calling Pref", "Activities", "Interests", "Description", "Bishop to Know"};

        private static String[] AuxHeaders = {"Last Name", "First Name", "Pref Name", "Cell Phone", "Email", "Current Address", "Birthday", 
                               "Gender","Priesthood", "Home Address", "Home Phone",  "Emergency Contact Name", "Emergency Contact Phone", "Prev Bishops", 
                               "Time in Ward", "Mission Information", "Patriarchal Blessing", 
                               "Endowed", "Temple Recommend", "Expiration Date", "Temple Worker",
                               "School", "Religion Class", "Employed","Employer", "Callings", "Music Skill", "Music Ability" , "Teaching Desire", 
                               "Teaching Skills", "Calling Pref", "Activities", "Interests", "Description"};

        private static String[] ClerkHeaders = { "Name", "Birth Date", "Address", "City", "State", "Postal", "Phone", "Prior Unit" };

        #endregion

		/// <summary>
		/// Creates differnet CSV files based on authentication types
		/// </summary>
		
        public static String MakeBishopricFile(List<MemberModel> members)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String header in BishopricHeaders)
            {
                sb.Append(header + ",");
            }
            sb.Append("\n");

            foreach (MemberModel member in members)
            {
                sb.Append("\"" + member.user.LastName + "\",\"" + member.user.FirstName + "\",\"" + member.memberSurvey.prefName + "\",\"" + member.memberSurvey.cellPhone + "\",\""
                   + member.user.Email + "\",\"" + member.memberSurvey.residence + "\",\"" + member.memberSurvey.birthday + "\",");
                //Gender
                if (member.memberSurvey.gender)
                    sb.Append("Male,\"");
                else
                    sb.Append("Female,\"");

                sb.Append(member.memberSurvey.priesthood + "\",\"" + member.memberSurvey.homeAddress + "\",\"" +member.memberSurvey.homePhone  + "\",\"" +
                    member.memberSurvey.homeWardStake + "\",\"" + member.memberSurvey.homeBishop + "\",\"" + member.memberSurvey.emergContact + "\",\"" + member.memberSurvey.emergPhone + "\",\""
                    + member.memberSurvey.prevBishops + "\",\"" + member.memberSurvey.timeInWard + "\",\"" + member.memberSurvey.missionInformation + "\"," + member.memberSurvey.patriarchalBlessing + "," 
                    + member.memberSurvey.endowed + "," + member.memberSurvey.templeRecommend + "," +
                    member.memberSurvey.templeExpDate + "," + member.memberSurvey.templeWorker + ",\"" + member.memberSurvey.schoolInfo + "\",\"" +
                    member.memberSurvey.religionClass + "\"," + member.memberSurvey.employed + ",\"" + member.memberSurvey.occupation + "\",\"" + member.memberSurvey.pastCallings
                    + "\"," + member.memberSurvey.musicSkill + ",\"" + member.memberSurvey.musicTalent + "\"," + member.memberSurvey.teachDesire + "," + member.memberSurvey.teachSkill +
                    ",\"" + member.memberSurvey.callingPref + "\",\"" + member.memberSurvey.activities + "\",\"" + member.memberSurvey.interests + "\",\"" + member.memberSurvey.description +
                    "\"\n");
            }

            return sb.ToString();

        }

        public static String MakeAuxFile(List<MemberModel> members)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String header in AuxHeaders)
            {
                sb.Append(header + ",");
            }
            sb.Append("\n");

            foreach (MemberModel member in members)
            {
                sb.Append("\"" + member.user.LastName + "\",\"" + member.user.FirstName + "\",\"" + member.memberSurvey.prefName + "\",\"" + member.memberSurvey.cellPhone + "\",\""
                   + member.user.Email + "\",\"" + member.memberSurvey.residence + "\",\"" + member.memberSurvey.birthday + "\",");
                //Gender
                if (member.memberSurvey.gender)
                    sb.Append("Male,\"");
                else
                    sb.Append("Female,\"");

                sb.Append(member.memberSurvey.priesthood + "\",\"" + member.memberSurvey.homeAddress + "\",\"" +
                    member.memberSurvey.homePhone + "\",\"" + member.memberSurvey.emergContact + "\",\"" + member.memberSurvey.emergPhone + "\",\""
                    + member.memberSurvey.prevBishops + "\",\"" + member.memberSurvey.timeInWard + "\",\"" + member.memberSurvey.missionInformation + "\"," + member.memberSurvey.patriarchalBlessing + ","
                    + member.memberSurvey.endowed + "," + member.memberSurvey.templeRecommend + "," +
                    member.memberSurvey.templeExpDate + "," + member.memberSurvey.templeWorker + ",\"" + member.memberSurvey.schoolInfo + "\",\"" +
                    member.memberSurvey.religionClass + "\"," + member.memberSurvey.employed + ",\"" + member.memberSurvey.occupation + "\",\"" + member.memberSurvey.pastCallings
                    + "\"," + member.memberSurvey.musicSkill + ",\"" + member.memberSurvey.musicTalent + "\"," + member.memberSurvey.teachDesire + "," + member.memberSurvey.teachSkill +
                    ",\"" + member.memberSurvey.callingPref + "\",\"" + member.memberSurvey.activities + "\",\"" + member.memberSurvey.interests + "\",\"" + member.memberSurvey.description +
                    "\"\n");
            }

            return sb.ToString();

        }

        /// <summary>
        /// Creates a CSV string used for the MLS Automatic Record Request Feature
        /// </summary>
        /// <returns> String in CSV format </returns>
        public static String MakeClerkFile(List<MemberModel> members, double WardID)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String header in ClerkHeaders)
            {
                sb.Append(header + ",");
            }
            sb.Append("\n");

            WardInfo wardInfo = WardInfo.get(WardID);

            //Creates a Dictionary of residences and strings to replace address in the CSV file.
            //This will attempt to replace the string chosen in the survey with the real address designated by the clerk
			Repository r = Repository.getInstance();
            List<Residence> residences = Cache.GetList(r.ResidenceIDs(WardID), x => Cache.getCacheKey<Residence>(x),
                                                                    y => Residence.get(y));
            Dictionary<string, string> residenceReplacer = new Dictionary<string,string>();
            foreach(Residence residence in residences)
            {
				if (residence.streetAddress != null && !residenceReplacer.ContainsKey(residence.residence))
                    residenceReplacer.Add(residence.residence, residence.streetAddress);
            }

            foreach (MemberModel member in members)
            {
                string birthday = "";
                PriorUnit priorUnit = PriorUnit.get(member.user.MemberID);
                string phone = member.memberSurvey.cellPhone.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
                string streetAddress = residenceReplacer.ContainsKey(member.memberSurvey.residence) ?
                    residenceReplacer[member.memberSurvey.residence] : member.memberSurvey.residence;

                try
                {
                    //Create Date string in the YYYYMMDD format
                    DateTime birthdayData = DateTime.Parse(member.memberSurvey.birthday);
                    birthday = birthdayData.ToString("yyyyMMdd");                      
                }
                catch
                {
                }

                sb.Append("\"" + member.user.LastName + ", " + member.user.FirstName + 
                    "\",\"" + birthday +
                    "\",\"" + streetAddress + 
                    "\",\"" + wardInfo.City + 
                    "\",\"" + wardInfo.State + 
                    "\",\"" + wardInfo.Zipcode + 
                    "\",\"" + phone + 
                    "\",\"" + (priorUnit != null ? priorUnit.priorUnit : "")  + "\"\n");
            }

            return sb.ToString();
        }

        internal static string MakeStakeFile(List<MemberModel> members)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String header in StakeHeaders)
            {
                sb.Append(header + ",");
            }
            sb.Append("\n");

            foreach (MemberModel member in members)
            {
                sb.Append("\"" + member.user.LastName + "\",\"" + member.memberSurvey.prefName + "\",\"" + member.CurrentWard + "\",\"" 
                    + member.memberSurvey.cellPhone + "\",\"" + member.user.Email + "\",\"" + member.memberSurvey.residence + "\",\"" + member.memberSurvey.birthday + "\",");
                //Gender
                if (member.memberSurvey.gender)
                    sb.Append("Male,\"");
                else
                    sb.Append("Female,\"");
                
                sb.Append(member.memberSurvey.emergContact + "\",\"" + member.memberSurvey.emergPhone + "\",\""
                    + member.memberSurvey.missionInformation + "\"," + member.memberSurvey.templeWorker + ",\"" + member.memberSurvey.schoolInfo + "\",\"" +
                    member.memberSurvey.religionClass + "\"," + member.memberSurvey.employed + ",\"" + member.memberSurvey.occupation + "\",\"" + member.memberSurvey.pastCallings
                    + "\"," + member.memberSurvey.musicSkill + ",\"" + member.memberSurvey.musicTalent + "\",\"" + member.memberSurvey.activities + "\",\"" + member.memberSurvey.interests + "\",\"" + member.memberSurvey.description +
                    "\"\n");
            }

            return sb.ToString();
        }

        internal static string MakeStakePresFile(List<MemberModel> members)
        {
            StringBuilder sb = new StringBuilder();
            foreach (String header in StakeHeaders)
            {
                sb.Append(header + ",");
            }
            sb.Append("\n");

            foreach (MemberModel member in members)
            {
                sb.Append("\"" + member.user.LastName + "\",\"" + member.user.FirstName + "\",\"" + member.memberSurvey.prefName + "\",\"" + member.CurrentWard + "\",\""
                    + member.memberSurvey.cellPhone + "\",\"" + member.user.Email + "\",\"" + member.memberSurvey.residence + "\",\"" + member.memberSurvey.birthday + "\",");
                //Gender
                if (member.memberSurvey.gender)
                    sb.Append("Male,\"");
                else
                    sb.Append("Female,\"");

                sb.Append(member.memberSurvey.priesthood + "\",\"" + member.memberSurvey.homeAddress + "\",\"" +
                    member.memberSurvey.homePhone + "\",\"" + member.memberSurvey.emergContact + "\",\"" + member.memberSurvey.emergPhone + "\",\""
                    + member.memberSurvey.prevBishops + "\",\"" + member.memberSurvey.timeInWard + "\",\"" + member.memberSurvey.missionInformation + "\"," + member.memberSurvey.patriarchalBlessing + ","
                    + member.memberSurvey.endowed + "," + member.memberSurvey.templeRecommend + "," +
                    member.memberSurvey.templeExpDate + "," + member.memberSurvey.templeWorker + ",\"" + member.memberSurvey.schoolInfo + "\",\"" +
                    member.memberSurvey.religionClass + "\"," + member.memberSurvey.employed + ",\"" + member.memberSurvey.occupation + "\",\"" + member.memberSurvey.pastCallings
                    + "\"," + member.memberSurvey.musicSkill + ",\"" + member.memberSurvey.musicTalent + "\"," + member.memberSurvey.teachDesire + "," + member.memberSurvey.teachSkill +
                    ",\"" + member.memberSurvey.callingPref + "\",\"" + member.memberSurvey.activities + "\",\"" + member.memberSurvey.interests + "\",\"" + member.memberSurvey.description +
                    "\"\n");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a teaching report for leadership based on the orgID and the selected Month
        /// </summary>
        internal static string MakeMonthlyTeachingReport(int orgID, int MonthID)
        {   
            //Get all the districts in the organization
            Organization org = Organization.get(orgID);
            IEnumerable<int> districts = Repository.getInstance().getDistricts(orgID);
            TeachingMonth month = TeachingMonth.get(MonthID);

            //Percentage tracking
            int visits = 0;
            int totalVisited = 0;

            //Create Title for CSV
            StringBuilder sb = new StringBuilder();
            sb.Append("\"" + org.Title + (org.ReportID == "Elders Quorum" ? " Home Teaching" : " Visiting Teaching") + " Report\"\n\""
                + TeachingMonth.monthNames[month.teachingMonth.Month - 1] + " " + month.teachingMonth.Year);
            sb.Append("\"\n\n");

            //Create String to append to report - this is a seperate string so a percentage can be created on the
            //way through the districts
            StringBuilder report = new StringBuilder(); 
            foreach (int districtID in districts)
            {
                //District Information
                District district = District.get(districtID);
                
                //Get all the companionships for a district
                IEnumerable<int> companionships = Repository.getInstance().getCompanionships(districtID);

                if(companionships.Count() != 0)
                    report.Append("\"District:\",\"" + district.Title + "\"\n\n");

                foreach (int CompanionshipID in companionships)
                {
                    //Companionship Information
                    Companionship comp = Companionship.get(CompanionshipID);
                    
                    List<MemberModel> companions = Cache.GetList(Repository.getInstance().getTeachers(CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                                                .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
                    List<MemberModel> teachees = Cache.GetList(Repository.getInstance().getTeachees(CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                                                .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();

                    if(companions.Count != 0 && teachees.Count != 0)
                        report.Append("\"Companionship\",\"\",\"Visited\",\"Needs Attention\"\n");

                    int row = 1;
                    while (row <= companions.Count() || row <= teachees.Count())
                    {
                        //Try to add the companion to the row
                        try
                        {
                            MemberModel member = companions[row - 1];

                            report.Append("\"" + member.user.LastName + ", " + member.memberSurvey.prefName + "\",\"\",");
                        }
                        catch
                        {
                            //Row can be skipped if the member was not in the list - append empty spots
                            report.Append("\"\",\"\",");
                        }

                        //Try to add a member and their visit status
                        try
                        {
                            MemberModel member = teachees[row - 1];
                            TeachingVisit visit = TeachingVisit.get(MonthID, member.user.MemberID, CompanionshipID);
                            
                            //Increase the amount of members visited
                            visits++;

                            string wasVisited = "";
                            string needsAttention = "-";
                            if (visit.reported)
                            {
                                wasVisited = visit.wasVisited ? "Y" : "N";
                                if (visit.wasVisited)
                                    totalVisited++;
                                if (visit.needsAttention)
                                    needsAttention = "Y";
                            }
                            else
                            {
                                // - : Indicates that the report has not been submitted
                                wasVisited = "-"; 
                            }

                            report.Append("\"" + wasVisited + "\",\"" + needsAttention + "\",\"" + member.user.LastName + ", " + member.memberSurvey.prefName + "\"");
                        }
                        catch
                        {
                            //Row can be skipped if the member was not in the list - append empty spots
                            report.Append("\"\",\"\"");
                        }

                        report.Append("\n");
                        row++;
                    }

                    //Add seperation between the companionships
                    report.Append("\n");
                }                
            }

            //Append Percentages and report
            sb.Append("\"Percentage:\",\"" + (int)(((double)totalVisited / (double)visits) * 100) + "%\"\n\n");
            sb.Append(report);

            return sb.ToString();
        }
    }
}