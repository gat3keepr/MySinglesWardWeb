using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Areas.api.Models
{
    public class MemberMobileModel
    {
        public MSWMobileUser user { get; set; }
		public Photo photo;
		public MSW.Models.dbo.Ward ward;
		public NotificationPreference notificationPreference;
        public MobileCallingInfo calling { get; set; }
        public BishopricData bishopric { get; set; }

		public static MemberMobileModel get(string UserName)
		{
            string MemberID = null;
            try
            {
                //Get ID out of cache, users are not cached by Username but my ID
                MemberID = Cache.Get(Cache.getCacheKey<MSWUser>(UserName)) as string;
            }
            catch
            {
                using (var db = new DBmsw())
                {
                    MemberID = db.tUsers.SingleOrDefault(x => x.UserName == UserName).MemberID.ToString();
                }
            }

			if (MemberID == null)
			{
				MSWMobileUser user = MSWMobileUser.getUser(UserName); 
				MemberID = user.MemberID.ToString();
			}

			return get(int.Parse(MemberID));
		}

		public static MemberMobileModel get(int MemberID)
		{
			return new MemberMobileModel(MemberID);
		}


        private MemberMobileModel(int MemberID)
        {
			user = MSWMobileUser.getUser(MemberID);

			try
			{
				photo = Photo.getPhoto(MemberID);
                ward = MSW.Models.dbo.Ward.get(user.WardStakeID);
                notificationPreference = NotificationPreference.get(MemberID);
            }
            catch
            {

            }

            /* Calling Information */
            try
            {
                if (!user.IsBishopric)
                {
                    List<Calling> callings = Cache.GetList(Repository.getInstance().MemberCallings(MemberID), x => Cache.getCacheKey<Calling>(x), y => Calling.get(y));
                    Calling c = callings[0];

                    calling = new MobileCallingInfo(c);
                }
                else
                {
                    //Request was made for bishopric user
                    calling = new MobileCallingInfo();
                    calling.CallingID = -1;
                    calling.MemberID = MemberID;
                    bishopric = BishopricData.get(MemberID);
                    calling.Title = bishopric.BishopricCalling == "" ? "Not Available" : bishopric.BishopricCalling;
                }

            }
            catch
            {
                calling = new MobileCallingInfo();
                calling.CallingID = 0;
                calling.MemberID = MemberID;
                calling.Title = "Not Available";
            }

        }		
    }

    public class MobileCallingInfo
    {
        public MobileCallingInfo(Calling c)
        {
            // member initialization
            CallingID = c.CallingID;
            MemberID = c.MemberID;
            Title = Organization.get(c.OrgID).Title + " - " + c.Title;
        }

        public MobileCallingInfo()
        {

        }

        public int CallingID { get; set; }
        public string Title { get; set; }
        public int MemberID { get; set; }
    }
}