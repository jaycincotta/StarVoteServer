using System;
using System.Collections.Generic;

namespace StarVoteServer
{
    public class BallotData
    {
        public string Email { get; set; }
        public string VoterId { get; set; }
        public List<RaceData> Races { get; set; }

        public void Validate(Election election)
        {
            if (Races.Count != election.Races.Count)
            {
                throw new ApplicationException($"Inconsistent Race Count.\nExpecting: \"{election.Races.Count}\"\nActual: {Races.Count}");
            }
            for (var i = 0; i < Races.Count; i++)
            {
                Races[i].Validate(election.Races[i]);
            }
        }
    }

    public class RaceData
    {
        public string Caption { get; set; }
        public List<string> Candidates { get; set; }
        public List<int> Scores { get; set; }

        public void Validate(Race race)
        {
            if (race.Caption != Caption)
                throw new ApplicationException($"Inconsistent Race Caption.\nExpecting: \"{race.Caption}\"\nActual: \"{Caption}\"");

            if (race.Candidates.Count != Candidates.Count)
                throw new ApplicationException($"Inconsistent Candidate Count.\nExpecting: {race.Candidates.Count} Actual: {Candidates.Count}");

            if (Candidates.Count != Scores.Count)
                throw new ApplicationException($"Inconsistent Score Count.\nExpecting: {Candidates.Count} Actual: {Scores.Count}");

            for (var i = 0; i < Candidates.Count; i++) {
                if (race.Candidates[i] != Candidates[i])
                    throw new ApplicationException($"Inconsistent Candidate Name.\nExpecting: \"{race.Candidates[i]}\"\nActual: \"{Candidates[i]}\"");
                if (Scores[i] < 0 || Scores[i] > 5)
                    throw new ApplicationException($"Illegal STAR Score.\nExpecting: a value in the range [0..5] Actual: {Scores[i]}");
            }
        }
    }
}
