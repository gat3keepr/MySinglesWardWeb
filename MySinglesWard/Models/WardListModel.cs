using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using MSW.Model;
using MSW.Utilities;
using MSW.Models.dbo;
using System.Text;
using Newtonsoft.Json;

namespace MSW.Models
{
	/// <summary>
	/// Gets the members and the bishopric and sorts them for the Ward List View
	/// </summary>
    public class WardListModel
    {
        public List<BishopricModel> bishopric { get; set; }
        public List<MemberModel> members { get; set; }
        public double WardID { get; set; }
        public WardListModel(double WardStakeID)
        {
			Repository r = Repository.getInstance();
            WardID = WardStakeID;

			bishopric = Cache.GetList(r.BishopricMembersID(WardStakeID), x => Cache.getCacheKey<BishopricModel>(x), y => BishopricModel.get(y));

			//Orders By Calling
			bishopric = bishopric.OrderBy(x => x.data.SortID).ToList();

			//Members
			members = Cache.GetList(r.WardMembersID(WardStakeID), x => Cache.getCacheKey<MemberModel>(x), y => MemberModel.get(y));

			//Default list order
			members = members.OrderBy(x => x.memberSurvey.gender).ThenBy(x => x.memberSurvey.prefName).ThenBy(x => x.user.LastName).ToList();
        }

        internal string ToJson(bool leadership)
        {
            return "{ \"ward\" : " + JsonConvert.SerializeObject(Ward.get(WardID)) + " , \"bishopric\" : " + bishopricJSON(bishopric) + ", \"members\" : " + (leadership ? leadershipJSON(members) : membersJSON(members)) + "}";
        }

        private string bishopricJSON(List<BishopricModel> bishopric)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");

            foreach(BishopricModel bm in bishopric)
            {
                json.Append(JsonConvert.SerializeObject(bm) + ",");
            }

            return json.ToString(0, json.ToString().Length - 1) + "]";
        }

        private string leadershipJSON(List<MemberModel> members)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");

            foreach (MemberModel m in members)
            {
                json.Append(m.leadershipJSON() + ",");
            }

            return json.ToString(0, json.ToString().Length - 1) + "]";
        }

        private string membersJSON(List<MemberModel> members)
        {
            StringBuilder json = new StringBuilder();
            json.Append("[");

            foreach (MemberModel m in members)
            {
                json.Append(m.membershipJSON() + ",");
            }

            return json.ToString(0, json.ToString().Length - 1) + "]";
        }
    }
}