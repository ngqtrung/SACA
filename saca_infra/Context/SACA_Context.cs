using Microsoft.EntityFrameworkCore;
using SACA_Common.Models;
using SACA_Infra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Context
{
    public class SACA_Context : DbContext
    {
        public SACA_Context(DbContextOptions<SACA_Context> options) : base(options)
        {
        }
        public DbSet<sys_role> sys_roles { get; set; } = null!;
        public DbSet<sys_account> sys_accounts { get; set; } = null!;
        public DbSet<contest> contests { get; set; } = null!;
        public DbSet<problem> problems { get; set; } = null!;
        public DbSet<test_case> test_cases { get; set; } = null!;
        public DbSet<contest_participant> contest_participants { get; set; } = null!;
        public DbSet<problem_submission> problem_submissions { get; set; } = null!;
        public DbSet<notification> notifications { get; set; } = null!;
        public DbSet<account_notification> account_notifications { get; set; } = null!;
        public DbSet<saca_file> saca_files { get; set; } = null!;
        public DbSet<submission_grading> submission_gradings { get; set; } = null!;
        public DbSet<best_submission> best_submissions { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new sys_role_configuration());
            modelBuilder.ApplyConfiguration(new sys_account_configuration());
            modelBuilder.ApplyConfiguration(new contest_configuration());
            modelBuilder.ApplyConfiguration(new problem_configuration());
            modelBuilder.ApplyConfiguration(new test_case_configuration());
            modelBuilder.ApplyConfiguration(new contest_participant_configuration());
            modelBuilder.ApplyConfiguration(new problem_submission_configuration());
            modelBuilder.ApplyConfiguration(new notification_configuration());
            modelBuilder.ApplyConfiguration(new account_notification_configuration());
            modelBuilder.ApplyConfiguration(new saca_file_configuration());
            modelBuilder.ApplyConfiguration(new submission_grading_configuration());
            modelBuilder.ApplyConfiguration(new best_submission_configuration());
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ExtendModel).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, "deleted");
                    var filter = Expression.Lambda(Expression.Equal(property, Expression.Constant(false)), parameter);

                    entityType.SetQueryFilter(filter);
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
