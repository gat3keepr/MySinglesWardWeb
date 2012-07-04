using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;
using System.Collections.Specialized;
using Newtonsoft.Json;

namespace MSW.Models.dbo
{
    [Serializable]
    public class MemberSurvey
    {
        public int memberID { get; set; }
        public string prefName { get; set; }
        public string residence { get; set; }
        public bool publishEmail { get; set; }
        public string birthday { get; set; }
        public bool gender { get; set; }
        public string priesthood { get; set; }
        public string cellPhone { get; set; }
        public bool publishCell { get; set; }
        public string homeAddress { get; set; }
        public string homeWardStake { get; set; }
        public string homePhone { get; set; }
        public string homeBishop { get; set; }
        public string emergContact { get; set; }
        public string emergPhone { get; set; }
        public string prevBishops { get; set; }
        public string timeInWard { get; set; }
        public string missionInformation { get; set; }
        public bool mission { get; set; }
        public string missionLocation { get; set; }
        public int? planMission { get; set; }
        public string planMissionTime { get; set; }
        public string patriarchalBlessing { get; set; }
        public string endowed { get; set; }
        public string templeRecommend { get; set; }
        public string templeExpDate { get; set; }
        public string templeWorker { get; set; }
        public string schoolInfo { get; set; }
        public bool enrolledSchool { get; set; }
        public string school { get; set; }
        public string major { get; set; }
        public string religionClass { get; set; }
        public string employed { get; set; }
        public string occupation { get; set; }
        public string pastCallings { get; set; }
        public string musicSkill { get; set; }
        public string musicTalent { get; set; }
        public string teachSkill { get; set; }
        public string teachDesire { get; set; }
        public string callingPref { get; set; }
        public string activities { get; set; }
        public string interests { get; set; }
        public string description { get; set; }
        public int status { get; set; }

        //Duplicate properties for survey form 
        public bool _PatriarchalBlessing { get; set; }
        public bool _TempleWorker { get; set; }
        public bool _TempleRecommend { get; set; }
        public bool _Endowed { get; set; }

        public static MemberSurvey getMemberSurvey(int MemberID)
        {
            MemberSurvey survey = Cache.Get(Cache.getCacheKey<MemberSurvey>(MemberID)) as MemberSurvey;

            if (survey == null)
            {
                try
                {
                    survey = new MemberSurvey(MemberID);

                    Cache.Set(Cache.getCacheKey<MemberSurvey>(MemberID), survey);
                }
                catch
                {
                    return null;
                }
            }

            return survey;
        }

        public static void saveMemberSurvey(MemberSurvey member)
        {
            Cache.Remove(Cache.getCacheKey<MemberSurvey>(member.memberID));

			using (var db = new DBmsw())
			{
				var survey = db.tSurveyDatas.SingleOrDefault(x => x.SurveyID == member.memberID);

				if (survey == null)
				{
					survey = new tSurveyData();
					db.tSurveyDatas.InsertOnSubmit(survey);
				}

				survey.SurveyID = member.memberID;
				if (member.status > 0)
				{
					survey.PrefName = Utilities.Cryptography.EncryptString(member.prefName);
					survey.Apartment = Utilities.Cryptography.EncryptString(member.residence);
					survey.PublishEmail = member.publishEmail;
					survey.Birthday = Utilities.Cryptography.EncryptString(member.birthday);
					survey.Gender = member.gender;
					survey.Priesthood = member.priesthood;
					survey.CellPhone = Utilities.Cryptography.EncryptString(member.cellPhone);
					survey.PublishCell = member.publishCell;
					survey.HomeAddress = Utilities.Cryptography.EncryptString(member.homeAddress);
					survey.HomeWardStake = member.homeWardStake;
					survey.HomePhone = Utilities.Cryptography.EncryptString(member.homePhone);
					survey.HomeBishop = member.homeBishop;
					survey.EmergContact = Utilities.Cryptography.EncryptString(member.emergContact);
					survey.EmergPhone = Utilities.Cryptography.EncryptString(member.emergPhone);
				}
				if (member.status > 1)
				{
					survey.PrevBishops = member.prevBishops;
					survey.Mission = member.mission;
					survey.MissionLocation = member.missionLocation;
					survey.PlanMission = member.planMission;
					survey.PlanMissionTime = member.planMissionTime;
					survey.PatriarchalBlessing = member.patriarchalBlessing == "Yes" ? true : false;
					survey.Endowed = member.endowed == "Yes" ? true : false;
					survey.TempleRecommend = member.templeRecommend == "Yes" ? true : false;
					survey.TempleExpDate = member.templeExpDate;
					survey.TempleWorker = member.templeWorker == "Yes" ? true : false;
				}
				if (member.status > 2)
				{
					survey.EnrolledSchool = member.enrolledSchool;
					survey.TimeInWard = member.timeInWard;
					survey.School = member.school;
					survey.Major = member.major;
					survey.ReligionClass = member.religionClass;
					survey.Employed = member.employed;
					survey.Occupation = member.occupation;
					survey.Callings = member.pastCallings;
					survey.MusicSkill = int.Parse(member.musicSkill);
					survey.MusicTalent = member.musicTalent;
					survey.TeachSkill = int.Parse(member.teachSkill);
					survey.TeachDesire = int.Parse(member.teachDesire);
					survey.CallingPref = member.callingPref;
					survey.Activities = member.activities;
					survey.Interests = member.interests;
					survey.Description = member.description;
				}

                survey.Status = member.status;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<MemberSurvey>(member.memberID), member);
			}
        }

