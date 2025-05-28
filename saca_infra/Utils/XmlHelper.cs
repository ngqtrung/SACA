using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using SACA_Common.DTOs.SysSetting.Request;
using SACA_Common.DTOs.SysSetting.Response;

namespace SACA_Infra.Utils
{
    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlElement("Setting")]
        public List<SysSettingView> SettingList { get; set; } = new List<SysSettingView>();
    }

    public class XmlHelper
    {
        // Đường dẫn file cố định tại AppContext.BaseDirectory
        private static readonly string DefaultFilePath = FindSettingsXmlPath();
        private readonly string _filePath;
        private readonly ILogger<XmlHelper> _logger;

        private static string FindSettingsXmlPath()
        {
            // Giả sử file Settings.xml nằm tại thư mục gốc của ứng dụng.
            return Path.Combine(AppContext.BaseDirectory, "Settings.xml");
        }

        public XmlHelper(ILogger<XmlHelper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _filePath = DefaultFilePath;
            LogFilePath();
            EnsureFileExists();
            LogFileContent(); // Log nội dung file để debug
        }

        private void LogFilePath()
        {
            _logger.LogInformation($"Đường dẫn file Settings.xml: {_filePath}");
        }

        /// <summary>
        /// Kiểm tra sự tồn tại của file XML. Nếu không tồn tại, ném exception.
        /// </summary>
        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogError($"Không tìm thấy file cấu hình tại {_filePath}.");
                throw new FileNotFoundException($"File cấu hình không tồn tại tại {_filePath}.");
            }
        }

        /// <summary>
        /// Log nội dung file XML để kiểm tra dữ liệu đang được load.
        /// </summary>
        private void LogFileContent()
        {
            try
            {
                string content = File.ReadAllText(_filePath);
                _logger.LogInformation($"Nội dung file Settings.xml:\n{content}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đọc nội dung file XML.");
            }
        }

        /// <summary>
        /// Load dữ liệu từ file Settings.xml.
        /// </summary>
        public Settings LoadSettings()
        {
            try
            {
                using (var reader = new StreamReader(_filePath))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    var settings = (Settings)serializer.Deserialize(reader);
                    return settings ?? new Settings();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi đọc file XML tại {_filePath}: {ex.Message}");
                return new Settings();
            }
        }

        /// <summary>
        /// Lưu dữ liệu cấu hình xuống file Settings.xml.
        /// </summary>
        private void SaveSettings(Settings settings)
        {
            try
            {
                var xmlNamespaces = new XmlSerializerNamespaces();
                xmlNamespaces.Add("", "");

                using (var writer = new StreamWriter(_filePath))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writer, settings, xmlNamespaces);
                }
                _logger.LogInformation($"Lưu cấu hình thành công tại {_filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi ghi file XML tại {_filePath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy toàn bộ danh sách cài đặt từ file XML.
        /// </summary>
        public List<SysSettingView> GetAllSettings()
        {
            var settings = LoadSettings();
            _logger.LogInformation($"Số lượng cài đặt được tải: {settings.SettingList.Count}");
            return settings.SettingList;
        }

        /// <summary>
        /// Cập nhật hoặc thêm mới một cài đặt vào file XML.
        /// </summary>
        public bool UpdateSetting(string userId, SysSettingUpdate request)
        {
            if (string.IsNullOrEmpty(request.key) || string.IsNullOrEmpty(request.value))
                throw new ArgumentException("Key và Value không được để trống.");

            var settings = LoadSettings();
            var existingSetting = settings.SettingList.FirstOrDefault(s => s.key == request.key);

            if (existingSetting != null)
            {
                existingSetting.value = request.value;
            }
            else
            {
                settings.SettingList.Add(new SysSettingView
                {
                    key = request.key,
                    value = request.value
                });
            }

            SaveSettings(settings);
            _logger.LogInformation($"Người dùng {userId} đã cập nhật cài đặt: {request.key}");
            return true;
        }
    }
}
