using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using MSW;
using MSW.Model;
using MSW.Utilities;
using MSW.Models.dbo;

namespace MSW.Models
{
	/// <summary>
	/// Genereates all the dropdowns for the site. Dynamic and most static. Stake Positions are found in the controller.
	/// </summary>
	public class DropDowns
	{
		private List<SelectListItem> ResidenceList;
		private SelectListItem[] CallingStatusList;
		private List<SelectListItem> OrganizationList;
		private List<SelectListItem> CallingList;
		private SelectListItem[] RolesList;
		private String[] RolesText;
		private String[] RolesValue;
		private SelectListItem[] CarrierList;
		private String[] CarrierText;
		private String[] CarrierValue;
		private SelectListItem[] SupportedWardsList;
		private SelectListItem[] SupportedStakesList;
		private String[] PriesthoodText;
		private String[] PriesthoodValue;
		private SelectListItem[] PriesthoodList;
		private String[] TimeWardText;
		private String[] TimeWardValue;
		private SelectListItem[] TimeWardList;
		private String[] SchoolText;
		private String[] SchoolValue;
		private SelectListItem[] SchoolList;
		private String[] EmployedText;
		private String[] EmployedValue;
		private SelectListItem[] EmployedList;
		private SelectListItem[] OrgPresets;
		private Repository r
		{
			get
			{
				return Repository.getInstance();
			}
		}

		public DropDowns()
		{

			PriesthoodList = new SelectListItem[7];
			TimeWardList = new SelectListItem[7];
			SchoolList = new SelectListItem[4];
			EmployedList = new SelectListItem[4];
			CallingStatusList = new SelectListItem[5];

		}

		public void generateLists()
		{
			generatePriesthoodList();
			generateTimeWardList();
			generateSchoolList();
			generateEmployedList();
		}

		//Used for members to join a ward
		public void generateSupportedWardList()
		{
			List<string> locationList = r.getWardSelectList();
			int position = 0;
			SupportedWardsList = new SelectListItem[locationList.Count + 1];
			foreach (string location in locationList)
			{
				SupportedWardsList[position++] = new SelectListItem { Text = location, Value = location };
			}

			SupportedWardsList[position++] = new SelectListItem { Text = "My location is not here.", Value = "0" };
		}

		//Used for wards to join a stake
		public void generateSupportedStakeList()
		{
			List<string> supportedStakes = r.getSelectStakeList();
			int position = 0;
			SupportedStakesList = new SelectListItem[supportedStakes.Count + 1];
			foreach (string location in supportedStakes)
			{
				SupportedStakesList[position++] = new SelectListItem { Text = location, Value = location };
			}

			SupportedStakesList[position++] = new SelectListItem { Text = "My location is not here.", Value = "0" };
		}

		public void generateRolesList()
		{
			RolesText = new String[] { "Activities", 
                                       "Clerk", 
                                       "Bishopric", 
                                       "Elders Quorum", 
                                       "Emergency Prep.",
                                       "Employment Specialist", 
                                       "FHE", 
                                       "Institute Rep", 
                                       "Ward Mission Leader", 
                                       "Music Chair", 
                                       "Relief Society", 
                                       "Service",
                                       "Teaching", 
                                       "Temple/Family History", 
                                       "Member", 
                                       "Prospective Member"};
			RolesValue = new String[] { "Activities", 
                                        "Clerk", 
                                        "Bishopric", 
                                        "Elders Quorum",
                                        "Emergency", 
                                        "Employment",
                                        "FHE",
                                        "Institute",
                                        "Mission",
                                        "Music",
                                        "Relief Society", 
                                        "Activities", //Used for Service co-chair Authentication
                                        "Teaching",
                                        "Temple/FamHist", 
                                        "Member", 
                                        "Member?" };

			RolesList = new SelectListItem[RolesText.Length];

			for (int i = 0; i < RolesText.Length; i++)
			{
				RolesList[i] = new SelectListItem { Text = RolesText[i], Value = RolesValue[i] };
			}
		}

		public void generateClerkFriendlyRolesList()
		{
			RolesText = new String[] { "Activities", 
                                       "Clerk", 
                                       "Elders Quorum", 
                                       "Emergency Prep.",
                                       "Employment Specialist", 
                                       "FHE", 
                                       "Institute Rep", 
                                       "Ward Mission Leader", 
                                       "Music Chair", 
                                       "Relief Society", 
                                       "Service",
                                       "Teaching", 
                                       "Temple/Family History", 
                                       "Member", 
                                       "Prospective Member"};
			RolesValue = new String[] { "Activities", 
                                        "Clerk",
                                        "Elders Quorum",
                                        "Emergency", 
                                        "Employment",
                                        "FHE",
                                        "Institute",
                                        "Mission",
                                        "Music",
                                        "Relief Society", 
                                        "Activities", //Used for Service co-chair Authentication
                                        "Teaching",
                                        "Temple/FamHist", 
                                        "Member", 
                                        "Member?" };

			RolesList = new SelectListItem[RolesText.Length];

			for (int i = 0; i < RolesText.Length; i++)
			{
				RolesList[i] = new SelectListItem { Text = RolesText[i], Value = RolesValue[i] };
			}
		}

