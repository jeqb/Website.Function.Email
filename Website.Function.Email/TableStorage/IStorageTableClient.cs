using Azure;
using Azure.Data.Tables;
using System.Threading.Tasks;

namespace Website.Function.Email.TableStorage
{
    public interface IStorageTableClient
    {
        public Task<Response> InsertEntityAsync(ITableEntity entity);
    }
}
