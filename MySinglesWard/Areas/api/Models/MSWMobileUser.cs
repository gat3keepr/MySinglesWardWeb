using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;
using MSW.Models.dbo;

namespace MSW.Areas.api.Models
{
	[Serializable]
	public class MSWMobileUser
	{
		public int MemberID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public double WardStakeID { get; set; }
		public bool IsBishopric { get; set; }
		public bool RecordsRequested { get; set; }
        public DateTime DateCreated { get; set; }
        public string PrefName { get; set; }
        public string CellPhone { get; set; }
        public string Residence { get; set; }

		public static MSWMobileUser getUser(int MemberID)
		{
            return new MSWMobileUser(MSWUser.getUser(MemberID));
		}

		public static MSWMobileUser getUser(string UserName)
		{
			return new MSWMobileUser(MSWUser.getUser(UserName));
		}

        private MSWMobileUser(MSWUser user)
		{
			MemberID = user.MemberID;

			UserName = user.UserName;
            Email = user.Email;
			WardStakeID = (user.WardStakeID != null) ? double.Parse(user.WardStakeID.ToString()) : 0;
			IsBishopric = user.IsBishopric;
            DateCreated = user.DateCreated;

			if (user.RecordsRequested == null)
				RecordsRequested = false;
			else
				RecordsRequested = bool.Parse(user.RecordsRequested.ToString());

                try
                {
                    LastName = user.LastName;
                    FirstName = user.FirstName;
                }
                catch
                {
                    LastName = " ";
                    FirstName = " ";
                }

                
            //Get Preferred Name for app
            try
            {
                MSW.Models.dbo.MemberSurvey survey = MSW.Models.dbo.MemberSurvey.getMemberSurvey(user.MemberID);
                PrefName = survey.prefName;
                Residence = survey.residence;
                CellPhone = survey.cellPhone;
            }
            catch
            {
                PrefName = FirstName;
                Residence = "Not Available";
                CellPhone = "Not Available";
            }

            //Change to the bishopric Data if the request comes from a bishopric member
            if(IsBishopric)
            {
                BishopricData data = BishopricData.get(MemberID);
               
                if (data.BishopricName != "")
                {
                    PrefName = data.BishopricName;
                    CellPhone = data.BishopricPhone;
                    Residence = data.BishopricAddress;
                }
            }
		}
	}
}