using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class Companionship
    {
        public int CompanionshipID { get; set; }
        public int DistrictID { get; set; }

		public static Companionship get(int CompanionshipID)
		{
			Companionship comp = Cache.Get(Cache.getCacheKey<Companionship>(CompanionshipID)) as Companionship;

			if (comp == null)
			{
				comp = new Companionship(CompanionshipID);
				Cache.Set(Cache.getCacheKey<Companionship>(CompanionshipID), comp);
			}

			return comp;
		}

		public static void save(Companionship comp)
		{
			Cache.Remove(Cache.getCacheKey<Companionship>(comp.CompanionshipID));

            _NukeCache(comp);

			using (var db = new DBmsw())
			{
				var targetComp = db.tCompanionships.SingleOrDefault(x => x.CompanionshipID == comp.CompanionshipID);
                
                targetComp.CompanionshipID = comp.CompanionshipID;
				targetComp.DistrictID = comp.DistrictID;                

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<Companionship>(comp.CompanionshipID), comp);
			}
		}

        public static Companionship create(tCompanionship comp)
        {
            using (var db = new DBmsw())
            {
                db.tCompanionships.InsertOnSubmit(comp);

                db.SubmitChanges();

                Companionship newComp = new Companionship(comp.CompanionshipID);
                Cache.Set(Cache.getCacheKey<Companionship>(newComp.CompanionshipID), newComp);

                _NukeCache(newComp);

                return newComp;
            }
        }

		public static void remove(Companionship comp)
		{
			Cache.Remove(Cache.getCacheKey<Companionship>(comp.CompanionshipID));

            _NukeCache(comp);

			using (var db = new DBmsw())
			{
				db.tCompanionships.DeleteOnSubmit(db.tCompanionships.SingleOrDefault(x => x.CompanionshipID == comp.CompanionshipID));
				db.SubmitChanges();
			}
		}

        private Companionship(int CompanionshipID)
		{
			using (var db = new DBmsw())
			{
				var comp = db.tCompanionships.SingleOrDefault(x => x.CompanionshipID == CompanionshipID);

				this.CompanionshipID = comp.CompanionshipID;
				DistrictID = comp.DistrictID;
			}
		}

        private static void _NukeCache(Companionship comp)
        {
            Repository r = Repository.getInstance();

            r.removeCompanionships(comp.DistrictID);
        }
    }
}