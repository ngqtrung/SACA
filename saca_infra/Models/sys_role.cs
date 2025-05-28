using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace SACA_Infra.Models
{
    [Table("sys_role")]
    [Comment("phan quyen")]
    public class sys_role : sys_role_properties
    {
        public ICollection<sys_account> sys_accounts { get; set; } = new List<sys_account>();
    }
    public class sys_role_properties : ExtendModel
    {
        public string name { get; set; } = null!;
        public string? description { get; set; }
    }
    public class sys_role_configuration : IEntityTypeConfiguration<sys_role>
    {
        public void Configure(EntityTypeBuilder<sys_role> builder)
        {
        }
    }
}
