using SACA_Common.DTOs;
using AutoMapper;
using SACA_Common.Exceptions;
using SACA_Infra.Const;
using SACA_Infra.Context;
using Microsoft.EntityFrameworkCore;
using SACA_Infra.Models;
using SACA_Common.Models;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.DTOs.Submission.Response;
using SACA_Common.Enums;
using SACA_Service.DTO.Judge0.Request;
using System.Reflection.Metadata.Ecma335;
using System.Diagnostics;
using System.IO.Compression;
using SACA_Common.DTOs.JPlag;
using System.Net.WebSockets;

namespace SACA_Service.Services
{
    public interface ISubmissionService
    {
        Task<bool> SubmitSolutionAsync(SubmitSolutionRequest form, string userId, bool is_lecture = false);
        Task<bool> DeleteAsync(string userId, string id);
        Task<bool> DeleteAsync(string userId, DeleteManyRequest form);
        Task<SubmissionView> GetDetailAsync(string id, string? userid = null);
        Task<PagedResponse<SubmissionTableView>> SearchAsync(SubmissionTableFilter filter, string? userId = null);
        Task<bool> ResubmitAsync(ResubmitSolutionRequest request, string userId, bool is_lecture = false);
    }

    public class SubmissionService : ISubmissionService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        private readonly IJudge0Service _judge0Service;
        public SubmissionService
        (
            SACA_Context context,
            IMapper mapper,
            IJudge0Service judge0Service)
        {
            _context = context;
            _mapper = mapper;
            _judge0Service = judge0Service;
        }

