using Company.Project.Application.Contracts;
using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;

namespace Company.Project.Application.Services
{
    public class ImageFileService : IImageFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ImageFileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ImageFile> UploadImageAsync(string fileName, string fileType, byte[] data)
        {
            var compressedData = ImageFile.Compress(data);
            var imageFile = new ImageFile
            {
                FileName = fileName,
                FileType = fileType,
                Data = compressedData
            };
            await _unitOfWork.ImageFiles.AddAsync(imageFile);
            await _unitOfWork.Completeasync();
            return imageFile;
        }

        public async Task<byte[]> GetImageAsync(int id)
        {
            var imageFile = await _unitOfWork.ImageFiles.GetByIdAsync(id);
            if (imageFile == null)
            {
                throw new KeyNotFoundException("Image not found.");
            }
            return ImageFile.Decompress(imageFile.Data);
        }
        
        
    }
}
