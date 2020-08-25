using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using System;
using System.IO;

namespace StarVoteServer.GoogleFunctions
{
    /// <summary>
    /// ServiceAccountBase encapsulates the details of authenticating using a service account
    /// </summary>
    public abstract class ServiceAccountBase : IDisposable
    {
        private string[] _scopes = { SheetsService.Scope.Spreadsheets };
        private string _applicationName = "StarVote";
        
        protected SheetsService _service;

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public ServiceAccountBase()
        {
            GoogleCredential credential;

            // Put your credentials json file in the root of the solution and make sure copy to output dir property is set to always copy 
            using (var stream = GenerateStreamFromString(BuildConfigString()))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(_scopes);
            }

            // Create Google Sheets API service.
            _service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = _applicationName
            });
        }

        static string BuildConfigString()
        {
            const string projectId = "starvote";
            string privateKeyId = Environment.GetEnvironmentVariable("private_key_id");
            string privateKey = Environment.GetEnvironmentVariable("private_key");
            string clientId = Environment.GetEnvironmentVariable("client_id");
            string clientEmail = Environment.GetEnvironmentVariable("client_email");

            return @$"{{
  ""type"": ""service_account"",
  ""project_id"": ""{projectId}"",
  ""private_key_id"": ""{privateKeyId}"",
  ""private_key"": ""{privateKey}"",
  ""client_email"": ""{clientEmail}"",
  ""client_id"": ""{clientId}"",
  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
  ""token_uri"": ""https://oauth2.googleapis.com/token"",
  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/metavote%40metavote.iam.gserviceaccount.com""
}}";
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_service == null)
            {
                return;
            }

            _service.Dispose();
            _service = null;
        }
    }
}
