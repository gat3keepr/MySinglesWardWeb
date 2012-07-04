using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW;
using System.Net.Mail;
using MSW.Models.dbo;
using MSW.Utilities;

namespace MSW.Models
{

	/// <summary>
	/// Grabs all the bishopric members in a ward to display on the ward list
	/// </summary>
    public class BishopricModel
    {
        public MSWUser user;
		public Photo photo;
		public MSW.Models.dbo.Ward ward;
		//Used for Bishopric
		public BishopricData data;

		public static BishopricModel get(int MemberID)
		{
			return new BishopricModel(MemberID);
		}

		private BishopricModel(int MemberID)
        {
			user = MSWUser.getUser(MemberID);
			photo = Photo.getPhoto(MemberID);
			ward = MSW.Models.dbo.Ward.get(user.WardStakeID);
			data = BishopricData.get(MemberID);
        }
    }
}