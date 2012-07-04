using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class MSWUser
	{
		public int MemberID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public double WardStakeID { get; set; }
		public bool IsBishopric { get; set; }
		public bool RecordsRequested { get; set; }
        public DateTime DateCreated { get; set; }

		public static MSWUser getUser(int MemberID)
		{
			MSWUser user = Cache.Get(Cache.getCacheKey<MSWUser>(MemberID)) as MSWUser;

			if (user == null)
			{
				using (var db = new DBmsw())
				{
					var dboUser = db.tUsers.SingleOrDefault(x => x.MemberID == MemberID);
					user = new MSWUser(dboUser);
				}
				
				Cache.Set(Cache.getCacheKey<MSWUser>(user.MemberID), user);
			}

			return user;
		}

		public static MSWUser getUser(string UserName)
		{
            string MemberID = null;
            UserName = UserName.ToLower();

            try
            {
                //Get ID out of cache, users are not cached by Username but my ID
                MemberID = Cache.Get(Cache.getCacheKey<MSWUser>(UserName.ToLower())) as string;
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
				using (var db = new DBmsw())
				{
					var dboUser = db.tUsers.SingleOrDefault(x => x.UserName == UserName.ToLower());
					MSWUser user = new MSWUser(dboUser);
					MemberID = user.MemberID.ToString();

					Cache.Set(Cache.getCacheKey<MSWUser>(UserName), user.MemberID.ToString());
					Cache.Set(Cache.getCacheKey<MSWUser>(user.MemberID), user);
				}
			}

			return getUser(int.Parse(MemberID));
		}

		public static void saveUser(MSWUser user)
		{
			Cache.Remove(Cache.getCacheKey<MSWUser>(user.MemberID));

			using (var db = new DBmsw())
			{
				var targetUser = db.tUsers.SingleOrDefault(x => x.MemberID == user.MemberID);

                //Having issue with bishopric members wardIDs changing
                if (targetUser.WardStakeID != user.WardStakeID && user.IsBishopric)
                {
                    MSWtools._sendException(new Exception("WARDID changed. OLD ID: " + targetUser.WardStakeID + " NEW ID: " + user.WardStakeID + " " + new Exception().StackTrace), user.UserName);
                }

				targetUser.MemberID = user.MemberID;
				targetUser.FirstName = Utilities.Cryptography.EncryptString(user.FirstName);
				targetUser.LastName = Utilities.Cryptography.EncryptString(user.LastName);
				targetUser.UserName = user.UserName;
				targetUser.Email = Utilities.Cryptography.EncryptString(user.Email);
				targetUser.WardStakeID = user.WardStakeID;
				targetUser.IsBishopric = user.IsBishopric;
				targetUser.RecordsRequested = user.RecordsRequested;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<MSWUser>(user.MemberID), user);
			}
		}

		private MSWUser(tUser user)
		{
			MemberID = user.MemberID;
			try
			{
				LastName = Utilities.Cryptography.DecryptString(user.LastName);
				FirstName = Utilities.Cryptography.DecryptString(user.FirstName);
			}
			catch
			{
				LastName = " ";
				FirstName = " ";
			}

			UserName = user.UserName;
			Email = Utilities.Cryptography.DecryptString(user.Email);
			WardStakeID = (user.WardStakeID != null) ? double.Parse(user.WardStakeID.ToString()) : 0;
			IsBishopric = user.IsBishopric;
            DateCreated = user.DateCreated;

			if (user.RecordsRequested == null)
				RecordsRequested = false;
			else
				RecordsRequested = bool.Parse(user.RecordsRequested.ToString());

		}
	}
}