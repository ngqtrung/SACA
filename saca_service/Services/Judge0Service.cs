using AutoMapper;
using Microsoft.Extensions.Logging;
using SACA_Common.DTOs.GradingMachine.Response;
using SACA_Service.DTO.Judge0.Request;
using SACA_Service.DTO.Judge0.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Services
{
    public interface IJudge0Service
    {
        Task<List<string>> Submit(Judge0_CreateSubmissionBatch form);
        Task<Judge0_GetSubmissionBatchResponse> GetSubmissions(List<string> ids);
        Task<string> GetVersion();
        Task<List<Judge0_GetWorkersResponse>> GetWorkerInfo();
        Task<Judge0_GetConfigInfoResponse> GetConfigInfo();
        Task<GradingMachineInfo> GetGradingMachineInfo();
    }
    public class Judge0Service : IJudge0Service
    {
        private readonly IHttpClientBase _httpClient;
        private readonly ILogger<Judge0Service> _logger;
        private readonly ISysSettingService _sysSettingService;
        private readonly IMapper _mapper;
        public Judge0Service
        (
            IHttpClientBase httpClient,
            ILogger<Judge0Service> logger,
            ISysSettingService sysSettingService,
            IMapper mapper
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _sysSettingService = sysSettingService;
            _mapper = mapper;
        }

        public async Task<Judge0_GetConfigInfoResponse> GetConfigInfo()
        {
            try
            {

                var settings = _sysSettingService.GetAll();
                _httpClient.BaseUrl = settings.FirstOrDefault(e => e.key == "config_judge0_api_url")?.value ?? "";
                var response = await _httpClient.GetAsync<Judge0_GetConfigInfoResponse>("/config_info");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new Judge0_GetConfigInfoResponse();
            }
        }

        public async Task<GradingMachineInfo> GetGradingMachineInfo()
        {
            var configInfoTask = GetConfigInfo();
            var versionTask = GetVersion();
            var workersTask = GetWorkerInfo();

            await Task.WhenAll(configInfoTask, versionTask, workersTask);

            var configInfo = configInfoTask.Result;
            var version = versionTask.Result;
            var workers = workersTask.Result;

            var info = new GradingMachineInfo();
            _mapper.Map(configInfo, info);
            info.version = version;
            info.queue_size = workers.Sum(e => e.size);
            info.worker_available = workers.Sum(e => e.available);
            info.worker_idle = workers.Sum(e => e.idle);
            info.worker_working = workers.Sum(e => e.working);
            info.worker_pause = workers.Sum(e => e.paused);
            info.job_failed = workers.Sum(e => e.failed);
            info.is_active = info.version != "";
            return info;
        }

        public async Task<Judge0_GetSubmissionBatchResponse> GetSubmissions(List<string> ids)
        {
            try
            {
                var settings = _sysSettingService.GetAll();
                _httpClient.BaseUrl = settings.FirstOrDefault(e => e.key == "config_judge0_api_url")?.value ?? "";
                var response = await _httpClient.GetAsync<Judge0_GetSubmissionBatchResponse>($"/submissions/batch?base64_encoded=true&tokens={string.Join(',', ids)}");
                foreach (var submission in response.submissions)
                {
                    if (submission != null && submission.stdout != null)
                    {
                        submission.stdout = Judge0ServiceExtension.DecodeBase64(submission.stdout);
                    }
                    if (submission != null && submission.message != null)
                    {
                        submission.message = Judge0ServiceExtension.DecodeBase64(submission.message);
                    }
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new Judge0_GetSubmissionBatchResponse();
            }
        }

        public async Task<string> GetVersion()
        {
            try
            {

                var settings = _sysSettingService.GetAll();
                _httpClient.BaseUrl = settings.FirstOrDefault(e => e.key == "config_judge0_api_url")?.value ?? "";
                var response = await _httpClient.GetAsync<string>("/version");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return "";
            }
        }

        public async Task<List<Judge0_GetWorkersResponse>> GetWorkerInfo()
        {
            try
            {

                var settings = _sysSettingService.GetAll();
                _httpClient.BaseUrl = settings.FirstOrDefault(e => e.key == "config_judge0_api_url")?.value ?? "";
                var response = await _httpClient.GetAsync<List<Judge0_GetWorkersResponse>>("/workers");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<Judge0_GetWorkersResponse>();
            }
        }

        public async Task<List<string>> Submit(Judge0_CreateSubmissionBatch form)
        {
            try
            {
                var settings = _sysSettingService.GetAll();
                _httpClient.BaseUrl = settings.FirstOrDefault(e => e.key == "config_judge0_api_url")?.value ?? "";
                var response = await _httpClient.PostAsync<List<Judge0_CreateSubmissionResponse>, Judge0_CreateSubmissionBatch>("/submissions/batch?base64_encoded=false", form);
                return response.Select(e => e.token).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return new List<string>();
            }
        }
    }
    public static class Judge0ServiceExtension
    {
        public static string DecodeBase64(string base64Encoded)
        {
            if (string.IsNullOrEmpty(base64Encoded))
                return string.Empty;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64Encoded);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                return base64Encoded;
            }
        }
    }
}
