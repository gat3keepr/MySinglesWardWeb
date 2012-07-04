using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dbo
{
    [Serializable]
    public class TeachingRecord
    {
        public int TeachingRecordID { get; set; }
        public int MemberID { get; set; }
        public int TeachingVisitID { get; set; }

        public static TeachingRecord get(int TeachingRecordID)
        {
            TeachingRecord record = Cache.Get(Cache.getCacheKey<TeachingRecord>(TeachingRecordID)) as TeachingRecord;

            if (record == null)
            {
                record = new TeachingRecord(TeachingRecordID);
                Cache.Set(Cache.getCacheKey<TeachingRecord>(TeachingRecordID), record);
            }

            return record;
        }

        public static void save(TeachingRecord record)
        {
            Cache.Remove(Cache.getCacheKey<TeachingRecord>(record.TeachingRecordID));

            _NukeCache(record.MemberID);

            using (var db = new DBmsw())
            {
                var targetRecord = db.tTeachingRecords.SingleOrDefault(x => x.TeachingRecordID == record.TeachingRecordID);

                targetRecord.TeachingRecordID = record.TeachingRecordID;
                targetRecord.MemberID = record.MemberID;
                targetRecord.TeachingVisitID = record.TeachingVisitID;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<TeachingRecord>(record.TeachingRecordID), record);
            }
        }

        public static TeachingRecord create(tTeachingRecord record)
        {
            using (var db = new DBmsw())
            {
                db.tTeachingRecords.InsertOnSubmit(record);

                db.SubmitChanges();

                TeachingRecord newRec = new TeachingRecord(record.TeachingRecordID);
                Cache.Set(Cache.getCacheKey<TeachingRecord>(newRec.TeachingRecordID), newRec);

                _NukeCache(newRec.MemberID);

                return newRec;
            }
        }

        public static void remove(TeachingRecord record)
        {
            Cache.Remove(Cache.getCacheKey<TeachingRecord>(record.TeachingRecordID));

            _NukeCache(record.MemberID);

            using (var db = new DBmsw())
            {
                db.tTeachingRecords.DeleteOnSubmit(db.tTeachingRecords.SingleOrDefault(x => x.TeachingRecordID == record.TeachingRecordID));
                db.SubmitChanges();
            }
        }

        private TeachingRecord(int TeachingRecordID)
        {
            using (var db = new DBmsw())
            {
                var record = db.tTeachingRecords.SingleOrDefault(x => x.TeachingRecordID == TeachingRecordID);

                this.TeachingRecordID = record.TeachingRecordID;
                MemberID = record.MemberID;
                TeachingVisitID = record.TeachingVisitID;
            }
        }

        private static void _NukeCache(int MemberID)
        {
            Repository.getInstance().removeTeachingRecords(MemberID);
        }

        internal static void create(int MemberID, int TeachingVisitID)
        {
            using (var db = new DBmsw())
            {
                //Check to see if there is a record of the visit in the database                   
                var visit = db.tTeachingRecords.SingleOrDefault(x => x.MemberID == MemberID && x.TeachingVisitID == TeachingVisitID);

                //if the record is in the database than nothing needs to happen
                if (visit != null)
                {
                    return;
                }
                else
                {
                    _NukeCache(MemberID);

                    //The record needs to be put in the database
                    tTeachingRecord tR = new tTeachingRecord();
                    tR.MemberID = MemberID;
                    tR.TeachingVisitID = TeachingVisitID;

                    TeachingRecord.create(tR);
                }
            }
        }
    }
}