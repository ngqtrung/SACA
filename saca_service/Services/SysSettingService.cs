using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SACA_Common.DTOs.SysSetting.Request;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Infra.Config;
using SACA_Infra.Utils;

namespace SACA_Service.Services
{
    public interface ISysSettingService
    {
        List<SysSettingView> GetAll();
        MailConfig GetEmailConfig();
        bool Update(string userId, List<SysSettingUpdate> request);
    }

    public class SysSettingService : ISysSettingService
    {
        private readonly XmlHelper _xmlHelper;
        private readonly ILogger<SysSettingService> _logger;

        public SysSettingService(XmlHelper xmlHelper, ILogger<SysSettingService> logger)
        {
            _xmlHelper = xmlHelper ?? throw new ArgumentNullException(nameof(xmlHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public List<SysSettingView> GetAll()
        {
            var settings = _xmlHelper.GetAllSettings();
            return settings;
        }

        public MailConfig GetEmailConfig()
        {
            var settings = GetAll();
            return new MailConfig
            {
                Email = settings.FirstOrDefault(e => e.key == "config_mail_email")?.value ?? "",
                Password = settings.FirstOrDefault(e => e.key == "config_mail_password")?.value ?? "",
                SmtpClient_Port = Convert.ToInt32(settings.FirstOrDefault(e => e.key == "config_mail_smtp_port")?.value ?? "538"),
                SmtpClient_Host = settings.FirstOrDefault(e => e.key == "config_mail_smtp_host")?.value ?? "538",
                Fullname = settings.FirstOrDefault(e => e.key == "config_mail_fullname")?.value ?? "538",
            };
        }

        public bool Update(string userId, List<SysSettingUpdate> request)
        {
            foreach (var setting in request)
            {
                _xmlHelper.UpdateSetting(userId, setting);
            }
            return true;
        }
    }
}
