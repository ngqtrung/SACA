using SACA_Common.DTOs;
using SACA_Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SACA_Common.DTOs.Contest.Request
{
    public class ContestTableFilter : Paging
    {
        public string? keyword { get; set; }
        [EnumDataType(typeof(eStatus_Contest))]
        public int? status { get; set; }
    }
}
