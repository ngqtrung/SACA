using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    public interface IFileService<T>
    {
        Task<T> Create(IFormFile file, string parent_id, string userid);

        Task<T> Create(byte[] bytes, string filename, string parent_id, string userid);

        Task<List<T>> Create(List<IFormFile> files, string parent_id, string actor_id);

        Task<T> CreateUnsave(string filename, string parent_id, string actor_id, long? filelength = 0, byte[]? bytes = null);

        Task<T> CreateUnsave(IFormFile file, string parent_id, string actor_id);

        Task<T> Update(IFormFile file, T document_file, string userid);

        Task<T> GetById(string id, string userid);

        Task<T> GetByParentId(string parent_id, string userid);

        Task<T> TryGetById(string id, string userid);

        Task<T> TryGetByParentId(string parent_id, string userid);

        Task<List<T>> GetMany(string parenet_id, string userid);

        Task<List<T>> TryGetMany(string parent_id, string userid);

        Stream GetStream(string path);

        Task<byte[]> GetBytes(string path);
        Task<byte[]> GetBytesById(string id, string userid);

        Task<bool> Delete(string id, string userid);

        Task<bool> TryDelete(string id, string userid);

        Task<string[]> DeleteByParentId(string parent_id, string userid);

        Task<string[]> TryDeleteByParentId(string parent_id, string userid);

        Task<int> Count(string parent_id);

        Task<int> Count(List<string> parent_ids);

        string GetFullPath(string path);

        string GetFileStorage();

        Task<T> CreateCopyFile(IFormFile file, string parent_id, byte[] bytes, string actor_id);
    }
    public class FileService<T> : IFileService<T> where T : saca_file_properties
    {
        public string FileStorage;
        private readonly SACA_Context _context;
        private readonly ILogger<FileService<T>> _logger;
        private readonly IMapper _mapper;
        public FileService
        (
            string fileStorage,
            SACA_Context context,
            ILogger<FileService<T>> logger,
            IMapper mapper)
        {
            FileStorage = fileStorage;
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<T> Create(IFormFile file, string parent_id, string userid)
        {
            var document_file = new saca_file_properties();
            document_file.name = Path.GetFileNameWithoutExtension(file.FileName);
            document_file.extension = Path.GetExtension(file.FileName);
            document_file.length = file.Length;
            document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
            await Save(document_file.path, file);
            document_file.parent_id = parent_id;
            document_file.Created(userid);
            var doc = _mapper.Map<T>(document_file);
            _context.Set<T>().Add(_mapper.Map<T>(document_file));
            if (await _context.SaveChangesAsync() > 0)
            {
                return _mapper.Map<T>(document_file);
            }
            else throw new Exception("Xảy ra lỗi trong quá trình lưu file");
        }
        public byte[] ReadAllBytes(string fileName)
        {
            byte[] buffer = null!;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            return buffer;
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        public async Task<T> Create(byte[] bytes, string filename, string parent_id, string userid)
        {
            var document_file = new saca_file_properties();
            document_file.name = Path.GetFileNameWithoutExtension(filename);
            document_file.extension = Path.GetExtension(filename);
            document_file.length = bytes.Length;
            document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
            await Save(document_file.path, bytes);

            document_file.parent_id = parent_id;
            document_file.Created(userid);

            _context.Set<T>().Add(_mapper.Map<T>(document_file));
            if (await _context.SaveChangesAsync() > 0)
            {
                return _mapper.Map<T>(document_file);
            }
            else throw new Exception("Xảy ra lỗi trong quá trình lưu file");
        }

        public async Task<List<T>> Create(List<IFormFile> files, string parent_id, string actor_id)
        {
            var document_files = new List<T>();
            foreach (var file in files)
            {
                var document_file = new saca_file_properties
                {
                    name = Path.GetFileNameWithoutExtension(file.FileName),
                    extension = Path.GetExtension(file.FileName),
                    length = file.Length
                };
                document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
                await Save(document_file.path, file);
                document_file.parent_id = parent_id;
                document_file.Created(actor_id);
                document_files.Add(_mapper.Map<T>(document_file));
            }
            if (document_files.Any())
            {
                _context.Set<T>().AddRange(document_files);
                if (await _context.SaveChangesAsync() > 0)
                {
                    return document_files;
                }
            }
            throw new BadException("Không có file nào được lưu");
        }

        public async Task<T> CreateUnsave(string filename, string parent_id, string actor_id, long? filelength = 0, byte[]? bytes = null)
        {
            T document_file = (T)new saca_file_properties();
            document_file.name = Path.GetFileNameWithoutExtension(filename);
            document_file.extension = Path.GetExtension(filename);
            document_file.length = filelength ?? 0;
            document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
            if (bytes != null) await Save(document_file.path, bytes);

            document_file.parent_id = parent_id;
            document_file.Created(actor_id);

            return document_file;
        }

        public async Task<T> CreateUnsave(IFormFile file, string parent_id, string actor_id)
        {
            var document_file = new saca_file_properties();
            document_file.name = Path.GetFileNameWithoutExtension(file.FileName);
            document_file.extension = Path.GetExtension(file.FileName);
            document_file.length = file.Length;
            document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
            await Save(document_file.path, file);
            document_file.parent_id = parent_id;
            document_file.Created(actor_id);
            return _mapper.Map<T>(document_file);
        }

        public async Task<T> CreateCopyFile(IFormFile file, string parent_id, byte[] bytes, string actor_id)
        {
            var document_file = new saca_file_properties();
            document_file.name = Path.GetFileNameWithoutExtension(file.FileName);
            document_file.extension = Path.GetExtension(file.FileName);
            document_file.length = file.Length;
            document_file.path = CreateFilePath($"{document_file.id}{document_file.extension}");
            await Save(document_file.path, bytes);
            document_file.parent_id = parent_id;
            document_file.Created(actor_id);
            return _mapper.Map<T>(document_file);
        }

        public async Task<T> Update(IFormFile file, T document_file, string userid)
        {
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

            _context.Set<T>().Update(document_file);
            if (await _context.SaveChangesAsync() > 0)
            {
                return document_file;
            }
            else throw new Exception("Xảy ra lỗi trong quá trình lưu file");
        }

        public async Task<T> GetById(string id, string userid)
        {
            var document_file = await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.id == id);
            if (document_file != null)
            {
                return document_file;
            }
            throw new BadException($"Không tìm thấy file với id '{id}'");
        }

        public async Task<T> GetByParentId(string parent_id, string userid)
        {
            var document_file = await _context.Set<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.parent_id == parent_id);
            if (document_file != null)
            {
                return document_file;
            }
            throw new BadException($"Không tìm thấy file");
        }

        public async Task<T> TryGetById(string id, string userid)
        {
            try
            {
                return await GetById(id, userid);
            }
            catch
            {
                return null!;
            }
        }

        public async Task<T> TryGetByParentId(string parent_id, string userid)
        {
            try
            {
                return await GetByParentId(parent_id, userid);
            }
            catch
            {
                return null!;
            }
        }

        public async Task<List<T>> GetMany(string parent_id, string userid)
        {
            var document_files = await _context.Set<T>()
                .AsNoTracking()
                .Where(o => o.parent_id == parent_id)
                .ToListAsync();
            if (document_files != null && document_files.Any())
            {
                return document_files;
            }
            throw new BadException($"Không tìm thấy file");
        }

        public async Task<List<T>> TryGetMany(string parent_id, string userid)
        {
            try
            {
                return await GetMany(parent_id, userid);
            }
            catch
            {
                return null!;
            }
        }

        public Stream GetStream(string path)
        {
            path = GetFullPath(path);
            if (File.Exists(path))
            {
                return File.OpenRead(GetFullPath(path));
            }
            throw new BadException($"File không tồn tại hoặc đã bị xoá");
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

        public async Task<bool> Delete(string id, string userid)
        {
            var document_file = await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(o => o.id == id);
            if (document_file != null)
            {
                try
                {
                    _context.Set<T>().Remove(document_file);
                    _context.SaveChanges();
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

        public async Task<bool> TryDelete(string id, string userid)
        {
            try
            {
                return await Delete(id, userid);
            }
            catch
            {
                return false;
            }
        }

        public async Task<string[]> DeleteByParentId(string parent_id, string userid)
        {
            var results = new List<string>();
            var document_files = await GetMany(parent_id, userid);
            foreach (var file in document_files)
            {
                var message = Delete(file.path);
                if (message != null)
                {
                    results.Add(message);
                }
            }
            _context.Set<List<T>>().Remove(document_files);
            _context.SaveChanges();

            return results.ToArray();
        }

        public async Task<string[]> TryDeleteByParentId(string parent_id, string userid)
        {
            try
            {
                return await DeleteByParentId(parent_id, userid);
            }
            catch
            {
                return null!;
            }
        }

        public async Task<int> Count(string parent_id)
        {
            try
            {
                return await _context.Set<T>().CountAsync(o => o.parent_id == parent_id);
            }
            catch
            {
                return 0;
            }
        }

        public async Task<int> Count(List<string> parent_ids)
        {
            try
            {
                if (parent_ids != null && parent_ids.Any())
                {
                    return await _context.Set<T>().CountAsync(o => parent_ids.Contains(o.parent_id!));
                }
                else return 0;
            }
            catch
            {
                return 0;
            }
        }

        public string GetFullPath(string path)
        {
            return Path.Combine(FileStorage, path);
        }

        public string GetFileStorage()
        {
            return FileStorage;
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

        protected async Task Save(string path, byte[] bytes)
        {
            try
            {
                var fullPath = GetFullPath(path);
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }
                await File.WriteAllBytesAsync(fullPath, bytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new Exception("Xảy ra lỗi trong quá trình lưu file");
            }
        }

        public async Task<byte[]> GetBytesById(string id, string userid)
        {
            var document_file = await _context.Set<T>()
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
