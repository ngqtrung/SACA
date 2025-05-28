using SACA_Common.DTOs;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.DTOs.Authenticate.Response;
using SACA_Common.Exceptions;
using SACA_Common;
using SACA_FE.Const;
using System.Net.Http;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Notification.Request;
using SACA_Common.Routes;
using SACA_Common.DTOs.Contest.Response;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Headers;

namespace SACA_FE.Services
{
    public interface IFileService
    {
        Task<bool> DeleteAsync(string id);
        Task<string> CreateAsync(IFormFile file);
    }

    public class FileService : IFileService
    {
        private readonly IHttpClientBase _httpClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public FileService(IHttpClientBase httpClient, IHttpContextAccessor contextAccessor)
        {
            _httpClient = httpClient;
            _contextAccessor = contextAccessor;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{FileRoutes.INDEX}/{id}";
            var result = await _httpClient.DeleteAsync<Response<bool>, object?>(path, null, header);
            return result.Result;
        }

        public async Task<string> CreateAsync(IFormFile file)
        {
            var authToken = _contextAccessor.HttpContext?.Request.Cookies["AuthToken"];
            var authorization = $"Bearer {authToken?.ToString()}";
            var header = new HttpRequestMessage().Headers;
            header.Add("Authorization", authorization);
            _httpClient.BaseUrl = StaticVariable.APIInfo.Host;
            var path = @$"{FileRoutes.INDEX}";

            var form = new MultipartFormDataContent();
            var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            ms.Position = 0;

            var fileContent = new StreamContent(ms);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            // Add file to form data (tên "file" phải đúng với bên API controller nhận)
            form.Add(fileContent, "file", file.FileName);


            var result = await _httpClient.PostAsync<Response<CreateResult>, MultipartFormDataContent>(path, form, header);
            if (result.Result == null)
            {
                throw new BadException("Upload file failed.");
            }
            return result.Result.id;
        }
    }
}
