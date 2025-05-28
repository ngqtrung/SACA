using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SACA_Common.Enums;
using SACA_Common.Utils;
using SACA_Infra.Context;
using SACA_Infra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SACA_Infra.SeedData
{
    public static class SeedData
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var _context = new SACA_Context(serviceProvider.GetRequiredService<DbContextOptions<SACA_Context>>()))
            {
                if (!_context.sys_roles.Any())
                {
                    _context.sys_roles.AddRange(new List<sys_role>
                    {
                        new sys_role
                        {
                            name = "Lecturer",
                            description = "Lecturer",
                            created_on = DateTime.Now,
                            created_by = "System",
                        },
                        new sys_role
                        {
                            name = "Student",
                            description = "Student",
                            created_on = DateTime.Now,
                            created_by = "System",
                        }
                    });
                    _context.SaveChanges();
                }
                if (!_context.sys_accounts.Any())
                {
                    var lecRole = _context.sys_roles.First(e => e.name == "Lecturer");
                    var salt = HashingHelper.GenerateSalt();
                    _context.sys_accounts.Add(new sys_account
                    {
                        email = "hunglmhe171165@fpt.edu.vn",
                        fullname = "Lecture",
                        username = "lecture",
                        status = (int)eStatus_Account.Active,
                        password_salt = salt,
                        password = HashingHelper.HashPassword("123456aA@", salt),
                        role_id = lecRole.id,
                        created_on = DateTime.Now,
                        created_by = "System"
                    });
                    _context.SaveChanges();
                }
            }
        }
    }
}
