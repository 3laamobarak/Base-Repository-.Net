using Company.Project.Application.Contracts;
using Company.Project.Application.DTO.ImageFile;
using Microsoft.AspNetCore.Mvc;

namespace Company.Project.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageFileController : ControllerBase
    {
        private readonly IImageFileService _imageFileService;
        public ImageFileController(IImageFileService imageFileService)
        {
            _imageFileService = imageFileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Only images and PDF are allowed.");

            if (file.Length > 10 * 1024 * 1024)
                return BadRequest("File too large. Max 10MB.");

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            var uploaded = await _imageFileService.UploadImageAsync(file.FileName, file.ContentType, ms.ToArray());
            return Ok(new { uploaded.Id, uploaded.FileName, uploaded.FileType });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            try
            {
                var file = await _imageFileService.DownloadFileAsync(id);
                return new FileContentResult(file.Data, file.FileType)
                {
                    FileDownloadName = file.FileName
                };
            }
            catch (KeyNotFoundException)
            {
                return NotFound("File not found.");
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                await _imageFileService.DeleteImageAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> ListAllImages()
        {
            var images = await _imageFileService.ListAllImagesAsync();
            var result = images.Select(x => new
            {
                x.Id,
                x.FileName,
                x.FileType,
                x.CreatedAt
            });
            return Ok(result);
        }

        [HttpPut("{id}/edit")]
        public async Task<IActionResult> EditImageMetadata(int id, [FromBody] EditImageMetaData request)
        {
            try
            {
                var updated = await _imageFileService.EditImageMetadataAsync(id, request.NewFileName, request.NewFileType);
                return Ok(new { updated.Id, updated.FileName, updated.FileType });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        
    }
}