using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SACA_Common.DTOs.SysSetting
{
    public class SysSettingBase
    {
        [XmlAttribute("key"), Required]
        public string key { get; set; } = null!;

        [XmlAttribute("value"), Required]
        public string value { get; set; } = null!;
    }
}
