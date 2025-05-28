using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SACA_Infra.Const
{
    public static class ErrorMessage
    {
        public const string UnAuthorize = "Bạn không có quyền thực hiện thao tác";
        public const string ContestHasEnded = "Cuộc thi đã kết thúc!";
        public const string LoginFail = "Tài khoản hoặc mật khẩu không chính xác";
        public const string WrongPassword = "Sai mật khẩu";
        public const string RePasswordDontMatch = "Mật khẩu mới không khớp";
        public const string NotFound = "Không tìm thấy dữ liệu";
        public const string InvalidPassword = "Mật khẩu không hợp lệ";
        //Contest
        public const string DuplicateContestCode = "Mã cuộc thi đã tồn tại";
        public const string InvalidModel = "Invalid model: Missing required fields.";

        //Problem
        public const string DuplicateProblemCode = "Mã bài toán đã tồn tại";
        public const string ContestIdIsRequired = "Id cuộc thi là bắt buộc";

        //TestCase
        public const string DuplicateTestCaseCode = "Mã test case đã tồn tại";
        public const string DuplicateEmail = "Tài khoản đã tồn tại";

        //Notification
        public const string TitleIsRequired = "Tiêu đề thông báo là bắt buộc";
        public const string DescriptionIsRequired = "Nội dung thông báo là bắt buộc";
        public const string ContestIsRequired = "Cuộc thi là bắt buộc";
        public const string ProblemIsRequired = "Vấn đề là bắt buộc";
        public const string NotificationNotFound = "Không tìm thấy thông báo";
        
        //Leader board
    }
}
