using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MSW.Model;

namespace MSW.Models
{
    public class GroupListModel
    {
        public List<List<Group>> groupsList { get; set; }
        public string[] groupNames = { "Stake", "Ward", "Elders Quorum", "Relief Society", "Activities", "FHE" };
        private List<Group> stakeGroups { get; set; }
        private List<Group> wardGroups { get; set; }
        private List<Group> eldersQuorumGroups { get; set; }
        private List<Group> reliefSocietyGroups { get; set; }
        private List<Group> activitiesGroups { get; set; }
        private List<Group> fheGroups { get; set; }
        public int MemberID { get; set; }

        public GroupListModel(IEnumerable<tGroup> groups, int MemberID)
        {
            this.MemberID = MemberID;
            stakeGroups = new List<Group>();
            wardGroups = new List<Group>();
            eldersQuorumGroups = new List<Group>();
            reliefSocietyGroups = new List<Group>();
            activitiesGroups = new List<Group>();
            fheGroups = new List<Group>();

            if(groups != null)
                _createLists(groups);

        }

        private void _createLists(IEnumerable<tGroup> groups)
        {
            foreach (var group in groups)
            {
                switch (group.Type)
                {
                    case 0:
                        stakeGroups.Add(new Group(group, MemberID));
                        break;
                    case 1:
                        wardGroups.Add(new Group(group, MemberID));
                        break;
                    case 2:
                        eldersQuorumGroups.Add(new Group(group, MemberID));
                        break;
                    case 3:
                        reliefSocietyGroups.Add(new Group(group, MemberID));
                        break;
                    case 4:
                        activitiesGroups.Add(new Group(group, MemberID));
                        break;
                    case 5:
                        fheGroups.Add(new Group(group, MemberID));
                        break;
                }
            }

            //Order Lists by Name
            groupsList = new List<List<Group>>();
            groupsList.Add(stakeGroups.OrderBy(x => x.Name).ToList());
            groupsList.Add(wardGroups.OrderBy(x => x.Name).ToList());
            groupsList.Add(eldersQuorumGroups.OrderBy(x => x.Name).ToList());
            groupsList.Add(reliefSocietyGroups.OrderBy(x => x.Name).ToList());
            groupsList.Add(activitiesGroups.OrderBy(x => x.Name).ToList());
            groupsList.Add(fheGroups.OrderBy(x => x.Name).ToList());
        }

    }
}