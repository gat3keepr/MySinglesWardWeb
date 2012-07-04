using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class TeachingOrganization
    {
        //ID of the organization to be taught
        public int TeachingOrganizationID { get; set; }

        public int TeacherID { get; set; }

        public static TeachingOrganization get(int TeachingOrganizationID)
        {
            TeachingOrganization to = Cache.Get(Cache.getCacheKey<TeachingOrganization>(TeachingOrganizationID)) as TeachingOrganization;

            if (to == null)
            {
                using (var db = new DBmsw())
                {
                    var tOrg = db.tTeachingOrganizations.SingleOrDefault(x => x.TeachingOrganizationID == TeachingOrganizationID);
                    if (tOrg == null)
                        return null;

                    to = new TeachingOrganization(tOrg);
                    Cache.Set(Cache.getCacheKey<TeachingOrganization>(TeachingOrganizationID), to);
                }
            }

            return to;
        }

        public static void save(TeachingOrganization to)
        {
            Cache.Remove(Cache.getCacheKey<TeachingOrganization>(to.TeachingOrganizationID));

            _NukeCache(to);

            using (var db = new DBmsw())
            {
                var targetMR = db.tTeachingOrganizations.SingleOrDefault(x => x.TeachingOrganizationID == to.TeachingOrganizationID);

                targetMR.TeachingOrganizationID = to.TeachingOrganizationID;
                targetMR.TeacherID = to.TeacherID;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<TeachingOrganization>(to.TeachingOrganizationID), to);
            }
        }

        public static TeachingOrganization create(tTeachingOrganization to)
        {
            using (var db = new DBmsw())
            {
                db.tTeachingOrganizations.InsertOnSubmit(to);

                db.SubmitChanges();

                TeachingOrganization newto = new TeachingOrganization(to.TeachingOrganizationID);
                Cache.Set(Cache.getCacheKey<TeachingOrganization>(to.TeachingOrganizationID), newto);

                _NukeCache(newto);

                return newto;
            }
        }

        public static void remove(TeachingOrganization to)
        {
            Cache.Remove(Cache.getCacheKey<TeachingOrganization>(to.TeachingOrganizationID));

            _NukeCache(to);

            using (var db = new DBmsw())
            {
                db.tTeachingOrganizations.DeleteOnSubmit(db.tTeachingOrganizations.SingleOrDefault(x => x.TeachingOrganizationID == to.TeachingOrganizationID));
                db.SubmitChanges();
            }
        }

        private TeachingOrganization(int TeachingOrganizationID)
        {
            using (var db = new DBmsw())
            {
                var to = db.tTeachingOrganizations.SingleOrDefault(x => x.TeachingOrganizationID == TeachingOrganizationID);

                this.TeachingOrganizationID = to.TeachingOrganizationID;
                TeacherID = to.TeacherID;
            }
        }

        private TeachingOrganization(tTeachingOrganization tOrg)
        {
            this.TeachingOrganizationID = tOrg.TeachingOrganizationID;
            TeacherID = tOrg.TeacherID;
        }

        private static void _NukeCache(TeachingOrganization to)
        {
            Repository r = Repository.getInstance();

            r.removeOrganizationsToTeach(to.TeacherID);
            r.removeOrganizationsToTeach(to.TeachingOrganizationID);
        }
    }
}