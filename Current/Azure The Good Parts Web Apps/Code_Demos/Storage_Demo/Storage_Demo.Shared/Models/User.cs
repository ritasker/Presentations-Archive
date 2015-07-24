using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Storage_Demo.Shared.Models
{
    public class User : TableEntity
    {
        public User()
        {
            
        }

        public User(string email, string firstName, string lastName)
        {
            Id = Guid.NewGuid().ToString("N");
            Email = email;
            FirstName = firstName;
            LastName = lastName;
        }

        [IgnoreProperty]
        public string Id
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

        public void SetAvatarUrl(string url)
        {
            AvatarUrl = url;
        }
    }
}