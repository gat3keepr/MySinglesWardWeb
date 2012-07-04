using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MSW.Model;
using System.IO;
using MSW.Models.dbo;
using System.Web.Mvc;

namespace MSW.Models
{
    public class GeneratePDF
    {

        internal static void AddBishopricPage(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 14f, Font.NORMAL);
            Font fontSmall = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD);

            //Find column length
            int extraLines = 0;
            string homeAddressX = member.memberSurvey.homeAddress.Replace("\r\n"," ");
			if (member.memberSurvey.homeAddress.Length > 23)
				extraLines += (member.memberSurvey.homeAddress.Length - 23) / 37 + 1;

			string homeStakeX = member.memberSurvey.homeWardStake.Replace("\r\n", " ");
			if (member.memberSurvey.homeWardStake.Length > 25)
				extraLines += (member.memberSurvey.homeWardStake.Length - 25) / 37 + 1;

			string prevBishopX = member.memberSurvey.prevBishops.Replace("\r\n", " ");
			if (member.memberSurvey.prevBishops.Length > 19)
				extraLines += (member.memberSurvey.prevBishops.Length - 19) / 37 + 1;

			string pastCallingsX = member.memberSurvey.pastCallings.Replace("\r\n", " ");
			if (member.memberSurvey.pastCallings.Length > 22)
				extraLines += (member.memberSurvey.pastCallings.Length - 22) / 37 + 1;

			string activities = member.memberSurvey.activities.Replace("\r\n", " ");
			if (member.memberSurvey.activities.Length > 10)
				extraLines += (member.memberSurvey.activities.Length - 10) / 37 + 1;

            //First Column
            Chunk homePhone = new Chunk("Home Phone: ", fontBold);
			Chunk homePhoneData = new Chunk(member.memberSurvey.homePhone, font);
            Chunk homeAddress = new Chunk("Home Address: ", fontBold);
            Chunk homeAddressData = new Chunk(homeAddressX, font);
            Chunk homeStake = new Chunk("Home Stake: ", fontBold);
            Chunk homeStakeData = new Chunk(homeStakeX, font);
            Chunk homeBishop = new Chunk("Home Bishop: ", fontBold);
			Chunk homeBishopData = new Chunk(member.memberSurvey.homeBishop, font);
            Chunk prevBishop = new Chunk("Previous Bishops: ", fontBold);
            Chunk prevBishopData = new Chunk(prevBishopX, font);
            Chunk patBlessing = new Chunk("Patriarchial Blessing: ", fontBold);
            Chunk patBlessingData = new Chunk(member.memberSurvey.patriarchalBlessing, font);

            Chunk endowed = new Chunk("Endowed: ", fontBold);
            Chunk endowedData = new Chunk(member.memberSurvey.endowed, font);

            Chunk templRec = new Chunk("Current Temple Rec: ", fontBold);
            Chunk templRecData = new Chunk(member.memberSurvey.templeRecommend, font);

            Chunk TempleExp = new Chunk("Temple Rec Exp: ", fontBold);
			Chunk TempleExpData = new Chunk(member.memberSurvey.templeExpDate, font);

            Chunk templWorker = new Chunk("Temple Worker: ", fontBold);
            Chunk templWorkerData = new Chunk(member.memberSurvey.templeWorker, font);

            Chunk pastCallings = new Chunk("Past Callings: ", fontBold);
            Chunk pastCallingsData = new Chunk(pastCallingsX, font);




            Phrase firstCP1 = new Phrase();
            firstCP1.Add(homePhone);
            firstCP1.Add(homePhoneData);
            firstCP1.Add(Chunk.NEWLINE);
            firstCP1.Add(homeAddress);
            firstCP1.Add(homeAddressData);
            firstCP1.Add(Chunk.NEWLINE);
            firstCP1.Add(homeStake);
            firstCP1.Add(homeStakeData);
            firstCP1.Add(Chunk.NEWLINE);
            firstCP1.Add(homeBishop);
            firstCP1.Add(homeBishopData);
            firstCP1.Add(Chunk.NEWLINE);
            firstCP1.Add(prevBishop);
            firstCP1.Add(prevBishopData);
            firstCP1.Add(Chunk.NEWLINE);

