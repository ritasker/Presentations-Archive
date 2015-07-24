using System;
using System.Configuration;
using Newtonsoft.Json;
using Storage_Demo.Shared.Messages;
using Storage_Demo.Shared.Storage;
using Storage_Demo.Shared.Models;

namespace Storage_Demo.Processor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Storage
            var storageAccessor = new StorageAccessor(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Get Message Off the Queue
            AzureQueueService azureQueueService = new AzureQueueService(storageAccessor.CreateQueueClient(), "image-resize");
            var message = azureQueueService.GetMessage();
            var resizeImageMessage = JsonConvert.DeserializeObject<ResizeImageMessage>(message.AsString);

            // Download Image From Blob Storage
            var blobStorageService = new BlobStorageService(storageAccessor.CreateBlobClient(), "uploads");
            byte[] image = blobStorageService.DownloadImage(resizeImageMessage.BlobUrl);

            // Resize Image
            var imageManipulator = new ImageManipulator();
            byte[] thumbnail = imageManipulator.ResizeImage(image, 200);

            // Upload Thumbnail to Blob Storage
            var thumbnailUrl = GetThumbnailUrl(resizeImageMessage.BlobUrl);
            blobStorageService.UploadImage(thumbnail, thumbnailUrl);

            // Update User with Thumbnail
            TableStorageService tableStorageService = new TableStorageService(storageAccessor.CreateTableClient(), "Users");
            User user = tableStorageService.Find(resizeImageMessage.UserId, resizeImageMessage.UserEmail);
            user.SetAvatarUrl(thumbnailUrl);
            tableStorageService.Update(user);

            azureQueueService.DeleteMessage(message);

            Console.WriteLine("Finished Processing Image.");
            Console.Read();
        }

        private static string GetThumbnailUrl(string blobUrl)
        {
            var indexOf = blobUrl.LastIndexOf('.');
            return blobUrl.Insert(indexOf, "_Thumbnail").ToLower();
        }
    }
}
