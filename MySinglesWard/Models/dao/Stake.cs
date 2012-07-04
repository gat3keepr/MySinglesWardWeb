using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class Stake
	{
		public double StakeID { get; set; }
		public string Location { get; set; }
		public string stake { get; set; }
		public string Password { get; set; }

		public static Stake get(double StakeID)
		{
			Stake stake = Cache.Get(Cache.getCacheKey<Stake>(StakeID)) as Stake;

			if (stake == null)
			{
                using (var db = new DBmsw())
                {
                    var dboStake = db.tSupportedStakes.SingleOrDefault(x => x.StakeID == StakeID);
                    stake = new Stake(dboStake);

                    Cache.Set(Cache.getCacheKey<Stake>(stake.StakeID), stake);
                }
			}

			return stake;
		}

		public static void save(Stake stake)
		{
			Cache.Remove(Cache.getCacheKey<Stake>(stake.StakeID));

            using (var db = new DBmsw())
            {
                var targetStake = db.tSupportedStakes.SingleOrDefault(x => x.StakeID == stake.StakeID);

                targetStake.StakeID = stake.StakeID;
                targetStake.Location = stake.Location;
                targetStake.Stake = stake.stake;
                targetStake.Password = Utilities.Cryptography.EncryptString(stake.Password);

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<Stake>(stake.StakeID), stake);
            }
		}

		public static Stake create(tSupportedStake stake)
		{
            using (var db = new DBmsw())
            {
                db.tSupportedStakes.InsertOnSubmit(stake);

                stake.Password = Utilities.Cryptography.EncryptString(stake.Password);
                db.SubmitChanges();

                Stake newStake = new Stake(stake);
                Cache.Set(Cache.getCacheKey<Stake>(stake.StakeID), newStake);

				Repository r = Repository.getInstance();
                r.removeUnitSelectionCache(newStake);

                return newStake;
            }
		}

		private Stake(tSupportedStake stake)
		{
			StakeID = stake.StakeID;
			Location = stake.Location;
			this.stake = stake.Stake;
            if(stake.Password != " ")
			    Password = Utilities.Cryptography.DecryptString(stake.Password);
		}
	}
}