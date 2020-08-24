using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using StarVoteServer.GoogleFunctions;

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

            return new OkObjectResult(responseMessage);
        }

        [FunctionName(nameof(ReadRange))]
        public static async Task<IActionResult> ReadRange(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    ILogger log)
        {
            log.LogInformation($"{nameof(ReadRange)}({Expand(req.Query)})");

            string doc = req.Query["doc"];
            string range = req.Query["range"];
            using var service = new ServiceAccount(doc);

            try
            {
                var result = await service.ReadRange(range).ConfigureAwait(false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
                return new BadRequestObjectResult(ex.ToString());
            }
        }

        [FunctionName(nameof(Test1))]
        public static async Task<IActionResult> Test1(
[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
ILogger log)
        {
            string doc = req.Query["doc"];
            string range = req.Query["range"];
            using var service = new ServiceAccount(doc);

            IList<IList<object>> list = new List<IList<object>>();
            list.Add(new List<object> { "A1", null, "C1" });
            list.Add(new List<object> { "A2", "B2", "C2" });

            try
            {
                var result = await service.WriteRange(range, list).ConfigureAwait(false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
                return new BadRequestObjectResult(ex.ToString());
            }
        }

        [FunctionName(nameof(Test2))]
        public static async Task<IActionResult> Test2(
[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
ILogger log)
        {
            string doc = req.Query["doc"];
            using var service = new ServiceAccount(doc);

            try
            {
                var result = await service.GetSheetInfo().ConfigureAwait(false);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.ToString());
                return new BadRequestObjectResult(ex.ToString());
            }
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