		public void generateCarrierList()
		{
			CarrierText = new String[] {"Cell Phone Provider",
                                       "ACS Wireless", 
                                       "Alltel", 
                                       "AT&T", 
                                       "Bell Mobility",
                                       "Blue Sky Frog", 
                                       "Bluegrass Cellular", 
                                       "Boost Mobile", 
                                       "Cricket",
                                       "Nextel", 
                                       "Qwest", 
                                       "Sprint", 
                                       "T-Mobile",
                                       "Tracfone", 
                                       "US Cellular", 
                                       "Verizon", 
                                       "Virgin Mobile"};
			CarrierValue = new String[] {null,
                                        "@paging.acswireless.com", 
                                        "@message.alltel.com", 
                                        "@txt.att.net",
                                        "@txt.bellmobility.ca", 
                                        "@blueskyfrog.com",
                                        "@sms.bluecell.com",
                                        "@myboostmobile.com",
                                        "@mms.mycricket.com",
                                        "@messaging.nextel.com",
                                        "@qwestmp.com",
                                        "@messaging.sprintpcs.com", 
                                        "@tmomail.net", 
                                        "@txt.att.net",
                                        "@email.uscc.net", 
                                        "@vtext.com", 
                                        "@vmobl.com" };

			CarrierList = new SelectListItem[CarrierText.Length];

			for (int i = 0; i < CarrierText.Length; i++)
			{
				CarrierList[i] = new SelectListItem { Text = CarrierText[i], Value = CarrierValue[i] };
			}
		}

		private void generateEmployedList()
		{
			EmployedText = new String[] { "Full-Time", "Part-Time", "Looking", "Just School" };
			EmployedValue = new String[] { "Full-Time", "Part-Time", "Looking", "Just School" };

			for (int i = 0; i < EmployedText.Length; i++)
			{
				EmployedList[i] = new SelectListItem { Text = EmployedText[i], Value = EmployedValue[i] };
			}
		}

		private void generateTimeWardList()
		{
			TimeWardText = new String[] { "One Semester", "Two Semesters", "More Than Two Semesters", "Less than 6 months",
            "6+ months", "12+ Months", "Indefinitely"};
			TimeWardValue = new String[] { "One Semester", "Two Semesters", "More Than Two Semesters", "Less than 6 months",
            "6+ months", "12+ Months", "Indefinitely" };

			for (int i = 0; i < TimeWardText.Length; i++)
			{
				TimeWardList[i] = new SelectListItem { Text = TimeWardText[i], Value = TimeWardValue[i] };
			}
		}

		private void generateSchoolList()
		{
			SchoolText = new String[] { "Yes, BYU", "Yes, UVU", "Other", "No" };
			SchoolValue = new String[] { "BYU", "UVU", "Other", "No" };

			for (int i = 0; i < SchoolText.Length; i++)
			{
				SchoolList[i] = new SelectListItem { Text = SchoolText[i], Value = SchoolValue[i] };
			}
		}

		private void generatePriesthoodList()
		{
			PriesthoodText = new String[] { "Deacon", "Teacher", "Priest", "Elder", "High Priest", "Not Ordained", "N/A" };
			PriesthoodValue = new String[] { "Deacon", "Teacher", "Priest", "Elder", "High Priest", "Not Ordained", "N/A" };

			for (int i = 0; i < PriesthoodText.Length; i++)
			{
				PriesthoodList[i] = new SelectListItem { Text = PriesthoodText[i], Value = PriesthoodValue[i] };
			}
		}

		private void generateCallingStatusList()
		{
			var StatusText = new String[] { "Submitted", "Approved", "Called", "Sustained", "Set Apart" };
			var StatusValue = new String[] { "0", "1", "2", "3", "4" };

			for (int i = 0; i < StatusText.Length; i++)
			{
				CallingStatusList[i] = new SelectListItem { Text = StatusText[i], Value = StatusValue[i] };
			}
		}

		public void generateApartmentList(string WardStakeID)
		{
			List<Residence> residences = Cache.GetList(r.ResidenceIDs(double.Parse(WardStakeID)), x => Cache.getCacheKey<Residence>(x),
																	y => Residence.get(y));

            residences = residences.OrderBy(x => x.SortID).ToList();
			try
			{
				ResidenceList = new List<SelectListItem>();
				foreach (var residence in residences)
				{
					ResidenceList.Add(new SelectListItem { Text = residence.residence, Value = residence.residence });
				}
			}
			catch
			{
				ResidenceList = new List<SelectListItem>();
			}
		}

