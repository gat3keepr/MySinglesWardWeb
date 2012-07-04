using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class MemberTalk
	{
		public int MemberID { get; set; }
		public DateTime LastSpoke { get; set; }

		public static MemberTalk get(int MemberID)
		{
			MemberTalk mt = Cache.Get(Cache.getCacheKey<MemberTalk>(MemberID)) as MemberTalk;

			if (mt == null)
			{
				using (var db = new DBmsw())
				{
					var mtDBO = db.tMemberTalks.SingleOrDefault(x => x.MemberID == MemberID);

					if (mtDBO == null)
						return null;

					mt = new MemberTalk(mtDBO);
					Cache.Set(Cache.getCacheKey<MemberTalk>(MemberID), mt);
				}
			}
			return mt;
		}

		public static void save(MemberTalk mt)
		{
			Cache.Remove(Cache.getCacheKey<MemberTalk>(mt.MemberID));

			using (var db = new DBmsw())
			{
				var targetMT = db.tMemberTalks.SingleOrDefault(x => x.MemberID == mt.MemberID);

				if (targetMT == null)
				{
					targetMT = new tMemberTalk();
					targetMT.MemberID = mt.MemberID;
					db.tMemberTalks.InsertOnSubmit(targetMT);
				}

				targetMT.LastSpoke = mt.LastSpoke;
				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<MemberTalk>(mt.MemberID), mt);
			}
		}

		internal static MemberTalk create(int MemberID, DateTime lastSpoke)
		{
			using (var db = new DBmsw())
			{
				var targetMT = new tMemberTalk();

				targetMT.MemberID = MemberID;
				targetMT.LastSpoke = lastSpoke;

				db.tMemberTalks.InsertOnSubmit(targetMT);
				db.SubmitChanges();

				MemberTalk mt = new MemberTalk(targetMT);
				Cache.Set(Cache.getCacheKey<MemberTalk>(MemberID), mt);
				return mt;
			}
		}

		private MemberTalk(tMemberTalk mtDBO)
		{
			this.MemberID = mtDBO.MemberID;
			LastSpoke = mtDBO.LastSpoke;
		}

	}
}