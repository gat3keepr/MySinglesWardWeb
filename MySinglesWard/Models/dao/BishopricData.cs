using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class BishopricData
	{
		public int MemberID { get; set; }
		public string BishopricName { get; set; }
		public string BishopricCalling { get; set; }
		public string BishopricPhone { get; set; }
		public string BishopricAddress { get; set; }
		public string WifeName { get; set; }
		public string WifePhone { get; set; }
        public string SortID { get; set; }

		public static BishopricData get(int MemberID)
		{
			BishopricData data = Cache.Get(Cache.getCacheKey<BishopricData>(MemberID)) as BishopricData;

			if (data == null)
			{
				data = new BishopricData(MemberID);

				Cache.Set(Cache.getCacheKey<BishopricData>(data.MemberID), data);
			}

			return data;
		}

		public static void save(BishopricData data)
		{
			Cache.Remove(Cache.getCacheKey<BishopricData>(data.MemberID));

			using (var db = new DBmsw())
			{
				db.DeferredLoadingEnabled = false;

				var targetData = db.tBishopricDatas.SingleOrDefault(x => x.MemberID == data.MemberID);
				if (targetData == null)
				{
					targetData = new tBishopricData();
					db.tBishopricDatas.InsertOnSubmit(targetData);
					targetData.MemberID = data.MemberID;
				}

                targetData.BishopricName = Utilities.Cryptography.EncryptString(data.BishopricName);
				targetData.BishopricCalling = data.BishopricCalling;
				targetData.BishopricPhone = Utilities.Cryptography.EncryptString(data.BishopricPhone);
				targetData.BishopricAddress = Utilities.Cryptography.EncryptString(data.BishopricAddress);
                targetData.WifeName = Utilities.Cryptography.EncryptString(data.WifeName != null ? data.WifeName : " ");
                targetData.WifePhone = Utilities.Cryptography.EncryptString(data.WifePhone != null ? data.WifePhone : " ");

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<BishopricData>(data.MemberID), new BishopricData(targetData.MemberID));
			}
		}

		private BishopricData(int MemberID)
		{
			const string BISHOP = "1";
            const string FIRST_COUNSELOR = "2";
            const string SECOND_COUNSELOR = "3";
            const string WARD_CLERK = "4";
            const string HIGH_COUNCIL = "5";
            const string NO_CALLING = "6";
            
            using (var db = new DBmsw())
			{
				var data = db.tBishopricDatas.SingleOrDefault(x => x.MemberID == MemberID);

				this.MemberID = MemberID;
				if (data == null)
				{
					BishopricName = "";
					BishopricCalling = "";
					BishopricPhone = "";
					BishopricAddress = "";
					WifeName = "";
					WifePhone = "";
                    SortID = NO_CALLING;
				}
				else
				{
					BishopricName = Utilities.Cryptography.DecryptString(data.BishopricName);
					BishopricPhone = Utilities.Cryptography.DecryptString(data.BishopricPhone);
					BishopricAddress = data.BishopricAddress != null ? Utilities.Cryptography.DecryptString(data.BishopricAddress) : " ";
					WifeName = data.WifeName != null ? Utilities.Cryptography.DecryptString(data.WifeName) : " ";
					WifePhone = data.WifePhone != null ? Utilities.Cryptography.DecryptString(data.WifePhone) : " ";
                    SortID = data.BishopricCalling;

                    switch (SortID)
                    {
                        case BISHOP:
                            BishopricCalling = "Bishop";
                            break;
                        case FIRST_COUNSELOR:
                            BishopricCalling = "First Counselor";
                            break;
                        case SECOND_COUNSELOR:
                            BishopricCalling = "Second Counselor";
                            break;
                        case WARD_CLERK:
                            BishopricCalling = "Ward Clerk";
                            break;
                        case HIGH_COUNCIL:
                            BishopricCalling = "High Councilman";
                            break;
                    }
				}
			}

		}
		public BishopricData() { }
	}
}