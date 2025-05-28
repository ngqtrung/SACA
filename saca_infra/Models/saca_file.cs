
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SACA_Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Models
{
    [Table(nameof(saca_file))]
    public class saca_file : saca_file_properties
    {
    }
    public class saca_file_properties : ExtendModel
    {
        public string? parent_id { get; set; }

        [Required(ErrorMessage = "FileName không được trống")]
        public string name { get; set; } = null!;

        public string extension { get; set; } = null!;

        [Required(ErrorMessage = "FilePath không được trống")]
        public string path { get; set; } = null!;

        public long length { get; set; }
    }
    public class saca_file_configuration : IEntityTypeConfiguration<saca_file>
    {
        public void Configure(EntityTypeBuilder<saca_file> builder)
        {
        }
    }
}
