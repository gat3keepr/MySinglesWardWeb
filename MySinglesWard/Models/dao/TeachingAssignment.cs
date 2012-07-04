using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class TeachingAssignment
    {
        public int MemberID { get; set; }

        //CompanionshipID is the ID of the companionship this member belongs to
        public int? CompanionshipID { get; set; }

        //HTID and VTID is the CompanionshipID of the companionship that teaches this member
        public int? HTID { get; set; }
        public int? VTID { get; set; }

        public static TeachingAssignment get(int MemberID)
        {
            TeachingAssignment teaching = Cache.Get(Cache.getCacheKey<TeachingAssignment>(MemberID)) as TeachingAssignment;

            if (teaching == null)
            {
                using (var db = new DBmsw())
                {
                    var teach = db.tTeachingAssignments.SingleOrDefault(x => x.MemberID == MemberID);

                    if (teach == null)
                        return null;

                    teaching = new TeachingAssignment(teach);
                    Cache.Set(Cache.getCacheKey<TeachingAssignment>(MemberID), teaching);
                }                
            }

            return teaching;
        }

        public static void save(TeachingAssignment teach)
        {
            Cache.Remove(Cache.getCacheKey<TeachingAssignment>(teach.MemberID));

            _NukeCache(teach);

            using (var db = new DBmsw())
            {
                var targetTeach = db.tTeachingAssignments.SingleOrDefault(x => x.MemberID == teach.MemberID);

                _NukeCache(targetTeach);

                targetTeach.MemberID = teach.MemberID;
                targetTeach.CompanionshipID = teach.CompanionshipID;
                targetTeach.HTID = teach.HTID;
                targetTeach.VTID = teach.VTID;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<TeachingAssignment>(teach.MemberID), teach);
            }
        }

        public static TeachingAssignment create(tTeachingAssignment newAssignment)
        {
            using (var db = new DBmsw())
            {
                db.tTeachingAssignments.InsertOnSubmit(newAssignment);

                db.SubmitChanges();

                TeachingAssignment newTeaching = new TeachingAssignment(newAssignment.MemberID);
                Cache.Set(Cache.getCacheKey<TeachingAssignment>(newTeaching.MemberID), newTeaching);

                _NukeCache(newTeaching);

                return newTeaching;
            }
        }

        public static void remove(TeachingAssignment teaching)
        {
            Cache.Remove(Cache.getCacheKey<TeachingAssignment>(teaching.MemberID));

            _NukeCache(teaching);

            using (var db = new DBmsw())
            {
                db.tTeachingAssignments.DeleteOnSubmit(db.tTeachingAssignments.SingleOrDefault(x => x.MemberID == teaching.MemberID));
                db.SubmitChanges();
            }
        }

        private TeachingAssignment(int MemberID)
        {
            using (var db = new DBmsw())
            {
                var teach = db.tTeachingAssignments.SingleOrDefault(x => x.MemberID == MemberID);

                this.MemberID = teach.MemberID;
                CompanionshipID = teach.CompanionshipID;
                HTID = teach.HTID;
                VTID = teach.VTID;
            }
        }

        private TeachingAssignment(tTeachingAssignment teach)
        {
            this.MemberID = teach.MemberID;
            CompanionshipID = teach.CompanionshipID;
            HTID = teach.HTID;
            VTID = teach.VTID;
        }

        private static void _NukeCache(TeachingAssignment teach)
        {
            Repository r = Repository.getInstance();

            if (teach.CompanionshipID != null)
                r.removeTeachers((int)teach.CompanionshipID);
            if (teach.HTID != null)
                r.removeTeachees((int)teach.HTID);
            if(teach.VTID != null)
                r.removeTeachees((int)teach.VTID);
        }

        private static void _NukeCache(tTeachingAssignment teach)
        {
            Repository r = Repository.getInstance();

            if (teach.CompanionshipID != null)
                r.removeTeachers((int)teach.CompanionshipID);
            if (teach.HTID != null)
                r.removeTeachees((int)teach.HTID);
            if (teach.VTID != null)
                r.removeTeachees((int)teach.VTID);
        }
    }
}