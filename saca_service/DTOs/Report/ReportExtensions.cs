using Aspose.Cells;
using eArchive.Report;

namespace SACA_Service.DTO.Report
{
    public static class ReportExtensions
    {
        public static string DiaDanh { get; set; } = "Hà Nội";
        public static byte[] ExportReport<T>(this ExcelReport<T> report, T filter, string extension = "pdf")
        {
            var reportDocument = report.Build(filter, false);
            byte[] fileBytes;
            using (var stream = new MemoryStream())
            {
                report.Export(reportDocument, stream, extension);
                fileBytes = stream.ToArray();
            }
            return fileBytes;
        }
        public static byte[] ExportReport<T>(this WordReport<T> report, T filter, string extension = "pdf")
        {
            var reportDocument = report.Build(filter, false);

            byte[] fileBytes;
            using (var stream = new MemoryStream())
            {
                report.Export(reportDocument, stream, extension);
                //((Aspose.Words.Document)reportDocument).Save(stream, Aspose.Words.SaveFormat.Pdf);
                fileBytes = stream.ToArray();
            }
            return fileBytes;
        }

        public static Workbook BuildWorkbook<T>(this ExcelReport<T> report, T filter, string extension = "xlsx")
        {
            var reportDocument = report.Build(filter, false);
            using (var stream = new MemoryStream())
            {
                report.Export(reportDocument, stream, extension);
                return new Workbook(stream);
            }
        }

    }
}
