using Company.Project.Application.DTO.ImageFile;
using Company.Project.Domain.Models;

namespace Company.Project.Application.Contracts
{
    public interface IImageFileService
    {
        Task<ImageFile> UploadImageAsync(string fileName, string fileType, byte[] data);
        Task<ImageDownloadDto> DownloadFileAsync(int id);
        Task DeleteImageAsync(int id);
        Task<IEnumerable<ImageFile>> ListAllImagesAsync();
        Task<ImageFile> EditImageMetadataAsync(int id, string newFileName, string newFileType);
    }
}
