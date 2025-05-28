using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SACA_Common.Controllers;
using SACA_Common.DTOs;
using SACA_Service.Services;
using SACA_Common.DTOs.TestCase.Request;
using SACA_Common.DTOs.TestCase.Response;
using SACA_Common.Routes;
using SACA_Infra.Models;
using SACA_Common.DTOs.File.Response;
using System.IO;

namespace SACA_API.Controllers
{
    [Route(FileRoutes.INDEX)]
    [ApiController]
    public class FileController : AuthorizeController
    {
        private ISacaFileService _sacaFileService;
        public FileController
        (
            ISacaFileService sacaFileService,
            ILogger<ISacaFileService> logger
        ) : base(logger)
        {
            _sacaFileService = sacaFileService;
        }
        [HttpPost(FileRoutes.ACTION.Create)]
        public async Task<IActionResult> Create([FromForm] IFormFile file)
        {
            try
            {
                return Ok(new Response<CreateResult>(await _sacaFileService.CreateAsync(file, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPut(FileRoutes.ACTION.Update)]
        public async Task<IActionResult> Update([FromForm] IFormFile file, [FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _sacaFileService.UpdateAsync(file, id, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpDelete(FileRoutes.ACTION.Delete)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<bool>(await _sacaFileService.DeleteAsync(id, UserHeader.user_id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(FileRoutes.ACTION.Get)]
        public async Task<IActionResult> GetFile([FromRoute] string id)
        {
            try
            {
                return Ok(new Response<byte[]>(await _sacaFileService.GetBytesById(id)));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet(FileRoutes.ACTION.DownloadFile)]
        public async Task<IActionResult> DownloadFile([FromRoute] string id)
        {
            try
            {
                var file = await _sacaFileService.GetByIdAsync(id);
                var bytes = await _sacaFileService.GetBytesById(id);
                var fileName = Path.GetFileName(file.path);
                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
