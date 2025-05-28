using eArchive.Report;
using eArchive.Report.Common;
using eArchive.Report.Metadata;
using FlexCel.Report;
using SACA_Service.DTO.Report;

namespace TaxNet_Report.Report.Sale
{
    public class ReportFilter_BangDiem_ScoreBoard : ReportFilterBase
    {
        public string? contest_id { get; set; }
        public string? class_code { get; set; }
        public string? subject_code { get; set; }
        public string? keyword { get; set; }
    }
    public class BangDiem_ScoreBoard
    {
        public int stt { get; set; }
        public string fullname { get; set; } = null!;
        public string email { get; set; } = null!;
        public string? roll_number { get; set; }
        public double score { get; set; }
    }

    [ExcelReport(ReportName = "BangDiem"
         , ID = "E94490A3-2819-4751-B34D-9DB75AC99CD6"
         , ReportExt = "xlsx"
         , Title = "BẢNG ĐIỂM"
    )]
    public class ScoreBoard<T> : ExcelReport<T> where T : ReportFilter_BangDiem_ScoreBoard
    {
        public string template { get; set; } = null!;
        public List<BangDiem_ScoreBoard> details { get; set; } = new List<BangDiem_ScoreBoard>();
        public ScoreBoard
        (
            string template,
            List<BangDiem_ScoreBoard> details
        )
        {
            this.template = template;
            this.details = details;
        }
        protected override void SetReportLocation(string tempFile)
        {
            base.SetReportLocation(template);
        }
        protected override bool OnLoad(FlexCelReport flexcelReport, T filter)
        {
            flexcelReport.AddTable("details", details);
            return true;
        }
    }
}
