using AutoMapper;
using SACA_Common.DTOs.Account;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Contest;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.File.Response;
using SACA_Common.DTOs.GradingMachine.Response;
using SACA_Common.DTOs.Notification;
using SACA_Common.DTOs.Notification.Request;
using SACA_Common.DTOs.Problem;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using SACA_Common.DTOs.Report.Contest;
using SACA_Common.DTOs.Submission.Response;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Common.DTOs.TestCase;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.DTOs.TestCase.Response;
using SACA_Infra.Models;
using SACA_Service.DTO.Judge0.Response;

namespace SACA_Service.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Contest
            CreateMap<ContestCreating, contest>();
            CreateMap<ContestUpdating, contest>()
                .ForMember(desc => desc.problems, opt => opt.Ignore())
                .ForMember(desc => desc.contest_participants, opt => opt.Ignore());
            CreateMap<contest, ContestView>()
                .ForMember(dest => dest.participants, opt => opt.MapFrom(src => src.contest_participants.Select(cp => cp.account).ToList()));
            CreateMap<contest, ContestTableView>();
            CreateMap<ContestView, ContestInfo>();

            //Problem
            CreateMap<ProblemCreating, problem>();
            CreateMap<ProblemUpdating, problem>()
                .ForMember(desc => desc.test_cases, opt => opt.Ignore());
            CreateMap<ProblemUpdating, ProblemCreating>()
                .ForMember(dest => dest.test_cases, opt => opt.MapFrom(src => src.test_cases.Cast<TestCaseCreating>().ToList()));
            CreateMap<problem, ProblemView>();
            CreateMap<problem, ProblemTableView>();
            CreateMap<ProblemFormView, ProblemCreating>()
                .IncludeBase<ProblemFormView, ProblemBase>();
            //.ForMember(dest => dest.test_cases, opt => opt.MapFrom(src => src.test_cases.Select(tc => ));
            CreateMap<ProblemFormView, ProblemBase>();
            CreateMap<ProblemView, ProblemInfo>();
            CreateMap<problem, ProblemInfo>();

            //Test case
            CreateMap<TestCaseCreating, test_case>();
            CreateMap<TestCaseUpdating, test_case>();
            CreateMap<test_case, TestCaseTableView>();
            CreateMap<test_case, TestCaseView>();
            CreateMap<TestCaseTableView, TestCaseCreating>()
                .IncludeBase<TestCaseTableView, TestCaseBase>();
            CreateMap<TestCaseTableView, TestCaseBase>();
            CreateMap<TestCaseUpdating, TestCaseCreating>();
            CreateMap<TestCaseView, TestCaseBase>();

            //Account
            CreateMap<AccountFormView, AccountCreating>()
                .IncludeBase<AccountFormView, AccountBase>();
            CreateMap<AccountFormView, AccountBase>();
            CreateMap<AccountCreating, sys_account>();
            CreateMap<AccountUpdating, sys_account>();
            CreateMap<AccountUpdating, AccountCreating>();
            CreateMap<sys_account, AccountView>()
                .ForMember(desc => desc.role_name, opt => opt.Ignore());
            CreateMap<contest_participant, AccountView>();
            //Notification
            CreateMap<NotificationCreating, notification>();

            //Sys_Setting
            CreateMap<sys_setting, SysSettingView>();

            //File
            CreateMap<saca_file, SacaFileView>();

            CreateMap<Judge0_GetConfigInfoResponse, GradingMachineInfo>();
        }
    }
}
