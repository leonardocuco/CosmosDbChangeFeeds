using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeFeedCosmosDB.Drivers;
using ChangeFeedCosmosDB.DTOs.Assignment;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChangeFeedCosmosDB
{
    public class ProviderAssignmentSync
    {
        private readonly ApiDriver apiDriver;

        public ProviderAssignmentSync(ApiDriver apiDriver)
        {
            this.apiDriver = apiDriver;
        }

        [FunctionName("ProviderAssignmentSync")]
        public async Task Run(
            [CosmosDBTrigger(
                databaseName: "Providers",
                collectionName: "Providers",
                ConnectionStringSetting = "CosmosDBConnectionString",
                LeaseConnectionStringSetting = "CosmosDBConnectionString",
                LeaseDatabaseName = "History",
                LeaseCollectionPrefix = "sassync_",
                LeaseCollectionName = "ProvidersLeases")]IReadOnlyList<Document> input,
            ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                foreach(var doc in input)
                {
                    JObject incommingProvider = JsonConvert.DeserializeObject<JObject>(doc.ToString());
                    var outputProvider = new AssignmentProvider(incommingProvider);
                    var result = apiDriver.PostAssignmentProvider(outputProvider);
                }           
            }
        }

        private void PrepareForHistorization(JObject obj)
        {
            var originalId = obj["id"];
            obj.Add("provider_id", originalId);
            obj["id"] = Guid.NewGuid();
            obj.Add("ModificationDate", DateTime.UtcNow);
        }
    }
}
