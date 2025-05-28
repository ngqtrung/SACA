using Microsoft.Extensions.DependencyInjection;
using SACA_Infra.Models;
using SACA_Service.Services;
using SACA_Service.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service
{
    public static class ServiceInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddSignalR();
            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<IContestService, ContestService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<ITestCaseService, TestCaseService>();
            services.AddScoped<ISysSettingService, SysSettingService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped(typeof(FileService<>), typeof(FileService<>));
            services.AddScoped<IHttpClientBase, HttpClientBase>();
            services.AddScoped<IJudge0Service, Judge0Service>();
            services.AddScoped<ISacaFileService, SacaFileService>();
            services.AddScoped<IGradingService, GradingService>();
            services.AddScoped<ILeaderBoardService, LeaderBoardService>();
            services.AddScoped<IJPlagService, JPlagService>();
            services.AddHostedService<CheckSubmissionStatusWorker>();
            services.AddHostedService<ScheduleContestWorker>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IReportService, ReportService>();
        }
    }
}
