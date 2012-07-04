using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Utilities;

namespace MSW.Models.dbo
{
	[Serializable]
	public class Calling
	{
		public enum Status { NONE, APPROVED, CALLED, SUSTAINED, SET_APART }
		public int CallingID { get; set; }
		public int OrgID { get; set; }
		public string Title { get; set; }
		public int MemberID { get; set; }
		public int CallingStatus { get; set; }
		public DateTime? Approved { get; set; }
		public DateTime? Called { get; set; }
		public DateTime? Sustained { get; set; }
		public DateTime? SetApart { get; set; }
		public bool ITStake { get; set; }
		public int SortID { get; set; }
		public string Description { get; set; }

		public static Calling get(int CallingID)
		{
			Calling calling = Cache.Get(Cache.getCacheKey<Calling>(CallingID)) as Calling;

			if(calling == null)
			{
				using (var db = new DBmsw())
				{
					var dboCalling = db.tCallings.SingleOrDefault(x => x.CallingID == CallingID);
					if (dboCalling == null)
						return null;

					calling = new Calling(dboCalling);

					Cache.Set(Cache.getCacheKey<Calling>(CallingID), calling);
				}
			}

			return calling;
		}

		public static void save(Calling calling)
		{
			Cache.Remove(Cache.getCacheKey<Calling>(calling.CallingID));

			Repository r = Repository.getInstance();
            r.NukeReportKeys(calling.CallingID);

			using (var db = new DBmsw())
			{
				var targetCalling = db.tCallings.SingleOrDefault(x => x.CallingID == calling.CallingID);

				targetCalling.OrgID = calling.OrgID;
				targetCalling.Title = calling.Title;
				targetCalling.MemberID = calling.MemberID;
				targetCalling.Approved = calling.Approved;
				targetCalling.Called = calling.Called;
				targetCalling.Sustained = calling.Sustained;
				targetCalling.SetApart = calling.SetApart;
				targetCalling.ITStake = calling.ITStake;
				targetCalling.Description = calling.Description;
                targetCalling.SortID = calling.SortID;

				db.SubmitChanges();

				Cache.Set(Cache.getCacheKey<Calling>(calling.CallingID), calling);
			}
		}

		public static void remove(Calling calling)
		{
			Cache.Remove(Cache.getCacheKey<Calling>(calling.CallingID));
			Cache.Remove("Callings:" + calling.OrgID);
			Repository.getInstance().NukeReportKeys(calling.CallingID);

			using (var db = new DBmsw())
			{
				db.tCallings.DeleteOnSubmit(db.tCallings.SingleOrDefault(x => x.CallingID == calling.CallingID));
				db.SubmitChanges();
			}
		}

		private Calling(tCalling calling)
        {
            OrgID = calling.OrgID;
            this.CallingID = calling.CallingID;
            Title = calling.Title;
            MemberID = calling.MemberID;

            if (calling.SetApart != null)
            {
                CallingStatus = (int) Status.SET_APART;
            }
            else if (calling.Sustained != null)
            {
                CallingStatus = (int)Status.SUSTAINED;
            }
            else if (calling.Called != null)
            {
                CallingStatus = (int)Status.CALLED;
            }
            else if (calling.Approved != null)
            {
                CallingStatus = (int)Status.APPROVED;
            }
            else 
            {
                CallingStatus = (int)Status.NONE;
            }

            Approved = calling.Approved;
            Called = calling.Called;
            Sustained = calling.Sustained;
            SetApart = calling.SetApart;

            ITStake = calling.ITStake;
            Description = calling.Description;
			SortID = calling.SortID;
        }

		//USED ONLY FOR INHERITENCE
		public Calling() 
		{
			
		}

    }
}