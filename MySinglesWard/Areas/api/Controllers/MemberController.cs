using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW.Model;
using Newtonsoft.Json;
using MSW.Utilities;
using MSW.Areas.api.Models;
using MSW.Models.dbo;
using System.Collections.Specialized;
using MSW.Controllers;

namespace MSW.Areas.api.Controllers
{
    public class MemberController : Controller
    {
        //
        // GET: /api/Member/

        [Authorize]
        public string CheckStatus()
        {
            string NEEDS_NOTHING = "NEEDS_NOTHING";
            string NEEDS_PHOTO = "NEEDS_PHOTO";
            string NEEDS_SURVEY = "NEEDS_SURVEY";
            string UNAUTHORIZED = "UNAUTHORIZED";

            MemberModel currentUser = MemberModel.get(User.Identity.Name);
            List<string> status = new List<string>();

            if (User.IsInRole("Member?"))
                status.Add(UNAUTHORIZED);

            if (currentUser.memberSurvey == null && !currentUser.user.IsBishopric)
                status.Add(NEEDS_SURVEY);
            else if (currentUser.photo.NewPhotoFileName == null && currentUser.photo.Status < (int)MSW.Models.dbo.PhotoStatus.CROPPED)
            {
                status.Add(NEEDS_PHOTO);
            }
            else
            {
                status.Add(NEEDS_NOTHING);
            }

            return JsonConvert.SerializeObject(new { status = status });
        }

        [Authorize]
        public string Index()
        {
            return JsonConvert.SerializeObject(MemberMobileModel.get(User.Identity.Name));
        }

        [Authorize(Roles = "Bishopric")]
        public string LeadershipInformation()
        {
            return JsonConvert.SerializeObject(MemberModel.get(User.Identity.Name));
        }

        [Authorize]
        public string Get(int id)
        {
            //Check to make sure the user is in the ward of the person requesting the information
            if (Repository.getInstance().WardMembersID(MSWMobileUser.getUser(User.Identity.Name).WardStakeID).Contains(id))
                return JsonConvert.SerializeObject(MemberMobileModel.get(id));

            return "";
        }

        [Authorize]
        [HttpPost]
        public string SaveNotificationPreferences(NotificationPreference pref)
        {
            try
            {
                NotificationPreference.save(pref);
            }
            catch (Exception e)
            {
                MSWtools._sendException(e);
                return JsonConvert.SerializeObject(new { success = false });
            }

            return JsonConvert.SerializeObject(new { success = true });
        }

        [Authorize]
        public string GetSurvey(int id)
        {
            //Check to make sure the user is in the ward of the person requesting the information
            if (Repository.getInstance().WardMembersID(MSWMobileUser.getUser(User.Identity.Name).WardStakeID).Contains(id))
                return JsonConvert.SerializeObject(MemberSurvey.getMemberSurvey(id));

            return "";
        }

        [Authorize]
        [HttpPost]
        public string SaveSurvey(MemberSurvey survey)
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            if (user.MemberID == survey.memberID)
                MemberSurvey.saveMemberSurvey(survey);
            else
                return JsonConvert.SerializeObject(new { success = false });

            return JsonConvert.SerializeObject(new { success = true });
        }

        [Authorize]
        public string changeWard(string wardID, string wardPassword)
        {
            MemberModel member = MemberModel.get(User.Identity.Name);

            if (wardID == member.user.WardStakeID.ToString())
            {
                return "";
            }

            if (member.user.IsBishopric)
            {
                Ward ward = Ward.get(double.Parse(wardID));
                if (ward.Password != wardPassword)
                {
                    return "";
                }

                MSWtools.removeBishopricMember(member.user, wardID, wardPassword);
            }
            else
                MSWtools._removeMemberFromWard(member, wardID);

            Session["WardStakeID"] = Double.Parse(wardID).ToString();

            return JsonConvert.SerializeObject(Ward.get(double.Parse(wardID)));
        }

        [Authorize]
        public string uploadPhoto()
        {
            HttpPostedFileBase photoFile = Request.Files["photofile"];

            int MemberID = MSWUser.getUser(User.Identity.Name).MemberID;
            new PhotoController()._processPhotoFromApp(photoFile, false, MemberID, Server.MapPath("\\Photo"));
            Photo photo = Photo.getPhoto(MemberID);

            return JsonConvert.SerializeObject(photo);
        }

        [Authorize]
        public string GetBishopricData(int id)
        {
            //Check to make sure the user is in the ward of the person requesting the information
            if (Repository.getInstance().BishopricMembersID(MSWMobileUser.getUser(User.Identity.Name).WardStakeID).Contains(id))
                return JsonConvert.SerializeObject(BishopricData.get(id));

            return "";
        }

        [Authorize]
        [HttpPost]
        public string SaveBishopricData(BishopricData data)
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            if (user.MemberID == data.MemberID)
                BishopricData.save(data);
            else
                return JsonConvert.SerializeObject(new { success = false });

            return JsonConvert.SerializeObject(new { success = true });
        }
    }
}
