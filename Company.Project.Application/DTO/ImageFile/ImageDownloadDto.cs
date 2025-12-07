namespace Company.Project.Application.DTO.ImageFile;

public class ImageDownloadDto
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public byte[] Data { get; set; }
}