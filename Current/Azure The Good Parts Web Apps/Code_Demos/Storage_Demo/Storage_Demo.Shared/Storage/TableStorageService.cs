using Microsoft.WindowsAzure.Storage.Table;
using Storage_Demo.Shared.Models;

namespace Storage_Demo.Shared.Storage
{
    public class TableStorageService
    {
        private readonly CloudTable _table;

        public TableStorageService(CloudTableClient tableClient, string tableName)
        {
            _table = tableClient.GetTableReference(tableName);
            _table.CreateIfNotExists();
        }

        public void Add(User user)
        {
            _table.Execute(TableOperation.Insert(user));
        }

        public User Find(string userId, string userEmail)
        {
            var tableResult = _table.Execute(TableOperation.Retrieve<User>(userId, userEmail));
            return tableResult.Result as User;
        }

        public void Update(User user)
        {
            _table.Execute(TableOperation.Merge(user));
        }
    }
}