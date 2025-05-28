using Microsoft.EntityFrameworkCore;
using SACA_Common.Enums;
using SACA_Common.Models;
using SACA_Infra.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Services
{
    public interface IGradingService
    {
        Task GradeSubmission(List<string> submissionIds);
    }
    public class GradingService : IGradingService
    {
        private readonly SACA_Context _context;
        public GradingService
        (
            SACA_Context context
        )
        {
            _context = context;
        }
        /// <summary>
        /// Đang chỉ có chấm điểm theo test case đạt được, chưa có chấm điểm theo các thể loại contest khác
        /// </summary>
        /// <param name="submissionIds"></param>
        /// <returns></returns>
        public async Task GradeSubmission(List<string> submissionIds)
        {
            var submissionDbs = await _context.problem_submissions
                                      .Include(e => e.problem)
                                        .ThenInclude(e => e.contest)
                                      .Include(e => e.submission_gradings)
                                      .Where(e => submissionIds.Contains(e.id)).ToListAsync();

            var unFinishedState = new List<int>
            {
                (int)eStatus_Judge0_Submission.InQueue,
                (int)eStatus_Judge0_Submission.Processing
            };
            foreach (var submissionDb in submissionDbs)
            {
                if (!submissionDb.submission_gradings.Any()) continue;
                // Đóng băng chưa
                var isFrozen = submissionDb.problem.contest.is_frozen;

                // khi tất cả các test case đã được chấm xong mới thực hiện chấm điểm 
                if (!submissionDb.submission_gradings.Any(e => unFinishedState.Contains(e.status)))
                {
                    if (submissionDb.problem.contest.contest_type == (int)eType_Contest.Normal)
                    {
                        var scoreOfEachTestcase = submissionDb.problem.score / submissionDb.submission_gradings.Count;
                        submissionDb.scrore = scoreOfEachTestcase * submissionDb.submission_gradings.Count(e => e.status == (int)eStatus_Judge0_Submission.Accepted);
                    }
                    if (submissionDb.problem.contest.contest_type == (int)eType_Contest.ICPC)
                    {
                        if (submissionDb.submission_gradings.Any(e => e.status != (int)eStatus_Judge0_Submission.Accepted))
                        {
                            submissionDb.scrore = 0;
                        }
                        else
                        {
                            submissionDb.scrore = submissionDb.problem.score;
                        }
                    }
                    submissionDb.runinng_time = submissionDb.submission_gradings.Max(e => e.runinng_time);
                    submissionDb.running_memory = submissionDb.submission_gradings.Max(e => e.running_memory);
                    // Lưu submission tốt nhất
                    var bestSubmissionDb = await _context.best_submissions
                                          .FirstOrDefaultAsync(e => e.user_id == submissionDb.account_id &&
                                                                    e.problem_id == submissionDb.problem_id
                                                              );

                    if (bestSubmissionDb == null)
                    {
                        bestSubmissionDb = new SACA_Infra.Models.best_submission
                        {
                            problem_id = submissionDb.problem_id,
                            user_id = submissionDb.account_id,
                            problem_submission_id = submissionDb.id,
                            submited_at = submissionDb.submitted_at,
                            score = submissionDb.scrore,
                            frozen_score = isFrozen ? 0 : submissionDb.scrore,
                            running_time = submissionDb.runinng_time,
                            running_memory = submissionDb.running_memory,
                            number_of_attempts = 1
                        };
                        bestSubmissionDb.Created(submissionDb.account_id);
                        _context.best_submissions.Add(bestSubmissionDb);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (bestSubmissionDb.score < submissionDb.scrore)
                        {
                            bestSubmissionDb.score = submissionDb.scrore;
                            bestSubmissionDb.frozen_score = isFrozen ? bestSubmissionDb.frozen_score : submissionDb.scrore;
                            bestSubmissionDb.submited_at = submissionDb.submitted_at;
                            bestSubmissionDb.running_time = submissionDb.runinng_time;
                            bestSubmissionDb.running_memory = submissionDb.running_memory;
                            bestSubmissionDb.problem_submission_id = submissionDb.id;
                        }
                        _context.best_submissions.Update(bestSubmissionDb);
                        await _context.SaveChangesAsync();
                    }

                }

                // Cập nhật trạng thái submission
                if (!submissionDb.submission_gradings.Any(e => unFinishedState.Contains(e.status)))
                {
                    if (!submissionDb.submission_gradings.Any(e => e.status != (int)eStatus_Judge0_Submission.Accepted))
                    {
                        submissionDb.status = (int)eStatus_Submission.Accepted;
                    }
                    else
                    {
                        if (submissionDb.scrore != 0)
                        {
                            submissionDb.status = (int)eStatus_Submission.PartialAccepted;
                        }
                        else
                        {
                            foreach (var grading in submissionDb.submission_gradings)
                            {
                                if (!unFinishedState.Contains(grading.status) && grading.status != (int)eStatus_Judge0_Submission.Accepted)
                                {
                                    switch (grading.status)
                                    {
                                        case (int)eStatus_Judge0_Submission.WrongAnswer:
                                            submissionDb.status = (int)eStatus_Submission.WrongAnswer;
                                            break;
                                        case (int)eStatus_Judge0_Submission.CompilationError:
                                            submissionDb.status = (int)eStatus_Submission.CompileError;
                                            break;
                                        case (int)eStatus_Judge0_Submission.TimeLimitExceeded:
                                            submissionDb.status = (int)eStatus_Submission.TimeLimitExceeded;
                                            break;
                                        case (int)eStatus_Judge0_Submission.InternalError:
                                        case (int)eStatus_Judge0_Submission.ExecFormatError:
                                            submissionDb.status = (int)eStatus_Submission.SystemError;
                                            break;
                                        case (int)eStatus_Judge0_Submission.RuntimeError_NZEC:
                                        case (int)eStatus_Judge0_Submission.RuntimeError_SIGABRT:
                                        case (int)eStatus_Judge0_Submission.RuntimeError_SIGFPE:
                                        case (int)eStatus_Judge0_Submission.RuntimeError_SIGSEGV:
                                        case (int)eStatus_Judge0_Submission.RuntimeError_SIGXFSZ:
                                        case (int)eStatus_Judge0_Submission.RuntimeError_Other:
                                            submissionDb.status = (int)eStatus_Submission.RuntimeError;
                                            break;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    submissionDb.status = (int)eStatus_Submission.Running;
                }
            }
            _context.problem_submissions.UpdateRange(submissionDbs);
            await _context.SaveChangesAsync();
        }
    }
}
