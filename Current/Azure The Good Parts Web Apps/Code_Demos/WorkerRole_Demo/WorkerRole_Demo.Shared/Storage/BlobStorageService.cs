using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.WindowsAzure.Storage.Blob;
using WorkerRole_Demo.Shared.Helpers;

namespace WorkerRole_Demo.Shared.Storage
{
    public class BlobStorageService
    {
        private readonly CloudBlobContainer _container;

        public BlobStorageService(CloudBlobClient blobClient, string containerName)
        {
            _container = blobClient.GetContainerReference(containerName);
            _container.CreateIfNotExists();
            _container.SetPermissions(new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});
        }

        public void UploadImage(Image image, string blobLocation)
        {
            using (var stream = image.ToStream(ImageFormat.Jpeg))
            {
                var blob = _container.GetBlockBlobReference(blobLocation);
                blob.UploadFromStream(stream);
            }
        }

        public void UploadImage(byte[] image, string blobLocation)
        {
            var blob = _container.GetBlockBlobReference(blobLocation);
            blob.UploadFromByteArray(image, 0, image.Length);
        }

        public byte[] DownloadImage(string blobUrl)
        {
            var blob = _container.GetBlockBlobReference(blobUrl);
            byte[] imageBytes = new byte[blob.StreamMinimumReadSizeInBytes];
            blob.DownloadToByteArray(imageBytes, 0);

            return imageBytes;
        }
    }
}