        public static void savePersonal(NameValueCollection member, int MemberID)
        {
            Cache.Remove(Cache.getCacheKey<MemberSurvey>(MemberID));
            
			using (var db = new DBmsw())
			{
				var survey = db.tSurveyDatas.SingleOrDefault(x => x.SurveyID == MemberID);

				if (survey == null)
				{
					survey = new tSurveyData();
					db.tSurveyDatas.InsertOnSubmit(survey);
					survey.Status = 0;
				}

				survey.SurveyID = MemberID;
				survey.PrefName = Utilities.Cryptography.EncryptString(member["prefName"]);
				survey.Apartment = Utilities.Cryptography.EncryptString(member["residence"]);
				survey.PublishEmail = bool.Parse(member["publishEmail"]);
				survey.Birthday = Utilities.Cryptography.EncryptString(member["birthday"]);
				survey.Gender = bool.Parse(member["gender"]);
				survey.Priesthood = member["priesthood"];
				survey.CellPhone = Utilities.Cryptography.EncryptString(
                    MSWtools.normalizePhoneNumber(member["cellPhone"] == null ? "" : member["cellPhone"]));
				survey.PublishCell = bool.Parse(member["publishCell"]);
				survey.HomeAddress = Utilities.Cryptography.EncryptString(member["homeAddress"] == null ? "" : member["homeAddress"]);
				survey.HomeWardStake = member["homeWardStake"] == null ? "" : member["homeWardStake"];
				survey.HomePhone = Utilities.Cryptography.EncryptString(member["homePhone"] == null ? "" : member["homePhone"]);
				survey.HomeBishop = member["homeBishop"] == null ? "" : member["homeBishop"];
				survey.EmergContact = Utilities.Cryptography.EncryptString(member["emergContact"] == null ? "" : member["emergContact"]);
				survey.EmergPhone = Utilities.Cryptography.EncryptString(member["emergPhone"] == null ? "" : member["emergPhone"]);

				if (survey.Status == 0)
					survey.Status++;
				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<MemberSurvey>(MemberID), new MemberSurvey(MemberID));
			}
        }

        public static void saveChurch(NameValueCollection member, int MemberID)
        {
            Cache.Remove(Cache.getCacheKey<MemberSurvey>(MemberID));
            //_checkForm(member);

			using (var db = new DBmsw())
			{
				var survey = db.tSurveyDatas.SingleOrDefault(x => x.SurveyID == MemberID);

				if (survey == null)
				{
                    throw new Exception();
				}

				survey.SurveyID = MemberID;
				survey.PrevBishops = member["prevBishops"] == null ? "" : member["prevBishops"];
				survey.Mission = bool.Parse(member["mission"]);
				survey.MissionLocation = member["missionLocation"] == null ? "" : member["missionLocation"];

				int? PlanMission = null;
				try
				{
					PlanMission = int.Parse(member["planMission"]);
				}
				catch { }
				survey.PlanMission = PlanMission;

				survey.PlanMissionTime = member["planMissionTime"] == null ? "" : member["planMissionTime"];
				survey.PatriarchalBlessing = bool.Parse(member["patriarchalBlessing"]);
				survey.Endowed = bool.Parse(member["endowed"]);
				survey.TempleRecommend = bool.Parse(member["templeRecommend"]);
				survey.TempleExpDate = member["templeExpDate"] == null ? "" : member["templeExpDate"];
				survey.TempleWorker = bool.Parse(member["templeWorker"]);
				survey.Callings = member["pastCallings"] == null ? "" : member["pastCallings"];

				if (survey.Status == 1)
					survey.Status++;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<MemberSurvey>(MemberID), new MemberSurvey(MemberID));
			}
        }

        public static void saveOther(NameValueCollection member, int MemberID)
        {
            Cache.Remove(Cache.getCacheKey<MemberSurvey>(MemberID));
            //_checkForm(member);

			using (var db = new DBmsw())
			{
				var survey = db.tSurveyDatas.SingleOrDefault(x => x.SurveyID == MemberID);

				if (survey == null)
				{
                    throw new Exception();
				}

				survey.SurveyID = MemberID;

				survey.TimeInWard = member["timeInWard"] == null ? "" : member["timeInWard"];
				survey.EnrolledSchool = bool.Parse(member["enrolledSchool"]);
				survey.School = member["school"] == null ? "" : member["school"];
				survey.Major = member["major"] == null ? "" : member["major"];
				survey.ReligionClass = member["religionClass"] == null ? "" : member["religionClass"];
				survey.Employed = member["employed"] == null ? "" : member["employed"];
				survey.Occupation = member["occupation"] == null ? "" : member["occupation"];
				survey.MusicSkill = int.Parse(member["musicSkill"]);
				survey.MusicTalent = member["musicTalent"] == null ? "" : member["musicTalent"];

                //Handle errors from certain browsers i think
                try
                {
                    survey.TeachSkill = int.Parse(member["teachSkill"]);
                }
                catch
                {
                    survey.TeachSkill = 0;
                }

                try
                {
                    survey.TeachDesire = int.Parse(member["teachDesire"]);
                }
                catch
                {
                    survey.TeachDesire = 0;
                }

				survey.CallingPref = member["callingPref"] == null ? "" : member["callingPref"];
				survey.Activities = member["activities"] == null ? "" : member["activities"];
				survey.Interests = member["interests"] == null ? "" : member["interests"];
				survey.Description = member["description"] == null ? "" : member["description"];

				if (survey.Status == 2)
					survey.Status++;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<MemberSurvey>(MemberID), new MemberSurvey(MemberID));
			}
        }

        private MemberSurvey(int MemberID)
        {
			using (var db = new DBmsw())
			{
				var survey = db.tSurveyDatas.SingleOrDefault(x => x.SurveyID == MemberID);

				this.memberID = survey.SurveyID;
				prefName = Utilities.Cryptography.DecryptString(survey.PrefName);
				residence = Utilities.Cryptography.DecryptString(survey.Apartment);
				publishEmail = survey.PublishEmail;
				birthday = Utilities.Cryptography.DecryptString(survey.Birthday);
				gender = survey.Gender;
				priesthood = survey.Priesthood;
				cellPhone = Utilities.Cryptography.DecryptString(survey.CellPhone);
				publishCell = survey.PublishCell;
				homeAddress = Utilities.Cryptography.DecryptString(survey.HomeAddress);
				homeWardStake = survey.HomeWardStake;
				homePhone = Utilities.Cryptography.DecryptString(survey.HomePhone);
				homeBishop = survey.HomeBishop;
				emergContact = Utilities.Cryptography.DecryptString(survey.EmergContact);
				emergPhone = Utilities.Cryptography.DecryptString(survey.EmergPhone);
				prevBishops = survey.PrevBishops;
				timeInWard = survey.TimeInWard;
				mission = survey.Mission;
				missionLocation = survey.MissionLocation;
				planMission = survey.PlanMission;
				planMissionTime = survey.PlanMissionTime;
				patriarchalBlessing = survey.PatriarchalBlessing ? "Yes" : "No";
				_PatriarchalBlessing = survey.PatriarchalBlessing;
				endowed = survey.Endowed ? "Yes" : "No";
				_Endowed = survey.Endowed;
				templeRecommend = survey.TempleRecommend ? "Yes" : "No";
				_TempleRecommend = survey.TempleRecommend;
				templeExpDate = survey.TempleExpDate;
				templeWorker = survey.TempleWorker ? "Yes" : "No";
				_TempleWorker = survey.TempleWorker;
				enrolledSchool = survey.EnrolledSchool;
				school = survey.School;
				major = survey.Major;
				religionClass = survey.ReligionClass;
				employed = survey.Employed;
				occupation = survey.Occupation;
				pastCallings = survey.Callings;
				musicSkill = survey.MusicSkill.ToString();
				musicTalent = survey.MusicTalent;
				teachSkill = survey.TeachSkill.ToString();
				teachDesire = survey.TeachDesire.ToString();
				callingPref = survey.CallingPref;
				activities = survey.Activities;
				interests = survey.Interests;
				description = survey.Description;

				status = survey.Status;
				_checkSurvey(this);
			}
        }

        public MemberSurvey() { }

        //Prepares the survey to interface with the rest of the website
        private static void _checkSurvey(MemberSurvey survey)
        {
            if (survey.status < 3)
            {
                survey.schoolInfo = "";
                survey.religionClass = "";
                survey.employed = "";
                survey.occupation = "";
                survey.pastCallings = "";
                survey.timeInWard = "";
                if (survey.musicTalent == null)
                {
                    survey.musicTalent = "";
                }
                if (survey.callingPref == null)
                {
                    survey.callingPref = "";
                }
                if (survey.activities == null)
                {
                    survey.activities = "";
                }
                if (survey.interests == null)
                {
                    survey.interests = "";
                }
                if (survey.description == null)
                {
                    survey.description = "";
                }
                survey.musicSkill = survey.musicSkill == "-1" ? "" : survey.musicSkill;
                survey.teachSkill = survey.teachSkill == "-1" ? "" : survey.teachSkill;
                survey.teachDesire = survey.teachDesire == "-1" ? "" : survey.teachDesire;

            }
            else
            {
                if (survey.enrolledSchool)
                    survey.schoolInfo = survey.school + ": " + survey.major;
                else
                    survey.schoolInfo = "Not Attending";
            }

            if (survey.status < 2)
            {
                survey.missionInformation = "";
                survey.prevBishops = "";
                survey.missionLocation = "";
                survey.planMissionTime = "";
                survey.templeExpDate = "";

                survey.templeWorker = "";
                survey.templeRecommend = "";
                survey.endowed = "";
                survey.patriarchalBlessing = "";
            }
            else
            {
                if (survey.mission)
                    survey.missionInformation = survey.missionLocation;
                else
                {
                    if (survey.planMission == 1)
                        survey.missionInformation = "Yes " + survey.planMissionTime;
                    else if (survey.planMission == 0)
                        survey.missionInformation = "No Plans";
                    else if (survey.planMission == -1)
                        survey.missionInformation = "Maybe " + survey.planMissionTime;
                }
            }

            if (survey.homePhone == null)
            {
                survey.homePhone = "";
            }
            if (survey.homeWardStake == null)
            {
                survey.homeWardStake = "";
            }
            if (survey.homeBishop == null)
            {
                survey.homeBishop = "";
            }
            if (survey.prevBishops == null)
            {
                survey.prevBishops = "";
            }
            if (survey.missionLocation == null)
            {
                survey.missionLocation = "";
            }
            if (survey.planMissionTime == null)
            {
                survey.planMissionTime = "";
            }
            if (survey.templeExpDate == null)
            {
                survey.templeExpDate = "";
            }
            if (survey.school == null)
            {
                survey.school = "";
            }
            if (survey.major == null)
            {
                survey.major = "";
            }
            if (survey.occupation == null)
            {
                survey.occupation = "";
            }
            if (survey.musicTalent == null)
            {
                survey.musicTalent = "";
            }
            if (survey.callingPref == null)
            {
                survey.callingPref = "";
            }
            if (survey.activities == null)
            {
                survey.activities = "";
            }
            if (survey.interests == null)
            {
                survey.interests = "";
            }
            if (survey.description == null)
            {
                survey.description = "";
            }
        }

        /// <summary>
        /// Generates the json used for leadership member survey information
        /// </summary>
        internal string toLeadershipJSON(double WardID)
        {
            //Creates a Dictionary of residences and strings to replace address in the CSV file.
            //This will attempt to replace the string chosen in the survey with the real address designated by the clerk
            Repository r = Repository.getInstance();
            List<Residence> residences = Cache.GetList(r.ResidenceIDs(WardID), x => Cache.getCacheKey<Residence>(x),
                                                                    y => Residence.get(y));

            string streetAddress = residences.Select(x => x.residence).Contains(residence) ?
                    residences.Single(x => x.residence == residence).residence : residence;

            return "{ \"memberID\": " + memberID + 
                ", \"prefName\": \"" + prefName + "\"" +
                ", \"residence\": \"" + residence + "\"" +
                ", \"streetAddress\": \"" + streetAddress + "\"" +
                ", \"cellPhone\": \"" + cellPhone + "\"" +
                ", \"birthday\": \"" + birthday + "\"" +
                ", \"gender\":" + gender +
                ", \"birthday\": \"" + birthday + "\"" +
                ", \"priesthood\": \"" + priesthood + "\"" +
                ", \"homeAddress\": \"" + homeAddress + "\"" +
                ", \"homeWardStake\": \"" + homeWardStake + "\"" +
                ", \"homePhone\": \"" + homePhone + "\"" +
                ", \"homeBishop\": \"" + homeBishop + "\"" +
                ", \"emergContact\": \"" + emergContact + "\"" +
                ", \"emergPhone\": \"" + emergPhone + "\"" +
                ", \"timeInWard\": \"" + timeInWard + "\"" +
                ", \"missionInformation\": \"" + missionInformation + "\"" +
                ", \"prevBishops\": \"" + prevBishops + "\"" +
                ", \"patriarchalBlessing\": \"" + patriarchalBlessing + "\"" +
                ", \"endowed\": \"" + endowed + "\"" +
                ", \"templeRecommend\": \"" + templeRecommend + "\"" +
                ", \"templeExpDate\": \"" + templeExpDate + "\"" +
                ", \"templeWorker\": \"" + templeWorker + "\"" +
                ", \"schoolInfo\": \"" + schoolInfo + "\"" +
                ", \"religionClass\": \"" + religionClass + "\"" +
                ", \"employed\": \"" + employed + "\"" +
                ", \"occupation\": \"" + occupation + "\"" +
                ", \"pastCallings\": \"" + pastCallings + "\"" +
                ", \"musicSkill\": \"" + musicSkill + "\"" +
                ", \"musicTalent\": \"" + musicTalent + "\"" +
                ", \"teachSkill\": \"" + teachSkill + "\"" +
                ", \"teachDesire\": \"" + teachDesire + "\"" +
                ", \"callingPref\": \"" + callingPref + "\"" +
                ", \"activities\": \"" + activities + "\"" +
                ", \"interests\": \"" + interests + "\"" +
                ", \"description\": \"" + description + "\"" +
                ", \"Status\": " + status + "" +
                ", \"publishCell\": " + JsonConvert.SerializeObject(publishCell) + "" +
                ", \"publishEmail\": " + JsonConvert.SerializeObject(publishEmail) + "" + "}";
        }

        /// <summary>
        /// Generates the json used for basic member survey information
        /// </summary>
        internal string toMembershipJSON(double WardID)
        {
            //Creates a Dictionary of residences and strings to replace address in the CSV file.
            //This will attempt to replace the string chosen in the survey with the real address designated by the clerk
			Repository r = Repository.getInstance();
            List<Residence> residences = Cache.GetList(r.ResidenceIDs(WardID), x => Cache.getCacheKey<Residence>(x),
                                                                    y => Residence.get(y));

            string streetAddress = residences.Select(x => x.residence).Contains(residence) ?
                    residences.Single(x => x.residence == residence).residence : residence;

            return "{ \"memberID\": " + memberID +
                ", \"prefName\": \"" + prefName + "\"" +
                ", \"residence\": \"" + residence + "\"" +
                ", \"streetAddress\": \"" + streetAddress + "\"" +
                ", \"cellPhone\": \"" + cellPhone + "\"" +
                ", \"publishCell\": " + JsonConvert.SerializeObject(publishCell) + "" +
                ", \"publishEmail\": " + JsonConvert.SerializeObject(publishEmail) + "" + "}";
        }
    }
}