using Company.Project.Domain.Models;

namespace Company.Project.Application.Contracts
{
    public interface IImageFileService
    {
        Task<ImageFile> UploadImageAsync(string fileName, string fileType, byte[] data);
        Task<byte[]> GetImageAsync(int id);
    }
}
