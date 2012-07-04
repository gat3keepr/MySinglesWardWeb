using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Utilities;
using MSW.Model;

namespace MSW.Models.dao
{
    [Serializable]
    public class TaughtRecord
    {
        public int TaughtRecordID { get; set; }
        public int MemberID { get; set; }
        public int TeachingVisitID { get; set; }

        public static TaughtRecord get(int TaughtRecordID)
        {
            TaughtRecord record = Cache.Get(Cache.getCacheKey<TaughtRecord>(TaughtRecordID)) as TaughtRecord;

            if (record == null)
            {
                record = new TaughtRecord(TaughtRecordID);
                Cache.Set(Cache.getCacheKey<TaughtRecord>(TaughtRecordID), record);
            }

            return record;
        }

        public static void save(TaughtRecord record)
        {
            Cache.Remove(Cache.getCacheKey<TaughtRecord>(record.TaughtRecordID));

            _NukeCache(record.MemberID);

            using (var db = new DBmsw())
            {
                var targetRecord = db.tTaughtRecords.SingleOrDefault(x => x.TaughtRecordID == record.TaughtRecordID);

                targetRecord.TaughtRecordID = record.TaughtRecordID;
                targetRecord.MemberID = record.MemberID;
                targetRecord.TeachingVisitID = record.TeachingVisitID;

                db.SubmitChanges();

                Cache.Set(Cache.getCacheKey<TaughtRecord>(record.TaughtRecordID), record);
            }
        }

        public static TaughtRecord create(tTaughtRecord record)
        {
            using (var db = new DBmsw())
            {
                db.tTaughtRecords.InsertOnSubmit(record);

                db.SubmitChanges();

                TaughtRecord newRec = new TaughtRecord(record.TaughtRecordID);
                Cache.Set(Cache.getCacheKey<TaughtRecord>(newRec.TaughtRecordID), newRec);

                _NukeCache(newRec.MemberID);

                return newRec;
            }
        }

        public static void remove(TaughtRecord record)
        {
            Cache.Remove(Cache.getCacheKey<TaughtRecord>(record.TaughtRecordID));

            _NukeCache(record.MemberID);

            using (var db = new DBmsw())
            {
                db.tTaughtRecords.DeleteOnSubmit(db.tTaughtRecords.SingleOrDefault(x => x.TaughtRecordID == record.TaughtRecordID));
                db.SubmitChanges();
            }
        }

        private TaughtRecord(int TaughtRecordID)
        {
            using (var db = new DBmsw())
            {
                var record = db.tTaughtRecords.SingleOrDefault(x => x.TaughtRecordID == TaughtRecordID);

                this.TaughtRecordID = record.TaughtRecordID;
                MemberID = record.MemberID;
                TeachingVisitID = record.TeachingVisitID;
            }
        }

        private static void _NukeCache(int MemberID)
        {
            Repository.getInstance().removeTaughtRecords(MemberID);
        }

        internal static void create(int MemberID, int TeachingVisitID)
        {
            using (var db = new DBmsw())
            {
                //Check to see if there is a record of the visit in the database                   
                var visit = db.tTaughtRecords.SingleOrDefault(x => x.MemberID == MemberID && x.TeachingVisitID == TeachingVisitID);

                //if the record is in the database than nothing needs to happen
                if (visit != null)
                {
                    return;
                }
                else
                {
                    _NukeCache(MemberID);

                    //The record needs to be put in the database
                    tTaughtRecord tR = new tTaughtRecord();
                    tR.MemberID = MemberID;
                    tR.TeachingVisitID = TeachingVisitID;

                    TaughtRecord.create(tR);
                }
            }
        }
    }
}