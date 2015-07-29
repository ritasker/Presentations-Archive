namespace WorkerRole_Demo.Shared.Messages
{
    public class ResizeImageMessage
    {
        public ResizeImageMessage(string userId, string email, string blobUrl)
        {
            UserId = userId;
            BlobUrl = blobUrl;
            UserEmail = email;
        }

        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public string BlobUrl { get; set; }
    }
}
