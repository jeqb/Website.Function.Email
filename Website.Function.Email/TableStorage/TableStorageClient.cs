using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.Function.Email.TableStorage
{
    /// <summary>
    /// Client to talk to Azure TableStorage.
    /// TODO: Throw this into a Nuget package. make resuable for this project and the
    /// backend project.
    /// </summary>
    public class TableStorageClient
    {
        private readonly string _storageUri;

        private readonly string _storageAccountName;

        private readonly string _storageAccountKey;

        public TableStorageClient(string storageUri, string storageAccountName, string storageAccountKey)
        {
            _storageUri = storageUri;

            _storageAccountName = storageAccountName;

            _storageAccountKey = storageAccountKey;
        }

        /// <summary>
        /// Given the name of a table, create an instance of the TableClient to talk to that table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private async Task<TableClient> CreateTableClientAsync(string tableName)
        {
            TableClient tableClient = new (
                new Uri(_storageUri),
                tableName,
                new TableSharedKeyCredential(_storageAccountName, _storageAccountKey)
                );

            return await Task.FromResult(tableClient);
        }
    
        /// <summary>
        /// Give it an entity and the name of the table to insert into storage.
        /// I'm on the fence about the tableName param. maybe another way to do it.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<Response> InsertEntityAsync(ITableEntity entity)
        {
            string tableName = entity.GetType().Name;

            TableClient tableClient = await CreateTableClientAsync(tableName);

            return await tableClient.AddEntityAsync(entity);
        }
    }
}
