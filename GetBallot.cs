using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Google;
using StarVoteServer.GoogleFunctions;
using System.IO;
using System.Collections.Generic;

namespace StarVoteServer
{
    public static class GetBallot
    {
        [FunctionName(nameof(Ballot))]
        public static async Task<IActionResult> Ballot(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Ballot/{docId}")] HttpRequest req, string docId, ILogger log)
        {
            using var service = new GoogleService(docId);
            string body = await new StreamReader(req.Body).ReadToEndAsync().ConfigureAwait(false);
            try
            {
                // First, validate that we can access the document.
                var info = await service.GetSheetInfo().ConfigureAwait(false);
                SheetInfo settingsSheet = null;
                SheetInfo votersSheet = null;
                List<SheetInfo> raceSheets = new List<SheetInfo>();
                foreach(var sheet in info.Sheets)
                {
                    // Ignore sheets that don't start with StarSymbol;
                    if (!sheet.Title.StartsWith(GoogleService.StarSymbol))
                        continue;
                    var title = sheet.Title.Substring(1).Trim(); // Get the rest of title after star
                    if ("Settings".Equals(title,StringComparison.OrdinalIgnoreCase) || sheet.SheetId == 0)
                    {
                        settingsSheet = sheet;
                    } else if ("Voters".Equals(title, StringComparison.OrdinalIgnoreCase) || sheet.SheetId == 1)
                    {
                        votersSheet = sheet;
                    } else
                    {
                        raceSheets.Add(sheet);
                    }
                }
                if (settingsSheet == null)
                {
                    throw new ApplicationException($"The document is missing a {GoogleService.StarSymbol}Settings tab");
                }
                if (votersSheet == null)
                {
                    throw new ApplicationException($"The document is missing a {GoogleService.StarSymbol}Voters tab");
                }
                if (raceSheets.Count == 0)
                {
                    throw new ApplicationException($@"The document does not have any races defined.
For each race, there should be a tab with the name of the race preceeded by {GoogleService.StarSymbol}.
For example, ""{GoogleService.StarSymbol}Best Pianist""");
                }
                var election = await service.GetElection(settingsSheet, votersSheet, raceSheets);

                return new OkObjectResult(election);
            }
            catch (GoogleApiException ex)
            {
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new NotFoundObjectResult($@"404: NOT FOUND from Google API
Could not find a Google Sheets document with documentId: {docId}");
                }
                if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return new ObjectResult($@"403: NOT AUTHORIZED from Google API
This service has not been authorized to access documentId: {docId}
Please share your Google Sheets document with service@starvote.iam.gserviceaccount.com and try again
Note that you must grant Editor access for the service to update your document")
                    {
                        StatusCode = 403
                    };
                }
                return new BadRequestObjectResult(ex);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.GetType().AssemblyQualifiedName);
                log.LogInformation(ex.ToString());
                return new BadRequestObjectResult(ex.ToString());
            }
        }
    }
}
