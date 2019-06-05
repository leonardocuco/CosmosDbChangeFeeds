using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChangeFeedCosmosDB.DTOs.Assignment
{
    public class AssignmentProvider
    {
        [JsonProperty("id")]
        public Guid ProviderId { get; set; }

        [JsonProperty("provider_external_id")]
        public string ProviderExternalId { get; set; }

        [JsonProperty("provider_commercial_name")]
        public string CommercialName { get; set; }

        [JsonProperty("provider_is_auto_accepting_mission")]
        public bool IsAutoAcceptingMission { get; set; }

        [JsonProperty("provider_is_auto_missionning")]
        public bool IsAutoMissionning { get; set; }

        public AssignmentProvider()
        {

        }

        public AssignmentProvider(JObject provider)
        {
            try
            {
                ProviderId = Guid.Parse(provider["id"]?.ToObject<string>() ?? throw new ArgumentNullException("id"));
                ProviderExternalId = provider["provider_external_id"]?.ToObject<string>() ?? throw new ArgumentNullException("provider_external_id");
                CommercialName = provider["provider_commercial_name"]?.ToObject<string>() ?? throw new ArgumentNullException("provider_commercial_name");
                IsAutoAcceptingMission = provider["provider_is_auto_accepting_mission"]?.ToObject<bool?>() ?? throw new ArgumentNullException("provider_is_auto_accepting_mission");
                IsAutoMissionning = provider["provider_is_auto_missionning"]?.ToObject<bool?>() ?? throw new ArgumentNullException("provider_is_auto_missionning");
            }
            catch (Exception e)
            {

                throw new Exception($"Error while parsing the provider : {provider.ToString()}. Message : {e.Message}");
            }
        }
    }
}
