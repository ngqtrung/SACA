using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SACA_Common.Enums;
using SACA_Infra.Context;
using SACA_Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Workers
{
    public class ScheduleContestWorker : BackgroundService
    {
        private readonly ILogger<CheckSubmissionStatusWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(1);
        public ScheduleContestWorker
        (
            IServiceProvider serviceProvider,
            ILogger<CheckSubmissionStatusWorker> logger
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    await UpdateContestStatus();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
        private async Task UpdateContestStatus()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<SACA_Context>();
                var statusCanUpdates = new List<int>
                {
                    (int) eStatus_Contest.Scheduled,
                    (int) eStatus_Contest.OnGoing,
                    (int) eStatus_Contest.Completed
                };
                var contestDbs = await _context.contests.Where(e => statusCanUpdates.Contains(e.status)).ToListAsync();
                var now = DateTime.Now;
                foreach (var contest in contestDbs)
                {
                    if (contest.start_at > now)
                    {
                        contest.status = (int)eStatus_Contest.Scheduled;
                    }
                    if (contest.start_at <= now && contest.end_at >= now)
                    {
                        contest.status = (int)eStatus_Contest.OnGoing;
                    }
                    if (contest.end_at < now)
                    {
                        contest.status = (int)eStatus_Contest.Completed;
                    }
                }
                _context.contests.UpdateRange(contestDbs);
                await _context.SaveChangesAsync();
            }
        }
    }
}
