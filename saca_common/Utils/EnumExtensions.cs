using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Common.Utils
{
    public static class EnumExtensions
    {
        public static IEnumerable<SelectListItem> GetEnumSelectListInOrder<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .Select(e => new
                {
                    EnumValue = e,
                    Order = GetSortOrder(e),
                    Description = GetDescription(e)
                })
                .OrderBy(x => x.Order)
                .Select(x => new SelectListItem
                {
                    Value = Convert.ToInt32(x.EnumValue).ToString(),
                    Text = x.Description
                });
        }

        private static double GetSortOrder<TEnum>(TEnum value)
        {
            var field = typeof(TEnum).GetField(value.ToString());
            var attr = field.GetCustomAttribute<SortOrderAttribute>();
            return attr?.Order ?? double.MaxValue; // mặc định cuối danh sách nếu không có
        }
        public static string GetDescription(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString() ?? "")!;
            var attribute = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute))!;
            return attribute?.Description ?? value.ToString();
        }
    }
}
