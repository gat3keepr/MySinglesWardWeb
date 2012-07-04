using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Utilities;

namespace MSW.Models.dbo
{
    [Serializable]
    public class WardInfo
    {
		public double WardID { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public int Zipcode { get; set; }

		public static WardInfo get(double WardID)
		{
			WardInfo ward = Cache.Get(Cache.getCacheKey<WardInfo>(WardID)) as WardInfo;

			if (ward == null)
			{
                using (var db = new DBmsw())
                {
                    var wardDBO = db.tWardInfos.SingleOrDefault(x => x.WardID == WardID);

                    if (wardDBO == null)
                        return null;

                    ward = new WardInfo(wardDBO);

                    Cache.Set(Cache.getCacheKey<WardInfo>(ward.WardID), ward);
                }
			}

			return ward;
		}

		public static void save(WardInfo ward)
		{
			Cache.Remove(Cache.getCacheKey<WardInfo>(ward.WardID));

            using (var db = new DBmsw())
            {
                var targetWard = db.tWardInfos.SingleOrDefault(x => x.WardID == ward.WardID);

                if (targetWard == null)
                {
                    targetWard = new tWardInfo();
                    db.tWardInfos.InsertOnSubmit(targetWard);
                }

                targetWard.WardID = ward.WardID;
                targetWard.City = ward.City;
                targetWard.State = ward.State;
                targetWard.Zipcode = ward.Zipcode;

                db.SubmitChanges();
                Cache.Set(Cache.getCacheKey<WardInfo>(ward.WardID), ward);
            }
		}

		public static WardInfo create(tWardInfo ward)
		{
            using (var db = new DBmsw())
            {
                db.tWardInfos.InsertOnSubmit(ward);

                db.SubmitChanges();

                WardInfo newWard = new WardInfo(ward.WardID);
                Cache.Set(Cache.getCacheKey<WardInfo>(ward.WardID), newWard);

                return newWard;
            }
		}

        private WardInfo(double WardID)
		{
            using (var db = new DBmsw())
            {
                var ward = db.tWardInfos.SingleOrDefault(x => x.WardID == WardID);

                this.WardID = ward.WardID;
                City = ward.City;
                State = ward.State;
                this.Zipcode = ward.Zipcode;
            }
		}

        private WardInfo(tWardInfo ward)
        {
            this.WardID = ward.WardID;
            City = ward.City;
            State = ward.State;
            this.Zipcode = ward.Zipcode;
        }
    }
}