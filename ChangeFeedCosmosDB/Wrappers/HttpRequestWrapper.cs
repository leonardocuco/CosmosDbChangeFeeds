using Newtonsoft.Json;
using Polly;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace ChangeFeedCosmosDB.Helpers
{
    public class HttpRequestWrapper
    {
        private const int RetryCount = 10;
        private const int WaitSecondsBeforeRetry = 2;
        private RestRequest _restRequest;
        private RestClient _restClient;
        private readonly string _endpoint;
        private readonly bool _uselocalProxy;
        private readonly string _bearerToken;

        public HttpRequestWrapper(string endPoint, bool useLocalProxy, string barearToken)
        {
            _restRequest = new RestRequest();
            _endpoint = endPoint;
            _uselocalProxy = useLocalProxy;
            this._bearerToken = barearToken;
        }

        public HttpRequestWrapper SetResourse(string resource)
        {
            _restRequest.Resource = resource;
            return this;
        }

        public HttpRequestWrapper SetMethod(Method method)
        {
            _restRequest.Method = method;
            return this;
        }

        public HttpRequestWrapper AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _restRequest.AddParameter(header.Key, header.Value, ParameterType.HttpHeader);
            }
            return this;
        }

        public HttpRequestWrapper AddJsonContent(object data)
        {
            _restRequest.RequestFormat = DataFormat.Json;
            _restRequest.AddHeader("Content-Type", "application/json");
            _restRequest.AddJsonBody(data);
            return this;
        }

        public HttpRequestWrapper AddEtagHeader(string value)
        {
            _restRequest.AddHeader("If-None-Match", value);
            return this;
        }


        public HttpRequestWrapper AddParameter(string name, object value)
        {
            _restRequest.AddParameter(name, value);
            return this;
        }

        public HttpRequestWrapper AddParameters(IDictionary<string, object> parameters)
        {
            foreach (var item in parameters)
            {
                _restRequest.AddParameter(item.Key, item.Value);
            }
            return this;
        }

        public IRestResponse Execute()
        {
            try
            {
                _restClient = CreateCustomClient();

                var response = _restClient.Execute(_restRequest);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public T Execute<T>()
        {
            _restClient = CreateCustomClient();

            var response = _restClient.Execute(_restRequest);
            var data = JsonConvert.DeserializeObject<T>(response.Content);
            return data;
        }

        public T Execute<T>(Func<T, bool> executeWhile)
        {
            _restClient = CreateCustomClient();

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .OrResult<bool>(result => result == false)
                .WaitAndRetryAsync(RetryCount, i => TimeSpan.FromSeconds(WaitSecondsBeforeRetry));

            T data = (T)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);

            retryPolicy.ExecuteAsync(async () =>
            {
                var response = _restClient.Execute(_restRequest);
                data = JsonConvert.DeserializeObject<T>(response.Content);
                return executeWhile.Invoke(data);
            }).GetAwaiter().GetResult();

            return data;
        }

        private RestClient CreateCustomClient()
        {
            if (_restClient != null)
                return _restClient;

            _restClient = new RestClient(_endpoint);
            if (_uselocalProxy)
                _restClient.Proxy = new WebProxy("127.0.0.1", 3128);

            _restClient.AddDefaultHeader("Authorization", string.Format("Bearer {0}", _bearerToken));
            _restClient.UseSerializer(() => new JsonNetSerializer());

            return _restClient;
        }
    }
}
