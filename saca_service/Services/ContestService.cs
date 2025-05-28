using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.DTOs.Submission.Request;
using SACA_Common.Enums;
using SACA_Common.Exceptions;
using SACA_Common.Models;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Models;
using System.IO.Compression;

namespace SACA_Service.Services
{
    public interface IContestService
    {
        Task<CreateResult> CreateAsync(string userId, ContestCreating form);
        Task<bool> UpdateAsync(string userId, ContestUpdating form);
        Task<bool> DeleteAsync(string userId, string id);
        Task<bool> DeleteAsync(string userId, DeleteManyRequest form);
        Task<ContestView> GetDetailAsync(string id);
        Task<PagedResponse<ContestTableView>> SearchAsync(ContestTableFilter filter, string? userId);
        Task<List<ContestTableView>> GetAllAsync();
        Task<List<string>> GetUserContestsAsync(string userId);
        Task<bool> FronzenContestAsync(string contestId, string userId);
        Task<DateTime> GetContestEndTime(string id);
        Task<ContestCreating> ImportExcel(IFormFile file);
    }
    public class ContestService : IContestService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly IProblemService _problemService;

        public ContestService(SACA_Context context, IMapper mapper, IAccountService accountService, IProblemService problemService)
        {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _problemService = problemService;
        }
        public ContestService(SACA_Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<ContestTableView>> GetAllAsync()
        {
            var contests = await _context.contests.AsNoTracking()
                .OrderByDescending(e => e.created_on)
                .Select(e => _mapper.Map<ContestTableView>(e))
                .ToListAsync();
            return contests;
        }
        public async Task<CreateResult> CreateAsync(string userId, ContestCreating form)
        {
            //form.problems.ForEach(e => e.file_id = null);
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (string.IsNullOrEmpty(form.code) || string.IsNullOrEmpty(form.title))
                    {
                        throw new BadException(ErrorMessage.InvalidModel);
                    }
                    var duplicateContestCode = await _context.contests.AsNoTracking()
                        .AnyAsync(e => e.code == form.code);
                    if (duplicateContestCode)
                    {
                        throw new BadException(ErrorMessage.DuplicateContestCode);
                    }
                    var contest = _mapper.Map<contest>(form);
                    contest.Created(userId);
                    _context.contests.Add(contest);
                    await _context.SaveChangesAsync();

                    List<string> participantIds = new List<string>();
                    //Xử lí logic tạo tài khoản thành viên tham gia
                    if (form.participants.Any())
                    {
                        var duplicateEmailsInForm = form.participants
                            .GroupBy(p => p.email.Trim().ToLower())
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();

                        if (duplicateEmailsInForm.Any())
                        {
                            throw new BadException(ErrorMessage.DuplicateEmail);
                        }

                        var participants = form.participants.Select(e => new AccountCreating
                        {
                            email = e.email,
                            fullname = e.fullname,
                            password = e.password,
                            username = e.username,
                            student_code = e.student_code,
                            roll_number = e.roll_number
                        }).ToList();

                        AddManyResponse response = await _accountService.AddManyAsync(userId, new AccountAddMany
                        {
                            accounts = participants
                        });
                        participantIds = response.ids;
                    }


                    if (participantIds.Any())
                    {
                        var contestParticipations = participantIds.Select(id => new contest_participant
                        {
                            contest_id = contest.id,
                            account_id = id,
                        }).ToList();

                        await _context.contest_participants.AddRangeAsync(contestParticipations);
                        await _context.SaveChangesAsync();
                    }

                    await _transaction.CommitAsync();
                    return new CreateResult(contest.id);
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(string userId, string id)
        {
            var contestList = _context.contests.ToList();
            var contest = await _context.contests
                .Include(e => e.contest_participants)
                .Include(e => e.problems)
                .FirstOrDefaultAsync(e => e.id == id);
            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            contest.Deleted(userId);
            foreach (var problem in contest.problems)
            {
                problem.Deleted(userId);
            }
            foreach (var participant in contest.contest_participants)
            {
                participant.Deleted(userId);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, DeleteManyRequest form)
        {
            var contests = await _context.contests
                .Include(e => e.contest_participants)
                .Include(e => e.problems)
                .Where(e => form.ids.Contains(e.id))
                .ToListAsync();
            foreach (var contest in contests)
            {
                contest.Deleted(userId);
                foreach (var problem in contest.problems)
                {
                    problem.Deleted(userId);
                }
                foreach (var participant in contest.contest_participants)
                {
                    participant.Deleted(userId);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ContestView> GetDetailAsync(string id)
        {
            var contest = await _context.contests.AsNoTracking()
                //.Where(e => e.id == id)
                .Include(e => e.problems)
                    .ThenInclude(e => e.file)
                .Include(e => e.problems)
                    .ThenInclude(e => e.test_cases)
                .Include(e => e.contest_participants)
                    .ThenInclude(e => e.account)
                .SingleOrDefaultAsync(e => e.id == id);
            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            contest.problems = contest.problems
                .OrderBy(p => p.code)
                .ToList();

            foreach (var problem in contest.problems)
            {
                problem.test_cases = problem.test_cases
                    .OrderBy(tc => tc.code)
                    .ToList();
            }

            contest.contest_participants = contest.contest_participants
                .OrderBy(cp => cp.account.email)
                .ToList();

            var response = _mapper.Map<ContestView>(contest);
            foreach (var participant in response.participants)
            {
                var match = contest.contest_participants
                    .FirstOrDefault(cp => cp.contest_id == id && cp.account_id == participant.id);

                if (match != null)
                {
                    participant.invitation_email_sent = match.invitation_email_sent;
                }
            }
            return response;
        }

        public async Task<PagedResponse<ContestTableView>> SearchAsync(ContestTableFilter filter, string? userId)
        {
            var contests = _context.contests.AsNoTracking()
                .OrderByDescending(e => e.created_on)
                .Include(e => e.contest_participants)
                .Where(e => userId == null || (e.contest_participants.Any(e => e.account_id == userId) && e.status != (int)eStatus_Contest.Draft))
                .Select(e => new ContestTableView
                {
                    id = e.id,
                    code = e.code,
                    description = e.description,
                    end_at = e.end_at,
                    start_at = e.start_at,
                    status = e.status,
                    title = e.title,
                    class_code = e.class_code,
                    subject_code = e.subject_code
                })
                .AsQueryable().GetQueryable(filter);
            return new PagedResponse<ContestTableView>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                total_items = await contests.CountAsync(),
                Items = await contests.Paged(filter.page_index, filter.page_size).ToListAsync()
            };
        }

        public async Task<bool> UpdateAsync(string userId, ContestUpdating form)
        {
            //form.problems.ForEach(e => e.file_id = null);
            var contest = await _context.contests
                .Include(c => c.problems)
                    .ThenInclude(p => p.test_cases)
                .Include(c => c.contest_participants)
                .FirstOrDefaultAsync(e => e.id == form.id);
            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            await ContestServiceExtension.UpdateContest(contest, form, userId, _context, _mapper, _problemService, _accountService);
            _context.Update(contest);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> FronzenContestAsync(string contestId, string userId)
        {
            var contest = await _context.contests.AsNoTracking()
                .Where(e => e.id == contestId)
                .FirstOrDefaultAsync();

            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }

            if (contest.is_frozen)
            {
                var bestSubmissions = await _context.best_submissions.AsNoTracking()
                    .Where(e => e.problem.contest_id == contest.id && e.frozen_score != e.score)
                    .ToListAsync();

                foreach (var bestSubmission in bestSubmissions)
                {
                    bestSubmission.frozen_score = bestSubmission.score;
                    _context.best_submissions.Update(bestSubmission);
                }
            }

            contest.is_frozen = !contest.is_frozen;
            contest.Modified(userId);
            _context.Update(contest);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<string>> GetUserContestsAsync(string userId)
        {
            var contest = await _context.contest_participants.AsNoTracking()
                                    .Where(c => c.account_id == userId).Select(c => c.contest_id).ToListAsync();
            return contest;
        }

        public async Task<DateTime> GetContestEndTime(string id)
        {
            var contest = await _context.contests.Where(e => e.id == id).FirstOrDefaultAsync();

            if (contest == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }

            return contest.end_at;
        }
        public async Task<ContestCreating> ImportExcel(IFormFile file)
        {
            var contest = new ContestCreating();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);

            // 1. Tìm contest_info.json để xác định thư mục gốc (VD: Contest A/)
            var contestInfoEntry = archive.Entries.FirstOrDefault(e => e.FullName.EndsWith("contest_info.json"));
            if (contestInfoEntry == null)
                throw new Exception("contest_info.json không tồn tại trong file zip.");

            var rootFolderPrefix = Path.GetDirectoryName(contestInfoEntry.FullName)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(rootFolderPrefix))
                throw new Exception("Không xác định được thư mục gốc chứa contest_info.json.");

            rootFolderPrefix += "/";

            // 2. Đọc contest_info.json
            using (var reader = new StreamReader(contestInfoEntry.Open()))
            {
                var json = await reader.ReadToEndAsync();
                contest = JsonConvert.DeserializeObject<ContestCreating>(json)
                          ?? throw new Exception("Lỗi deserialize contest_info.json");
            }

            // 3. Tìm tất cả entry thuộc thư mục Problems
            var problemsPrefix = rootFolderPrefix + "Problems/";

            var problemEntries = archive.Entries
                .Where(e => e.FullName.StartsWith(problemsPrefix))
                .ToList();

            if (problemEntries.Count == 0)
                throw new Exception("Không tìm thấy thư mục Problems trong file zip.");

            // 4. Tạo zip mới chỉ chứa thư mục Problems/
            using var problemStream = new MemoryStream();
            using (var problemArchive = new ZipArchive(problemStream, ZipArchiveMode.Create, true))
            {
                foreach (var entry in problemEntries)
                {
                    var relativePath = entry.FullName.Substring(problemsPrefix.Length);
                    var newEntry = problemArchive.CreateEntry(relativePath);
                    using var src = entry.Open();
                    using var dest = newEntry.Open();
                    await src.CopyToAsync(dest);
                }
            }

            problemStream.Position = 0;
            var problemFormFile = new FormFile(problemStream, 0, problemStream.Length, "file", "problems.zip");

            // 5. Gọi ProblemService.ImportExcel() để lấy toàn bộ danh sách bài toán
            var problems = await _problemService.ImportExcel(problemFormFile);
            contest.problems.AddRange(problems);

            return contest;
        }



    }
    public static class ContestServiceExtension
    {
        public static void Validate(ContestCreating form)
        {
        }
        public static async Task UpdateContest(contest contest, ContestUpdating form, string userId, SACA_Context _context, IMapper _mapper, IProblemService _problemService, IAccountService _accountService)
        {
            using (var _transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    bool frozen = contest.is_frozen;
                    _mapper.Map(form, contest);
                    contest.is_frozen = frozen;
                    //Section: Problem
                    //Retrieve all problems that are included in the request, excluded the problem that doesn't have an id, i.e new problem
                    var requestProblemIds = form.problems.Where(e => e.id != null).Select(e => e.id).ToList();
                    var existedProblems = contest.problems.Where(e => requestProblemIds.Contains(e.id)).ToList();
                    //Remove problems that are no longer included (and its related testcases)
                    var removeProblems = contest.problems.Where(e => !requestProblemIds.Contains(e.id)).ToList();
                    foreach (var removeProblem in removeProblems)
                    {
                        removeProblem.Deleted(userId);
                        foreach (var removeTestcase in removeProblem.test_cases)
                        {
                            removeTestcase.Deleted(userId);
                        }
                        await _context.SaveChangesAsync();
                    }
                    //Add or update problems
                    if (existedProblems.Any()) await _problemService.UpdateManyAsync(userId, form.problems.Where(p => existedProblems.Select(e => e.id).Contains(p.id)).ToList());
                    var newProblems = form.problems.Where(e => e.id == null);
                    if (newProblems != null && newProblems.Any()) await _problemService.AddManyAsync(_mapper.Map<List<ProblemCreating>>(newProblems), contest.id, userId);

                    //Section: Participant
                    var duplicateEmailsInForm = form.participants
                            .GroupBy(p => p.email.Trim().ToLower())
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();

                    if (duplicateEmailsInForm.Any())
                    {
                        throw new BadException(ErrorMessage.DuplicateEmail);
                    }

                    var requestParticipantIds = form.participants.Select(p => p.id).ToList();
                    var existingAccounts = await _context.sys_accounts
                        .Where(a => requestParticipantIds.Contains(a.id))
                        .ToListAsync();

                    //Remove participants that are no longer included
                    var removeParticipants = contest.contest_participants
                        .Where(p => !requestParticipantIds.Contains(p.account_id))
                        .ToList();

                    foreach (var removeParticipant in removeParticipants)
                    {
                        removeParticipant.Deleted(userId);
                        await _context.SaveChangesAsync();
                    }
                    if (existingAccounts.Any()) await _accountService.UpdateManyAsync(userId, form.participants.Where(p => existingAccounts.Select(e => e.id).Contains(p.id)).ToList());

                    var newParticipants = form.participants.Where(e => e.id == null);
                    if (newParticipants != null && newParticipants.Any())
                    {
                        AddManyResponse response = await _accountService.AddManyAsync(userId, new AccountAddMany
                        {
                            accounts = _mapper.Map<List<AccountCreating>>(newParticipants)
                        });

                        if (response.ids.Any())
                        {
                            var contestParticipations = response.ids.Select(id => new contest_participant
                            {
                                contest_id = contest.id,
                                account_id = id,
                            }).ToList();

                            await _context.contest_participants.AddRangeAsync(contestParticipations);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await _transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await _transaction.RollbackAsync();
                    Console.WriteLine($"Error in UpdateContest:", ex.Message);
                    throw;
                }
            }
        }

        public static IQueryable<ContestTableView> GetQueryable(this IQueryable<ContestTableView> query, ContestTableFilter filter)
        {
            if (filter.keyword != null && !string.IsNullOrWhiteSpace(filter.keyword))
            {
                query = query.Where(e => e.code.ToLower().Contains(filter.keyword.ToLower()) ||
                e.title.ToLower().Contains(filter.keyword.ToLower()) ||
                (!string.IsNullOrEmpty(e.description) && e.description.ToLower().Contains(filter.keyword.ToLower())));
            }
            if (filter.status != null)
            {
                query = query.Where(x => x.status == filter.status);
            }
            //return query.ApplyFilterBase(filter);
            return query;
        }
    }
}
