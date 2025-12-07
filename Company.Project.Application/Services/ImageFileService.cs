using Company.Project.Application.Contracts;
using Company.Project.Application.DTO.ImageFile;
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
            var compressed = ImageFile.Compress(data);
            var imageFile = new ImageFile
            {
                FileName = fileName,
                FileType = fileType,
                Data = compressed
            };

            await _unitOfWork.ImageFiles.AddAsync(imageFile);
            await _unitOfWork.CompleteAsync();
            return imageFile;
        }

        public async Task<ImageDownloadDto> DownloadFileAsync(int id)
        {
            var imageFile = await _unitOfWork.ImageFiles.GetByIdAsync(id);
            if (imageFile == null)
                throw new KeyNotFoundException("Image not found.");

            return new ImageDownloadDto
            {
                Id = imageFile.Id,
                FileName = imageFile.FileName,
                FileType = imageFile.FileType,
                Data = ImageFile.Decompress(imageFile.Data)
            };
        }

        public async Task DeleteImageAsync(int id)
        {
            var imageFile = await _unitOfWork.ImageFiles.GetByIdAsync(id);
            if (imageFile == null)
                throw new KeyNotFoundException("Image not found.");

            await _unitOfWork.ImageFiles.DeleteAsync(imageFile);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ImageFile>> ListAllImagesAsync()
        {
            return await _unitOfWork.ImageFiles.GetAllAsync();
        }

        public async Task<ImageFile> EditImageMetadataAsync(int id, string newFileName, string newFileType)
        {
            var imageFile = await _unitOfWork.ImageFiles.GetByIdAsync(id);
            if (imageFile == null)
                throw new KeyNotFoundException("Image not found.");

            imageFile.FileName = newFileName;
            imageFile.FileType = newFileType;
            imageFile.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ImageFiles.UpdateAsync(imageFile);
            await _unitOfWork.CompleteAsync();
            return imageFile;
        }
    }
}
