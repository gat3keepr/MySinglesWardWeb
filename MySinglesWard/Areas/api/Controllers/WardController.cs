using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSW.Models.dbo;
using Newtonsoft.Json;
using MSW.Utilities;
using MSW.Areas.api.Models;
using MSW.Model;

namespace MSW.Areas.api.Controllers
{
    public class WardController : Controller
    {
        //
        // GET: /api/WardList/
        [Authorize]
        public string ListIDs()
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            //Dont let the user get the list of all the people who are not in a ward
            if (user.WardStakeID == 0)
            {
                return JsonConvert.SerializeObject(new { members = new List<MemberMobileModel>() });
            }

            //Members
            List<MemberMobileModel> members = Cache.GetList(Repository.getInstance().WardMembersID(user.WardStakeID), x => Cache.getCacheKey<MemberMobileModel>(x), y => MemberMobileModel.get(y));
            members = members.OrderBy(x => x.user.LastName).ThenBy(x => x.user.PrefName).ThenBy(x => x.user.FirstName).ToList();

            return JsonConvert.SerializeObject(new { members = members.Select(x => x.user.MemberID) });
        }

        [Authorize]
        public string List()
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            //Dont let the user get the list of all the people who are not in a ward
            if (user.WardStakeID == 0)
            {
                return JsonConvert.SerializeObject(new { members = new List<MemberMobileModel>() });
            }

            //Members
            List<MemberMobileModel> members = Cache.GetList(Repository.getInstance().WardMembersID(user.WardStakeID), x => Cache.getCacheKey<MemberMobileModel>(x), y => MemberMobileModel.get(y));
            members = members.OrderBy(x => x.user.LastName).ThenBy(x => x.user.PrefName).ThenBy(x => x.user.FirstName).ToList();

            return JsonConvert.SerializeObject(new { members = members });
        }


        [Authorize]
        public string Bishopric()
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            //Dont let the user get the list of all the people who are not in a ward
            if (user.WardStakeID == 0)
            {
                return JsonConvert.SerializeObject(new { members = new List<MemberMobileModel>() });
            }

            //Members
            List<MemberMobileModel> bishopric = Cache.GetList(Repository.getInstance().BishopricMembersID(user.WardStakeID), x => Cache.getCacheKey<MemberMobileModel>(x), y => MemberMobileModel.get(y));
            bishopric = bishopric.OrderBy(x => x.bishopric.SortID).ToList();

            return JsonConvert.SerializeObject( bishopric );
        }

        [Authorize]
        public string Residences()
        {
            MSWUser user = MSWUser.getUser(User.Identity.Name);

            //Residences
            List<Residence> residences = Cache.GetList(Repository.getInstance().ResidenceIDs(user.WardStakeID), x => Cache.getCacheKey<Residence>(x),
                                                                    y => Residence.get(y));

            residences = residences.OrderBy(x => x.SortID).ToList();
            return JsonConvert.SerializeObject(new { residences = residences.Select(x => x.residence).ToList() });
        }

        [Authorize]
        public string getLocations()
        {
            List<string> locationList = Repository.getInstance().getWardSelectList();
            locationList.Sort();

            return JsonConvert.SerializeObject(new { locations = locationList });
        }

        [Authorize]
        public string getStakes(string location)
        {
            return JsonConvert.SerializeObject(new { stakes = Repository.getInstance().getSelectedStakeList(location) });
        }

        [Authorize]
        public string getWards(string location, string stake)
        {
            List<string> wards = new List<string>();
            List<string> ids = new List<string>();
            var w = Repository.getInstance().getSelectedWardList(location, stake);

            foreach(var ward in w)
            {
                wards.Add(ward.Text);
                ids.Add(ward.Value);
            }

            return JsonConvert.SerializeObject(new { wards = wards, ids = ids });
        }
    }
}
