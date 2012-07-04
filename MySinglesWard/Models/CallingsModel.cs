using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using MSW.Models.dbo;
using MSW.Utilities;

namespace MSW.Models
{
	public class CallingsModel
	{
		public List<OrganizationModel> organizations;

		public CallingsModel(double WardID)
		{
			organizations = new List<OrganizationModel>();

            Repository r = Repository.getInstance();
			foreach (int orgID in r.OrganizationIDs(WardID))
			{
				organizations.Add(new OrganizationModel(orgID));
			}
			organizations = organizations.OrderBy(x => x.data.SortID).ToList();
		}

	}

	/// <summary>
	/// Helper Class used to display organizations on the main calling management view
	/// </summary>
	public class OrganizationModel
	{
		public Organization data { get; set; }
		public string ReportID { get; set; }
		public int? LeaderCallingID { get; set; }
		public List<CallingModel> Callings { get; set; }
		public List<SelectListItem> CallingList { get; set; }
		//public List<ListItem> Members { get; set; }

		//Co-Leader Callings
		public List<ListItem> CoLeaders { get; set; }

		public OrganizationModel(int OrgID)
		{
			data = Organization.get(OrgID);
			ReportID = data.ReportID;
			LeaderCallingID = data.LeaderCallingID;
			//Callings
			Callings = new List<CallingModel>();
			foreach (int callingID in Repository.getInstance().CallingIDs(OrgID))
			{
				Callings.Add(new CallingModel(callingID));
			}
			Callings = Callings.OrderBy(x => x.SortID).ToList();

			CallingList = new List<SelectListItem>();
			foreach (var calling in Callings)
			{
				CallingList.Add(new SelectListItem { Text = calling.Title, Value = calling.CallingID.ToString() });
			}			

			//Co-Leader Callings
            List<Calling> coLeaderCallings = Cache.GetList(Repository.getInstance().CoLeaderIDs(OrgID), x => Cache.getCacheKey<Calling>(x), y => Calling.get(y));
			CoLeaders = new List<ListItem>();
			foreach (var calling in coLeaderCallings)
			{
				CoLeaders.Add(new ListItem { Text = calling.Title, Value = calling.CallingID.ToString() });
			}
		}
	}

	/// <summary>
	/// Helper class to show information about callings on the main calling view
	/// </summary>
	public class CallingModel : Calling
	{
		public MemberModel member { get; set; }

		public CallingModel(int CallingID)
		{
			Calling calling = Calling.get(CallingID);
			this.OrgID = calling.OrgID;
			this.CallingID = calling.CallingID;
			Title = calling.Title;
			this.MemberID = calling.MemberID;

			if (calling.SetApart != null)
			{
				this.CallingStatus = (int)Status.SET_APART;
			}
			else if (calling.Sustained != null)
			{
				this.CallingStatus = (int)Status.SUSTAINED;
			}
			else if (calling.Called != null)
			{
				this.CallingStatus = (int)Status.CALLED;
			}
			else if (calling.Approved != null)
			{
				this.CallingStatus = (int)Status.APPROVED;
			}
			else
			{
				this.CallingStatus = (int)Status.NONE;
			}

			this.Approved = calling.Approved;
			this.Called = calling.Called;
			this.Sustained = calling.Sustained;
			this.SetApart = calling.SetApart;

			this.ITStake = calling.ITStake;
			this.Description = calling.Description;
			this.SortID = calling.SortID;
			if(this.MemberID != 0)
				member = MemberModel.get(this.MemberID);
		}
	}

	/// <summary>
	/// Used to show releases on the release report. 
	/// Releases have different information than a calling so a new class was needed
	/// </summary>
	public class ReleaseModel
	{
		public PendingRelease release { get; set; }
		public Calling calling { get; set; }
		public MemberModel member { get; set; }

		public ReleaseModel(int CallingID)
		{
			release = PendingRelease.get(CallingID);
			calling = Calling.get(CallingID);
			if (release.MemberID != 0)
				member = MemberModel.get(release.MemberID);
		}
	}


}