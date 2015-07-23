using Microsoft.WindowsAzure.Storage;
using Storage_Demo.Uploader.Models;
using System.Configuration;
using Faker;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Drawing;
using System.IO;
using System;
using System.Drawing.Imaging;

namespace Storage_Demo.Uploader
{
    class Program
    {
        static void Main(string[] args)
        {
            var connStr = ConfigurationManager.AppSettings["StorageConnectionString"];
            CloudStorageAccount storageAccount = string.IsNullOrEmpty(connStr) 
                ? CloudStorageAccount.DevelopmentStorageAccount
                : CloudStorageAccount.Parse(connStr);

            // Get Storage Clients
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Setup Table
            CloudTable table = tableClient.GetTableReference("Users");
            table.CreateIfNotExists();

            // Setup Blob
            CloudBlobContainer container = blobClient.GetContainerReference("uploads");
            container.CreateIfNotExists();
            

            // Setup Blob
            CloudQueue queue = queueClient.GetQueueReference("image-resize");
            queue.CreateIfNotExists();

            // Save User Record
            var usr = new User(Internet.Email(), Name.First(), Name.Last());
            table.Execute(TableOperation.InsertOrMerge(usr));

            // Upload Image to Blob
            Image avatar = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Images\Avatar-AirBender.jpg"));

            using (var stream = avatar.ToStream(ImageFormat.Jpeg))
            {
                var blob = container.GetBlobReferenceFromServer(string.Format("{0}\\{1}.jpg", usr.UserId, Guid.NewGuid().ToString("N")));
                blob.UploadFromStream(stream);                
            }

            // Add message on the Queue
        }

        
    }

    public static class ImageHelpers
    {
        public static Stream ToStream(this Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }
    }
}
