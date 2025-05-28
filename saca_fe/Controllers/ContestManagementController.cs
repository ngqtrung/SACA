using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Contest.Request;
using SACA_Common.DTOs.Contest.Response;
using SACA_Common.DTOs.File.Request;
using SACA_Common.DTOs.Problem.Request;
using SACA_Common.DTOs.Problem.Response;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.DTOs.TestCase.Response;
using SACA_FE.Services;
using System.Threading.Tasks;

namespace SACA_FE.Controllers
{
    [Authorize(AuthenticationSchemes = "SACA", Roles = "Lecturer")]
    public class ContestManagementController : Controller
    {
        private readonly IContestService _contestService;
        private readonly IAccountService _accountService;
        private readonly IProblemService _problemService;
        private readonly ITestcaseService _testcaseService;
        private readonly IMailService _mailService;
        private readonly IFileService _fileService;
        private readonly IReportService _reportService;
        public ContestManagementController(IContestService contestService, IMailService mailService, IFileService fileService, IReportService reportService, IAccountService accountService, IProblemService problemService, ITestcaseService testcaseService)
        {
            _contestService = contestService;
            _mailService = mailService;
            _fileService = fileService;
            _reportService = reportService;
            _accountService = accountService;
            _problemService = problemService;
            _testcaseService = testcaseService;
        }
        public async Task<IActionResult> Index(ContestTableFilter request)
        {
            try
            {
                var contests = await _contestService.SearchAsync(request);
                var allContests = await _contestService.GetAllContestAsync();
                ViewBag.AllContests = allContests;
                return View(contests);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index", "Authen");
            }
        }

        private IActionResult ContestFormView(string actionView, ContestFormView model)
        {
            if (actionView != "Create" && actionView != "Edit")
            {
                throw new ArgumentException("Invalid action. Must be 'Create' or 'Edit'.", nameof(actionView));
            }

            ViewData["Action"] = actionView;
            return View("Create", model);
        }

