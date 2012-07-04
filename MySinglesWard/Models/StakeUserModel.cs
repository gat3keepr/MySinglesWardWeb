using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Models.dbo;
using MSW.Utilities;

namespace MSW.Models
{
    public class StakeUserModel
    {
		public StakeUser user;
		public StakePhoto photo;
		public StakeData data;
		public string calling { get; set; }

		public static StakeUserModel get(int MemberID)
		{
			return new StakeUserModel(MemberID);
		}

		public static StakeUserModel get(string UserName)
        {
            string MemberID = null;
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
				StakeUser user = StakeUser.getStakeUser(UserName);
				MemberID = user.MemberID.ToString();

				Cache.Set(Cache.getCacheKey<StakeUser>(UserName), user.MemberID.ToString());
				Cache.Set(Cache.getCacheKey<StakeUser>(user.MemberID), user);
			}

			return get(int.Parse(MemberID));
		}

        private StakeUserModel(int MemberID)
        {
			user = StakeUser.getStakeUser(MemberID);

            try
            {
				photo = StakePhoto.getStakePhoto(MemberID);
				data = StakeData.get(MemberID);
				calling = _getCalling(int.Parse(data.StakeCalling));
            }
            catch
            {

            }
        }

        public static string _getCalling(int callingID)
        {
            switch(callingID)
            {
                case 1:
                    return "Stake President";
                case 2:
                    return "First Counselor";
                case 3:
                    return "Second Counselor";
                case 4: 
                    return "Executive Secretary";
                case 5:
					return "High Counselman";
				case 6:
                    return "Stake Relief Society";
                case 7:
                    return "Stake Clerk";
                case 8:
                    return "Stake Activities";
                case 9: 
                    return "Other Stake Calling";

            }
            return "Other Stake Calling"; 
        }
    }
}