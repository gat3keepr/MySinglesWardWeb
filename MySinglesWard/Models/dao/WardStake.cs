using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Utilities;

namespace MSW.Models.dbo
{
	[Serializable]
	public class WardStake
	{
		public double WardID { get; set; }
		public double StakeID { get; set; }
		public bool Approved { get; set; }

		public static WardStake get(double WardID)
		{
			WardStake wardStake = Cache.Get(Cache.getCacheKey<WardStake>(WardID)) as WardStake;

			if (wardStake == null)
			{
                using (var db = new DBmsw())
                {
                    tWardStake dboWardStake = db.tWardStakes.FirstOrDefault(x => x.WardStakeID == WardID);

                    if (dboWardStake == null)
                        return null;
                    wardStake = new WardStake(dboWardStake);

                    Cache.Set(Cache.getCacheKey<WardStake>(WardID), wardStake);
                }
			}

			return wardStake;
		}

		public static void save(WardStake wardStake)
		{
			Cache.Remove(Cache.getCacheKey<WardStake>(wardStake.WardID));			
			_nukeCacheKeys(wardStake.StakeID, Stake.get(wardStake.StakeID).stake);

            using (var db = new DBmsw())
            {
                var targetWardStake = db.tWardStakes.SingleOrDefault(x => x.WardStakeID == wardStake.WardID);
                _nukeCacheKeys(wardStake.StakeID, Stake.get(targetWardStake.StakeID).stake);

                targetWardStake.WardStakeID = wardStake.WardID;
                targetWardStake.StakeID = wardStake.StakeID;
                targetWardStake.Approved = wardStake.Approved;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<WardStake>(wardStake.WardID), wardStake);
            }
		}

		public static void create(tWardStake wardStake)
		{
            using (var db = new DBmsw())
            {
                db.tWardStakes.InsertOnSubmit(wardStake);
                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<WardStake>(wardStake.WardStakeID), new WardStake(wardStake.WardStakeID));
                _nukeCacheKeys(wardStake.StakeID, Stake.get(wardStake.StakeID).stake);
            }
		}

		public static void remove(WardStake wardStake)
		{
			Cache.Remove(Cache.getCacheKey<WardStake>(wardStake.WardID));
			_nukeCacheKeys(wardStake.StakeID, Stake.get(wardStake.StakeID).stake);

            using (var db = new DBmsw())
            {
                var targetWardStake = db.tWardStakes.SingleOrDefault(x => x.WardStakeID == wardStake.WardID);
                db.tWardStakes.DeleteOnSubmit(targetWardStake);
                db.SubmitChanges();
            }

		}

		private WardStake(tWardStake wardStake)
		{
			this.WardID = wardStake.WardStakeID;
			StakeID = wardStake.StakeID;
			Approved = wardStake.Approved;
		}

		private WardStake(double WardID)
		{
            using (var db = new DBmsw())
            {
                tWardStake wardStake = db.tWardStakes.FirstOrDefault(x => x.WardStakeID == WardID);

                this.WardID = wardStake.WardStakeID;
                StakeID = wardStake.StakeID;
                Approved = wardStake.Approved;
            }
		}

		private static void _nukeCacheKeys(double StakeID, string stakeName)
		{
			Repository r = Repository.getInstance();
			r.removeStakeWards(StakeID);
			Cache.Remove("SelectList-Stake-" + true + ":" + StakeID);
			Cache.Remove("SupportedWards:" + stakeName);
		}
	}
}