        public IActionResult Create()
        {
            ContestFormView model = new ContestFormView();
            return ContestFormView("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContestFormView model, IFormCollection form)
        {
            List<ProblemFormView>? listProblemFormView = new List<ProblemFormView>();
            List<AccountFormView>? listAccountFormView = new List<AccountFormView>();
            string? problemsData = form["problemsData"];
            string? membersData = form["membersData"];
            string? settingsJson = form["settingsData"];

            if (!string.IsNullOrEmpty(problemsData))
            {
                listProblemFormView = JsonConvert.DeserializeObject<List<ProblemFormView>>(problemsData) ?? new List<ProblemFormView>();
                model.problems.Items = listProblemFormView.Select(MapToProblemTableView).ToList();
            }
            if (!string.IsNullOrEmpty(membersData))
            {
                listAccountFormView = JsonConvert.DeserializeObject<List<AccountFormView>>(membersData) ?? new List<AccountFormView>();
                model.participants.Items = listAccountFormView.Select(MapToAccountTableView).ToList();
            }
            if (!string.IsNullOrEmpty(settingsJson))
            {
                var settings = JsonConvert.DeserializeObject<ContestFormView>(settingsJson);
                model.contest_type = settings.contest_type;
                model.grading_type = settings.grading_type;
                model.leaderboard_enabled = settings.leaderboard_enabled;
                model.plagiarism_detection_enabled = settings.plagiarism_detection_enabled;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contestCreating = new ContestCreating
                    {
                        code = model.code,
                        contest_type = model.contest_type,
                        description = model.description,
                        class_code = model.class_code,
                        duration = model.duration,
                        end_at = model.end_at,
                        grading_type = model.grading_type,
                        leaderboard_enabled = model.leaderboard_enabled,
                        participants = listAccountFormView!.Select(afv => new AccountCreating()
                        {
                            email = afv.email,
                            fullname = afv.fullname,
                            password = null,
                            username = null,
                            roll_number = afv.roll_number,
                            student_code = afv.student_code
                        }).ToList(),
                        plagiarism_detection_enabled = model.plagiarism_detection_enabled,
                        problems = listProblemFormView!.Select(pfv => new ProblemCreating()
                        {
                            code = pfv.code,
                            description = pfv.description,
                            file_id = pfv.file_id,
                            max_attempts = pfv.max_attempts,
                            note = pfv.note,
                            score = pfv.score,
                            tags = pfv.tags,
                            title = pfv.title,
                            test_cases = pfv.test_cases.Select((tc, index) => new TestCaseCreating()
                            {
                                problem_id = pfv.id,
                                code = tc.code,
                                description = tc.description,
                                input = tc.input,
                                output = tc.output,
                                score = tc.score,
                                order = index + 1,
                                testcase_type = tc.testcase_type,
                                execution_time = tc.execution_time,
                                memory_limit = tc.memory_limit
                            }).ToList(),
                            default_execution_time = pfv.default_execution_time,
                            default_memory_limit = pfv.default_memory_limit,
                        }).ToList(),
                        start_at = model.start_at,
                        subject_code = model.subject_code,
                        title = model.title,
                        programming_languages = model.programming_languages,
                        penalty_time = model.penalty_time,
                        status = model.status
                    };
                    Console.WriteLine("Status: " + model.status);
                    Response<CreateResult> response = await _contestService.CreateAsync(contestCreating);
                    TempData["APIMessage"] = "Contest created successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while creating the contest: " + ex.Message);
                    TempData["APIErrorMessage"] = ex.Message;
                    Console.WriteLine(ex.StackTrace);
                }
            }
            //return RedirectToAction("Index");
            // If we got here, something failed, redisplay form
            ViewData["IsDrafted"] = true;
            return ContestFormView("Create", model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Contest ID is required.");
                }

                var response = await _contestService.GetDetailAsync(id);
                //ContestView? contestView = response.Result;
                ContestFormView model = new ContestFormView();
                if (response != null)
                {
                    model = MapToContestFormView(response);
                }
                ViewBag.ContestId = id;
                return ContestFormView("Edit", model);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContestFormView model, IFormCollection form)
        {
            List<ProblemFormView>? listProblemFormView = new List<ProblemFormView>();
            List<AccountFormView>? listAccountFormView = new List<AccountFormView>();
            string? problemsData = form["problemsData"];
            string? membersData = form["membersData"];
            string? settingsJson = form["settingsData"];

            if (!string.IsNullOrEmpty(problemsData))
            {
                listProblemFormView = JsonConvert.DeserializeObject<List<ProblemFormView>>(problemsData) ?? new List<ProblemFormView>();
                model.problems.Items = listProblemFormView.Select(MapToProblemTableView).ToList();
            }
            if (!string.IsNullOrEmpty(membersData))
            {
                listAccountFormView = JsonConvert.DeserializeObject<List<AccountFormView>>(membersData) ?? new List<AccountFormView>();
                model.participants.Items = listAccountFormView.Select(MapToAccountTableView).ToList();
            }
            if (!string.IsNullOrEmpty(settingsJson))
            {
                var settings = JsonConvert.DeserializeObject<ContestFormView>(settingsJson);
                model.contest_type = settings.contest_type;
                model.grading_type = settings.grading_type;
                model.leaderboard_enabled = settings.leaderboard_enabled;
                model.plagiarism_detection_enabled = settings.plagiarism_detection_enabled;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contestUpdating = new ContestUpdating
                    {
                        id = model.id,
                        class_code = model.class_code,
                        code = model.code,
                        contest_type = model.contest_type,
                        description = model.description,
                        duration = model.duration,
                        end_at = model.end_at,
                        grading_type = model.grading_type,
                        leaderboard_enabled = model.leaderboard_enabled,
                        participants = listAccountFormView!.Select(afv => new AccountUpdating()
                        {
                            id = afv.id,
                            email = afv.email,
                            fullname = afv.fullname,
                            password = null,
                            username = null,
                            roll_number = afv.roll_number,
                            student_code = afv.student_code
                        }).ToList(),
                        plagiarism_detection_enabled = model.plagiarism_detection_enabled,
                        problems = listProblemFormView!.Select(pfv => new ProblemUpdating()
                        {
                            id = pfv.id,
                            contest_id = pfv.contest_id,
                            code = pfv.code,
                            description = pfv.description,
                            file_id = pfv.file_id,
                            max_attempts = pfv.max_attempts,
                            note = pfv.note,
                            score = pfv.score,
                            tags = pfv.tags,
                            title = pfv.title,
                            default_execution_time = pfv.default_execution_time,
                            default_memory_limit = pfv.default_memory_limit,
                            test_cases = pfv.test_cases.Select((tc, index) => new TestCaseUpdating()
                            {
                                id = tc.id,
                                problem_id = pfv.id,
                                code = tc.code,
                                description = tc.description,
                                input = tc.input,
                                output = tc.output,
                                score = tc.score,
                                order = index + 1,
                                testcase_type = tc.testcase_type,
                                execution_time = tc.execution_time,
                                memory_limit = tc.memory_limit
                            }).ToList()
                        }).ToList(),
                        start_at = model.start_at,
                        subject_code = model.subject_code,
                        title = model.title,
                        programming_languages = model.programming_languages,
                        penalty_time = model.penalty_time,
                        status = model.status
                    };
                    Response<bool> response = await _contestService.UpdateAsync(contestUpdating);
                    TempData["APIMessage"] = "Contest updated successfully";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["APIErrorMessage"] = ex.Message;
                    Console.WriteLine(ex.StackTrace);
                }
            }
            ViewData["IsDrafted"] = true;
            return ContestFormView("Edit", model);
        }

