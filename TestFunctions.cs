using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Runtime.InteropServices.ComTypes;
using System.Collections.Generic;
using System.Linq;
using StarVoteServer.Google;

namespace StarVote
{
    public static class TestFunctions
    {
        [FunctionName(nameof(Ping))]
        public static IActionResult Ping(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(Ping)}({Expand(req.Query)})");

            string name = req.Query["name"];

            string responseMessage = string.IsNullOrEmpty(name)
                ? "Hi!"
                : $"Hello, {name}!";

            var acct = new ServiceAccount();
            log.LogInformation(acct.Info);
            return new OkObjectResult(responseMessage);
        }

        [FunctionName(nameof(ReadRange))]
        public static IActionResult ReadRange(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            log.LogInformation($"{nameof(ReadRange)}({Expand(req.Query)})");

            string doc = req.Query["doc"];
            string sheet = req.Query["sheet"];
            string range = req.Query["range"];

            string responseMessage = $"Read {range} from {sheet} in {doc}";

            return new OkObjectResult(responseMessage);
        }
        static string Expand(IQueryCollection collection)
        {
            var list = new List<string>();
            foreach (var key in collection.Keys)
            {
                var values = collection[key].ToArray();
                var value = String.Join("|", values);
                list.Add($"{key}={value}");
            }
            return String.Join(", ", list.ToArray());
        }
    }
}
