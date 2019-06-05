using ChangeFeedCosmosDB.DTOs.Assignment;
using ChangeFeedCosmosDB.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChangeFeedCosmosDB.Drivers
{
    public class ApiDriver
    {
        private readonly string _baseUrlAssignmentApi = "http://assapivexptkl4b3go2rm3y.azurewebsites.net/api";
        private readonly bool _useLocalProxy = true;
        private const string _bearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6InRlc3Qga2lkIn0.eyJpc3MiOiJodHRwczovL21hYW0tc3RnLmF4YS5jb20iLCJzdWIiOiIxMjM0NTY3ODkwIiwiYXVkIjoidGVzdC1hcGkiLCJuYW1lIjoiZ3Vpc3NpbW8iLCJpYXQiOjE1MTYyMzkwMjIsInNjb3BlIjoicmVhZCBvciB3cml0ZSJ9.sZQkvvg_SKdEGECi_FNRoXO71HeKnaRC_eaBR9tEDco";
        public ApiDriver()
        {

        }


        public IRestResponse PostAssignmentProvider(AssignmentProvider provider)
        {
            var accept_request = new HttpRequestWrapper(_baseUrlAssignmentApi, _useLocalProxy, _bearerToken)
                         .SetMethod(Method.POST)
                         .SetResourse($"/providers")
                         .AddJsonContent(new List<AssignmentProvider>() { provider });

            return accept_request.Execute();
        }
    }
}
