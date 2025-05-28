using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs;
using SACA_Common.DTOs.Account.Request;
using SACA_Common.DTOs.Account.Response;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.Enums;
using SACA_Common.Exceptions;
using SACA_Common.Models;
using SACA_Common.Utils;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Infra.Models;


namespace SACA_Service.Services
{
    public interface IAccountService
    {
        Task<List<AccountView>> GetAllAsync();
        Task<CreateResult> CreateAsync(string userId, AccountCreating form);
        Task<bool> UpdateAsync(string userId, AccountUpdating form);
        Task<bool> DeleteAsync(string userId, string id);
        Task<AccountView> GetDetailAsync(string id);
        Task<AddManyResponse> AddManyAsync(string userId, AccountAddMany accounts);
        Task<bool> UpdateManyAsync(string userId, List<AccountUpdating> forms);
        Task<PagedResponse<AccountView>> SearchAsync(AccountTableFilter filter);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest form, string accountId);
        Task<PagedResponse<AccountView>> SearchContestMemberAsync(MemberTableFilter filter);
        Task<List<AccountCreating>> ImportExcel(IFormFile file);
    }

    public class AccountService : IAccountService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        private static readonly Dictionary<string, List<AccountAddMany>> _pendingAccounts = new();


        public AccountService(SACA_Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AccountView>> GetAllAsync()
        {
            var accounts = await _context.sys_accounts.AsNoTracking()
                .Select(e => _mapper.Map<AccountView>(e))
                .ToListAsync();
            return accounts;
        }
        public async Task<PagedResponse<AccountView>> SearchAsync(AccountTableFilter filter)
        {
            var accounts = _context.sys_accounts
                .Include(a => a.sys_role)
                .Where(e => filter.keyword == null ||
                e.fullname.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                e.username.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                e.email.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()))
                .Where(e => filter.status == null || e.status == filter.status)
                .OrderByDescending(e => e.created_on)
                .AsNoTracking()
                .Select(e => new AccountView
                {
                    id = e.id,
                    fullname = e.fullname,
                    email = e.email,
                    username = e.username,
                    status = e.status,
                    last_login = e.last_login,
                    role_name = e.sys_role.name
                });

            return new PagedResponse<AccountView>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                Items = await accounts.Paged(filter.page_index, filter.page_size).ToListAsync(),
                total_items = await accounts.CountAsync()
            };
        }
        public async Task<PagedResponse<AccountView>> SearchContestMemberAsync(MemberTableFilter filter)
        {
            var accounts = _context.contest_participants
                .Include(c => c.account)
                    .ThenInclude(a => a.sys_role)
                .Where(c => filter.contest_id == null || c.contest_id == filter.contest_id)
                .Where(c => filter.keyword == null ||
                       c.account.fullname.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                       c.account.username.ToLower().Trim().Contains(filter.keyword.ToLower().Trim()) ||
                       c.account.email.ToLower().Trim().Contains(filter.keyword.ToLower().Trim())
                )
                .OrderByDescending(e => e.created_on)
                .AsNoTracking()
                .Select(c => new AccountView
                {
                    id = c.account.id,
                    fullname = c.account.fullname,
                    email = c.account.email,
                    username = c.account.username,
                    status = c.account.status,
                    last_login = c.account.last_login,
                    role_name = c.account.sys_role.name
                });

            return new PagedResponse<AccountView>
            {
                page_index = filter.page_index,
                page_size = filter.page_size,
                Items = await accounts.Paged(filter.page_index, filter.page_size).ToListAsync(),
                total_items = await accounts.CountAsync()
            };
        }
        public async Task<AddManyResponse> AddManyAsync(string userId, AccountAddMany request)
        {
            if (request == null || request.accounts == null || !request.accounts.Any())
            {
                throw new ArgumentException("Account list cannot be empty.");
            }

            var accountEntities = _mapper.Map<List<sys_account>>(request.accounts);

            //Email được phép trùng lặp, miễn là không nằm trong cùng một cuộc thi. Thế nên không check ở đây được, phải check ở function mà nhạn được tất cả account của cuộc thi đấy
            //var emails = accountEntities.Select(a => a.email).ToList();

            //var existingEmails = await _context.sys_accounts
            //    .Where(a => emails.Contains(a.email))
            //    .Select(a => a.email)
            //    .ToListAsync();

            //if (existingEmails.Any())
            //{
            //    throw new BadException($"Emails already exist: {string.Join(", ", existingEmails)}");
            //}

            foreach (var account in accountEntities)
            {
                account.username = AccountServiceExtension.CreateUsername();
                account.password = AccountServiceExtension.CreatePassword();
                account.sys_generated_password = account.password;
                AccountServiceExtension.SeedNewAccount(account);
                account.Created(userId);
            }

            _context.sys_accounts.AddRange(accountEntities);
            await _context.SaveChangesAsync();

            return new AddManyResponse() { ids = accountEntities.Select(acc => acc.id).ToList() };
        }


        public async Task<CreateResult> CreateAsync(string userId, AccountCreating form)
        {
            var existingAccount = await _context.sys_accounts.AsNoTracking()
                .AnyAsync(e => e.email == form.email);
            if (existingAccount)
            {
                throw new BadException("Email already exists.");
            }
            var account = _mapper.Map<sys_account>(form);
            account.username = AccountServiceExtension.CreateUsername();
            account.password = AccountServiceExtension.CreatePassword();
            AccountServiceExtension.SeedNewAccount(account);
            account.Created(userId);
            _context.sys_accounts.Add(account);
            await _context.SaveChangesAsync();
            return new CreateResult(account.id);
        }

        public async Task<bool> DeleteAsync(string userId, string id)
        {
            var account = await _context.sys_accounts.FindAsync(id);
            if (account == null)
            {
                throw new NotFoundException("Account not found.");
            }
            account.Deleted(userId);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<AccountView> GetDetailAsync(string id)
        {
            var Account = await _context.sys_accounts.AsNoTracking()
                .Where(e => e.id == id)
                .Select(e => new AccountView
                {
                    id = e.id,
                    fullname = e.fullname,
                    email = e.email,
                    username = e.username,
                    status = e.status,
                    last_login = e.last_login,
                    role_name = e.sys_role.name,
                    password = e.password
                })
                .FirstOrDefaultAsync();
            if (Account == null)
            {
                throw new NotFoundException("Account not found.");
            }
            return Account;
        }

        public async Task<bool> UpdateAsync(string userId, AccountUpdating form)
        {
            var Account = await _context.sys_accounts.FirstOrDefaultAsync(e => e.id == form.id);
            if (Account == null)
            {
                throw new NotFoundException("Account not found.");
            }
            //var existingAccount = await _context.sys_accounts.AsNoTracking()
            //    .AnyAsync(e => e.email == form.email);
            //if (existingAccount)
            //{
            //    throw new BadException("Email already exists.");
            //}

            AccountServiceExtension.UpdateAccount(Account, form, userId, _context, _mapper);
            _context.Update(Account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateManyAsync(string userId, List<AccountUpdating> forms)
        {
            if (forms == null || !forms.Any())
            {
                throw new BadException("No accounts provided for update.");
            }

            var accountIds = forms.Select(f => f.id).ToList();
            var accounts = await _context.sys_accounts
                .Where(e => accountIds.Contains(e.id))
                .ToListAsync();

            if (accounts.Count != forms.Count)
            {
                throw new NotFoundException("One or more accounts not found.");
            }

            //var emails = forms.Select(f => f.email).ToList();

            //var existingEmailsInForm = emails
            //    .GroupBy(email => email)
            //    .Where(g => g.Count() > 1)
            //    .Select(g => g.Key)
            //    .ToList();

            //var existingEmailsInDB = await _context.sys_accounts
            //    .AsNoTracking()
            //    .Where(e => emails.Contains(e.email) && !accountIds.Contains(e.id))
            //    .Select(e => e.email)
            //    .ToListAsync();

            //if (existingEmailsInDB.Any() || existingEmailsInForm.Any())
            //{
            //    var conflictingEmails = existingEmailsInDB.Concat(existingEmailsInForm).Distinct();
            //    throw new BadException($"Email already exists: {string.Join(", ", conflictingEmails)}");
            //}

            foreach (var form in forms)
            {
                var account = accounts.First(e => e.id == form.id);
                AccountServiceExtension.UpdateAccount(account, form, userId, _context, _mapper);
                _context.Update(account);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest form, string accountId)
        {
            var account = await _context.sys_accounts.FirstOrDefaultAsync(e => e.id == accountId);
            if (account == null)
            {
                throw new BadException(ErrorMessage.NotFound);
            }
            if (form.new_password != form.re_password)
            {
                throw new BadException(ErrorMessage.RePasswordDontMatch);
            }
            if (!AuthenServiceExtension.ValidatePassword(form.new_password))
            {
                throw new BadException(ErrorMessage.InvalidPassword);
            }
            account.password_salt = HashingHelper.GenerateSalt();
            account.password = HashingHelper.HashPassword(form.new_password, account.password_salt);
            _context.sys_accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AccountCreating>> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new BadException("File is empty.");

            var studentList = new List<AccountCreating>();
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var workbook = new Aspose.Cells.Workbook(stream);
                    var worksheet = workbook.Worksheets[0];
                    var cells = worksheet.Cells;

                    int rowCount = cells.MaxDataRow;
                    int colCount = cells.MaxDataColumn;

                    int emailCol = -1;
                    int fullNameCol = -1;
                    int rollNumberCol = -1;

                    // Tìm vị trí cột "Email" và "FullName"
                    for (int col = 0; col <= colCount; col++)
                    {
                        var header = cells[0, col]?.StringValue?.Trim();
                        if (string.Equals(header, "Email", StringComparison.OrdinalIgnoreCase))
                            emailCol = col;
                        else if (string.Equals(header, "FullName", StringComparison.OrdinalIgnoreCase))
                            fullNameCol = col;
                        else if (string.Equals(header, "RollNumber", StringComparison.OrdinalIgnoreCase))
                            rollNumberCol = col;
                    }

                    if (emailCol == -1 || fullNameCol == -1 || rollNumberCol == -1)
                        throw new BadException("Missing required headers: Email or FullName");

                    // Đọc dữ liệu
                    for (int row = 1; row <= rowCount; row++)
                    {
                        string email = cells[row, emailCol]?.StringValue?.Trim() ?? "";
                        string fullName = cells[row, fullNameCol]?.StringValue?.Trim() ?? "";
                        string rollnumber = cells[row, rollNumberCol]?.StringValue?.Trim() ?? "";
                        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(fullName) && !studentList.Any(e => e.email == email))
                        {
                            studentList.Add(new AccountCreating
                            {
                                email = email,
                                fullname = fullName,
                                roll_number = rollnumber
                            });
                        }
                    }
                }
                return studentList;
            }
            catch (Exception ex)
            {
                return new List<AccountCreating>();
            }
        }
    }

    public static class AccountServiceExtension
    {
        public static void UpdateAccount(sys_account Account, AccountUpdating form, string userId, SACA_Context _context, IMapper _mapper)
        {
            string existingUsername = Account.username;
            string existingPassword = Account.password;

            _mapper.Map(form, Account);

            // Restore original values if they were null in AccountUpdating form
            Account.username ??= existingUsername;
            Account.password ??= existingPassword;
        }


        public static string CreateUsername()
        {
            return "saca" + new Random().Next(1000, 9999);
        }

        public static string CreatePassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
        public static void SeedNewAccount(sys_account account)
        {
            account.status = (int)eStatus_Account.Active;
            account.failed_count = 0;
            account.role_id = "74121575-7c1d-4059-b216-0dcd491f98b2"; //TODO: fix this later
            account.password_salt = HashingHelper.GenerateSalt();
            account.password = HashingHelper.HashPassword(account.password, account.password_salt);
        }
    }
}
