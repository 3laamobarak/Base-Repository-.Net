using Company.Project.Application.Contracts;
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
        public async Task<IActionResult> UploadImage( IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Validate file type (e.g., only allow images)
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest("Unsupported file type.");

            // Validate file size (e.g., max 5 MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
                return BadRequest("File size exceeds the 5 MB limit.");

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var imageFile = await _imageFileService.UploadImageAsync(file.FileName, file.ContentType, memoryStream.ToArray());
            return Ok(new { imageFile.Id, imageFile.FileName });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            try
            {
                var imageData = await _imageFileService.GetImageAsync(id);
                var contentType = "application/octet-stream"; // Default content type

                // Optionally, determine the content type based on the file extension
                // This assumes the file name has an extension (e.g., .jpg, .png)
                var extension = Path.GetExtension(imageData.ToString()).ToLowerInvariant();
                if (extension == ".jpg" || extension == ".jpeg") contentType = "image/jpeg";
                else if (extension == ".png") contentType = "image/png";
                else if (extension == ".gif") contentType = "image/gif";

                return File(imageData, contentType);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Image not found.");
            }
        }
    }
}