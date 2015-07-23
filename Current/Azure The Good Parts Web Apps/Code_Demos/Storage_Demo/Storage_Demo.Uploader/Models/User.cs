using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Storage_Demo.Uploader.Models
{
    class User : TableEntity
    {
        public User(string email, string firstName, string lastName)
        {
            UserId = Guid.NewGuid().ToString("N");
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        public void SetAvatarUrl(string url)
        {
            AvatarUrl = url;
        }

        [IgnoreProperty]
        public string UserId
        {
            get { return PartitionKey; }
            private set { PartitionKey = value; }
        }

        [IgnoreProperty]
        public string Email
        {
            get { return RowKey; }
            private set { RowKey = value; }
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }
    }
}
