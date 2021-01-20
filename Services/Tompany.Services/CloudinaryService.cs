using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tompany.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            this.cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string fileName)
        {
            byte[] image;

            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            image = memoryStream.ToArray();

            using var destinationStream = new MemoryStream(image);

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, destinationStream),
                PublicId = fileName,
            };

            var result = await this.cloudinary.UploadAsync(uploadParams);

            return result.Url.AbsoluteUri;
        }
    }
}
