using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.DTOs.TestCase.Response;
using SACA_Common.Exceptions;
using SACA_Common.Models;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Services
{
    public interface ITestCaseService
    {
        Task<CreateResult> CreateAsync(string userId, TestCaseCreating form);
        Task<bool> AddManyAsync(List<TestCaseCreating> form, string problemId, string userId);
        Task<bool> UpdateAsync(string userId, TestCaseUpdating form);
        Task<bool> DeleteAsync(string userId, string id);
        Task<bool> DeleteAsync(string userId, DeleteManyRequest form);
        Task<TestCaseView> GetDetailAsync(string id);
        Task<PagedResponse<TestCaseTableView>> SearchAsync(TestCaseTableFilter filter);
        Task<bool> UpdateManyAsync(string userId, List<TestCaseUpdating> forms);
        Task<List<TestCaseCreating>> ImportExcel(IFormFile file);
    }

    public class TestCaseService : ITestCaseService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        public TestCaseService(SACA_Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CreateResult> CreateAsync(string userId, TestCaseCreating form)
        {
            var duplicateTestCaseCode = await _context.test_cases.AsNoTracking()
                .AnyAsync(e => e.code == form.code);
            if (duplicateTestCaseCode)
            {
                throw new BadException(ErrorMessage.DuplicateTestCaseCode);
            }
            var test_case = _mapper.Map<test_case>(form);
            //Xử lí logic tạo tài khoản thành viên tham gia
            test_case.Created(userId);
            _context.test_cases.Add(test_case);
            await _context.SaveChangesAsync();
            return new CreateResult(test_case.id);
        }

        public async Task<bool> AddManyAsync(List<TestCaseCreating> form, string problemId, string userId)
        {
            var testcaseCodes = form.Select(e => e.code).Distinct().ToList();
            var duplicateTestcaseCode = await _context.test_cases.AsNoTracking()
                .AnyAsync(e => testcaseCodes.Contains(e.code) && e.problem_id == problemId);
            if (testcaseCodes.Count != form.Count)
            {
                duplicateTestcaseCode = true;
            }
            if (duplicateTestcaseCode)
            {
                throw new BadException(ErrorMessage.DuplicateTestCaseCode);
            }
            var testcases = form.Select(request =>
            {
                var testcase = _mapper.Map<test_case>(request);
                testcase.Created(userId);
                testcase.problem_id = problemId;
                return testcase;
            }).ToList();

            await _context.test_cases.AddRangeAsync(testcases);
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, string id)
        {
            var test_case = await _context.test_cases
                .FirstOrDefaultAsync(e => e.id == id);
            if (test_case == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            test_case.Deleted(userId);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string userId, DeleteManyRequest form)
        {
            var test_cases = await _context.test_cases
                .Where(e => form.ids.Contains(e.id))
                .ToListAsync();
            foreach (var test_case in test_cases)
            {
                test_case.Deleted(userId);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TestCaseView> GetDetailAsync(string id)
        {
            var test_case = await _context.test_cases.AsNoTracking()
                .Where(e => e.id == id)
                .Select(e => _mapper.Map<TestCaseView>(e))
                .FirstOrDefaultAsync();
            if (test_case == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            return test_case;
        }

        public async Task<PagedResponse<TestCaseTableView>> SearchAsync(TestCaseTableFilter filter)
        {
            var testcases = _context.test_cases.AsNoTracking()
                .Select(e => _mapper.Map<TestCaseTableView>(e))
                .AsQueryable().GetQueryable(filter);
            return new PagedResponse<TestCaseTableView>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                total_items = await testcases.CountAsync(),
                Items = await testcases.Paged(filter.page_index, filter.page_size).ToListAsync()
            };
        }
        public async Task<bool> UpdateAsync(string userId, TestCaseUpdating form)
        {
            var test_case = await _context.test_cases.FirstOrDefaultAsync(e => e.id == form.id);
            if (test_case == null)
            {
                throw new NotFoundException(ErrorMessage.NotFound);
            }
            TestCaseServiceExtension.UpdateTestCase(test_case, form, userId, _context, _mapper);
            _context.Update(test_case);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateManyAsync(string userId, List<TestCaseUpdating> forms)
        {
            if (forms == null || !forms.Any())
            {
                throw new BadException("No testcases provided for update.");
            }

            var testcaseIds = forms.Select(f => f.id).ToList();

            var testcases = await _context.test_cases
                .Where(tc => testcaseIds.Contains(tc.id))
                .ToListAsync();

            if (testcases.Count != forms.Count)
            {
                throw new NotFoundException("One or more testcases not found.");
            }

            var problemId = forms.First().problem_id;
            var testcaseCodes = forms.Select(f => f.code).ToList();

            var existingCodesInForm = testcaseCodes
                .GroupBy(code => code)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            var existingCodesInDB = await _context.test_cases
                .AsNoTracking()
                .Where(p => p.problem_id == problemId && testcaseCodes.Contains(p.code) && !testcaseIds.Contains(p.id))
                .Select(p => p.code)
                .ToListAsync();

            if (existingCodesInDB.Any() || existingCodesInForm.Any())
            {
                var conflictingCodes = existingCodesInDB.Concat(existingCodesInForm).Distinct();
                throw new BadException($"Testcase code already exists: {string.Join(", ", conflictingCodes)}");
            }

            foreach (var form in forms)
            {
                var testcase = testcases.First(p => p.id == form.id);
                TestCaseServiceExtension.UpdateTestCase(testcase, form, userId, _context, _mapper);
                _context.Update(testcase);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<TestCaseCreating>> ImportExcel(IFormFile file)
        {
            var testCases = new List<TestCaseCreating>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, true))
                {
                    var grouped = archive.Entries
                        .Where(e => e.FullName.EndsWith(".inp") || e.FullName.EndsWith(".out"))
                        .GroupBy(e =>
                        {
                            // Extract folder name as code (e.g., "testcase_1" from "Testcases/testcase_1/1.inp")
                            var parts = e.FullName.Split('/');
                            return parts.Length >= 2 ? parts[1] : null;
                        });

                    foreach (var group in grouped)
                    {
                        if (group.Key == null) continue;

                        var inpEntry = group.FirstOrDefault(e => e.FullName.EndsWith(".inp"));
                        var outEntry = group.FirstOrDefault(e => e.FullName.EndsWith(".out"));

                        if (inpEntry == null || outEntry == null) continue;

                        string input = "";
                        string output = "";

                        using (var reader = new StreamReader(inpEntry.Open()))
                        {
                            input = await reader.ReadToEndAsync();
                        }

                        using (var reader = new StreamReader(outEntry.Open()))
                        {
                            output = await reader.ReadToEndAsync();
                        }

                        var testCase = new TestCaseCreating
                        {
                            code = group.Key,
                            input = input.Trim(),
                            output = output.Trim(),
                            score = 0,
                            testcase_type = 0,
                            order = 0
                        };
                        if(!testCases.Any(e => e.code ==  testCase.code)) testCases.Add(testCase);
                    }
                }
            }

            return testCases;
        }
    }

    public static class TestCaseServiceExtension
    {
        public static void Validate(TestCaseCreating form)
        {
        }
        public static void UpdateTestCase(test_case test_case, TestCaseUpdating form, string userId, SACA_Context _context, IMapper _mapper)
        {
            _mapper.Map(form, test_case);
        }
        public static IQueryable<TestCaseTableView> GetQueryable(this IQueryable<TestCaseTableView> query, TestCaseTableFilter filter)
        {
            return query;
        }
    }
}
