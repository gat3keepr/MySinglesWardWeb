using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class StakeUser
	{
		public int MemberID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool HasPic { get; set; }
		public double StakeID { get; set; }
		public bool IsPresidency { get; set; }

		public static StakeUser getStakeUser(string UserName)
		{
            string MemberID = null;
            UserName = UserName.ToLower();

            try
            {
                //Get ID out of cache, users are not cached by Username but my ID
                MemberID = Cache.Get(Cache.getCacheKey<StakeUser>(UserName)) as string;
            }
            catch
            {
                using (var db = new DBmsw())
                {
                    MemberID = db.tStakeUsers.SingleOrDefault(x => x.UserName == UserName).MemberID.ToString();
                }
            }

			if (MemberID == null)
			{
                using (var db = new DBmsw())
                {
                    var dboUser = db.tStakeUsers.SingleOrDefault(x => x.UserName == UserName);
                    StakeUser user = new StakeUser(dboUser);
                    MemberID = user.MemberID.ToString();

                    Cache.Set(Cache.getCacheKey<StakeUser>(UserName), user.MemberID.ToString());
                    Cache.Set(Cache.getCacheKey<StakeUser>(user.MemberID), user);
                }
			}

			return getStakeUser(int.Parse(MemberID));
		}

		public static StakeUser getStakeUser(int MemberID)
		{
			StakeUser user = Cache.Get(Cache.getCacheKey<StakeUser>(MemberID)) as StakeUser;

			if (user == null)
			{
                using (var db = new DBmsw())
                {
                    var dboUser = db.tStakeUsers.SingleOrDefault(x => x.MemberID == MemberID);
                    user = new StakeUser(dboUser);

                    Cache.Set(Cache.getCacheKey<StakeUser>(user.MemberID), user);
                }
			}

			return user;
		}

		public static void saveUser(StakeUser user)
		{
			Cache.Remove(Cache.getCacheKey<StakeUser>(user.MemberID));

            using (var db = new DBmsw())
            {
                var targetUser = db.tStakeUsers.SingleOrDefault(x => x.MemberID == user.MemberID);

                targetUser.MemberID = user.MemberID;
                targetUser.FirstName = Utilities.Cryptography.EncryptString(user.FirstName);
                targetUser.LastName = Utilities.Cryptography.EncryptString(user.LastName);
                targetUser.UserName = user.UserName;
                targetUser.Email = Utilities.Cryptography.EncryptString(user.Email);
                targetUser.HasPic = user.HasPic;
                targetUser.StakeID = user.StakeID;
                targetUser.isPresidency = user.IsPresidency;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<StakeUser>(user.MemberID), user);
            }
		}

		private StakeUser(tStakeUser user)
		{
			MemberID = user.MemberID;
			FirstName = Utilities.Cryptography.DecryptString(user.FirstName);
			LastName = Utilities.Cryptography.DecryptString(user.LastName);
			UserName = user.UserName;
			Email = Utilities.Cryptography.DecryptString(user.Email);
			HasPic = user.HasPic;
			StakeID = user.StakeID;
			IsPresidency = user.isPresidency;
		}

	}
}