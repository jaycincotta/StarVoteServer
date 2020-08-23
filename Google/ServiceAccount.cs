using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace StarVoteServer.Google
{
    public class ServiceAccount
    {
        public string Info { get; set; }
        public ServiceAccount()
        {
            Info = BuildConfigString();
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
    }
}
