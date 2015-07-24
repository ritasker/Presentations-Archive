using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using Faker;
using Storage_Demo.Shared.Messages;
using Storage_Demo.Shared.Models;
using Storage_Demo.Shared.Storage;

namespace Storage_Demo.Uploader
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Storage
            var storageAccessor = new StorageAccessor(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Table Storage
            TableStorageService tableStorageService = new TableStorageService(storageAccessor.CreateTableClient(), "Users");
            User user = new User(Internet.Email(), Name.First(), Name.Last());
            tableStorageService.Add(user);

            // Blob Storage
            var blobStorageService = new BlobStorageService(storageAccessor.CreateBlobClient(), "uploads");
            var blobLocation = string.Format("{0}/{1}.jpg", user.Id, Guid.NewGuid().ToString("N"));
            Image avatar = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Images\Avatar-AirBender.jpg"));
            blobStorageService.UploadImage(avatar, blobLocation);

            // Azure Queues
            AzureQueueService azureQueueService = new AzureQueueService(storageAccessor.CreateQueueClient(), "image-resize");
            azureQueueService.AddMessage(new ResizeImageMessage(user.Id, user.Email, blobLocation));

            Console.WriteLine("User Details");
            Console.WriteLine("Id : {0}", user.Id);
            Console.WriteLine("Email : {0}", user.Email);
            Console.WriteLine("First Name : {0}", user.FirstName);
            Console.WriteLine("Last Name : {0}", user.LastName);
            Console.WriteLine("Avatar Url : {0}", user.AvatarUrl);

            Console.WriteLine();
            Console.WriteLine("Blob Url : {0}", blobLocation);

            Console.Read();
        }
    }
}
