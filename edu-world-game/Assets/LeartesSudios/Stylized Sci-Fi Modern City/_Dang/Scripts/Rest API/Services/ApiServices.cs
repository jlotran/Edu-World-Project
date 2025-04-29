using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace EduWorld
{
    public class ApiServices
    {
        private static readonly HttpClient client = new HttpClient(); // ✅ Dùng chung HttpClient
        private readonly string baseUrl;
        private string token = "";

        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(5); // ✅ Giới hạn 5 request đồng thời

        public ApiServices(string baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public void SetToken(string newToken)
        {
            token = newToken;
        }

        private async Task<string> SendRequest(HttpRequestMessage request)
        {
            await semaphore.WaitAsync(); // ✅ Chặn nếu có quá nhiều request cùng lúc
            try
            {
                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    return await HandleResponse(response);
                }
            }
            finally
            {
                semaphore.Release(); // ✅ Giải phóng slot khi xong
            }
        }

        public async Task<string> Get(string endpoint, Dictionary<string, string> headers = null)
        {
            await Task.Delay(500); // ✅ Chống spam API
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}{endpoint}");
            AddHeaders(request, headers);
            return await SendRequest(request);
        }

        public async Task<string> Post(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            await Task.Delay(500);
            string json = JsonConvert.SerializeObject(data);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}{endpoint}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            AddHeaders(request, headers);
            return await SendRequest(request);
        }

        public async Task<string> Put(string endpoint, object data, Dictionary<string, string> headers = null)
        {
            await Task.Delay(500);
            string json = JsonConvert.SerializeObject(data);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}{endpoint}")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            AddHeaders(request, headers);
            return await SendRequest(request);
        }

        public async Task<string> Delete(string endpoint, Dictionary<string, string> headers = null)
        {
            await Task.Delay(500);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}{endpoint}");
            AddHeaders(request, headers);
            return await SendRequest(request);
        }

        private void AddHeaders(HttpRequestMessage request, Dictionary<string, string> headers)
        {
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Car", token);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

        private static async Task<string> HandleResponse(HttpResponseMessage response)
        {
            string result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                GC.Collect(); // ✅ Giải phóng dữ liệu JSON khỏi RAM
                return result;
            }
            else
            {
                Debug.LogError($"API Error: {response.StatusCode} - {result}");
                return null;
            }
        }
    }
}
