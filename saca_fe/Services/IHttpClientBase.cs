using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SACA_Common.Exceptions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace SACA_FE.Services
{
    public interface IHttpClientBase
    {
        string BaseUrl { get; set; }

        TimeSpan Timeout { get; set; }

        Task<O> GetAsync<O>(string url, HttpRequestHeaders headers = null!);

        Task<O> PostAsync<O, M>(string url, M model = default!, HttpRequestHeaders headers = null!);

        Task<O> PutAsync<O, M>(string url, M model, HttpRequestHeaders headers = null!);

        Task<O> DeleteAsync<O, M>(string url, M model = default!, HttpRequestHeaders headers = null!);
        Task<IFormFile> GetFileAsIFormFile(string url, object requestBody = null!, HttpRequestHeaders headers = null!);
    }
    public class HttpClientBase : IHttpClientBase
    {
        private readonly IHttpClientFactory _clientFactory;

        private string _baseUrl = null!;

        public string BaseUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_baseUrl))
                {
                    return _baseUrl;
                }

                return _baseUrl.EndsWith('/') ? _baseUrl : _baseUrl + '/';
            }
            set
            {
                _baseUrl = value;
            }
        }

        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 5, 0);


        public HttpClientBase(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<O> GetAsync<O>(string url, HttpRequestHeaders headers = null!)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            using HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.Timeout = Timeout;
            using HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return ConverntToResult<O>(await response.Content.ReadAsStringAsync());
            }
            var response2 = await response.Content.ReadAsStringAsync();
            throw new BadException(response2);
        }

        public async Task<O> PostAsync<O, M>(string url, M model = default!, HttpRequestHeaders headers = null!)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            if (model != null && typeof(M) != typeof(MultipartFormDataContent))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }), Encoding.UTF8, "application/json");
            }

            using HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.Timeout = Timeout;
            if (typeof(M) == typeof(MultipartFormDataContent))
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> item2 in headers)
                    {
                        client.DefaultRequestHeaders.Add(item2.Key, item2.Value);
                    }
                }

                dynamic fromData = model!;
                using HttpResponseMessage httpResponseMessage = await client.PostAsync(url, (MultipartFormDataContent)fromData);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var response1 = await httpResponseMessage.Content.ReadAsStringAsync();
                    return ConverntToResult<O>(response1);
                }
                else
                {
                    var responseError = await httpResponseMessage.Content.ReadAsStringAsync();
                    throw new BadException(responseError);
                }
            }

            if (typeof(M) == typeof(FormUrlEncodedContent))
            {
                dynamic fromData2 = model!;
                using HttpResponseMessage httpResponseMessage2 = await client.PostAsync(url, (FormUrlEncodedContent)fromData2);
                if (httpResponseMessage2.IsSuccessStatusCode)
                {
                    return ConverntToResult<O>(await httpResponseMessage2.Content.ReadAsStringAsync());
                }

                var responseError = await httpResponseMessage2.Content.ReadAsStringAsync();
                throw new BadException(responseError);
            }

            using HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return ConverntToResult<O>(await response.Content.ReadAsStringAsync());
            }
            var response2 = await response.Content.ReadAsStringAsync();
            throw new BadException(response2);
        }

        public async Task<O> PutAsync<O, M>(string url, M model = default!, HttpRequestHeaders headers = null!)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            if (model != null && typeof(M) != typeof(MultipartFormDataContent))
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }), Encoding.UTF8, "application/json");
            }

            using HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.Timeout = Timeout;
            if (typeof(M) == typeof(MultipartFormDataContent))
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> item2 in headers)
                    {
                        client.DefaultRequestHeaders.Add(item2.Key, item2.Value);
                    }
                }

                dynamic fromData = model!;
                using HttpResponseMessage httpResponseMessage = await client.PutAsync(url, (MultipartFormDataContent)fromData!);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var response1 = await httpResponseMessage.Content.ReadAsStringAsync();
                    return ConverntToResult<O>(response1);
                }
                else
                {
                    var responseError = await httpResponseMessage.Content.ReadAsStringAsync();
                    throw new BadException(responseError);
                }
            }

            if (typeof(M) == typeof(FormUrlEncodedContent))
            {
                dynamic fromData2 = model!;
                using HttpResponseMessage httpResponseMessage2 = await client.PutAsync(url, (FormUrlEncodedContent)fromData2!);
                if (httpResponseMessage2.IsSuccessStatusCode)
                {
                    return ConverntToResult<O>(await httpResponseMessage2.Content.ReadAsStringAsync());
                }

                var responseError = await httpResponseMessage2.Content.ReadAsStringAsync();
                throw new BadException(responseError);
            }

            using HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return ConverntToResult<O>(await response.Content.ReadAsStringAsync());
            }
            var response2 = await response.Content.ReadAsStringAsync();
            throw new BadException(response2);
        }

        public async Task<O> DeleteAsync<O, M>(string url, M model = default!, HttpRequestHeaders headers = null!)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            if (model != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(model, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }), Encoding.UTF8, "application/json");
            }

            using HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(BaseUrl);
            client.Timeout = Timeout;
            using HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return ConverntToResult<O>(await response.Content.ReadAsStringAsync());
            }

            var response2 = await response.Content.ReadAsStringAsync();
            throw new BadException(response2);
        }

        private O ConverntToResult<O>(string value)
        {
            if (IsStructure(typeof(O)))
            {
                if (typeof(O) == typeof(bool))
                {
                    return Convert.ToBoolean((dynamic)value);
                }

                if (typeof(O) == typeof(int))
                {
                    return int.Parse((dynamic)value);
                }

                if (typeof(O) == typeof(decimal))
                {
                    return decimal.Parse((dynamic)value);
                }

                if (typeof(O) == typeof(float))
                {
                    return float.Parse((dynamic)value);
                }

                return (O)(dynamic)value;
            }

            if (typeof(O) == typeof(string))
            {
                return (dynamic)value;
            }

            return JsonConvert.DeserializeObject<O>(value)!;
        }

        private bool IsStructure(Type source)
        {
            return source.IsValueType && !source.IsPrimitive && !source.IsEnum;
        }

        public async Task<string> CallSoapApiAsync(string url, string soapXml, HttpRequestHeaders headers = null!)
        {
            using (var client = _clientFactory.CreateClient())
            {
                if (headers != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> item2 in headers)
                    {
                        client.DefaultRequestHeaders.Add(item2.Key, item2.Value);
                    }
                }
                client.BaseAddress = new Uri(BaseUrl);
                client.Timeout = Timeout;
                var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");

                var response = await client.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var response2 = await response.Content.ReadAsStringAsync();
                    throw new BadException(response2);
                }
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<IFormFile> GetFileAsIFormFile(string url, object requestBody = null!, HttpRequestHeaders headers = null!)
        {
            // Tạo request với phương thức POST
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            if (headers != null)
            {
                foreach (KeyValuePair<string, IEnumerable<string>> item in headers)
                {
                    request.Headers.Add(item.Key, item.Value);
                }
            }

            // Thêm nội dung body (nếu có)
            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }), Encoding.UTF8, "application/json");
            }

            // Tạo HttpClient từ factory
            using HttpClient client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseUrl);
            client.Timeout = Timeout;

            // Gửi yêu cầu
            using HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung tệp
                var fileBytes = await response.Content.ReadAsByteArrayAsync();
                var fileName = response.Content.Headers.ContentDisposition?.FileName ?? "downloadedFile";

                // Tạo stream từ byte[]
                var stream = new MemoryStream(fileBytes);

                // Tạo IFormFile
                return new FormFile(stream, 0, stream.Length, "file", fileName)
                {
                    Headers = new HeaderDictionary(),
                    ContentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream"
                };
            }

            // Xử lý lỗi
            var responseContent = await response.Content.ReadAsStringAsync();
            throw new ApplicationException(response.ReasonPhrase + " GetFileAsIFormFile: " + client.BaseAddress?.ToString() + url + " : " + responseContent);
        }

    }
}
