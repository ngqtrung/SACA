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
    public class CheckSubmissionStatusWorker : BackgroundService
    {
        private readonly ILogger<CheckSubmissionStatusWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);
        public CheckSubmissionStatusWorker
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

                    await CheckSubmissionStatus();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
        private async Task CheckSubmissionStatus()
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<SACA_Context>();
                var _judge0Service = scope.ServiceProvider.GetRequiredService<IJudge0Service>();
                var _gradingService = scope.ServiceProvider.GetRequiredService<IGradingService>();
                var unFinishedState = new List<int>
                {
                    (int) eStatus_Judge0_Submission.InQueue,
                    (int) eStatus_Judge0_Submission.Processing
                };
                var submissionDbs = await _context.submission_gradings.Where(e => e.judge0_token != null &&
                                                                                unFinishedState.Contains(e.status)
                                                                          )
                                                                            .Take(20)
                                                                            .ToListAsync();
                if (submissionDbs.Any())
                {
                    var results = await _judge0Service.GetSubmissions(submissionDbs.Select(e => e.judge0_token ?? "").ToList());
                    if (results != null && results.submissions != null)
                    {
                        var listSubmissionChange = new List<string>();
                        foreach (var submission in results.submissions)
                        {
                            if (submission == null) continue;
                            var submissionDb = submissionDbs.FirstOrDefault(e => e.judge0_token == submission.token);
                            if (submissionDb == null) continue;
                            if (submissionDb.status != submission.status.id)
                            {
                                submissionDb.status = submission.status.id;
                                submissionDb.runinng_time = Convert.ToDouble(submission.time);
                                submissionDb.running_memory = Convert.ToDouble(submission.memory);
                                submissionDb.actual_output = submission.stdout;
                                listSubmissionChange.Add(submissionDb.problem_submission_id);
                            }
                        }
                        _context.submission_gradings.UpdateRange(submissionDbs);
                        await _context.SaveChangesAsync();
                        listSubmissionChange = listSubmissionChange.Distinct().ToList();
                        if (listSubmissionChange.Count > 0)
                        {
                            await _gradingService.GradeSubmission(listSubmissionChange);
                        }

                    }
                }
            }
        }
    }
}
