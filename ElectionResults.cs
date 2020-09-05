using System;
using System.Collections.Generic;
using System.Text;

namespace StarVoteServer
{
    public class ElectionResults
    {
        public string Title { get; set; }
        public List<RaceResults> Races { get; set; }
    }

    public class RaceResults
    {
        public string Title { get; set; }
        public string[] Candidates { get; set; }
        public Vote[] Votes { get; set; }
    }

    public class Vote
    {
        public string VoterId { get; set; }
        public int[] Scores { get; set; }
    }

}