        public async Task<bool> SubmitSolutionAsync(SubmitSolutionRequest form, string userId, bool is_lecture = false)
        {
            //Xử lí logic nộp bài
            //Lưu thông tin bài nộp
            // kiểm tra contest
            var problem = await _context.problems.Include(e => e.test_cases)
                          .Include(e => e.contest)
                            .ThenInclude(e => e.contest_participants)
                          .FirstOrDefaultAsync(e => e.id == form.problem_id);
            if (problem == null)
            {
                throw new BadException(ErrorMessage.NotFound);
            }
            if (!problem.contest.contest_participants.Any(e => e.account_id == userId) && !is_lecture)
            {
                throw new BadException(ErrorMessage.UnAuthorize);
            }
            if (problem.contest.status != (int)eStatus_Contest.OnGoing && !is_lecture)
            {
                throw new BadException(ErrorMessage.ContestHasEnded);
            }
            if (!problem.contest.programming_languages.Contains((eType_ContestProgrammingLanguage)form.programming_language))
            {
                throw new BadException("Contest is not available in this language!");
            }
            var submission = new problem_submission
            {
                account_id = userId,
                //file_path = form.client_file_path,
                problem_id = form.problem_id,
                source_code = form.source_code,
                programming_language = form.programming_language,
                submitted_at = DateTime.Now,
                status = (int)eStatus_Submission.InQueue
            };
            submission.Created(userId);
            _context.problem_submissions.Add(submission);
            await _context.SaveChangesAsync();

            //submit to judge0
            try
            {
                if (problem.test_cases.Any())
                {
                    var gradings = problem.test_cases.Select(e => new submission_grading
                    {
                        status = (int)eStatus_Judge0_Submission.InQueue,
                        problem_submission_id = submission.id,
                        test_case_id = e.id,
                        created_on = DateTime.Now,
                        created_by = userId
                    }).ToList();
                    _context.submission_gradings.AddRange(gradings);
                    await _context.SaveChangesAsync();
                    var bestSubmissionDb = await _context.best_submissions.Where(e => e.user_id == userId).FirstOrDefaultAsync();
                    if (bestSubmissionDb == null)
                    {
                        bestSubmissionDb = new SACA_Infra.Models.best_submission
                        {
                            problem_id = submission.problem_id,
                            user_id = submission.account_id,
                            problem_submission_id = submission.id,
                            submited_at = submission.submitted_at,
                            score = submission.scrore,
                            running_time = submission.runinng_time,
                            running_memory = submission.running_memory,
                            number_of_attempts = 1
                        };
                        bestSubmissionDb.Created(submission.account_id);
                        _context.best_submissions.Add(bestSubmissionDb);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (bestSubmissionDb.score < submission.problem.score)
                        {
                            bestSubmissionDb.number_of_attempts++;
                        }
                        _context.best_submissions.Update(bestSubmissionDb);
                        await _context.SaveChangesAsync();
                    }
                    if (problem.contest.grading_type == (int)eType_Grading.Immediately)
                    {
                        var tokens = await _judge0Service.Submit(new DTO.Judge0.Request.Judge0_CreateSubmissionBatch
                        {
                            submissions = problem.test_cases.Select(e => new DTO.Judge0.Request.Judge0_CreateSubmission
                            {
                                language_id = submission.programming_language,
                                source_code = submission.source_code ?? "",
                                stdin = e.input,
                                expected_output = e.output,
                                cpu_time_limit = e.execution_time / 1000.0,
                                memory_limit = e.memory_limit <= 2048 ? 2048 : e.memory_limit >= 512000 ? 512000 : e.memory_limit
                            }).ToList()
                        });
                        for (int index = 0; index < gradings.Count(); index++)
                        {
                            var token = tokens[index];
                            var grading = gradings[index];
                            grading.judge0_token = token;
                        }

                    }
                    _context.submission_gradings.UpdateRange(gradings);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {

            }
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, string id)
        {
            var submission = await _context.problem_submissions
                .FirstOrDefaultAsync(e => e.id == id);
            if (submission == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            submission.Deleted(userId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, DeleteManyRequest form)
        {
            var submissions = await _context.problem_submissions
                .Where(e => form.ids.Contains(e.id))
                .ToListAsync();
            foreach (var submission in submissions)
            {
                submission.Deleted(userId);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SubmissionView> GetDetailAsync(string id, string? userId = null)
        {
            var submissionDb = await _context.problem_submissions
                .Include(e => e.problem)
                    .ThenInclude(e => e.contest)
                .Include(e => e.submission_gradings)
                   .ThenInclude(e => e.test_case)
                .Include(e => e.submitter)
                .AsNoTracking()
                .Where(e => e.id == id && (userId == null || e.account_id == userId))
                .FirstOrDefaultAsync();
            if (submissionDb == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            var submissionView = new SubmissionView
            {
                id = submissionDb.id,
                source_code = submissionDb.source_code,
                programming_language = submissionDb.programming_language,
                submitted_at = submissionDb.submitted_at,
                problem_title = submissionDb.problem.title,
                contest_code = submissionDb.problem.contest.code,
                userid = submissionDb.submitter.id,
                username = submissionDb.submitter.username,
                total_testcase = submissionDb.submission_gradings.Count(),
                passed_testcase = submissionDb.submission_gradings.Count(e => e.status == (int)eStatus_Judge0_Submission.Accepted),
                gradings = submissionDb.submission_gradings.OrderBy(e => e.test_case.order).Where(e => (userId == null || e.test_case.testcase_type == (int)eType_TestCase.Sample))
                .Select(e => new GradingView
                {
                    input = e.test_case.input,
                    expected_output = e.test_case.output,
                    actual_output = e.actual_output,
                    testcase_id = e.test_case_id,
                    running_memory = e.running_memory,
                    running_time = e.runinng_time,
                    status = (eStatus_Judge0_Submission)e.status,
                    testcase_code = e.test_case.code,
                }).ToList()
            };
            return submissionView;
        }

        public async Task<PagedResponse<SubmissionTableView>> SearchAsync(SubmissionTableFilter filter, string? userId = null)
        {
            var submissions = _context.problem_submissions
                .Include(e => e.problem)
                .Include(e => e.submitter)
                .AsNoTracking()
                .Where(e => e.problem.contest_id == filter.contest_id)
                .Where(e => filter.problem_id == null || e.problem_id == filter.problem_id)
                .Where(e => userId == null || userId == e.account_id)
                .OrderByDescending(e => e.submitted_at)
                .Select(e => new SubmissionTableView
                {
                    id = e.id,
                    file_path = e.file_path,
                    programming_language = e.programming_language,
                    submitted_at = e.submitted_at,
                    problem_title = e.problem.title,
                    submitted_by = e.submitter.username,
                    submitted_by_name = e.submitter.fullname,
                    runinng_time = e.runinng_time,
                    running_memory = e.running_memory,
                    status = e.status,
                    is_frozen = e.problem.contest.is_frozen,
                    score = e.scrore
                })
                .AsQueryable().GetQueryable(filter);
            return new PagedResponse<SubmissionTableView>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                total_items = await submissions.CountAsync(),
                Items = await submissions.Paged(filter.page_index, filter.page_size).ToListAsync()
            };
        }
        public async Task<bool> ResubmitAsync(ResubmitSolutionRequest request, string userId, bool is_lecture = false)
        {
            if (!is_lecture)
                throw new BadException(ErrorMessage.UnAuthorize);

            var submissions = await _context.problem_submissions
                .Include(s => s.problem)
                    .ThenInclude(p => p.test_cases)
                .Where(s => s.problem.contest_id == request.contestId)
                .Where(s => request.submissionIds.Count == 0 || request.submissionIds.Contains(s.id))
                .ToListAsync();

            // Bắt đầu Transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                ////Handle bestsubmission 
                var bestSubmissions = await _context.best_submissions
                                      .Where(s => s.problem.contest_id == request.contestId)
                                      .Where(e => request.submissionIds.Count == 0 || request.submissionIds.Contains(e.problem_submission_id)).ToArrayAsync();
                if (request.submissionIds.Count == 0)
                {
                    _context.best_submissions.RemoveRange(bestSubmissions);
                }
                else
                {
                    var submissionDbs = await _context.problem_submissions
                                        .Include(s => s.problem)
                                        .Where(e => !request.submissionIds.Contains(e.id) && e.problem.contest_id == request.contestId)
                                        .ToListAsync();
                    foreach (var bestSubmission in bestSubmissions)
                    {
                        var newBestSubmission = submissionDbs.OrderByDescending(e => e.scrore).FirstOrDefault(e => e.problem_id == bestSubmission.problem_id && e.account_id == bestSubmission.user_id);
                        if (newBestSubmission == null)
                        {
                            _context.best_submissions.Remove(bestSubmission);
                        }
                        else
                        {
                            bestSubmission.score = newBestSubmission.scrore;
                            bestSubmission.submited_at = newBestSubmission.submitted_at;
                            bestSubmission.running_time = newBestSubmission.runinng_time;
                            bestSubmission.running_memory = newBestSubmission.running_memory;
                            bestSubmission.problem_submission_id = newBestSubmission.id;
                            _context.best_submissions.Update(bestSubmission);
                        }
                    }
                }
                var gradingsToAdd = new List<submission_grading>();

                var submissionIds = submissions.Select(e => e.id).ToList();
                var oldGradings = await _context.submission_gradings.AsNoTracking()
                    .Where(e => submissionIds.Contains(e.problem_submission_id))
                    .ToListAsync();

                _context.submission_gradings.RemoveRange(oldGradings);
                foreach (var submission in submissions)
                {
                    var problem = submission.problem;
                    if (problem.test_cases == null || !problem.test_cases.Any()) continue;

                    var gradings = problem.test_cases.Select(tc => new submission_grading
                    {
                        status = (int)eStatus_Judge0_Submission.InQueue,
                        problem_submission_id = submission.id,
                        test_case_id = tc.id,
                        created_on = DateTime.Now,
                        created_by = userId
                    }).ToList();

                    gradingsToAdd.AddRange(gradings);

                    var tokens = await _judge0Service.Submit(new Judge0_CreateSubmissionBatch
                    {
                        submissions = problem.test_cases.Select(tc => new Judge0_CreateSubmission
                        {
                            language_id = submission.programming_language,
                            source_code = submission.source_code ?? "",
                            stdin = tc.input,
                            expected_output = tc.output,
                            cpu_time_limit = tc.execution_time / 1000.0,
                            memory_limit = tc.memory_limit <= 2048 ? 2048 : tc.memory_limit >= 512000 ? 512000 : tc.memory_limit
                        }).ToList()
                    });

                    for (int i = 0; i < tokens.Count; i++)
                    {
                        gradings[i].judge0_token = tokens[i];
                    }

                    submission.status = (int)eStatus_Submission.InQueue;
                    _context.problem_submissions.Update(submission);
                }

                _context.submission_gradings.AddRange(gradingsToAdd);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex);
                return false;
            }
        }
    }

    public static class SubmissionServiceExtension
    {
        public static IQueryable<SubmissionTableView> GetQueryable(this IQueryable<SubmissionTableView> query, SubmissionTableFilter filter)
        {
            if (filter.keyword != null)
            {
                query = query.Where(e =>
                    e.problem_title.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                    e.submitted_by.ToLower().Trim().Contains(filter.keyword.ToLower().Trim())
                );
            }
            if (filter.status != null)
            {
                query = query.Where(e => e.status == filter.status);
            }
            if (filter.programming_language != null)
            {
                query = query.Where(e => e.programming_language == filter.programming_language);
            }
            return query;
        }
    }
}
