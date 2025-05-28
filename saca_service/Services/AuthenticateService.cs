using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SACA_Common.DTOs.Authenticate.Request;
using SACA_Common.DTOs.Authenticate.Response;
using SACA_Common.Enums;
using SACA_Common.Exceptions;
using SACA_Common.Utils;
using SACA_Common.Validations;
using SACA_Infra.Const;
using SACA_Infra.Context;
using SACA_Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Service.Services
{
    public interface IAuthenticateService
    {
        Task<LoginResponse> AuthencateAsync(LoginRequest form);
        Task<bool> ChangePasswordAsync(ChangePasswordRequest form, string userId);
    }
    public class AuthenticateService : IAuthenticateService
    {
        private readonly SACA_Context _context;
        private readonly IMapper _mapper;
        public AuthenticateService
        (
            SACA_Context context,
            IMapper mapper
        )
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<LoginResponse> AuthencateAsync(LoginRequest form)
        {
            var account = await _context.sys_accounts
                                        .Include(e => e.sys_role)
                                        .FirstOrDefaultAsync(a => a.username.Equals(form.username)) 
                                        ?? throw new BadException(ErrorMessage.LoginFail);
            var correctPassword = HashingHelper.VerifyPassword(form.password, account.password, account.password_salt);
            if (!correctPassword)
            {
                account.failed_count++;
                _context.sys_accounts.Update(account);
                await _context.SaveChangesAsync();
                throw new BadException(ErrorMessage.LoginFail);
            }
            if (account.status != (int)eStatus_Account.Active)
            {
                throw new BadException(ErrorMessage.LoginFail);
            }
            account.last_login = DateTime.Now;
            _context.sys_accounts.Update(account);
            await _context.SaveChangesAsync();
            //Generate refresh token
            var accessToken = JwtHelper.RenderAccessToken(account);
            var response = new LoginResponse
            {
                email = account.email,
                user_id = account.id,
                username = account.username,
                token = accessToken,
                fullname = account.fullname,
                role = account.sys_role.name
            };
            //Set token to cache
            return response;
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordRequest form, string userId)
        {
            var account = await _context.sys_accounts.FirstOrDefaultAsync(e => e.id == userId);
            if(account == null)
            {
                throw new BadException(ErrorMessage.WrongPassword);
            }
            if(form.new_password != form.re_password)
            {
                throw new BadException(ErrorMessage.RePasswordDontMatch);
            }
            if(!HashingHelper.VerifyPassword(form.old_password, account.password, account.password_salt))
            {
                throw new BadException(ErrorMessage.WrongPassword);
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
    }
    public static class AuthenServiceExtension
    {

        public static bool ValidatePassword(string passwordText)
        {
            var validator = new PasswordValidator
            {
                MinLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonLetterOrDigit = true,
                RequireUppercase = true
            };
            return validator.Validate(passwordText);
        }
    }
}
