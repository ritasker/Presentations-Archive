using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Newtonsoft.Json;
using WorkerRole_Demo.Shared.Messages;
using WorkerRole_Demo.Shared.Models;
using WorkerRole_Demo.Shared.Storage;

namespace WorkerRole_Demo.WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private AzureQueueService _azureQueueService;
        private BlobStorageService _blobStorageService;
        private TableStorageService _tableStorageService;
        private ImageManipulator _imageManipulator;

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole_Demo.WorkerRole is running");

            try
            {
                this.RunAsync(this._cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this._runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var storageAccessor = new StorageAccessor(CloudConfigurationManager.GetSetting("StorageConnectionString"));

            _azureQueueService = new AzureQueueService(storageAccessor.CreateQueueClient(), "image-resize");
            _blobStorageService = new BlobStorageService(storageAccessor.CreateBlobClient(), "uploads");
            _tableStorageService = new TableStorageService(storageAccessor.CreateTableClient(), "Users");

            _imageManipulator = new ImageManipulator();

            bool result = base.OnStart();

            Trace.TraceInformation("WorkerRole_Demo.WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole_Demo.WorkerRole is stopping");

            this._cancellationTokenSource.Cancel();
            this._runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole_Demo.WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                if (_azureQueueService.MessagesAvailable())
                {
                    var message = _azureQueueService.GetMessage();
                    var resizeImageMessage = JsonConvert.DeserializeObject<ResizeImageMessage>(message.AsString);

                    byte[] image = _blobStorageService.DownloadImage(resizeImageMessage.BlobUrl);

                    // Resize Image
                    byte[] thumbnail = _imageManipulator.ResizeImage(image, 200);

                    // Upload Thumbnail to Blob Storage
                    var thumbnailUrl = GetThumbnailUrl(resizeImageMessage.BlobUrl);
                    _blobStorageService.UploadImage(thumbnail, thumbnailUrl);

                    // Update User with Thumbnail
                    User user = _tableStorageService.Find(resizeImageMessage.UserId, resizeImageMessage.UserEmail);
                    user.SetAvatarUrl(thumbnailUrl);
                    _tableStorageService.Update(user);

                    _azureQueueService.DeleteMessage(message);
                }
            }
        }

        private static string GetThumbnailUrl(string blobUrl)
        {
            var indexOf = blobUrl.LastIndexOf('.');
            return blobUrl.Insert(indexOf, "_Thumbnail").ToLower();
        }
    }
}
