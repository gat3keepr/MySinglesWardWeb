using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class PriorUnit
    {
        public int MemberID { get; set; }
		public string priorUnit { get; set; }

		public static PriorUnit get(int MemberID)
		{
			PriorUnit pu = Cache.Get(Cache.getCacheKey<PriorUnit>(MemberID)) as PriorUnit;

			if (pu == null)
			{
                using (var db = new DBmsw())
                {
                    var puDBO = db.tPriorUnits.SingleOrDefault(x => x.MemberID == MemberID);

                    if (puDBO == null)
                        return null;

                    pu = new PriorUnit(puDBO);
                    Cache.Set(Cache.getCacheKey<PriorUnit>(MemberID), pu);
                }
			}
			return pu;
		}

		public static void save(PriorUnit pu)
		{
			Cache.Remove(Cache.getCacheKey<PriorUnit>(pu.MemberID));

            using (var db = new DBmsw())
            {
                var targetPU = db.tPriorUnits.SingleOrDefault(x => x.MemberID == pu.MemberID);

                if (targetPU == null)
                {
                    targetPU = new tPriorUnit();
                    targetPU.MemberID = pu.MemberID;
                    db.tPriorUnits.InsertOnSubmit(targetPU);
                }

                targetPU.PriorUnit = pu.priorUnit;
                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<PriorUnit>(pu.MemberID), pu);
            }
		}

		internal static PriorUnit create(int MemberID, string priorUnit)
		{
            using (var db = new DBmsw())
            {
                var targetPU = new tPriorUnit();

                targetPU.MemberID = MemberID;
                targetPU.PriorUnit = priorUnit;

                db.tPriorUnits.InsertOnSubmit(targetPU);
                db.SubmitChanges();

                PriorUnit pu = new PriorUnit(targetPU);
                Cache.Set(Cache.getCacheKey<PriorUnit>(MemberID), pu);
                return pu;
            }
		}

        private PriorUnit(tPriorUnit puDBO)
		{
			this.MemberID = puDBO.MemberID;
			priorUnit = puDBO.PriorUnit;
		}
    }
}