using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;
using MSW.Utilities;

namespace MSW.Models.dbo
{
    [Serializable]
    public class OrganizationMember
    {
        public int OrgID { get; set; }
		public int MemberID { get; set; }
        public int? PendingOrgID { get; set; }
        public int status { get; set; }
        public enum Status { NONE, PENDING, APPROVED }

		public static OrganizationMember get(int MemberID)
		{
            OrganizationMember orgMember = Cache.Get(Cache.getCacheKey<OrganizationMember>(MemberID)) as OrganizationMember;

			if (orgMember == null)
			{
                using (var db = new DBmsw())
                {
                    tOrganizationMember dboOrgMember = db.tOrganizationMembers.SingleOrDefault(x => x.MemberID == MemberID);

                    if (dboOrgMember == null)
                        return null;
                    orgMember = new OrganizationMember(dboOrgMember);

                    Cache.Set(Cache.getCacheKey<OrganizationMember>(MemberID), orgMember);
                }
			}

			return orgMember;
		}

		public static void save(OrganizationMember orgMember)
		{
			Cache.Remove(Cache.getCacheKey<OrganizationMember>(orgMember.MemberID));
			_nukeCacheKeys(orgMember.OrgID);

            using (var db = new DBmsw())
            {
                var targetOrgMember = db.tOrganizationMembers.SingleOrDefault(x => x.MemberID == orgMember.MemberID);
                _nukeCacheKeys(targetOrgMember.OrgID);

                targetOrgMember.OrgID = orgMember.OrgID;
                targetOrgMember.MemberID = orgMember.MemberID;
                targetOrgMember.PendingOrgID = orgMember.PendingOrgID;
                targetOrgMember.Status = orgMember.status;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<OrganizationMember>(orgMember.MemberID), orgMember);
            }
		}

		public static void create(tOrganizationMember orgMember)
		{
            using (var db = new DBmsw())
            {
                db.tOrganizationMembers.InsertOnSubmit(orgMember);
                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<OrganizationMember>(orgMember.MemberID), new OrganizationMember(orgMember.MemberID));
                
                //Nuke the cache for both pending and regular
                _nukeCacheKeys(orgMember.OrgID);
                if(orgMember.PendingOrgID != null)
                    _nukeCacheKeys((int)orgMember.PendingOrgID);
            }
		}

		public static void remove(OrganizationMember orgMember)
		{
            if (orgMember == null)
                return;

            Cache.Remove(Cache.getCacheKey<OrganizationMember>(orgMember.MemberID));
			_nukeCacheKeys(orgMember.OrgID);

            using (var db = new DBmsw())
            {
                var targetOrgMember = db.tOrganizationMembers.SingleOrDefault(x => x.MemberID == orgMember.MemberID);
                db.tOrganizationMembers.DeleteOnSubmit(targetOrgMember);
                db.SubmitChanges();
            }
		}

		private OrganizationMember(tOrganizationMember orgMember)
		{
			OrgID = orgMember.OrgID;
			MemberID = orgMember.MemberID;
            PendingOrgID = orgMember.PendingOrgID;
            status = orgMember.Status;
		}

        private OrganizationMember(int MemberID)
		{
            using (var db = new DBmsw())
            {
                tOrganizationMember orgMember = db.tOrganizationMembers.FirstOrDefault(x => x.MemberID == MemberID);

                this.MemberID = orgMember.MemberID;
                OrgID = orgMember.OrgID;
                PendingOrgID = orgMember.PendingOrgID;
                status = orgMember.Status;
            }
		}

		private static void _nukeCacheKeys(int orgID)
		{
			Repository r = Repository.getInstance();
            r.removeOrganizationMembership(orgID);
		}
    }
}