        [HttpPost("FrozenContest")]
        public async Task<IActionResult> FrozenContest(string contestId)
        {
            await _contestService.FrozenContestAsync(contestId);
            TempData["APIMessage"] = "Successfully.";
            return RedirectToAction("Index", "Ranking", new { ContestId = contestId });
        }
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Contest ID is required.");
                }
                var response = await _contestService.GetDetailAsync(id);
                ContestFormView model = new ContestFormView();
                if (response != null)
                {
                    model = MapToContestFormView(response);
                }
                ViewBag.ContestId = id;
                return View("Detail", model);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult OpenMembersModal(string modalAction)
        {
            try
            {
                if (!string.IsNullOrEmpty(modalAction) && modalAction == "Edit")
                {
                    ViewData["Action"] = "EditMember";
                }
                else
                {
                    ViewData["Action"] = "CreateMember";
                }
                return PartialView("Modals/_MemberAddOrUpdateModal", new AccountFormView());
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        public IActionResult OpenMembersDetailModal()
        {
            return PartialView("Modals/_MemberDetailModal", new AccountFormView());
        }

        public IActionResult OpenMembersImportModal()
        {
            return PartialView("Modals/_MemberImportModal", new AccountImporting());
        }

        [HttpPost]
        public async Task<IActionResult> ImportMembers(AccountImporting model)
        {
            if (!ModelState.IsValid)
                return ValidateModel(model);

            List<AccountCreating> accountCreatings = new();
            if (model.import_file != null && model.import_file.Length > 0)
            {
                accountCreatings = await _accountService.ImportExcel(model.import_file);
            }

            //TempData["APIMessage"] = "Members imported successfully!";

            return Json(new
            {
                status = "success",
                accounts = accountCreatings
            });
        }

        [HttpPost]
        public IActionResult ValidateMember(AccountFormView model, IFormCollection form)
        {
            string? memberEmails = form["member_emails"];
            string? action = form["action"];
            string? originalEmail = form["original_email"];
            var listMemberEmail = new List<String>();
            if (!string.IsNullOrEmpty(memberEmails))
            {
                listMemberEmail = JsonConvert.DeserializeObject<List<String>>(memberEmails) ?? new List<String>();
            }
            if (!string.IsNullOrEmpty(model.email))
            {
                int count = listMemberEmail.Count(e => e == model.email);
                bool isDuplicate = false;
                if (!string.IsNullOrEmpty(action))
                {
                    if (action == "EditMember" && !string.IsNullOrEmpty(originalEmail) && originalEmail == model.email)
                    {
                        isDuplicate = false;
                    }
                    else
                    {
                        isDuplicate = count > 0;
                    }
                }
                if (isDuplicate)
                {
                    ModelState.AddModelError("email", "Email is not unique.");
                }
            }
            return ValidateModel(model);
        }

        public IActionResult OpenProblemsModal(string modalAction)
        {
            try
            {
                if (!string.IsNullOrEmpty(modalAction) && modalAction == "Edit")
                {
                    ViewData["Action"] = "EditProblem";
                }
                else
                {
                    ViewData["Action"] = "CreateProblem";
                }
                return PartialView("Modals/_ProblemAddOrUpdateModal", new ProblemFormView());
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        public IActionResult OpenProblemsDetailModal()
        {
            return PartialView("Modals/_ProblemDetailModal", new ProblemFormView());
        }

        public IActionResult OpenProblemsImportModal()
        {
            return PartialView("Modals/_ProblemImportModal", new ProblemImporting());
        }

        [HttpPost]
        public async Task<IActionResult> ImportProblems(ProblemImporting model)
        {
            if (!ModelState.IsValid)
                return ValidateModel(model);

            List<ProblemCreating> problemCreatings = new();
            if (model.import_file != null && model.import_file.Length > 0)
            {
                problemCreatings = await _problemService.ImportExcel(model.import_file);
            }

            return Json(new
            {
                status = "success",
                problems = problemCreatings
            });
        }

        [HttpPost]
        public IActionResult ValidateProblem(ProblemFormView model, IFormCollection form)
        {
            string? problemCodes = form["problem_codes"];
            string? action = form["action"]; //either CreateProblem or EditProblem
            string? originalCode = form["original_code"];
            var listProblemCode = new List<String>();
            if (!string.IsNullOrEmpty(problemCodes))
            {
                listProblemCode = JsonConvert.DeserializeObject<List<String>>(problemCodes) ?? new List<String>();
            }
            if (!string.IsNullOrEmpty(model.code))
            {
                int count = listProblemCode.Count(code => code == model.code);
                bool isDuplicate = false;
                if (!string.IsNullOrEmpty(action))
                {
                    if (action == "EditProblem" && !string.IsNullOrEmpty(originalCode) && originalCode == model.code)
                    {
                        isDuplicate = false;
                    }
                    else
                    {
                        isDuplicate = count > 0;
                    }
                }
                if (isDuplicate)
                {
                    ModelState.AddModelError("code", "Code is not unique.");
                }
            }
            return ValidateModel(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided." });

            try
            {
                var file_id = await _fileService.CreateAsync(file);

                return Ok(new { file_id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("file", "Upload file has exception. Check console for error message");
                return StatusCode(500, new { message = "File upload failed.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFile([FromBody] DeleteFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.file_id))
                    return BadRequest(new { message = "file_id is missing." });

                bool result = await _fileService.DeleteAsync(request.file_id);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("file", "Delete file has exception. Check console for error message");
                return StatusCode(500, new { message = "File upload failed.", detail = ex.Message });
            }
        }

        public IActionResult OpenTestcasesModal(string modalAction, double default_execution_time = 0, double default_memory_limit = 0, double score = 0, string problem_action = "CreateProblem")
        {
            try
            {
                if (!string.IsNullOrEmpty(modalAction) && modalAction == "Edit")
                {
                    ViewData["Action"] = "EditTestcase";
                }
                else
                {
                    ViewData["Action"] = "CreateTestcase";
                }
                ViewData["ProblemAction"] = problem_action;
                TestCaseFormView model = new TestCaseFormView()
                {
                    execution_time = default_execution_time,
                    memory_limit = default_memory_limit,
                    score = score
                };
                return PartialView("Modals/_TestcaseAddOrUpdateModal", model);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        public IActionResult OpenTestcasesDetailModal()
        {

            return PartialView("Modals/_TestcaseDetailModal", new TestCaseFormView());
        }

        public IActionResult OpenTestcaseImportModal(double default_execution_time = 0, double default_memory_limit = 0, double score = 0)
        {
            return PartialView("Modals/_TestcaseImportModal", new TestCaseImporting()
            {
                default_execution_time = default_execution_time,
                default_memory_limit = default_memory_limit,
                score = score
            });
        }

        [HttpPost]
        public async Task<IActionResult> ImportTestcases(TestCaseImporting model, double default_execution_time = 0, double default_memory_limit = 0, double score = 0)
        {
            if (!ModelState.IsValid)
                return ValidateModel(model);

            List<TestCaseCreating> testcaseCreatings = new();
            if (model.import_file != null && model.import_file.Length > 0)
            {
                testcaseCreatings = await _testcaseService.ImportExcel(model.import_file);
            }

            int testcaseCount = testcaseCreatings.Count;
            double distributedScore = testcaseCount > 0 ? score / testcaseCount : 0;

            foreach (var testcaseCreating in testcaseCreatings)
            {
                testcaseCreating.execution_time ??= default_execution_time;
                testcaseCreating.memory_limit ??= default_memory_limit;
                testcaseCreating.score = distributedScore;
            }

            return Json(new
            {
                status = "success",
                testcases = testcaseCreatings
            });
        }

        [HttpPost]
        public IActionResult ValidateTestCase(TestCaseFormView model, IFormCollection form)
        {
            string? testcaseCodes = form["testcase_codes"];
            string? action = form["action"];
            string? originalTcCode = form["original_tc_code"];
            var listTestcaseCode = new List<String>();
            if (!string.IsNullOrEmpty(testcaseCodes))
            {
                listTestcaseCode = JsonConvert.DeserializeObject<List<String>>(testcaseCodes) ?? new List<String>();
            }
            if (!string.IsNullOrEmpty(model.code))
            {
                int count = listTestcaseCode.Count(code => code == model.code);
                bool isDuplicate = false;
                if (!string.IsNullOrEmpty(action))
                {
                    if (action == "EditTestcase" && !string.IsNullOrEmpty(originalTcCode) && originalTcCode == model.code)
                    {
                        isDuplicate = false;
                    }
                    else
                    {
                        isDuplicate = count > 0;
                    }
                }
                if (isDuplicate)
                {
                    ModelState.AddModelError("code", "Code is not unique.");
                }
            }
            return ValidateModel(model);
        }

        public IActionResult OpenDeleteModal(string target, string id)
        {
            switch (target)
            {
                case "Member":
                    ViewData["MemberID"] = id;
                    return PartialView("Modals/_MemberDeleteModal");
                case "Problem":
                    ViewData["ProblemID"] = id;
                    return PartialView("Modals/_ProblemDeleteModal");
                case "Testcase":
                    ViewData["TestcaseID"] = id;
                    return PartialView("Modals/_TestcaseDeleteModal");
                default:
                    return BadRequest("Invalid target specified.");
            }
        }

        [HttpPost]
        public IActionResult GetMembersTab([FromBody] List<AccountTableView> accounts, [FromQuery] int page_index = 1, [FromQuery] int page_size = 5, [FromQuery] bool is_drafted = false, [FromQuery] bool is_view_only = false, string? contest_id = null)
        {
            try
            {
                var accountsPaged = accounts.AsQueryable().Paged(page_index, page_size);
                ViewData["IsMembersDrafted"] = is_drafted;
                ViewData["IsViewOnly"] = is_view_only;
                if (contest_id != null) ViewData["ContestId"] = contest_id;
                return PartialView("Tabs/_MembersList", new PagedResponse<AccountTableView>()
                {
                    Items = accountsPaged.ToList(),
                    page_index = page_index,
                    page_size = page_size,
                    total_items = accounts.Count()
                });
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult GetProblemsTab([FromBody] List<ProblemTableView> problems, [FromQuery] int page_index = 1, [FromQuery] int page_size = 5, [FromQuery] bool is_drafted = false, [FromQuery] bool is_view_only = false)
        {
            try
            {
                var problemsPaged = problems.AsQueryable().Paged(page_index, page_size);
                ViewData["IsProblemsDrafted"] = is_drafted;
                ViewData["IsViewOnly"] = is_view_only;
                return PartialView("Tabs/_ProblemsList", new PagedResponse<ProblemTableView>()
                {
                    Items = problemsPaged.ToList(),
                    page_index = page_index,
                    page_size = 5,
                    total_items = problems.Count()
                });
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult GetTestcasesTab([FromBody] List<TestCaseTableView> testcases, [FromQuery] int page_index = 1, [FromQuery] int page_size = 5, [FromQuery] bool is_drafted = false, [FromQuery] bool is_view_only = false)
        {
            try
            {
                var testcasesPaged = testcases.AsQueryable().Paged(page_index, page_size);
                ViewData["IsTestcasesDrafted"] = is_drafted;
                ViewData["IsViewOnly"] = is_view_only;
                return PartialView("Tabs/_TestcasesList", new PagedResponse<TestCaseTableView>()
                {
                    Items = testcasesPaged.ToList(),
                    page_index = page_index,
                    page_size = 5,
                    total_items = testcases.Count()
                });
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
        private IActionResult ValidateModel<T>(T model) where T : class
        {
            if (ModelState.IsValid)
            {
                return Json(new { status = "success", data = model });
            }

            return Json(new
            {
                status = "failure",
                formErrors = ModelState
                    .Where(kvp => kvp.Value.Errors.Any())
                    .Select(kvp => new { key = kvp.Key, errors = kvp.Value.Errors.Select(e => e.ErrorMessage) })
            });
        }
        [HttpPost]
        public async Task<IActionResult> SendMailInvite(string contestId, string account_ids)
        {
            try
            {
                List<string> parse_account_ids = account_ids?.Split(',').ToList() ?? new List<string>();
                await _mailService.SendMailAsync(new SACA_Common.DTOs.Mail.Request.SendMailInvite
                {
                    contest_id = contestId,
                    account_ids = parse_account_ids
                });
                TempData["APIMessage"] = "Gửi thư mời thành công!";
                return RedirectToAction("Detail", new { id = contestId });
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Detail", new { id = contestId });
            }
        }

        private ContestFormView MapToContestFormView(ContestView src)
        {
            return new ContestFormView()
            {
                id = src.id,
                code = src.code,
                title = src.title,
                description = src.description,
                start_at = src.start_at,
                end_at = src.end_at,
                subject_code = src.subject_code,
                duration = src.duration,
                class_code = src.class_code,
                contest_type = src.contest_type,
                grading_type = src.grading_type,
                leaderboard_enabled = src.leaderboard_enabled,
                plagiarism_detection_enabled = src.plagiarism_detection_enabled,
                programming_languages = src.programming_languages,
                penalty_time = src.penalty_time,
                status = src.status,
                problems = new PagedResponse<ProblemTableView>()
                {
                    Items = src.problems.Select(p => MapToProblemTableView(p)).ToList(),
                    page_index = 1,
                    page_size = 5,
                    total_items = src.problems.Count()
                },
                participants = new PagedResponse<AccountTableView>()
                {
                    Items = src.participants.Select(p => MapToAccountTableView(p)).ToList(),
                    page_index = 1,
                    page_size = 5,
                    total_items = src.participants.Count()
                }
            };
        }

        private ProblemTableView MapToProblemTableView(ProblemView src)
        {
            return new ProblemTableView()
            {
                id = src.id,
                code = src.code,
                title = src.title,
                description = src.description,
                tags = src.tags,
                score = src.score,
                max_attempts = src.max_attempts ?? 0,
                contest_id = src.contest_id,
                note = src.note,
                test_cases = src.test_cases,
                default_execution_time = src.default_execution_time,
                default_memory_limit = src.default_memory_limit,
                file_id = src.file_id,
                file = src.file
            };
        }

        private ProblemTableView MapToProblemTableView(ProblemFormView src)
        {
            return new ProblemTableView()
            {
                id = src.id,
                code = src.code,
                title = src.title,
                description = src.description,
                tags = src.tags,
                score = src.score,
                max_attempts = src.max_attempts ?? 0,
                contest_id = src.contest_id,
                note = src.note,
                test_cases = src.test_cases,
                default_execution_time = src.default_execution_time,
                default_memory_limit = src.default_memory_limit
            };
        }

        private AccountTableView MapToAccountTableView(AccountView src)
        {
            return new AccountTableView()
            {
                id = src.id,
                status = src.status,
                username = src.username,
                email = src.email,
                fullname = src.fullname,
                last_login = src.last_login,
                password = src.password,
                roll_number = src.roll_number,
                student_code = src.student_code,
                invitation_email_sent = src.invitation_email_sent
            };
        }

        private AccountTableView MapToAccountTableView(AccountFormView src)
        {
            return new AccountTableView()
            {
                id = src.id,
                status = null,
                username = src.username ?? "",
                email = src.email,
                fullname = src.fullname,
                last_login = null,
                password = src.password ?? "",
                roll_number = src.roll_number,
                student_code = src.student_code
            };
        }

        [HttpGet("ContestManagement/Delete")]
        public async Task<IActionResult> Delete([FromQuery] string id)
        {
            try
            {
                var response = await _contestService.DeleteAsync(id);
                var currentQueries = HttpContext.Request.Query
                   .Where(q => q.Key != "id")
                   .ToDictionary(q => q.Key, q => q.Value.ToString());
                TempData["APIMessage"] = "Contest deleted successfully";
                return RedirectToAction("Index", currentQueries);
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpPost("Export")]
        public async Task<IActionResult> Export(ExportContestRequest form)
        {
            try
            {
                if (form.contest_ids == null || !form.contest_ids.Any())
                {
                    // Không có contest nào được chọn, trả về thông báo lỗi hoặc trạng thái để đóng modal
                    TempData["APIErrorMessage"] = "No contests selected";
                    return RedirectToAction("Index");
                }
                var file = await _reportService.ExportContests(form);

                return File(file.OpenReadStream(), file.ContentType, file.FileName);

            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportContestParticipants(string contestId)
        {
            try
            {
                var file = await _reportService.ExportContestParticipants(contestId);
                return File(file.OpenReadStream(), file.ContentType, $"{file.FileName}.xlsx");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportContestProblems(string contestId)
        {
            try
            {
                var file = await _reportService.ExportContestProblems(contestId);
                return File(file.OpenReadStream(), file.ContentType, $"{file.FileName}.xlsx");
            }
            catch (Exception ex)
            {
                TempData["APIErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult OpenContestmportModal()
        {
            return PartialView("Modals/_ContestImportModal", new ContestImporting());
        }

        [HttpPost]
        public async Task<IActionResult> ImportContest(ContestImporting model)
        {
            if (!ModelState.IsValid)
                return ValidateModel(model);

            ContestCreating contestCreating = new();
            if (model.import_file != null && model.import_file.Length > 0)
            {
                contestCreating = await _contestService.ImportExcel(model.import_file);
            }

            TempData["APIMessage"] = "Contest imported successfully!";

            return ContestFormView("Create", new ContestFormView()
            {
                code = contestCreating.code,
                title = contestCreating.title,
                description = contestCreating.description,
                start_at = contestCreating.start_at,
                end_at = contestCreating.end_at,
                subject_code = contestCreating.subject_code,
                duration = contestCreating.duration,
                class_code = contestCreating.class_code,
                contest_type = contestCreating.contest_type,
                grading_type = contestCreating.grading_type,
                leaderboard_enabled = contestCreating.leaderboard_enabled,
                plagiarism_detection_enabled = contestCreating.plagiarism_detection_enabled,
                programming_languages = contestCreating.programming_languages,
                penalty_time = contestCreating.penalty_time,
                status = contestCreating.status,
                problems = new PagedResponse<ProblemTableView>()
                {
                    Items = contestCreating.problems.Select(pc => new ProblemTableView()
                    {
                        code = pc.code,
                        title = pc.title,
                        description = pc.description,
                        tags = pc.tags,
                        score = pc.score,
                        max_attempts = pc.max_attempts ?? 0,
                        contest_id = pc.contest_id,
                        note = pc.note,
                        test_cases = pc.test_cases.Select(tcc => new TestCaseTableView()
                        {
                            code = tcc.code,
                            problem_id = tcc.problem_id,
                            description = tcc.description,
                            input = tcc.input,
                            output = tcc.output,
                            execution_time = tcc.execution_time,
                            memory_limit = tcc.memory_limit,
                            order = tcc.order,
                            score = tcc.score,
                            testcase_type = tcc.testcase_type
                        }).ToList(),
                        default_execution_time = pc.default_execution_time,
                        default_memory_limit = pc.default_memory_limit,
                        file_id = pc.file_id,
                    }).ToList(),
                    page_index = 1,
                    page_size = 5,
                    total_items = contestCreating.problems.Count()
                },
                participants = new PagedResponse<AccountTableView>()
                {
                    Items = contestCreating.participants.Select(pc => new AccountTableView()
                    {
                        status = null,
                        username = pc.username,
                        email = pc.email,
                        fullname = pc.fullname,
                        last_login = null,
                        password = pc.password,
                        roll_number = pc.roll_number,
                        student_code = pc.student_code,
                        invitation_email_sent = false
                    }).ToList(),
                    page_index = 1,
                    page_size = 5,
                    total_items = contestCreating.participants.Count()
                }
            });
        }
    }
}
