using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace WorkerRole_Demo.Shared.Storage
{
    public class StorageAccessor
    {
        private readonly CloudStorageAccount _storageAccount;

        public StorageAccessor(string connectionString)
        {
            _storageAccount = string.IsNullOrEmpty(connectionString)
                ? CloudStorageAccount.DevelopmentStorageAccount
                : CloudStorageAccount.Parse(connectionString);
        }

        public CloudTableClient CreateTableClient()
        {
            return _storageAccount.CreateCloudTableClient();
        }

        public CloudBlobClient CreateBlobClient()
        {
            return _storageAccount.CreateCloudBlobClient();
        }

        public CloudQueueClient CreateQueueClient()
        {
            return _storageAccount.CreateCloudQueueClient();
        }
    }
}