using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarVoteServer
{
    public class ElectionSettings
    {
        public string Election { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string FaqUrl { get; set; }
        public string SupportEmail { get; set; }
        public string AuditEmail { get; set; }
        public bool PrivateResults { get; set; }
        public bool AnonymousVoters { get; set; }
        public bool VerifiedVoters { get; set; }
        public bool BallotUpdates { get; set; }
    }
}
