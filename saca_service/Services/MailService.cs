using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Org.BouncyCastle.Asn1.X509;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Mail.Request;
using SACA_Common.DTOs.SysSetting.Response;
using SACA_Infra.Context;
using SACA_Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Services
{
    public interface IMailService
    {
        Task<bool> SendMailInvite(SendMailInvite request);
    }
    public class MailService : IMailService
    {
        private readonly SACA_Context _context;
        private readonly IHostEnvironment _env;
        private readonly ISysSettingService _sysSettingService;
        public MailService
        (
            SACA_Context context,
            IHostEnvironment env,
            ISysSettingService sysSettingService
        )
        {
            _context = context;
            _env = env;
            _sysSettingService = sysSettingService;
        }

        public async Task<bool> SendMailInvite(SendMailInvite request)
        {
            var members = await _context.contest_participants
                .Include(e => e.contest)
                .Include(c => c.account)
                .Where(c => c.contest_id == request.contest_id && (request.account_ids.Count == 0 || request.account_ids.Contains(c.account_id)))
                .ToListAsync();
            var tasks = new List<Task>();
            var mailConfig = _sysSettingService.GetEmailConfig();
            foreach (var member in members)
            {
                member.invitation_email_sent = true;
                var emailContent = SmtpHelper.BuildHtmlBody(
                                   Path.Combine(_env.ContentRootPath, "Templates", "InviteParticipateContest.html"),
                                                new KeyValuePair<string, string>("student_name", member.account.fullname),
                                                new KeyValuePair<string, string>("contest_name", member.contest.code),
                                                new KeyValuePair<string, string>("start_time", member.contest.start_at.ToString("dd/MM/yyyy hh:mm:ss tt")),
                                                new KeyValuePair<string, string>("username", member.account.username),
                                                new KeyValuePair<string, string>("password", member.account.sys_generated_password ?? "")
                                   );
                tasks.Add(SmtpHelper.SendMail(member.account.email, $"Thông báo tham gia cuộc thi {member.contest.code}", emailContent, mailConfig));
            }
            await Task.WhenAll(tasks);
            _context.contest_participants.UpdateRange(members);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
