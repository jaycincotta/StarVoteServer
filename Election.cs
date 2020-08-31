using System;
using System.Collections.Generic;

namespace StarVoteServer
{
    public class Election
    {
        public string Title { get; set; }
        public ElectionSettings Settings { get; set; }
        public List<Race> Races { get; set; }
        public List<string> AuthorizedVoters { get; set; }

        public static Election DefaultValue()
        {
            var settings = new ElectionSettings
            {
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow + new TimeSpan(7, 0, 0, 0, 0),
                InfoUrl = "",
                AdminEmail = "you@gmail.com",
                SupportEmail = "you@gmail.com",
                AuditEmail = "you@gmail.com",
                EmailVerification = true,
                VoterAuthorization = false,
                BallotUpdates = false,
                PublicResults = true
            };
            var races = new List<Race> {
                new Race {
                    Caption = "Race 1",
                    Candidates = new List<string> { "Candidate 1", "Candidate 2", "Candidate 3" }
                },
                new Race {
                    Caption = "Race 2",
                    Candidates = new List<string> { "A", "B", "C", "D", "E" }
                }
            };

            return new Election
            {
                Title = "My Election",
                Settings = settings,
                Races = races,
                AuthorizedVoters = null
            };
        }
    }

    public class ElectionSettings
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string InfoUrl { get; set; }
        public string AdminEmail { get; set; }
        public string SupportEmail { get; set; }
        public string AuditEmail { get; set; }
        public bool? EmailVerification { get; set; }
        public bool? VoterAuthorization { get; set; }
        public bool? BallotUpdates { get; set; }
        public bool? PublicResults { get; set; }
    }

    public class Race
    {
        public string Caption { get; set; }
        public List<string> Candidates { get; set; }
    }

}
