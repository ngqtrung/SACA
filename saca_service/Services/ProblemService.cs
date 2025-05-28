using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.Exceptions;
using SACA_Common.Models;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Models;
using System.IO.Compression;

namespace SACA_Service.Services
{
    public interface IProblemService
    {
        Task<CreateResult> CreateAsync(ProblemCreating form, string userId);
        Task<bool> AddManyAsync(List<ProblemCreating> form, string contestId, string userId);
        Task<bool> UpdateAsync(ProblemUpdating form, string userId);
        Task<bool> DeleteAsync(string userId, string id);
        Task<bool> DeleteAsync(DeleteManyRequest form, string userId);
        Task<ProblemView> GetDetailAsync(string id);
        Task<PagedResponse<ProblemTableView>> SearchAsync(ProblemTableFilter filter);
        Task<bool> UpdateManyAsync(string userId, List<ProblemUpdating> forms);
        Task<List<ProblemCreating>> ImportExcel(IFormFile file);
    }
    public class ProblemService : IProblemService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        private readonly ITestCaseService _testcaseService;
        public ProblemService(SACA_Context context, IMapper mapper, ITestCaseService testcaseService)
        {
            _context = context;
            _mapper = mapper;
            _testcaseService = testcaseService;
        }

        public async Task<CreateResult> CreateAsync(ProblemCreating form, string userId)
        {
            form.Validate();
            var duplicateProblemCode = await _context.problems.AsNoTracking()
                .AnyAsync(e => e.code == form.code && e.contest_id == form.contest_id);
            if (duplicateProblemCode)
            {
                throw new BadException(ErrorMessage.DuplicateProblemCode);
            }

            var problem = _mapper.Map<problem>(form);
            problem.Created(userId);
            _context.problems.Add(problem);
            await _context.SaveChangesAsync();
            return new CreateResult(problem.id);
        }
        public async Task<bool> AddManyAsync(List<ProblemCreating> form, string contestId, string userId)
        {
            var problemCodes = form.Select(e => e.code).Distinct().ToList();
            var duplicateProblemCode = await _context.problems.AsNoTracking()
                .AnyAsync(e => problemCodes.Contains(e.code) && e.contest_id == contestId );
            if (problemCodes.Count != form.Count)
            {
                duplicateProblemCode = true;
            }
            if (duplicateProblemCode)
            {
                throw new BadException(ErrorMessage.DuplicateProblemCode);
            }
            var problems = form.Select(request =>
            {
                var problem = _mapper.Map<problem>(request);
                problem.Created(userId);
                problem.contest_id = contestId;
                return problem;
            }).ToList();

            await _context.problems.AddRangeAsync(problems);
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, string id)
        {
            var problem = await _context.problems
                .Include(e => e.test_cases)
                .FirstOrDefaultAsync(e => e.id == id);
            if (problem == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            problem.Deleted(userId);
            foreach (var testCase in problem.test_cases)
            {
                testCase.Deleted(userId);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(DeleteManyRequest form, string userId)
        {
            var problems = await _context.problems
                .Include(e => e.test_cases)
                .Where(e => form.ids.Contains(e.id))
                .ToListAsync();
            foreach (var problem in problems)
            {
                problem.Deleted(userId);
                foreach (var testCase in problem.test_cases)
                {
                    testCase.Deleted(userId);
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProblemView> GetDetailAsync(string id)
        {
            var problem = await _context.problems.AsNoTracking()
                .Where(e => e.id == id)
                .Select(e => _mapper.Map<ProblemView>(e))
                .FirstOrDefaultAsync();
            if (problem == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            return problem;
        }

        public async Task<PagedResponse<ProblemTableView>> SearchAsync(ProblemTableFilter filter)
        {
            IQueryable<problem> query = _context.problems.AsNoTracking().OrderBy(e => e.code)
                .Include(e => e.test_cases)
                .Where(e => filter.contest_id == null || e.contest_id == filter.contest_id)
                .Where(e => filter.keyword == null ||
                            e.code.Trim().ToLower().Contains(filter.keyword.Trim().ToLower()) ||
                            e.description != null && e.description.Trim().ToLower().Contains(filter.keyword.Trim().ToLower()) ||
                            e.title.Trim().ToLower().Contains(filter.keyword.Trim().ToLower()) ||
                            e.tags != null && e.tags.Trim().ToLower().Contains(filter.keyword.Trim().ToLower())
                );

            var problems = query.Select(e => _mapper.Map<ProblemTableView>(e)).AsQueryable();

            return new PagedResponse<ProblemTableView>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                total_items = await problems.CountAsync(),
                Items = await problems.Paged(filter.page_index, filter.page_size).ToListAsync()
            };
        }

        public async Task<bool> UpdateAsync(ProblemUpdating form, string userId)
        {
            form.Validate();
            var problem = await _context.problems
                .Include(e => e.test_cases)
                .FirstOrDefaultAsync(e => e.id == form.id);
            if (problem == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            var duplicateProblemCode = await _context.problems.AsNoTracking()
                .AnyAsync(e => e.code == form.code && e.contest_id == form.contest_id && e.id != form.id);
            if (duplicateProblemCode)
            {
                throw new BadException(ErrorMessage.DuplicateProblemCode);
            }

            await ProblemServiceExtension.UpdateProblem(problem, form, userId, _context, _mapper, _testcaseService);
            _context.Update(problem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateManyAsync(string userId, List<ProblemUpdating> forms)
        {
            if (forms == null || !forms.Any())
            {
                throw new BadException("No problems provided for update.");
            }

            var problemIds = forms.Select(f => f.id).ToList();

            var problems = await _context.problems
                .Include(p => p.test_cases)
                .Where(p => problemIds.Contains(p.id))
                .ToListAsync();

            if (problems.Count != forms.Count)
            {
                throw new NotFoundException("One or more problems not found.");
            }

            var contestId = forms.First().contest_id;
            var problemCodes = forms.Select(f => f.code).ToList();

            var existingCodesInForm = problemCodes
                .GroupBy(code => code)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            var existingCodesInDB = await _context.problems
                .AsNoTracking()
                .Where(p => p.contest_id == contestId && problemCodes.Contains(p.code) && !problemIds.Contains(p.id))
                .Select(p => p.code)
                .ToListAsync();

            if (existingCodesInDB.Any() || existingCodesInForm.Any())
            {
                var conflictingCodes = existingCodesInDB.Concat(existingCodesInForm).Distinct();
                throw new BadException($"Problem code already exists: {string.Join(", ", conflictingCodes)}");
            }

            foreach (var form in forms)
            {
                var problem = problems.First(p => p.id == form.id);
                await ProblemServiceExtension.UpdateProblem(problem, form, userId, _context, _mapper, _testcaseService);
                _context.Update(problem);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<ProblemCreating>> ImportExcel(IFormFile file)
        {
            var problems = new List<ProblemCreating>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, true))
                {
                    var problemFolders = archive.Entries
                        .Where(e => e.FullName.EndsWith("problem_info.json"))
                        .ToList();

                    foreach (var entry in problemFolders)
                    {
                        var problemFolder = Path.GetDirectoryName(entry.FullName)?.Replace('\\', '/');
                        if (string.IsNullOrEmpty(problemFolder)) continue;

                        // Đọc thông tin bài toán
                        ProblemCreating? problem;
                        using (var reader = new StreamReader(entry.Open()))
                        {
                            var json = await reader.ReadToEndAsync();
                            problem = JsonConvert.DeserializeObject<ProblemCreating>(json);
                        }

                        if (problem == null) continue;

                        // Lấy tất cả các file .inp và .out
                        var testcaseEntries = archive.Entries
                            .Where(e => e.FullName.StartsWith($"{problemFolder}/Testcases/") &&
                                        (e.FullName.EndsWith(".inp") || e.FullName.EndsWith(".out")))
                            .ToList();

                        // Tạo file ZIP tạm chứa các testcase
                        var tempZipStream = new MemoryStream();
                        using (var tempArchive = new ZipArchive(tempZipStream, ZipArchiveMode.Create, true))
                        {
                            foreach (var testEntry in testcaseEntries)
                            {
                                var entryPath = testEntry.FullName[(problemFolder.Length + 1)..]; // relative path
                                var newEntry = tempArchive.CreateEntry(entryPath);
                                using var source = testEntry.Open();
                                using var target = newEntry.Open();
                                await source.CopyToAsync(target);
                            }
                        }

                        // Tạo IFormFile từ zip stream
                        tempZipStream.Position = 0;
                        var tempFormFile = new FormFile(tempZipStream, 0, tempZipStream.Length, "file", "testcases.zip");

                        // Import testcases
                        var testCases = await _testcaseService.ImportExcel(tempFormFile);

                        // Gán thông tin mặc định từ problem nếu cần
                        double totalScore = problem.score;
                        int count = testCases.Count;
                        double perScore = count > 0 ? totalScore / count : 0;

                        foreach (var tc in testCases)
                        {
                            tc.score = perScore;
                            tc.execution_time ??= problem.default_execution_time ?? 0;
                            tc.memory_limit ??= problem.default_memory_limit ?? 0;
                        }

                        problem.test_cases = testCases;
                        if(!problems.Any(e => e.code == problem.code)) problems.Add(problem);
                    }
                }
            }

            return problems;
        }
    }
    public static class ProblemServiceExtension
    {
        public static void Validate(this ProblemCreating form)
        {
            if (string.IsNullOrEmpty(form.contest_id))
            {
                throw new BadException(ErrorMessage.ContestIdIsRequired);
            }
        }
        public static async Task UpdateProblem(problem problem, ProblemUpdating form, string userId, SACA_Context _context, IMapper _mapper, ITestCaseService _testcaseService)
        {
            _mapper.Map(form, problem);
            var requestTestCaseIds = form.test_cases.Where(e => e.id != null).Select(e => e.id).ToList();
            var existedTestcases = problem.test_cases.Where(e => requestTestCaseIds.Contains(e.id)).ToList();

            var removeTestCases = problem.test_cases.Where(e => !requestTestCaseIds.Contains(e.id)).ToList();
            _context.test_cases.RemoveRange(removeTestCases);

            if (existedTestcases.Any()) await _testcaseService.UpdateManyAsync(userId, form.test_cases.Where(p => existedTestcases.Select(e => e.id).Contains(p.id)).ToList());
            var newTestcases = form.test_cases.Where(e => e.id == null);
            if (newTestcases != null && newTestcases.Any()) await _testcaseService.AddManyAsync(_mapper.Map<List<TestCaseCreating>>(newTestcases), problem.id, userId);
            //foreach (var requestTestCase in form.test_cases)
            //{
            //    var testCaseDb = problem.test_cases.FirstOrDefault(e => e.id == requestTestCase.id);
            //    if (testCaseDb != null)
            //    {
            //        _mapper.Map(requestTestCase, testCaseDb);
            //        _context.test_cases.Update(testCaseDb);
            //    }
            //    else
            //    {
            //        var newTestCase = _mapper.Map<test_case>(requestTestCase);
            //        newTestCase.Created(userId);
            //        newTestCase.problem_id = problem.id;
            //        _context.test_cases.Add(newTestCase);
            //        problem.test_cases.Add(newTestCase);
            //    }
            //}
        }
        public static IQueryable<ProblemTableView> GetQueryable(this IQueryable<ProblemTableView> query, ProblemTableFilter filter)
        {
            if (filter.keyword != null && !string.IsNullOrWhiteSpace(filter.keyword))
            {
                query = query.Where(e => e.code.ToLower().Contains(filter.keyword.ToLower()) ||
                e.title.ToLower().Contains(filter.keyword.ToLower()) ||
                (!string.IsNullOrEmpty(e.description) && e.description.ToLower().Contains(filter.keyword.ToLower())));
            }
            //return query.ApplyFilterBase(filter);
            return query;
        }
    }
}
