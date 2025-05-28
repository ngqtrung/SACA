using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Utils
{
    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            var result = value.ToString();
            var field = value.GetType().GetField(value.ToString());
            if (field != null)
            {
                var attribute = field.GetCustomAttribute<DescriptionAttribute>();
                if (attribute != null)
                {
                    result = attribute.Description;
                }
                else
                {
                    var commentAttribute = field.GetCustomAttribute<CommentAttribute>();
                    if (commentAttribute != null)
                    {
                        result = commentAttribute.Comment;
                    }
                }
            }
            return result;
        }
    }
}
