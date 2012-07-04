using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using MSW.Model;
using MSW;
using System.IO;
using System.Net.Mail;
using MSW.Models;
using MSW.Models.dbo;
using System.Globalization;

namespace MSW.Utilities
{
    public class MSWtools
    {
        /// <summary>
        /// Emails me when an exception occurs. Not all exceptions are sent.
        /// REMOVE when logging is implemented.
        /// </summary>
        public static void _sendException(Exception e, string username = "")
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress("gatekeeper@mysinglesward.com", "Porter Hoskins"));
            message.From = new MailAddress("no-reply@mysinglesward.com", "MySinglesWard Exception");
            message.Subject = "Error";
            message.IsBodyHtml = true;
            message.Body = "Here is the exception:\n" + e + "\n" + e.Message + "/n" + e.StackTrace + "<br />Username: " + username;

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            try
            {
                client.Send(message);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Emails the bishopric user his ward password
        /// </summary>
        public static void _WardPasswordRecover(string password, string email)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress("no-reply@mysinglesward.com", "MySinglesWard");
            message.Subject = "Ward Password Recover";
            message.IsBodyHtml = true;
            message.Body = "Here is your ward password: " + password;

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Send(message);
        }

        /// <summary>
        /// Emails the stake user his stake password
        /// </summary>
        public static void _StakePasswordRecover(string password, string email)
        {
            password = Utilities.Cryptography.DecryptString(password);
            email = Utilities.Cryptography.DecryptString(email);

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress("no-reply@mysinglesward.com", "MySinglesWard");
            message.Subject = "Stake Password Recover";
            message.IsBodyHtml = true;
            message.Body = "<img src=\'http://www.mysinglesward.com/Content/images/banner_email.jpg\' alt=\'MySinglesWard.com\'><br/>Here is your stake password: " + password;

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Send(message);
        }

        /// <summary>
        /// Emails a user his/her username and password for recovery purposes
        /// </summary>
        public static void _EmailPassword(string username, string newPassword, string email)
        {
            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            message.To.Add(new MailAddress(email));
            message.From = new MailAddress("no-reply@mysinglesward.com", "MySinglesWard");
            message.Subject = "Password Reset";
            message.IsBodyHtml = true;
            message.Body = "<img src=\'http://www.mysinglesward.com/Content/images/banner_email.jpg\' alt=\'MySinglesWard.com\'><br/>Here is your email: <strong>"
                + email + "</strong><br />Here is your new password: <strong>" + newPassword + "</strong><br /><br />Please change your password after you log in.";

            SmtpClient client = new SmtpClient();
            client.EnableSsl = true;
            client.Send(message);
        }

        /// <summary>
        /// Emails a new member a welcome message with a record of their username
        /// </summary>
        public static void _EmailNewMember(string email, string username)
        {
            try
            {
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.Bcc.Add(new MailAddress(email));
                message.From = new MailAddress("no-reply@mysinglesward.com", "MySinglesWard");
                message.Subject = "Welcome to MySinglesWard.com";
                message.IsBodyHtml = true;
                message.Body = "<img src=\'http://www.mysinglesward.com/Content/images/banner_email.jpg\' alt=\'MySinglesWard.com\'>"
                    + "<br /><br /><p>Welcome to MySinglesWard.com. </p>"
                     + "<p>Your email to sign in with is: <strong>" + email + "</strong></p>"
                    + "<p>MySinglesWard is an online ward tool that gives members access to an online ward list. When you "
                    + "become part of the ward leadership, you will have access to Stewardship Information to help you learn about the people "
                    + "in your ward. This information has been designed to help you with your calling.</p>"
                    + "<p>Once you have selected a ward and taken the member survey, you will be given access to the online ward list. "
                    + "Please take the time to fill out the entire survey and upload a photo of yourself to help the ward get "
                    + "to know you better.</p>"
                    + "<p>If you have any questions please email <a href=\"mailto:support@mysinglesward.com\">support@mysinglesward.com</a>."
                    + "</p><br/><p>Sincerely,<br/><br/>Porter Hoskins<br/>MySinglesWard.com</p>";

                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;
                client.Send(message);
            }
            catch
            {
                //Issues with testing or connectivity require this to be in a try/catch
            }
        }

