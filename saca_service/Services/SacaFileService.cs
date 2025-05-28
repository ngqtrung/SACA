using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SACA_Common.DTOs;
using SACA_Common.Exceptions;
using SACA_Common.Models;
using SACA_Infra.Context;
using SACA_Infra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SACA_Service.Services
{
    public interface ISacaFileService
    {
        Task<CreateResult> CreateAsync(IFormFile file, string userId);

        Task<bool> UpdateAsync(IFormFile file, string id, string userId);

        Task<saca_file> GetByIdAsync(string id);

        Task<bool> DeleteAsync(string id, string userId);

        string GetFullPath(string path);

        string GetFileStorage();
        Task<byte[]> GetBytesById(string id);

    }
    public class SacaFileService : ISacaFileService
    {
        public string FileStorage;
        private readonly SACA_Context _context;
        private readonly ILogger<SacaFileService> _logger;
        private readonly IMapper _mapper;
        private readonly ISysSettingService _sysSettingService;
        public SacaFileService
        (
            ISysSettingService sysSettingService,
            SACA_Context context,
            ILogger<SacaFileService> logger,
            IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _sysSettingService = sysSettingService;
            FileStorage = GetFileStorage();
        }
        public async Task<CreateResult> CreateAsync(IFormFile file, string userId)
        {
            var document_file = new saca_file();
            document_file.name = Path.GetFileNameWithoutExtension(file.FileName);
            document_file.extension = Path.GetExtension(file.FileName);
            document_file.length = file.Length;
            document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
            await Save(document_file.path, file);
            document_file.Created(userId);
            _context.saca_files.Add(document_file);
            if (await _context.SaveChangesAsync() > 0)
            {
                return new CreateResult(document_file.id);
            }
            else throw new Exception("Xảy ra lỗi trong quá trình lưu file");
        }

        public async Task<bool> UpdateAsync(IFormFile file, string id, string userId)
        {
            var document_file = await GetByIdAsync(id);
            var extensionOld = document_file.extension;
            var extensionNew = Path.GetExtension(file.FileName);
            document_file.name = Path.GetFileNameWithoutExtension(file.FileName);
            document_file.extension = extensionNew;
            document_file.length = file.Length;
            if (extensionNew != extensionOld)
            {
                var path = CreateFilePath($"{document_file.id}{document_file.extension}");
                await Save(path, file);
                Delete(document_file.path);
                document_file.path = path;
            }
            else
            {
                /* document_file.path: overwrite existed file */
                await Save(document_file.path, file);
            }

            _context.saca_files.Update(document_file);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            else throw new Exception("Xảy ra lỗi trong quá trình lưu file");
        }

        public async Task<saca_file> GetByIdAsync(string id)
        {
            var document_file = await _context.saca_files
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.id == id);
            if (document_file != null)
            {
                return document_file;
            }
            throw new BadException($"Không tìm thấy file với id '{id}'");
        }

        public async Task<byte[]> GetBytes(string path)
        {
            path = GetFullPath(path);
            if (File.Exists(path))
            {
                return await File.ReadAllBytesAsync(GetFullPath(path));
            }
            throw new BadException($"File không tồn tại hoặc đã bị xoá");
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var document_file = await GetByIdAsync(id);
            if (document_file != null)
            {
                try
                {
                    document_file.Deleted(userId);
                    _context.saca_files.Update(document_file);
                    await _context.SaveChangesAsync();
                    Delete(document_file.path);
                    return true;
                }
                catch (Exception ex)
                {
                    throw new BadException($"{ex.ToString()}");
                }

            }
            throw new BadException($"Không tìm thấy file với id '{id}'");
        }

        public string GetFullPath(string path)
        {
            return Path.Combine(FileStorage, path);
        }

        public string GetFileStorage()
        {
            if (FileStorage == null)
            {
                var settings = _sysSettingService.GetAll();
                FileStorage = settings.FirstOrDefault(o => o.key == "config_file_directory")?.value ?? "";
            }
            return FileStorage;
        }

        public string Delete(string path)
        {
            try
            {
                var fullPath = GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return null!;
                }
                else
                {
                    return "File không tồn tại";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception("Xảy ra lỗi trong quá trình xoá file");
            }
        }

        /* function helper */

        protected string CreateFilePath(string fileName, DateTime? dateTime = null)
        {
            try
            {
                fileName = Replace(fileName);
                var date = dateTime ?? DateTime.Now;
                var folder = Path.Combine(date.Year.ToString(), date.Month.ToString(), date.Day.ToString());
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return Path.Combine(date.Year.ToString(), date.Month.ToString(), date.Day.ToString(), fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception("Tạo đường dẫn file không thành công");
            }
        }

        protected string Replace(string value)
        {
            string regex = $"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]";
            Regex removeInvalidChars = new Regex(regex, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant);
            value = removeInvalidChars.Replace(value, "_");
            return value;
        }

        protected async Task Save(string path, IFormFile file)
        {
            try
            {
                var fullPath = GetFullPath(path);
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }
                using (FileStream fs = File.Create(fullPath))
                {
                    await file.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception("Xảy ra lỗi trong quá trình lưu file");
            }
        }

        //protected async Task Save(string path, byte[] bytes)
        //{
        //    try
        //    {
        //        var fullPath = GetFullPath(path);
        //        var directory = Path.GetDirectoryName(fullPath);
        //        if (!Directory.Exists(directory))
        //        {
        //            Directory.CreateDirectory(directory!);
        //        }
        //        await File.WriteAllBytesAsync(fullPath, bytes);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message);
        //        throw new Exception("Xảy ra lỗi trong quá trình lưu file");
        //    }
        //}

        public async Task<byte[]> GetBytesById(string id)
        {
            var document_file = await _context.saca_files
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.id == id);
            if (document_file != null)
            {
                return await GetBytes(document_file.path);
            }
            throw new BadException($"Không tìm thấy file");
        }
    }
}
