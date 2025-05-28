using SACA_Common.DTOs.Report.ScoreBoard.Request;

namespace SACA_FE.DTOs.Report
{
    public class ExportScoreBoardRequest : GetScoreBoardRequest
    {
        public string Extension { get; set; } = "pdf";
    }
}
