using System.IO.Compression;

namespace Company.Project.Domain.Models
{
    public class ImageFile : BaseEntity
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] Data { get; set; }
        
        public static byte[] Compress(byte[] data)
        {
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        // Decompress the image data
        public static byte[] Decompress(byte[] compressedData)
        {
            using var input = new MemoryStream(compressedData);
            using var output = new MemoryStream();
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            {
                gzip.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