        /// <summary>
        /// Decrypts the encryped survey information
        /// </summary>
        public static void Decrypt(MemberSurvey member)
        {
            member.cellPhone = Utilities.Cryptography.DecryptString(member.cellPhone);
            member.birthday = Utilities.Cryptography.DecryptString(member.birthday);
            member.emergContact = Utilities.Cryptography.DecryptString(member.emergContact);
            member.emergPhone = Utilities.Cryptography.DecryptString(member.emergPhone);
            member.prefName = Utilities.Cryptography.DecryptString(member.prefName);
            member.homeAddress = Utilities.Cryptography.DecryptString(member.homeAddress);
            member.homePhone = Utilities.Cryptography.DecryptString(member.homePhone);
            member.residence = Utilities.Cryptography.DecryptString(member.residence);
        }

        /// <summary>
        /// Encrypts the required information from the member survey
        /// </summary>
        public static void Encrypt(MemberSurvey member)
        {
            member.cellPhone = Utilities.Cryptography.EncryptString(member.cellPhone);
            member.birthday = Utilities.Cryptography.EncryptString(member.birthday);
            member.emergContact = Utilities.Cryptography.EncryptString(member.emergContact);
            member.emergPhone = Utilities.Cryptography.EncryptString(member.emergPhone);
            member.prefName = Utilities.Cryptography.EncryptString(member.prefName);
            member.homeAddress = Utilities.Cryptography.EncryptString(member.homeAddress);
            member.homePhone = Utilities.Cryptography.EncryptString(member.homePhone);
            member.residence = Utilities.Cryptography.EncryptString(member.residence);
        }

        /// <summary>
        /// Checks a string to make sure it is a valid phone number
        /// </summary>
        public static bool _isPhone(string inputPhone)
        {
            string strRegex = @"^(\(?\d\d\d\)?)?( |-|\.)?\d\d\d( |-|\.)?\d{4,4}(( |-|\.)?[ext\.]+ ?\d+)?$";
            Regex re = new Regex(strRegex);
            return re.IsMatch(inputPhone);

        }

        /// <summary>
        /// Checks a string to make sure it is a valid date
        /// </summary>
        public static bool _isDate(string inputDate)
        {
            string strRegex = @"^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$";
            Regex re = new Regex(strRegex);
            return re.IsMatch(inputDate);

        }

        /// <summary>
        /// sends the message to all the groups in the list
        /// </summary>
        internal static void SendNotificationByGroupID(IEnumerable<int> SelectedGroups, string message)
        {
            using (var db = new DBmsw())
            {
                var groupsMembers = (from gu in db.tGroupUsers
                                     join gps in db.tGroups on gu.GroupID equals gps.GroupID
                                     join pref in db.tNotificationPreferences on gu.MemberID equals pref.MemberID
                                     join email in db.tUsers on gu.MemberID equals email.MemberID
                                     join phone in db.tSurveyDatas on gu.MemberID equals phone.SurveyID
                                     where SelectedGroups.Contains<int>(gu.GroupID)
                                     select new { gps.Type, gu.Leader, pref, email.Email, phone.CellPhone });

                HashSet<string> emails = new HashSet<string>();
                foreach (var member in groupsMembers)
                {
                    if (member.pref.email)
                    {
                        if (_NotificationRequested(member.Type, member.pref))
                        {
                            emails.Add(Utilities.Cryptography.DecryptString(member.Email));
                        }
                    }

                    if (member.pref.txt)
                    {
                        if (_NotificationRequested(member.Type, member.pref))
                        {
                            string phone = _parsePhone(Utilities.Cryptography.DecryptString(member.CellPhone));
                            emails.Add(phone + member.pref.carrier);
                        }
                    }
                }

                System.Net.Mail.MailMessage newMessage = new System.Net.Mail.MailMessage();
                foreach (var email in emails)
                {
                    newMessage.Bcc.Add(new MailAddress(email));
                }
                newMessage.From = new MailAddress("m@mysinglesward.com", "MSW");
                newMessage.IsBodyHtml = false;
                newMessage.Body = message;

                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;
                client.Send(newMessage);
            }
        }

        /// <summary>
        /// Sends the message to all the users in the list
        /// </summary>
        internal static void SendNotificationByMemberID(string message, IEnumerable<int> memberIDs, IEnumerable<int> bishopricIDs)
        {
            HashSet<string> emails = new HashSet<string>();
            List<MemberModel> members = members = Cache.GetList(memberIDs, x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));

