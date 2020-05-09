﻿using Newtonsoft.Json;
using Questor.Vehicle.Domain.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vehicle.IntegrationTests.Utils
{
    public class Request
    {
        private readonly HttpClient Client;
        public Request(HttpClient client) => Client = client;

        public async Task<(EStatusCode status, TResult result)> Get<TResult>(Uri uri, dynamic data = null) where TResult : class
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri($"{uri}?{GetUrlString(data)}"),
                Method = HttpMethod.Get
            };

            try { 
                var response = await Client.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResult>(json);
                return ((EStatusCode)response.StatusCode, result);
            }
            catch(Exception e)
            {
                return (EStatusCode.Error, null);
            }
        }

        public async Task<(EStatusCode status, TResult result)> Post<TResult>(Uri uri, dynamic data) where TResult : class {
            var content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");

            try
            {
                var response = await Client.PostAsync(uri, content);
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TResult>(json);
                return ((EStatusCode)response.StatusCode, result);
            }
            catch (Exception e)
            {
                return (EStatusCode.Error, null);
            }
        }

        private string GetUrlString(object data = null)
        {
            if (data == null) return string.Empty;
            var props = data
                .GetType().GetProperties()
                .Where(p => p.GetValue(data) != null)
                .Select(p => $"{p.Name}={JsonConvert.SerializeObject(p.GetValue(data)).Replace(@"""", "")}");
            return string.Join("&", props);
        }
    }
}
