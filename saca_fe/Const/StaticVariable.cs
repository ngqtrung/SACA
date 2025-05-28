using SACA_Common.Configurations;
using SACA_FE.DTOs.Config;

namespace SACA_FE.Const
{
    public static class StaticVariable
    {
        public static APIInfo APIInfo { get; set; } = AppSettings.Get<APIInfo>("APIInfo");
    }
}
