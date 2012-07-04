using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
	[Serializable]
	public class Ward
	{
		public double? StakeID { get; set; }
		public double WardStakeID { get; set; }
		public string Location { get; set; }
		public string Stake { get; set; }
		public string ward { get; set; }
		public string Password { get; set; }

		public static Ward get(double WardStakeID)
		{
			Ward ward = Cache.Get(Cache.getCacheKey<Ward>(WardStakeID)) as Ward;

			if (ward == null)
			{				
				ward = new Ward(WardStakeID);
				
				Cache.Set(Cache.getCacheKey<Ward>(ward.WardStakeID), ward);
			}

			return ward;
		}

		public static void save(Ward ward)
		{
			Cache.Remove(Cache.getCacheKey<Ward>(ward.WardStakeID));

            using (var db = new DBmsw())
            {
                var targetWard = db.tSupportedWards.SingleOrDefault(x => x.WardStakeID == ward.WardStakeID);

                if (targetWard == null)
                {
                    targetWard = new tSupportedWard();
                    db.tSupportedWards.InsertOnSubmit(targetWard);
                }

                targetWard.WardStakeID = ward.WardStakeID;
                targetWard.Location = ward.Location;
                targetWard.Stake = ward.Stake;
                targetWard.Ward = ward.ward;
                targetWard.Password = Utilities.Cryptography.EncryptString(ward.Password);

                db.SubmitChanges();
                Cache.Set(Cache.getCacheKey<Ward>(ward.WardStakeID), ward);
            }
		}

		public static Ward create(tSupportedWard ward)
		{
            using (var db = new DBmsw())
            {
                db.tSupportedWards.InsertOnSubmit(ward);

                ward.Password = Utilities.Cryptography.EncryptString(ward.Password);
                db.SubmitChanges();

                Ward newWard = new Ward(ward.WardStakeID);
                Cache.Set(Cache.getCacheKey<Ward>(ward.WardStakeID), newWard);

				Repository r = Repository.getInstance();
                r.removeUnitSelectionCache(newWard);

                return newWard;
            }
		}

		private Ward(double WardStakeID)
		{
            using (var db = new DBmsw())
            {
                var ward = db.tSupportedWards.SingleOrDefault(x => x.WardStakeID == WardStakeID);
                if (ward == null)
                    ward = db.tSupportedWards.SingleOrDefault(x => x.WardStakeID == 0);

                StakeID = db.tWardStakes.Where(x => x.WardStakeID == ward.WardStakeID).Select(x => x.StakeID).SingleOrDefault();
                this.WardStakeID = ward.WardStakeID;
                Location = ward.Location;
                Stake = ward.Stake;
                this.ward = ward.Ward;
                Password = (this.WardStakeID != 0) ? Utilities.Cryptography.DecryptString(ward.Password) : " ";
            }
		}
    }
}