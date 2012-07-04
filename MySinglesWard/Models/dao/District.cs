using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class District
    {
        public int DistrictID { get; set; }
        public int OrgID { get; set; }

        //CallingID not a MemberID
        public int? DistrictLeaderID { get; set; }

        //LeaderAssigned is a boolean to flag whether or not the actual district leader has been given district leader rights
        public bool LeaderAssigned { get; set; }
        public string Title { get; set; }

        public static District get(int DistrictID)
        {
            District district = Cache.Get(Cache.getCacheKey<District>(DistrictID)) as District;

            if (district == null)
            {
                district = new District(DistrictID);
                Cache.Set(Cache.getCacheKey<District>(DistrictID), district);
            }

            return district;
        }

        public static void save(District district)
        {
            Cache.Remove(Cache.getCacheKey<District>(district.DistrictID));

            _NukeCache(district);

            using (var db = new DBmsw())
            {
                var targetDist = db.tDistricts.SingleOrDefault(x => x.DistrictID == district.DistrictID);

                targetDist.DistrictID = district.DistrictID;
                targetDist.OrgID = district.OrgID;
                targetDist.DistrictLeaderID = district.DistrictLeaderID;
                targetDist.Title = district.Title;
                targetDist.LeaderAssigned = district.LeaderAssigned;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<District>(district.DistrictID), district);
            }
        }

        public static District create(tDistrict district)
        {
            using (var db = new DBmsw())
            {
                db.tDistricts.InsertOnSubmit(district);

                db.SubmitChanges();

                District newDistrict = new District(district.DistrictID);
                Cache.Set(Cache.getCacheKey<TeachingVisit>(newDistrict.DistrictID), newDistrict);

                _NukeCache(newDistrict);

                return newDistrict;
            }
        }

        public static void remove(District district)
        {
            Cache.Remove(Cache.getCacheKey<District>(district.DistrictID));

            _NukeCache(district);

            using (var db = new DBmsw())
            {
                db.tDistricts.DeleteOnSubmit(db.tDistricts.SingleOrDefault(x => x.DistrictID == district.DistrictID));
                db.SubmitChanges();
            }
        }

        private District(int DistrictID)
        {
            using (var db = new DBmsw())
            {
                var district = db.tDistricts.SingleOrDefault(x => x.DistrictID == DistrictID);

                this.DistrictID = district.DistrictID;
                Title = district.Title;
                DistrictLeaderID = district.DistrictLeaderID;
                OrgID = district.OrgID;
                LeaderAssigned = district.LeaderAssigned;
            }
        }

        private static void _NukeCache(District district)
        {
			try
			{
				Repository r = Repository.getInstance();
				Organization org = Organization.get(district.OrgID);

				//Remove districts from organization cache
				r.removeDistricts(org.OrgID);
			}
			catch
			{
				//Sometimes the organizationID is set to zero yo disassociate the district
			}
        }

        /// <summary>
        /// Remove a member from the district leader calling if a member has been assigned and sustained
        /// </summary>
        internal static bool removeDistrictLeader(District district)
        {
            if (district.DistrictLeaderID == null)
                return false;

            //Get the calling and make sure the person actually needs to be removed
            Calling calling = Calling.get((int)district.DistrictLeaderID);

            if (calling.MemberID != 0)
                if (calling.CallingStatus >= (int)Calling.Status.SUSTAINED)
                {
                    MSWUser member = MSWUser.getUser(calling.MemberID);
                    try
                    {
                        //Remove the member from the District Leader Role
                        if (district.LeaderAssigned)
                        {
                            System.Web.Security.Roles.RemoveUserFromRole(member.UserName, "District Leader");
                        }
                    }
                    catch (Exception e)
                    {
                        MSWtools._sendException(e);
                    }
                }
            return false;
        }

        /// <summary>
        /// Releases a member from the district leader calling if a member has been assigned and sustained
        /// </summary>
        internal static bool releaseDistrictLeader(District district)
        {
            if (district.DistrictLeaderID == null)
                return false;

            //Get the calling and make sure the person actually needs to be removed
            PendingRelease calling = PendingRelease.get((int)district.DistrictLeaderID);

            if (calling.MemberID != 0)
            {
                MSWUser member = MSWUser.getUser(calling.MemberID);
                try
                {
                    //Remove the member from the District Leader Role
                    if (district.LeaderAssigned)
                    {
                        System.Web.Security.Roles.RemoveUserFromRole(member.UserName, "District Leader");
                    }
                }
                catch (Exception e)
                {
                    MSWtools._sendException(e);
                }
            }
            return false;
        }


        /// <summary>
        /// Assigns the current district leader to the District Leader Role if a member has been assigned and sustained
        /// </summary>
        internal static bool assignDistrictLeader(District district)
        {
            if (district.DistrictLeaderID != null && !district.LeaderAssigned)
            {
                Calling calling = Calling.get((int)district.DistrictLeaderID);
                if (calling.MemberID != 0 && calling.CallingStatus >= (int)Calling.Status.SUSTAINED)
                {
                    MSWUser member = MSWUser.getUser(calling.MemberID);
                    try
                    {
                        //Add the user to the role
                        System.Web.Security.Roles.AddUserToRole(member.UserName, "District Leader");
                        district.LeaderAssigned = true;
                        District.save(district);
                        return true;
                    }
                    catch (Exception e)
                    {
                        MSWtools._sendException(e);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the district by the leader of the district.
        /// </summary>
        /// /// <returns>
        /// Null if it is not a district leader calling
        /// </returns>
        internal static District getByDistrictLeaderID(int districtLeaderID)
        {
            using (var db = new DBmsw())
            {
                try
                {
                    var districtID = db.tDistricts.Where(x => x.DistrictLeaderID == districtLeaderID).Select(x => x.DistrictID).SingleOrDefault();

                    return District.get(districtID);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}