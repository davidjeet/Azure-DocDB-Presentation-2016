using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc5.DocDB.Infrastructure.Configuration;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;


namespace Mvc5.DocDB.Infrastructure.Repository.DocumentDB
{
    public abstract class BaseRepository
    {
        protected DocumentClient client { get; set; }

        protected Database database { get; set; }

        protected DocumentCollection collection { get; set; }

        protected async Task Initialize(string collectionName)
        {
            try
            {
                if (client == null)
                {
                    client = new DocumentClient(new Uri(DocumentDBSettings.Current().DocDBUrl), DocumentDBSettings.Current().DocDBPrimaryKey);
                    database = await client.RetrieveOrCreateDatabaseAsync(DocumentDBSettings.Current().DatabaseName);
                    collection = await client.RetrieveOrCreateCollectionAsync(database.SelfLink, collectionName);
                }
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("Status code {0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                throw baseException;
            }
            catch (Exception ex)
            {
                Exception baseException = ex.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", ex.Message, baseException.Message);
                throw baseException;
            }
        }
    }
}