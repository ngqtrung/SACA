using SACA_FE.Services;

namespace SACA_FE
{
    public static class ServiceInjection
    {
        public static void AddServices(this IServiceCollection services)
        {
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddScoped<IHttpClientBase, HttpClientBase>();
            services.AddScoped<IAuthenService, AuthenService>();
            services.AddScoped<IContestService, ContestService>();
            services.AddScoped<INotifiService, NotifiService>();
            services.AddScoped<IProblemService, ProblemService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISubmissionService, SubmissionService>();
            services.AddScoped<ILeadboardService, LeadboardService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IGradingMachineService, GradingMachineService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IPlagiarismService, PlagiarismService>();
            services.AddScoped<ITestcaseService, TestCaseService>();
        }
    }
}
