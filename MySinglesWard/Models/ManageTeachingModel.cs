using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Models.dbo;
using MSW.Model;
using System.Web.Mvc;

namespace MSW.Models
{
    /// <summary>
    /// Model used to populate the Manage Teaching View
    /// </summary>
    public class ManageTeachingModel
    {
        public Organization org { get; set; }
        public List<DistrictModel> districts { get; set; }
        public List<SelectListItem> orgCallings { get; set; }
        public ManageTeachingModel(int orgID)
        {
            this.org = Organization.get(orgID);
            this.districts = new List<DistrictModel>();
            orgCallings = new List<SelectListItem>();
            Repository r = Repository.getInstance();

            //Get districts that belong to this organization
            List<District> districts = Cache.GetList(r.getDistricts(orgID), x => Cache.getCacheKey<District>(x), y => District.get(y))
                                            .OrderBy(x => x.Title).ToList();

            foreach (var district in districts)
            {
                this.districts.Add(new DistrictModel(district));
            }

            //Gets the organizations callings for district leader selection
            List<Calling> callings = Cache.GetList(r.CallingIDs(orgID), x => Cache.getCacheKey<Calling>(x), y => Calling.get(y));

            foreach (var calling in callings)
            {
                
                MemberModel member = null;
                if (calling.MemberID != 0 && calling.CallingStatus >= (int)Calling.Status.SUSTAINED)
                {
                    member = MemberModel.get(calling.MemberID);
                }

                string memberName = ((member != null) ? " - " + member.user.LastName + ", " + member.memberSurvey.prefName : "");

                orgCallings.Add(new SelectListItem { Text = calling.Title + memberName,
                    Value = calling.CallingID.ToString(), Selected = false });
            }
        }


    }

    public class DistrictModel
    {
        public District district { get; set; }
        public int? DistrictLeaderID { get; set; }
        public List<CompanionshipModel> companionships { get; set; }
        public CallingModel districtLeader { get; set; }

        public DistrictModel(District district)
        {
            this.district = district;
            DistrictLeaderID = district.DistrictLeaderID;
            companionships = new List<CompanionshipModel>();

            Repository r = Repository.getInstance();

            //Get districts that belong to this organization
            List<Companionship> comps = Cache.GetList(r.getCompanionships(district.DistrictID), x => Cache.getCacheKey<Companionship>(x), y => Companionship.get(y));

            foreach (var comp in comps)
            {
                this.companionships.Add(new CompanionshipModel(comp));
            }

            //Get the calling for district Leader
            if (district.DistrictLeaderID != null)
                districtLeader = new CallingModel((int)district.DistrictLeaderID);
        }

    }

    public class CompanionshipModel
    {
        public Companionship comp { get; set; }
        public List<MemberModel> teachers { get; set; }
        public List<MemberModel> teachees { get; set; }

        public CompanionshipModel(Companionship comp)
        {
            this.comp = comp;
            Repository r = Repository.getInstance();

            teachers = Cache.GetList(r.getTeachers(comp.CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
            teachees = Cache.GetList(r.getTeachees(comp.CompanionshipID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y))
                .OrderBy(x => x.user.LastName).ThenBy(x => x.memberSurvey.prefName).ToList();
        }
    }

}