            Phrase secondCP1 = new Phrase();
            secondCP1.Add(patBlessing);
            secondCP1.Add(patBlessingData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(endowed);
            secondCP1.Add(endowedData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(templRec);
            secondCP1.Add(templRecData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(TempleExp);
            secondCP1.Add(TempleExpData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(templWorker);
            secondCP1.Add(templWorkerData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(pastCallings);
            secondCP1.Add(pastCallingsData);
            secondCP1.Add(Chunk.NEWLINE);

            Phrase thirdCP1 = new Phrase();
            //Lock down second column

            if (extraLines > 9)
            {
                try { activities = activities.Substring(0, activities.Length - ((extraLines - 9) * 36)); }
                catch { }
            }
            Chunk Activities = new Chunk("Suggested Ward Activities: ", fontBold);
            Chunk ActivitiesData = new Chunk(activities, font);
            thirdCP1.Add(Activities);
            thirdCP1.Add(ActivitiesData);
           /* for (int i = 0; i < 28 - extraLines - 17; i++)
            {
                thirdCP1.Add(new Chunk(Chunk.NEWLINE));
            }*/


            Paragraph firstColumn1 = new Paragraph();
            firstColumn1.Add(firstCP1);
            firstColumn1.Leading = 20f;
            firstColumn1.SpacingAfter = 18f;
            Paragraph firstColumn2 = new Paragraph();
            firstColumn2.Add(secondCP1);
            firstColumn2.Leading = 20f;
            firstColumn2.SpacingAfter = 18f;
            Paragraph firstColumn3 = new Paragraph();
            firstColumn3.Add(thirdCP1);
            firstColumn3.Leading = 18f;

            PdfPCell column1 = new PdfPCell();
            column1.BorderColor = new BaseColor(255, 255, 255);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");                
            }
            picture.SpacingAfter = 12f;

            column1.AddElement(picture);
            column1.AddElement(firstColumn1);
            column1.AddElement(firstColumn2);
            column1.AddElement(firstColumn3);


            //Second Column
			Paragraph heading = new Paragraph(member.memberSurvey.prefName + " "
				+ member.user.LastName, new Font(Font.FontFamily.HELVETICA, 28f, Font.BOLD));
            heading.Add(new Chunk(Chunk.NEWLINE));
            heading.Add(new Chunk(member.user.FirstName + " " + member.user.LastName, fontSmall));
            heading.SpacingAfter = 18f;
            Chunk apartment = new Chunk("Residence: ", fontBold);
			Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Chunk birthday = new Chunk("Birthday: ", fontBold);
			Chunk birthdayData = new Chunk(member.memberSurvey.birthday, font);
            Chunk Email = new Chunk("Email: ", fontBold);
			Chunk EmailData = new Chunk(member.user.Email, font);
            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
			Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Chunk priesthood = new Chunk("Priesthood Office: ", fontBold);
			Chunk priesthoodData = new Chunk(member.memberSurvey.priesthood, font);
            Chunk missioninfo = new Chunk("Mission Info: ", fontBold);
            Chunk missioninfoData = new Chunk(member.memberSurvey.missionInformation, font);
            Chunk timeInWard = new Chunk("Staying in Ward: ", fontBold);
			Chunk timeInWardData = new Chunk(member.memberSurvey.timeInWard, font);
            Chunk school = new Chunk("School Info: ", fontBold);
            Chunk schoolData = new Chunk(member.memberSurvey.schoolInfo, font);
            Chunk religionClass = new Chunk("Religion Class: ", fontBold);
			Chunk religionClassData = new Chunk(member.memberSurvey.religionClass, font);
            Chunk employed = new Chunk("Employed: ", fontBold);
			Chunk employedData = new Chunk(member.memberSurvey.employed, font);
            Chunk employer = new Chunk("Employer: ", fontBold);
			Chunk employerData = new Chunk(member.memberSurvey.occupation, font);
            Chunk musicSkill = new Chunk("Music Skill: ", fontBold);
			Chunk musicSkillData = new Chunk(member.memberSurvey.musicSkill.ToString(), font);
            Chunk musicAbility = new Chunk("Music Ability: ", fontBold);
			Chunk musicAbilityData = new Chunk(member.memberSurvey.musicTalent, font);
            Chunk teachingSkills = new Chunk("Teaching Skills: ", fontBold);
			Chunk teachingSkillsData = new Chunk(member.memberSurvey.teachSkill.ToString(), font);
            Chunk teachingDesire = new Chunk("Teaching Desire: ", fontBold);
			Chunk teachingDesireData = new Chunk(member.memberSurvey.teachDesire.ToString(), font);
            Chunk CallingPref = new Chunk("Area to Serve: ", fontBold);
			Chunk CallingPrefData = new Chunk(member.memberSurvey.callingPref, font);
            Chunk Description = new Chunk("Self Description: ", fontBold);
			string Descriptionription = member.memberSurvey.description;
            if (Descriptionription == null)
                Descriptionription = " ";
            Descriptionription = Descriptionription.Replace("\r\n", " ");
			if (member.memberSurvey.description.Length > 100)
            {
                Descriptionription = Descriptionription.Substring(0, 90);
                Descriptionription = Descriptionription + "... See website for more";
            }
            Chunk DescriptionData = new Chunk(Descriptionription, font);

            string InterestsString = member.memberSurvey.interests;
            if (InterestsString == null)
                InterestsString = " ";
            InterestsString = InterestsString.Replace("\r\n", " ");
            if (InterestsString.Length > 100)
            {
                InterestsString = InterestsString.Substring(0, 90);
                InterestsString = InterestsString + "... See website for more";
            }
            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(InterestsString, font);

            Chunk EmergContact = new Chunk("Emergency Contact: ", fontBold);
            Chunk EmergContactData = new Chunk(member.memberSurvey.emergContact + " ", font);
            Chunk EmergPhoneData = new Chunk(member.memberSurvey.emergPhone, font);

            Phrase firstCP2 = new Phrase();
            firstCP2.Add(apartment);
            firstCP2.Add(apartmentData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(birthday);
            firstCP2.Add(birthdayData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(Email);
            firstCP2.Add(EmailData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(cellPhone);
            firstCP2.Add(cellPhoneData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(priesthood);
            firstCP2.Add(priesthoodData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(missioninfo);
            firstCP2.Add(missioninfoData);
            firstCP2.Add(Chunk.NEWLINE);

            Phrase secondCP2 = new Phrase();
            secondCP2.Add(timeInWard);
            secondCP2.Add(timeInWardData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(school);
            secondCP2.Add(schoolData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(religionClass);
            secondCP2.Add(religionClassData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(employed);
            secondCP2.Add(employedData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(employer);
            secondCP2.Add(employerData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(musicSkill);
            secondCP2.Add(musicSkillData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(musicAbility);
            secondCP2.Add(musicAbilityData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(teachingSkills);
            secondCP2.Add(teachingSkillsData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(teachingDesire);
            secondCP2.Add(teachingDesireData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(CallingPref);
            secondCP2.Add(CallingPrefData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(EmergContact);
            secondCP2.Add(EmergContactData);
            secondCP2.Add(EmergPhoneData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(Description);
            secondCP2.Add(DescriptionData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(interests);
            secondCP2.Add(interestsData);
            secondCP2.Add(Chunk.NEWLINE);

            Paragraph secondColumn1 = new Paragraph();
            secondColumn1.Add(heading);
            secondColumn1.Leading = 20f;
            secondColumn1.SpacingAfter = 18f;
            Paragraph secondColumn2 = new Paragraph();
            secondColumn2.Add(firstCP2);
            secondColumn2.Leading = 20f;
            secondColumn2.SpacingAfter = 18f;
            Paragraph secondColumn3 = new Paragraph();
            secondColumn3.Add(secondCP2);
            secondColumn3.Leading = 20f;

            PdfPCell column2 = new PdfPCell();
            column2.BorderColor = new BaseColor(255, 255, 255);
            column2.PaddingLeft = 15;
            
            column2.AddElement(secondColumn1);
            column2.AddElement(secondColumn2);
            column2.AddElement(secondColumn3);

            PdfPTable memberTable = new PdfPTable(2);
            memberTable.TotalWidth = 525f;
            memberTable.LockedWidth = true;

            memberTable.AddCell(column1);
            memberTable.AddCell(column2);

            doc.Add(memberTable);
        }

        internal static void AddAuxInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 14f, Font.NORMAL);
            Font fontSmall = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD);

            //Find column length
            int extraLines = 0;
            if (member.memberSurvey.homeAddress.Length > 23)
                extraLines += (member.memberSurvey.homeAddress.Length - 23) / 37 + 1;
            /*if (member.memberSurvey.homeStake.Length > 25)
                extraLines += (member.memberSurvey.homeStake.Length - 25) / 37 + 1;
            if (member.memberSurvey.prevBishop.Length > 19)
                extraLines += (member.memberSurvey.prevBishop.Length - 19) / 37 + 1;*/
            if (member.memberSurvey.pastCallings.Length > 22)
                extraLines += (member.memberSurvey.pastCallings.Length - 22) / 37 + 1;
            if (member.memberSurvey.activities.Length > 10)
                extraLines += (member.memberSurvey.activities.Length - 10) / 37 + 1;

            //First Column
            Chunk homePhone = new Chunk("Home Phone: ", fontBold);
            Chunk homePhoneData = new Chunk(member.memberSurvey.homePhone, font);
            Chunk homeAddress = new Chunk("Home Address: ", fontBold);
            Chunk homeAddressData = new Chunk(member.memberSurvey.homeAddress, font);
            Chunk patBlessing = new Chunk("Patriarchial Blessing: ", fontBold);
            Chunk patBlessingData = new Chunk(member.memberSurvey.patriarchalBlessing, font);

            Chunk endowed = new Chunk("Endowed: ", fontBold);
            Chunk endowedData = new Chunk(member.memberSurvey.endowed, font);

            Chunk templRec = new Chunk("Current Temple Rec: ", fontBold);
            Chunk templRecData = new Chunk(member.memberSurvey.templeRecommend, font);

            Chunk TempleExp = new Chunk("Temple Rec Exp: ", fontBold);
            Chunk TempleExpData = new Chunk(member.memberSurvey.templeExpDate, font);

            Chunk templWorker = new Chunk("Temple Worker: ", fontBold);
            Chunk templWorkerData = new Chunk(member.memberSurvey.templeWorker, font);

            Chunk pastCallings = new Chunk("Past Callings: ", fontBold);
            Chunk pastCallingsData = new Chunk(member.memberSurvey.pastCallings, font);

            Phrase firstCP1 = new Phrase();
            firstCP1.Add(homePhone);
            firstCP1.Add(homePhoneData);
            firstCP1.Add(Chunk.NEWLINE);
            firstCP1.Add(homeAddress);
            firstCP1.Add(homeAddressData);
            firstCP1.Add(Chunk.NEWLINE);


            Phrase secondCP1 = new Phrase();
            secondCP1.Add(patBlessing);
            secondCP1.Add(patBlessingData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(endowed);
            secondCP1.Add(endowedData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(templRec);
            secondCP1.Add(templRecData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(TempleExp);
            secondCP1.Add(TempleExpData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(templWorker);
            secondCP1.Add(templWorkerData);
            secondCP1.Add(Chunk.NEWLINE);
            secondCP1.Add(pastCallings);
            secondCP1.Add(pastCallingsData);
            secondCP1.Add(Chunk.NEWLINE);

            Phrase thirdCP1 = new Phrase();
            //Lock down second column
            string activities = member.memberSurvey.activities;

            if (extraLines > 9)
            {
                try { activities = activities.Substring(0, activities.Length - ((extraLines - 9) * 37)); }
                catch { }
            }
            Chunk Activities = new Chunk("Suggested Ward Activities: ", fontBold);
            Chunk ActivitiesData = new Chunk(activities, font);
            thirdCP1.Add(Activities);
            thirdCP1.Add(ActivitiesData);
            /*for (int i = 0; i < 28 - extraLines - 12; i++)
            {
                thirdCP1.Add(new Chunk(Chunk.NEWLINE));
            }*/


            Paragraph firstColumn1 = new Paragraph();
            firstColumn1.Add(firstCP1);
            firstColumn1.Leading = 20f;
            firstColumn1.SpacingAfter = 18f;
            Paragraph firstColumn2 = new Paragraph();
            firstColumn2.Add(secondCP1);
            firstColumn2.Leading = 20f;
            firstColumn2.SpacingAfter = 18f;
            Paragraph firstColumn3 = new Paragraph();
            firstColumn3.Add(thirdCP1);
            firstColumn3.Leading = 18f;

            PdfPCell column1 = new PdfPCell();
            column1.BorderColor = new BaseColor(255, 255, 255);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
            }
            picture.SpacingAfter = 12f;

            column1.AddElement(picture);
            column1.AddElement(firstColumn1);
            column1.AddElement(firstColumn2);
            column1.AddElement(firstColumn3);


            //Second Column
            Paragraph heading = new Paragraph(member.memberSurvey.prefName + " "
                + member.user.LastName, new Font(Font.FontFamily.HELVETICA, 28f, Font.BOLD));
            heading.Add(new Chunk(Chunk.NEWLINE));
            heading.Add(new Chunk(member.user.FirstName + " " + member.user.LastName, fontSmall));
            heading.SpacingAfter = 18f;
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Chunk birthday = new Chunk("Birthday: ", fontBold);
            Chunk birthdayData = new Chunk(member.memberSurvey.birthday, font);
            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Chunk priesthood = new Chunk("Priesthood Office: ", fontBold);
            Chunk priesthoodData = new Chunk(member.memberSurvey.priesthood, font);
            Chunk missioninfo = new Chunk("Mission Info: ", fontBold);
            Chunk missioninfoData = new Chunk(member.memberSurvey.missionInformation, font);
            Chunk timeInWard = new Chunk("Staying in Ward: ", fontBold);
            Chunk timeInWardData = new Chunk(member.memberSurvey.timeInWard, font);
            Chunk school = new Chunk("School Info: ", fontBold);
            Chunk schoolData = new Chunk(member.memberSurvey.schoolInfo, font);
            Chunk religionClass = new Chunk("Religion Class: ", fontBold);
            Chunk religionClassData = new Chunk(member.memberSurvey.religionClass, font);
            Chunk employed = new Chunk("Employed: ", fontBold);
            Chunk employedData = new Chunk(member.memberSurvey.employed, font);
            Chunk employer = new Chunk("Employer: ", fontBold);
            Chunk employerData = new Chunk(member.memberSurvey.occupation, font);
            Chunk musicSkill = new Chunk("Music Skill: ", fontBold);
            Chunk musicSkillData = new Chunk(member.memberSurvey.musicSkill.ToString(), font);
            Chunk musicAbility = new Chunk("Music Ability: ", fontBold);
            Chunk musicAbilityData = new Chunk(member.memberSurvey.musicTalent, font);
            Chunk teachingSkills = new Chunk("Teaching Skills: ", fontBold);
            Chunk teachingSkillsData = new Chunk(member.memberSurvey.teachSkill.ToString(), font);
            Chunk teachingDesire = new Chunk("Teaching Desire: ", fontBold);
            Chunk teachingDesireData = new Chunk(member.memberSurvey.teachDesire.ToString(), font);
            Chunk CallingPref = new Chunk("Area to Serve: ", fontBold);
            Chunk CallingPrefData = new Chunk(member.memberSurvey.callingPref, font);
            Chunk Description = new Chunk("Self Description: ", fontBold);
            string Descriptionription = member.memberSurvey.description;
            if (Descriptionription == null)
                Descriptionription = " ";
            Descriptionription = Descriptionription.Replace("\r\n", " ");
            if (member.memberSurvey.description.Length > 100)
            {
                Descriptionription = Descriptionription.Substring(0, 90);
                Descriptionription = Descriptionription + "... See website for more";
            }
            Chunk DescriptionData = new Chunk(Descriptionription, font);

            string InterestsString = member.memberSurvey.interests;
            if (InterestsString == null)
                InterestsString = " ";
            InterestsString = InterestsString.Replace("\r\n", " ");
            if (InterestsString.Length > 100)
            {
                InterestsString = InterestsString.Substring(0, 90);
                InterestsString = InterestsString + "... See website for more";
            }
            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(InterestsString, font);
            Chunk EmergContact = new Chunk("Emergency Contact: ", fontBold);
            Chunk EmergContactData = new Chunk(member.memberSurvey.emergContact + " ", font);
            Chunk EmergPhoneData = new Chunk(member.memberSurvey.emergPhone, font);

            Phrase firstCP2 = new Phrase();
            firstCP2.Add(apartment);
            firstCP2.Add(apartmentData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(birthday);
            firstCP2.Add(birthdayData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(Email);
            firstCP2.Add(EmailData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(cellPhone);
            firstCP2.Add(cellPhoneData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(priesthood);
            firstCP2.Add(priesthoodData);
            firstCP2.Add(Chunk.NEWLINE);
            firstCP2.Add(missioninfo);
            firstCP2.Add(missioninfoData);
            firstCP2.Add(Chunk.NEWLINE);

            Phrase secondCP2 = new Phrase();
            secondCP2.Add(timeInWard);
            secondCP2.Add(timeInWardData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(school);
            secondCP2.Add(schoolData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(religionClass);
            secondCP2.Add(religionClassData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(employed);
            secondCP2.Add(employedData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(employer);
            secondCP2.Add(employerData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(musicSkill);
            secondCP2.Add(musicSkillData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(musicAbility);
            secondCP2.Add(musicAbilityData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(teachingSkills);
            secondCP2.Add(teachingSkillsData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(teachingDesire);
            secondCP2.Add(teachingDesireData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(CallingPref);
            secondCP2.Add(CallingPrefData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(EmergContact);
            secondCP2.Add(EmergContactData);
            secondCP2.Add(EmergPhoneData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(Description);
            secondCP2.Add(DescriptionData);
            secondCP2.Add(Chunk.NEWLINE);
            secondCP2.Add(interests);
            secondCP2.Add(interestsData);
            secondCP2.Add(Chunk.NEWLINE);


            Paragraph secondColumn1 = new Paragraph();
            secondColumn1.Add(heading);
            secondColumn1.Leading = 20f;
            secondColumn1.SpacingAfter = 18f;
            Paragraph secondColumn2 = new Paragraph();
            secondColumn2.Add(firstCP2);
            secondColumn2.Leading = 20f;
            secondColumn2.SpacingAfter = 18f;
            Paragraph secondColumn3 = new Paragraph();
            secondColumn3.Add(secondCP2);
            secondColumn3.Leading = 20f;

            PdfPCell column2 = new PdfPCell();
            column2.BorderColor = new BaseColor(255, 255, 255);
            column2.PaddingLeft = 15;

            column2.AddElement(secondColumn1);
            column2.AddElement(secondColumn2);
            column2.AddElement(secondColumn3);

            PdfPTable memberTable = new PdfPTable(2);
            memberTable.TotalWidth = 525f;
            memberTable.LockedWidth = true;

            memberTable.AddCell(column1);
            memberTable.AddCell(column2);

            doc.Add(memberTable);
        }

        internal static void AddActivitesInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk musicSkill = new Chunk("Music Skill: ", fontBold);
            Chunk musicSkillData = new Chunk(member.memberSurvey.musicSkill.ToString(), font);
            Phrase musicPhrase = new Phrase();
            musicPhrase.Add(musicSkill);
            musicPhrase.Add(musicSkillData);
            Chunk musicAbility = new Chunk("Music Ability: ", fontBold);
            Chunk musicAbilityData = new Chunk(member.memberSurvey.musicTalent, font);
            Phrase musicAPhrase = new Phrase();
            musicAPhrase.Add(musicAbility);
            musicAPhrase.Add(musicAbilityData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }

            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Chunk Activities = new Chunk("Suggested Ward Activities: ", fontBold);
            Chunk ActivitiesData = new Chunk(member.memberSurvey.activities, font);
            Phrase actPhrase = new Phrase();
            actPhrase.Add(Activities);
            actPhrase.Add(ActivitiesData);

            

            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            nested.AddCell(new Phrase(musicPhrase));
            nested.AddCell(new Phrase(musicAPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(InterestsPhrase));
            nested2.AddCell(new Phrase(actPhrase));            
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);

        }

        internal static void AddEmergencyInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName + " (" + member.user.FirstName + " " + member.user.LastName + ")", fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk EmergContact= new Chunk("Emergency Contact: ", fontBold);
            Chunk EmergContactData = new Chunk(member.memberSurvey.emergContact + " " + member.memberSurvey.emergPhone, font);
            Phrase EmergPhrase = new Phrase();
            EmergPhrase.Add(EmergContact);
            EmergPhrase.Add(EmergContactData);

            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }

            Chunk homeBishop = new Chunk("Home Bishop: ", fontBold);
            Chunk homeBishopData = new Chunk(member.memberSurvey.homeBishop, font);
            Phrase homeBishopPhrase = new Phrase();
            homeBishopPhrase.Add(homeBishop);
            homeBishopPhrase.Add(homeBishopData);

            Chunk homePhone = new Chunk("Home Phone: ", fontBold);
            Chunk homePhoneData = new Chunk(member.memberSurvey.homePhone, font);
            Phrase homePhonePhrase = new Phrase();
            homePhonePhrase.Add(homePhone);
            homePhonePhrase.Add(homePhoneData);

            Chunk homeAddress = new Chunk("Home Address: ", fontBold);
            Chunk homeAddressData = new Chunk(member.memberSurvey.homeAddress, font);
            Phrase homeAddressPhrase = new Phrase();
            homeAddressPhrase.Add(homeAddress);
            homeAddressPhrase.Add(homeAddressData);
            
            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            nested.AddCell(new Phrase(EmergPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(homeBishopPhrase));
            nested2.AddCell(new Phrase(homeAddressPhrase));
            nested2.AddCell(new Phrase(homePhonePhrase));
            nested2.AddCell(new Phrase(InterestsPhrase));
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);

        }

        internal static void AddEmploymentInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);
            
            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }
           
            Chunk school = new Chunk("School Info: ", fontBold);
            Chunk schoolData = new Chunk(member.memberSurvey.schoolInfo, font);
            Phrase schoolPhrase = new Phrase();
            schoolPhrase.Add(school);
            schoolPhrase.Add(schoolData);
           
            Chunk employed = new Chunk("Employed: ", fontBold);
            Chunk employedData = new Chunk(member.memberSurvey.employed, font);
            Phrase employedPhrase = new Phrase();
            employedPhrase.Add(employed);
            employedPhrase.Add(employedData);

            Chunk employer = new Chunk("Employer: ", fontBold);
            Chunk employerData = new Chunk(member.memberSurvey.occupation, font);
            Phrase employerPhrase = new Phrase();
            employerPhrase.Add(employer);
            employerPhrase.Add(employerData);
            
            Chunk Description = new Chunk("Self Description: ", fontBold);
            Chunk DescriptionData = new Chunk(member.memberSurvey.description, font);
            Phrase descPhrase = new Phrase();
            descPhrase.Add(Description);
            descPhrase.Add(DescriptionData);

            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(schoolPhrase));
            nested2.AddCell(new Phrase(employedPhrase));
            nested2.AddCell(new Phrase(employerPhrase));
            nested2.AddCell(new Phrase(InterestsPhrase));
            
            
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);
        }

        internal static void AddFamHistInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }

            Chunk Activities = new Chunk("Suggested Ward Activities: ", fontBold);
            Chunk ActivitiesData = new Chunk(member.memberSurvey.activities, font);
            Phrase actPhrase = new Phrase();
            actPhrase.Add(Activities);
            actPhrase.Add(ActivitiesData);
           
            
            Chunk CallingPref = new Chunk("Area to Serve: ", fontBold);
            Chunk CallingPrefData = new Chunk(member.memberSurvey.callingPref, font);
            Phrase CallingPrefPhrase = new Phrase();
            CallingPrefPhrase.Add(CallingPref);
            CallingPrefPhrase.Add(CallingPrefData);
            

            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            nested.AddCell(new Phrase(InterestsPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(actPhrase));
            nested2.AddCell(new Phrase(CallingPrefPhrase));
            
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);
            
        }

        internal static void AddFHEInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk musicSkill = new Chunk("Music Skill: ", fontBold);
            Chunk musicSkillData = new Chunk(member.memberSurvey.musicSkill.ToString(), font);
            Phrase musicPhrase = new Phrase();
            musicPhrase.Add(musicSkill);
            musicPhrase.Add(musicSkillData);
            Chunk musicAbility = new Chunk("Music Ability: ", fontBold);
            Chunk musicAbilityData = new Chunk(member.memberSurvey.musicTalent, font);
            Phrase musicAPhrase = new Phrase();
            musicAPhrase.Add(musicAbility);
            musicAPhrase.Add(musicAbilityData);

            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }
           
            Chunk teachingSkills = new Chunk("Teaching Skills: ", fontBold);
            Chunk teachingSkillsData = new Chunk(member.memberSurvey.teachSkill.ToString(), font);
            Phrase tSkillsPhrase = new Phrase();
            tSkillsPhrase.Add(teachingSkills);
            tSkillsPhrase.Add(teachingSkillsData);

            Chunk teachingDesire = new Chunk("Teaching Desire: ", fontBold);
            Chunk teachingDesireData = new Chunk(member.memberSurvey.teachDesire.ToString(), font);
            Phrase tDesirePhrase = new Phrase();
            tDesirePhrase.Add(teachingDesire);
            tDesirePhrase.Add(teachingDesireData);

            Chunk Description = new Chunk("Self Description: ", fontBold);
            Chunk DescriptionData = new Chunk(member.memberSurvey.description, font);
            Phrase descPhrase = new Phrase();
            descPhrase.Add(Description);
            descPhrase.Add(DescriptionData);

            Chunk Activities = new Chunk("Suggested Ward Activities: ", fontBold);
            Chunk ActivitiesData = new Chunk(member.memberSurvey.activities, font);
            Phrase actPhrase = new Phrase();
            actPhrase.Add(Activities);
            actPhrase.Add(ActivitiesData);

            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            nested.AddCell(new Phrase(musicPhrase));
            nested.AddCell(new Phrase(musicAPhrase));
            nested.AddCell(new Phrase(descPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(tSkillsPhrase));
            nested2.AddCell(new Phrase(tDesirePhrase));
            nested2.AddCell(new Phrase(InterestsPhrase));
            nested2.AddCell(new Phrase(actPhrase));
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);
            
        }
        internal static void AddInstituteInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }
           
            Chunk religionClass = new Chunk("Religion Class: ", fontBold);
            Chunk religionClassData = new Chunk(member.memberSurvey.religionClass, font);
            Phrase relPhrase = new Phrase();
            relPhrase.Add(religionClass);
            relPhrase.Add(religionClassData);


            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(relPhrase));
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);

        }
        internal static void AddMissionInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk pastCallings = new Chunk("Past Callings: ", fontBold);
            Chunk pastCallingsData = new Chunk(member.memberSurvey.pastCallings, font);
            Phrase callingPhrase = new Phrase();
            callingPhrase.Add(pastCallings);
            callingPhrase.Add(pastCallingsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }

            
            Chunk missioninfo = new Chunk("Mission Info: ", fontBold);
            Chunk missioninfoData = new Chunk(member.memberSurvey.missionInformation, font);
            Phrase missionPhrase = new Phrase();
            missionPhrase.Add(missioninfo);
            missionPhrase.Add(missioninfoData);

            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(missionPhrase));
            nested2.AddCell(new Phrase(callingPhrase));
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);

        }

        internal static void AddMusicInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }
            
            Chunk musicSkill = new Chunk("Music Skill: ", fontBold);
            Chunk musicSkillData = new Chunk(member.memberSurvey.musicSkill.ToString(), font);
            Phrase musicPhrase = new Phrase();
            musicPhrase.Add(musicSkill);
            musicPhrase.Add(musicSkillData);
            Chunk musicAbility = new Chunk("Music Ability: ", fontBold);
            Chunk musicAbilityData = new Chunk(member.memberSurvey.musicTalent, font);
            Phrase musicAPhrase = new Phrase();
            musicAPhrase.Add(musicAbility);
            musicAPhrase.Add(musicAbilityData);
           
            Chunk CallingPref = new Chunk("Area to Serve: ", fontBold);
            Chunk CallingPrefData = new Chunk(member.memberSurvey.callingPref, font);
            Phrase areaPhrase = new Phrase();
            areaPhrase.Add(CallingPref);
            areaPhrase.Add(CallingPrefData);

            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            nested.AddCell(new Phrase(InterestsPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(musicPhrase));
            nested2.AddCell(new Phrase(musicAPhrase));
            nested2.AddCell(new Phrase(areaPhrase));
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);

        }
        internal static void AddTeachingInfo(MemberModel member, Document doc)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);
            Chunk apartment = new Chunk("Residence: ", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("Email: ", fontBold);
            Chunk EmailData = new Chunk(member.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("Cell Phone: ", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.cellPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Chunk interests = new Chunk("Interests & Hobbies: ", fontBold);
            Chunk interestsData = new Chunk(member.memberSurvey.interests, font);
            Phrase InterestsPhrase = new Phrase();
            InterestsPhrase.Add(interests);
            InterestsPhrase.Add(interestsData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }


            
            Chunk teachingSkills = new Chunk("Teaching Skills: ", fontBold);
            Chunk teachingSkillsData = new Chunk(member.memberSurvey.teachSkill.ToString(), font);
            Phrase tSkillPhrase = new Phrase();
            tSkillPhrase.Add(teachingSkills);
            tSkillPhrase.Add(teachingSkillsData);
            Chunk teachingDesire = new Chunk("Teaching Desire: ", fontBold);
            Chunk teachingDesireData = new Chunk(member.memberSurvey.teachDesire.ToString(), font);
            Phrase tDesirePhrase = new Phrase();
            tDesirePhrase.Add(teachingDesire);
            tDesirePhrase.Add(teachingDesireData);
            Chunk CallingPref = new Chunk("Area to Serve: ", fontBold);
            Chunk CallingPrefData = new Chunk(member.memberSurvey.callingPref, font);
            Phrase areaPhrase = new Phrase();
            areaPhrase.Add(CallingPref);
            areaPhrase.Add(CallingPrefData);

            //Build Table
            PdfPTable table = new PdfPTable(3);
            table.TotalWidth = 400f;
            table.LockedWidth = true;
            PdfPCell header = new PdfPCell(new Phrase(name));
            header.Colspan = 3;
            table.AddCell(header);
            table.AddCell(picture);
            PdfPTable nested = new PdfPTable(1);
            nested.AddCell(new Phrase(ApartmentPhrase));
            nested.AddCell(new Phrase(CellPhrase));
            nested.AddCell(new Phrase(EmailPhrase));
            nested.AddCell(new Phrase(InterestsPhrase));
            PdfPCell nesthousing = new PdfPCell(nested);
            nesthousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing);
            PdfPTable nested2 = new PdfPTable(1);
            nested2.AddCell(new Phrase(tSkillPhrase));
            nested2.AddCell(new Phrase(tDesirePhrase));
            nested2.AddCell(new Phrase(areaPhrase));
            PdfPCell nesthousing2 = new PdfPCell(nested2);
            nesthousing.Padding = 0f;
            table.AddCell(nesthousing2);
            doc.Add(table);

        }


        internal static void AddWardMember(MemberModel member, PdfPTable members)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontName = new Font(Font.FontFamily.HELVETICA, 14f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(member.memberSurvey.prefName + " "
                + member.user.LastName, fontBold);

            Chunk apartment = new Chunk("", fontBold);
            Chunk apartmentData = new Chunk(member.memberSurvey.residence, font);
            Phrase ApartmentPhrase = new Phrase();
            ApartmentPhrase.Add(apartment);
            ApartmentPhrase.Add(apartmentData);

            Chunk Email = new Chunk("", fontBold);
            Chunk EmailData = new Chunk(member.memberSurvey.publishEmail ? member.user.Email : "", font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("", fontBold);
            Chunk cellPhoneData = new Chunk(member.memberSurvey.publishCell ? member.memberSurvey.cellPhone : "", font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);

            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(member.user.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }

            //Build Table
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 300f;
            table.LockedWidth = true;

            PdfPCell pictureCell = new PdfPCell(picture);
            pictureCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            table.AddCell(pictureCell);

            
            PdfPTable nested = new PdfPTable(1);
            nested.TotalWidth = 150f;
            nested.LockedWidth = true;

            PdfPCell nameCell = new PdfPCell(new Phrase(member.memberSurvey.prefName + " "
                + member.user.LastName, fontName));
            nameCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(nameCell);

            PdfPCell residenceCell = new PdfPCell(new Phrase(ApartmentPhrase));
            residenceCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(residenceCell);

            PdfPCell phoneCell = new PdfPCell(new Phrase(CellPhrase));
            phoneCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(phoneCell);

            PdfPCell emailCell = new PdfPCell(new Phrase(EmailPhrase));
            emailCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(emailCell);

            PdfPCell nestedHousing = new PdfPCell(nested);
            nestedHousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);

            nestedHousing.PaddingLeft = -120;
            table.AddCell(nestedHousing);
            table.SpacingAfter = 5;

            PdfPCell tableHousing = new PdfPCell(table);
            tableHousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            tableHousing.PaddingLeft = 60;
            members.AddCell(tableHousing);
        }

        internal static void _AddBishopricMember(BishopricModel user, PdfPTable bishopric)
        {
            Font font = new Font(Font.FontFamily.HELVETICA, 10f, Font.NORMAL);
            Font fontName = new Font(Font.FontFamily.HELVETICA, 14f, Font.NORMAL);
            Font fontBold = new Font(Font.FontFamily.HELVETICA, 10f, Font.BOLD);

            Chunk name = new Chunk(user.data.BishopricName, fontBold);

            Chunk calling = new Chunk("", fontBold);
            Chunk callingData = new Chunk(user.data.BishopricCalling, font);
            Phrase callingPhrase = new Phrase();
            callingPhrase.Add(calling);
            callingPhrase.Add(callingData);

            Chunk Email = new Chunk("", fontBold);
            Chunk EmailData = new Chunk(user.user.Email, font);
            Phrase EmailPhrase = new Phrase();
            EmailPhrase.Add(Email);
            EmailPhrase.Add(EmailData);

            Chunk cellPhone = new Chunk("", fontBold);
            Chunk cellPhoneData = new Chunk(user.data.BishopricPhone, font);
            Phrase CellPhrase = new Phrase();
            CellPhrase.Add(cellPhone);
            CellPhrase.Add(cellPhoneData);
            
            Image picture = null;
            try
            {
                picture = Image.GetInstance(_getPhoto(user.data.MemberID));
                picture.ScaleAbsolute(80, 80);
            }
            catch
            {
                picture = Image.GetInstance(AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg");
                picture.ScaleAbsolute(80, 80);
            }

            //Build Table
            PdfPTable table = new PdfPTable(2);
            table.TotalWidth = 300f;
            table.LockedWidth = true;

            PdfPCell pictureCell = new PdfPCell(picture);
            pictureCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            table.AddCell(pictureCell);


            PdfPTable nested = new PdfPTable(1);
            nested.TotalWidth = 150f;
            nested.LockedWidth = true;

			PdfPCell nameCell = new PdfPCell(new Phrase(user.data.BishopricName, fontName));
            nameCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(nameCell);

            PdfPCell callingCell = new PdfPCell(new Phrase(callingPhrase));
            callingCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(callingCell);

            PdfPCell phoneCell = new PdfPCell(new Phrase(CellPhrase));
            phoneCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(phoneCell);

            PdfPCell emailCell = new PdfPCell(new Phrase(EmailPhrase));
            emailCell.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            nested.AddCell(emailCell);

            PdfPCell nestedHousing = new PdfPCell(nested);
            nestedHousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);

            nestedHousing.PaddingLeft = -120;
            table.AddCell(nestedHousing);
            table.SpacingAfter = 5;

            PdfPCell tableHousing = new PdfPCell(table);
            tableHousing.BorderColor = new iTextSharp.text.BaseColor(255, 255, 255);
            tableHousing.PaddingLeft = 60;
            bishopric.AddCell(tableHousing);
        }

        private static string _getPhoto(int MemberID)
        {
			try
			{
				Photo photo = Photo.getPhoto(MemberID);
				return AppDomain.CurrentDomain.BaseDirectory + "Photo/" + photo.FileName;
			}
			catch
			{
				return AppDomain.CurrentDomain.BaseDirectory + "Photo/profile-1.jpg";
			}
        }
    }
}