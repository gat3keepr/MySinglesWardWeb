using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class Residence
	{
        public int id { get; set; }
        public double WardStakeID { get; set; }
		public string residence { get; set; }
        public string streetAddress { get; set; }
        public int SortID { get; set; }

		public static Residence get(int id)
		{
			Residence residence = Cache.Get(Cache.getCacheKey<Residence>(id)) as Residence;

			if (residence == null)
			{
				residence = new Residence(id);

				Cache.Set(Cache.getCacheKey<Residence>(id), residence);
			}

			return residence;
		}

		public static void save(Residence residence)
		{
			Cache.Remove(Cache.getCacheKey<Residence>(residence.id));
            Cache.Remove("Residences:" + residence.WardStakeID);

            using (var db = new DBmsw())
            {
                var targetResidence = db.tResidences.SingleOrDefault(x => x.id == residence.id);

                if (targetResidence == null)
                {
                    targetResidence = new tResidence();
                    targetResidence.id = residence.id;
                    db.tResidences.InsertOnSubmit(targetResidence);
                }

                targetResidence.Residence = residence.residence;
                targetResidence.WardStakeID = residence.WardStakeID;
                targetResidence.StreetAddress = residence.streetAddress;
                targetResidence.SortID = residence.SortID;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<Residence>(residence.id), residence);
            }
		}

		public static int create(tResidence residence)
		{
            Cache.Remove("Residences:" + residence.WardStakeID);

            using (var db = new DBmsw())
            {
                var targetRes = new tResidence();
                db.tResidences.InsertOnSubmit(targetRes);

                targetRes.Residence = residence.Residence;
                targetRes.WardStakeID = residence.WardStakeID;

                //Get the max for the SortID
                try
                {
                    targetRes.SortID = db.tResidences.Where(x => x.WardStakeID == residence.WardStakeID).Max(x => x.SortID) + 1;
                }
                catch (InvalidOperationException e)
                {
                    targetRes.SortID = 1;
                }

                db.SubmitChanges();
                Cache.Set(Cache.getCacheKey<Residence>(targetRes.id), new Residence(targetRes.id));
                return targetRes.id;
            }
		}

		public static void remove(Residence residence)
		{
			Cache.Remove(Cache.getCacheKey<Residence>(residence.id));
			Cache.Remove("Residences:" + residence.WardStakeID);

            using (var db = new DBmsw())
            {
                var targetResidence = db.tResidences.SingleOrDefault(x => x.id == residence.id);

                db.tResidences.DeleteOnSubmit(targetResidence);
                db.SubmitChanges();
            }
		}

		private Residence(int id)
		{
            using (var db = new DBmsw())
            {
                var dboResidence = db.tResidences.SingleOrDefault(x => x.id == id);

                WardStakeID = dboResidence.WardStakeID;
                residence = dboResidence.Residence;
                streetAddress = dboResidence.StreetAddress;
                this.id = dboResidence.id;
                this.SortID = dboResidence.SortID;
            }
		}
	}
}