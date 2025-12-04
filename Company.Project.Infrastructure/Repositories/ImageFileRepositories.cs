using Company.Project.Domain.Interfaces;
using Company.Project.Domain.Models;

namespace Company.Project.Infrastructure.Repositories
{
    public class ImageFileRepositories : BaseRepository<ImageFile> , IImageFileRepository
    {
        public ImageFileRepositories(Context context) : base(context)
        {
        }
        
    }
}
