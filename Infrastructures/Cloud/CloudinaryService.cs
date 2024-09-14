using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Eduhunt.AppSettings;
namespace Eduhunt.Infrastructures.Cloud
{

    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(CloudinarySetting cloudinary)
        {
            var account = new Account { ApiKey = cloudinary.ApiKey, ApiSecret = cloudinary.ApiKey, Cloud = cloudinary.CloudName };

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadSingleAsync(Stream fileStream, string fileName)
        {
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, fileStream),
                UploadPreset = "tstdfsn5"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }

        public async Task<List<string>> UploadMultipleAsync(List<(Stream fileStream, string fileName)> files)
        {
            var uploadTasks = new List<Task<ImageUploadResult>>();

            foreach (var file in files)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.fileName, file.fileStream),
                    UploadPreset = "tstdfsn5"
                };
                uploadTasks.Add(_cloudinary.UploadAsync(uploadParams));
            }

            var results = await Task.WhenAll(uploadTasks);
            var urls = new List<string>();

            foreach (var result in results)
            {
                urls.Add(result.SecureUrl.ToString());
            }

            return urls;
        }
    }

}
