using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MSW.Models.Groups
{
    public class Ward
    {
        public string WardName { get; set; }
        public double WardStakeID { get; set; }

        public Ward(string WardName, double WardStakeID)
        {
            this.WardName = WardName;
            this.WardStakeID = WardStakeID;
        }

    }
}