		private void generateReportList()
		{
			RolesText = new String[] { "Activities", 
                                       "Clerk", 
                                       "Bishopric", 
                                       "Elders Quorum", 
                                       "Emergency Prep.",
                                       "Employment", 
                                       "Home Evening", 
                                       "Institute Rep", 
                                       "Ward Mission", 
                                       "Music", 
                                       "Relief Society",
                                       "Teaching", 
                                       "Temple/Family History"};
			RolesValue = new String[] { "Activities", 
                                        "Clerk", 
                                        "Bishopric", 
                                        "Elders Quorum",
                                        "Emergency", 
                                        "Employment",
                                        "FHE",
                                        "Institute",
                                        "Mission",
                                        "Music",
                                        "Relief Society",
                                        "Teaching",
                                        "Temple/FamHist", };

			RolesList = new SelectListItem[RolesText.Length];

			for (int i = 0; i < RolesText.Length; i++)
			{
				RolesList[i] = new SelectListItem { Text = RolesText[i], Value = RolesValue[i] };
			}
		}

		private void generateOrganizationList(double? WardStakeID)
		{
			OrganizationList = new List<SelectListItem>();
			try
			{
                using (var db = new DBmsw())
                {
                    var orgs = db.tOrganizations.Where(x => x.WardID == WardStakeID).OrderBy(x => x.SortID);
                    foreach (var org in orgs)
                    {
                        OrganizationList.Add(new SelectListItem { Text = org.Title, Value = org.OrgID.ToString() });
                    }
                }
			}
			catch { }
		}

		private void generateCallingList(int CallingID)
		{
			if (CallingID != 0)
			{
                using (var db = new DBmsw())
                {
                    var thisCallling = db.tCallings.Where(x => x.CallingID == CallingID).SingleOrDefault();
                    var callings = db.tCallings.Where(x => x.OrgID == thisCallling.OrgID).OrderBy(x => x.SortID);
                    CallingList = new List<SelectListItem>();

                    try
                    {
                        foreach (var calling in callings)
                        {
                            CallingList.Add(new SelectListItem { Text = calling.Title, Value = calling.CallingID.ToString() });
                        }
                    }
                    catch { }
                }
			}
			else
				CallingList = new List<SelectListItem>();
		}

		private void generateOrgPresets()
		{
			String[] OrgText = new String[] {"Custom",
                                       "Bishopric",
                                       "Clerk",                                         
                                       "Elders Quorum", 
                                       "Relief Society",
                                       "Sunday School",
                                       "Home Evening", 
                                       "Activities",
                                       "Ward Mission",
                                       "Temple/Family History",  
                                       "Music",
                                       "Publicity Committee", 
                                       "Institute Rep", 
                                       "Emergency Prep.",
                                       "Employment"};
			int[] OrgValue = new int[] { -1, 
                                        (int)SortID.BISHOPRIC, 
                                        (int)SortID.CLERK, 
                                        (int)SortID.ELDERS_QUORUM, 
                                        (int)SortID.RELIEF_SOCIETY,
                                        (int)SortID.SUNDAY_SCHOOL, 
                                        (int)SortID.HOME_EVENING,
                                        (int)SortID.ACTIVITIES,
                                        (int)SortID.WARD_MISSION,
                                        (int)SortID.TEMPLE_FAMHIST,
                                        (int)SortID.MUSIC,
                                        (int)SortID.PUBLICITY,
                                        (int)SortID.INSTITUTE,
                                        (int)SortID.EMERGENCY,
                                        (int)SortID.EMPLOYMENT};

			OrgPresets = new SelectListItem[OrgText.Length];

			for (int i = 0; i < OrgText.Length; i++)
			{
				OrgPresets[i] = new SelectListItem { Text = OrgText[i], Value = OrgValue[i].ToString() };
			}
		}

		public IEnumerable<SelectListItem> getApartmentList()
		{
			return ResidenceList;
		}

		public IEnumerable<SelectListItem> getReportList()
		{
			generateReportList();
			return RolesList;
		}

		public IEnumerable<SelectListItem> getOrgPresets()
		{
			generateOrgPresets();
			return OrgPresets;
		}

		public IEnumerable<SelectListItem> getCallingList(int CallingID)
		{
			generateCallingList(CallingID);
			return CallingList;
		}

		public IEnumerable<SelectListItem> getCallingStatusList()
		{
			generateCallingStatusList();
			return CallingStatusList;
		}

		public IEnumerable<SelectListItem> getCarrierList()
		{
			return CarrierList;
		}

		public IEnumerable<SelectListItem> getOrganizationList(double? WardStakeID)
		{
			generateOrganizationList(WardStakeID);
			return OrganizationList;
		}

		public IEnumerable<SelectListItem> getPriesthoodList()
		{
			return PriesthoodList;
		}

		public IEnumerable<SelectListItem> getTimeWardList()
		{
			return TimeWardList;
		}

		public IEnumerable<SelectListItem> getSchoolList()
		{
			return SchoolList;
		}

		public IEnumerable<SelectListItem> getEmployedList()
		{
			return EmployedList;
		}

		public IEnumerable<SelectListItem> getSupportedWardsList()
		{
			return SupportedWardsList;
		}

		public IEnumerable<SelectListItem> getSupportedStakesList()
		{
			return SupportedStakesList;
		}

		public IEnumerable<SelectListItem> getRolesList()
		{
			return RolesList;
		}
	}
}