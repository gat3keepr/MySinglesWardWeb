using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class StakeData
	{
		public int MemberID { get; set; }
		public string StakeName { get; set; }
		public string StakeCalling { get; set; }
		public string StakePhone { get; set; }

		public static StakeData get(int MemberID)
		{
			StakeData data = Cache.Get(Cache.getCacheKey<StakeData>(MemberID)) as StakeData;

			if (data == null)
			{
				data = new StakeData(MemberID);

				Cache.Set(Cache.getCacheKey<StakeData>(data.MemberID), data);
			}

			return data;
		}

		public static void save(StakeData user)
		{
			Cache.Remove(Cache.getCacheKey<StakeData>(user.MemberID));

            using (var db = new DBmsw())
            {
                var targetData = db.tStakeDatas.SingleOrDefault(x => x.MemberID == user.MemberID);
                if (targetData == null)
                {
                    targetData = new tStakeData();
                    db.tStakeDatas.InsertOnSubmit(targetData);
                    targetData.MemberID = user.MemberID;
                }

                targetData.StakeName = Utilities.Cryptography.EncryptString(user.StakeName);
                targetData.StakeCalling = user.StakeCalling;
                targetData.StakePhone = Utilities.Cryptography.EncryptString(user.StakePhone);

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<StakeData>(user.MemberID), user);
            }
		}

		private StakeData(int MemberID)
		{
            using (var db = new DBmsw())
            {
                var data = db.tStakeDatas.SingleOrDefault(x => x.MemberID == MemberID);

                this.MemberID = MemberID;
                if (data == null)
                {
                    StakeName = "";
                    StakeCalling = "";
                    StakePhone = "";
                }
                else
                {
                    StakeName = Utilities.Cryptography.DecryptString(data.StakeName);
                    StakeCalling = data.StakeCalling;
                    StakePhone = Utilities.Cryptography.DecryptString(data.StakePhone);
                }
            }
		}
		public StakeData() { }
	}
}