            List<BishopricModel> bishopric = Cache.GetList(bishopricIDs, x => Cache.getCacheKey<BishopricModel>(x), y => BishopricModel.get(y));

            foreach (var member in members)
            {
                try
                {
                    if (member.notificationPreference.email)
                    {
                        emails.Add(member.user.Email);
                    }

                    if (member.notificationPreference.txt)
                    {
                        string phone = _parsePhone(member.memberSurvey.cellPhone);
                        emails.Add(phone + member.notificationPreference.carrier);
                    }
                }
                catch (Exception e) //The notifications have been fixed but the try will be here just incase anything gone wrong
                {
                    _sendException(e);
                }
            }

            foreach (var member in bishopric)
            {
                emails.Add(member.user.Email);
            }

            try
            {
                System.Net.Mail.MailMessage newMessage = new System.Net.Mail.MailMessage();
                foreach (var email in emails)
                {
                    newMessage.Bcc.Add(new MailAddress(email));
                }
                newMessage.From = new MailAddress("m@mysinglesward.com", "MSW");
                newMessage.IsBodyHtml = false;
                newMessage.Body = message;

                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;
                client.Send(newMessage);
            }
            catch (Exception e)
            {
                _sendException(e);
            }
        }


        private static bool _NotificationRequested(int type, tNotificationPreference pref)
        {
            if (type == (int)GroupType.STAKE && pref.stake)
                return true;
            if (type == (int)GroupType.WARD && pref.ward)
                return true;
            if (type == (int)GroupType.ELDERS_QUORUM && pref.elders)
                return true;
            if (type == (int)GroupType.RELIEF_SOCIETY && pref.reliefsociety)
                return true;
            if (type == (int)GroupType.ACTIVITIES && pref.activities)
                return true;
            if (type == (int)GroupType.FHE && pref.fhe)
                return true;

            return false;

        }

        /// <summary>
        /// Takes a phone number down to only numbers
        /// </summary>
        internal static string _parsePhone(string phone)
        {
            phone = phone.Replace("(", "");
            phone = phone.Replace(")", "");
            phone = phone.Replace("-", "");
            phone = phone.Replace(" ", "");

            return phone;
        }

        /// <summary>
        /// Checks to make sure the user wardID is not null
        /// </summary>
        /*internal static MSWUser _checkWardID(MSWUser user)
        {
            MSW.Models.dbo.Ward ward = null;
            try
            {
                ward = MSW.Models.dbo.Ward.get(user.WardStakeID);
            }
            catch (NullReferenceException e)
            {
                //Will catch when ward doesn't exist, wardID needs to be set to zero
                user.WardStakeID = 0;
                MSWUser.saveUser(user);
                _sendException(e, user.UserName);
            }

            return user;
        }*/

        /// <summary>
        /// Checks to make sure the stake user's StakeID is not null
        /// </summary>
        internal static StakeUser _checkStakeID(StakeUser user)
        {
            Stake stake = Stake.get(user.StakeID);

            if (stake == null)
            {
                user.StakeID = 0;
                StakeUser.saveUser(user);
            }

            return user;

        }

        /// <summary>
        /// Removes a mmember from a ward. This takes care of all the 
        /// database work and caching to make sure the member is completely gone.
        /// </summary>
        internal static void _removeMemberFromWard(MemberModel member, string ChosenWard = "0")
        {
            using (var db = new DBmsw())
            {
                int MemberID = member.user.MemberID;
                //Nuke Cache Keys for old ward
                _NukeCacheKeys(member);

                //Removes Records Requested
                member.user.RecordsRequested = false;

                //Appends Ward Data to survey if member is leaving a ward supported by MySinglesWard
                try
                {
                    if (member.user.WardStakeID != 0)
                    {
                        //Adds Bishop to PrevBishop Field on survey
                        List<BishopricModel> bishopric = Cache.GetList(Repository.getInstance().BishopricMembersID(member.user.WardStakeID), x => Cache.getCacheKey<BishopricModel>(x), y => BishopricModel.get(y));
                        
                        const string BISHOP = "1";
                        BishopricModel bishop = bishopric.FirstOrDefault(x => x.data.BishopricCalling == BISHOP);
                        if (bishop != null)
                        {
                            try
                            {
                                member.memberSurvey.prevBishops = bishop.data.BishopricName + " - " + bishop.data.BishopricPhone + "\n";
                                MemberSurvey.saveMemberSurvey(member.memberSurvey);
                            }
                            catch
                            {
                                //Do nothing. The member does not have a survey or the bishop did not fill out his survey
                            }
                        }


                        //Create a prior unit object for MLS record request report
                        PriorUnit pu = PriorUnit.get(MemberID);
                        Ward ward = Ward.get(member.user.WardStakeID);
                        if (pu != null)
                        {
                            pu.priorUnit = ward.Location + " " + ward.Stake + " Stake " + ward.ward + " Ward";
                            PriorUnit.save(pu);
                        }
                        else
                        {
                            PriorUnit.create(MemberID, ward.Location + " " + ward.Stake + " Stake " + ward.ward + " Ward");
                        }
                    }

                }
                catch (Exception e)
                {
                    _sendException(e);
                }

                //Removes the user from their organization
                OrganizationMember.remove(OrganizationMember.get(member.user.MemberID));

                //Adds User to new ward, 0 if being removed - ChosenWard if changing wards
                member.user.WardStakeID = double.Parse(ChosenWard);

                //Removes User From Group Roles
                var memberGroupRoles = db.tMemberGroupRoles.SingleOrDefault(x => x.MemberID == MemberID);
                if (memberGroupRoles != null)
                    db.tMemberGroupRoles.DeleteOnSubmit(memberGroupRoles);

                //Removes User from Groups
                var groups = db.tGroupUsers.Where(x => x.MemberID == MemberID);
                if (groups != null)
                    db.tGroupUsers.DeleteAllOnSubmit(groups);

                //Removes User as leader of group
                var leaderGroups = db.tGroups.Where(x => x.LeaderID == MemberID);
                var coLeaderGroups = db.tGroups.Where(x => x.CoLeaderID == MemberID);

                foreach (var group in leaderGroups)
                {
                    group.LeaderID = null;
                }

                foreach (var group in coLeaderGroups)
                {
                    group.CoLeaderID = null;
                }

                //Calling Removal
                var memberCallings = new List<int>();
                memberCallings.AddRange(db.tCallings.Where(x => x.MemberID == MemberID).Select(x => x.CallingID).ToList());
                memberCallings.AddRange(db.tPendingReleases.Where(x => x.MemberID == MemberID).Select(x => x.CallingID).ToList());
                foreach (var callingID in memberCallings)
                {
                    try
                    {
                        Calling calling = Calling.get(callingID);
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

                var pendingReleases = db.tPendingReleases.Where(x => x.MemberID == member.user.MemberID);
                foreach (var release in pendingReleases)
                {
                    PendingRelease.remove(PendingRelease.get(release.CallingID));
                }

                var callingRoles = db.tMemberRoles.Where(x => x.MemberID == member.user.MemberID).SingleOrDefault();
                if (callingRoles != null)
                    db.tMemberRoles.DeleteOnSubmit(callingRoles);

                //Removes Member From Roles
                string[] roles = System.Web.Security.Roles.GetRolesForUser(member.user.UserName);
                System.Web.Security.Roles.RemoveUserFromRoles(member.user.UserName, roles);
                System.Web.Security.Roles.AddUserToRole(member.user.UserName, "Member?");

                MSWUser.saveUser(member.user);

                //Nuke Cache keys for new ward
                _NukeCacheKeys(MemberModel.get(member.user.MemberID));

                //Remove Stewardship Reports
                NukeStewardshipReports(ChosenWard);
                db.SubmitChanges();

                //Remove member from current teaching assignment
                removeMemberFromTeachingAssignment(member.user.MemberID);
            }
        }

        /// <summary>
        /// Removes a member from a teaching assignment upon leaving a ward
        /// </summary>
        public static void removeMemberFromTeachingAssignment(int MemberID)
        {
            //Remove member from current teaching assignment
            TeachingAssignment tA = TeachingAssignment.get(MemberID);
            if (tA != null)
            {
                tA.CompanionshipID = null;
                tA.HTID = null;
                tA.VTID = null;
                TeachingAssignment.save(tA);
            }
        }

        /// <summary>
        /// Removes the cache information for a member
        /// </summary>
        private static void _NukeCacheKeys(MemberModel member)
        {
            if (member.user.WardStakeID == 0)
                return;

            //NUKE Cache keys
            double WardID = member.user.WardStakeID;

            if (member.user.IsBishopric)
                Cache.Remove("Bishopric:" + WardID);
            else
                Cache.Remove("Ward:" + WardID);
            if (member.ward != null)
                Cache.Remove("Stake:" + member.ward.StakeID);
            Cache.Remove("WardMemberNames:" + WardID);
            Cache.Remove("Ward-all:" + WardID);

            if (member.memberSurvey != null)
            {
                if (member.memberSurvey.gender)
                    Cache.Remove("Ward-elders:" + WardID);
                else
                    Cache.Remove("Ward-sisters:" + WardID);
            }

            foreach (var group in member.GroupList)
            {
                Cache.Remove("groupMembers:" + group.GroupID);
            }

            Cache.Remove("SelectList-Stake-" + false + ":" + WardID);
            if (member.ward != null)
                Cache.Remove("SelectList-Stake-" + true + ":" + member.ward.StakeID);

            Cache.Remove("MemberCallings:" + member.user.MemberID);

            Repository.getInstance().NukeReportKeys(WardID);
        }

        /// <summary>
        /// Deletes all the stewardship reports for the given ward
        /// </summary>
        public static void NukeStewardshipReports(string WardID)
        {
            //Values come from name dropdown on views
            string[] types = { "", "name", "lastName", "apartment" };

            foreach (string type in types)
            {
                //PDF
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\WardList-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Bishopric-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\EQ-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\RS-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Act-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Emergency-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Employment-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\FamHist-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\FHE-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Institute-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Mission-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Music-" + type + WardID);
                _DeleteFile(AppDomain.CurrentDomain.BaseDirectory + "Print\\Teaching-" + type + WardID);
            }
        }

        /// <summary>
        /// Deletes a file
        /// </summary>
        private static void _DeleteFile(string fileName)
        {
            try
            {
                FileInfo file = new FileInfo(fileName);
                file.Delete();
            }
            catch
            {

            }

        }

        /// <summary>
        /// Used to get the date of a sunday for Sacrament meeting talk recording
        /// </summary>
        public static DateTime GetFirstDayInWeek(DateTime dayInWeek, DayOfWeek firstDay)
        {
            int difference = ((int)dayInWeek.DayOfWeek) - ((int)firstDay);
            difference = (7 + difference) % 7;
            return dayInWeek.AddDays(-difference).Date;
        }

        /// <summary>
        /// Returns unformattedNumber in the format of (###) ###-####. Note: this does not accept phone numbers with more than 10 digits or less than 7.  
        /// </summary>
        internal static string normalizePhoneNumber(string unformattedNumber)
        {
            try
            {
                //Strip non-digits
                string pureDigits = Regex.Replace(unformattedNumber, "[^0-9]", "");
                //Format
                return String.Format("{0:(###) ###-####}", double.Parse(pureDigits));
            }
            catch
            {
                return unformattedNumber;
            }
        }

        internal static string removeBishopricMember(MSWUser user, string newWardID = "0", string password = "")
        {
            try
            {
                Cache.Remove("Bishopric:" + user.WardStakeID);
                MSWtools.NukeStewardshipReports(user.WardStakeID.ToString());
                if (double.Parse(newWardID) == 0)
                {
                    user.WardStakeID = 0;
                    MSWUser.saveUser(user);
                    return "0";
                }
            }
            catch
            {
                user.WardStakeID = 0;
                MSWUser.saveUser(user);
                return "0";
            }

            double ward = 0;
            Ward newWard;
            try
            {
                newWard = Ward.get(Double.Parse(newWardID));
                if (newWard.Password.Equals(password))
                {
                    user.WardStakeID = Double.Parse(newWardID);
                    MSWUser.saveUser(user);
                    ward = Double.Parse(newWardID);

                    Cache.Remove("Bishopric:" + ward);
                    MSWtools.NukeStewardshipReports(ward.ToString());
                    return ward.ToString();
                }
            }
            catch
            {
                user.WardStakeID = 0;
                MSWUser.saveUser(user);
            }

            throw new Exception("Password Didn't Match");
        }
    }

}