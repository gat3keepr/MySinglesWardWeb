using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MemcachedProviders.Cache;

namespace TeachingMonthChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Get the teaching month and add it to the database. Then flush the cache
            using (var db = new MSWDatabase())
            {
                var month = db.tTeachingMonths;
                if (month.ToList()[month.Count() - 1].teachingMonth.Month != DateTime.Today.Month)
                {
                    tTeachingMonth newMonth = new tTeachingMonth();
                    newMonth.teachingMonth = DateTime.Today;
                    db.tTeachingMonths.InsertOnSubmit(newMonth);
                    db.SubmitChanges();

                    DistCache.RemoveAll();
                }
            }
        }
    }
}
