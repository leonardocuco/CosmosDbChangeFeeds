using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChangeFeedCosmosDB
{
    public static class AssignmentHistorization
    {
        [FunctionName("AssignmentHistorization")]
        public static async Task Run(
            [CosmosDBTrigger(
                databaseName: "Assignment",
                collectionName: "Assignments",
                ConnectionStringSetting = "CosmosDBConnectionString",
                LeaseConnectionStringSetting = "CosmosDBConnectionString",
                LeaseDatabaseName = "History",
                LeaseCollectionPrefix = "history_",
                LeaseCollectionName = "AssignmentsLeases")]IReadOnlyList<Document> input,
            [CosmosDB(
                databaseName: "History",
                collectionName: "Assignments",
                ConnectionStringSetting = "CosmosDBConnectionString")]IAsyncCollector<dynamic> documentCollector,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var doc in input)
                {
                    try
                    {
                        JObject obj = JsonConvert.DeserializeObject<JObject>(doc.ToString());
                        PrepareForHistorization(obj);
                        await documentCollector.AddAsync(obj);
                    }
                    catch (System.Exception e)
                    {
                        log.LogError(e.Message);
                    }
                }

            }
        }

        private static void PrepareForHistorization(JObject obj)
        {
            var originalId = obj["id"];
            obj.Add("assignment_id", originalId);
            obj["id"] = Guid.NewGuid();

            obj.Add("ModificationDate", DateTime.UtcNow);
        }
    }
}
