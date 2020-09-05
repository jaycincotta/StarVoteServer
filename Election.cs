using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarVoteServer
{
    // This class is used internally by the service and includes some information not shared with client
    public class Election : ElectionBallot

    {
        public string TimeZone { get; set; }
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

        public static async Task<ElectionBallot> ReadBallot(GoogleFunctions.GoogleService service)
        {
            var election = await Election.ReadElection(service).ConfigureAwait(false);
            var ballot = new ElectionBallot { Title = election.Title, Races = election.Races, Settings = election.Settings};
            return ballot;
        }


            public static async Task<Election> ReadElection(GoogleFunctions.GoogleService service)
        {
            // First, validate that we can access the document.
            var info = await service.GetSheetInfo().ConfigureAwait(false);
            SheetInfo settingsSheet = null;
            SheetInfo votersSheet = null;
            List<SheetInfo> raceSheets = new List<SheetInfo>();
            foreach (var sheet in info.Sheets)
            {
                // Ignore sheets that don't start with StarSymbol;
                if (!sheet.Title.StartsWith(GoogleFunctions.GoogleService.StarSymbol))
                    continue;

                var title = sheet.Title.Substring(1).Trim(); // Get the rest of title after star
                if ("Settings".Equals(title, StringComparison.OrdinalIgnoreCase) || sheet.SheetId == 0)
                {
                    settingsSheet = sheet;
                }
                else if ("Voters".Equals(title, StringComparison.OrdinalIgnoreCase) || sheet.SheetId == 1)
                {
                    votersSheet = sheet;
                }
                else
                {
                    raceSheets.Add(sheet);
                }
            }
            if (settingsSheet == null)
            {
                throw new ApplicationException($"The document is missing a {GoogleFunctions.GoogleService.StarSymbol}Settings tab");
            }
            if (votersSheet == null)
            {
                throw new ApplicationException($"The document is missing a {GoogleFunctions.GoogleService.StarSymbol}Voters tab");
            }
            if (raceSheets.Count == 0)
            {
                throw new ApplicationException($@"The document does not have any races defined.
For each race, there should be a tab with the name of the race preceeded by {GoogleFunctions.GoogleService.StarSymbol}.
For example, ""{GoogleFunctions.GoogleService.StarSymbol}Best Pianist""");
            }
            var election = await service.GetElection(settingsSheet, votersSheet, raceSheets);
            election.Title = info.Title;
            election.TimeZone = info.TimeZone;
            return election;
        }
    }

    // This is the base class containing the fields that may be shared with the client
    // describing the election ballot
    public class ElectionBallot
    {
        public string Title { get; set; }
        public ElectionSettings Settings { get; set; }
        public List<Race> Races { get; set; }
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
        public int? SheetId { get; set; }
    }

}
