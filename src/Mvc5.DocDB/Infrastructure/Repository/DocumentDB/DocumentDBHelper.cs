using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Mvc5.DocDB.Infrastructure.Configuration;

namespace Mvc5.DocDB.Infrastructure.Repository.DocumentDB
{
    public static class DocumentDBHelper
    {
        public static DocumentClient RetrieveClient()
        {
            return new DocumentClient(new Uri(DocumentDBSettings.Current().DocDBUrl), DocumentDBSettings.Current().DocDBPrimaryKey);           
        }

        public static Database RetrieveDatabase(this DocumentClient client)
        {
            var database = client.CreateDatabaseQuery().Where(db => db.Id == DocumentDBSettings.Current().DatabaseName).AsEnumerable().FirstOrDefault();
            if (database == null) throw new InvalidProgramException("No mataching DocumentDB database was found");
            return database;
        }

        public static async Task<Database> RetrieveOrCreateDatabaseAsync(this DocumentClient client, string databaseId)
        {
            // Try to retrieve the database (Microsoft.Azure.Documents.Database object) whose Id is equal to databaseId            
            var database = client.CreateDatabaseQuery().Where(db => db.Id == databaseId).AsEnumerable().FirstOrDefault();

            // If the previous call didn't return a Database, it is necessary to create it
            if (database == null)
            {
                database = await client.CreateDatabaseAsync(new Database { Id = databaseId });
                Console.WriteLine("Created Database: id - {0} and selfLink - {1}", database.Id, database.SelfLink);
            }

            return database;
        }

        public static async Task<DocumentCollection> RetrieveOrCreateCollectionAsync(this DocumentClient client, string databaseSelfLink, string id)
        {
            // Try to retrieve the collection (Microsoft.Azure.Documents.DocumentCollection) whose Id is equal to collectionId
            var collection = client.CreateDocumentCollectionQuery(databaseSelfLink).Where(c => c.Id == id).ToArray().FirstOrDefault();

            // If the previous call didn't return a Collection, it is necessary to create it
            if (collection == null)
            {
                collection = await client.CreateDocumentCollectionAsync(databaseSelfLink, new DocumentCollection { Id = id });
            }

            return collection;
        }

        public static DocumentCollection RetrieveOrCreateCollection(this DocumentClient client, string databaseLink, string CollectionId)
        {
            var col = client.CreateDocumentCollectionQuery(databaseLink)
                              .Where(c => c.Id == CollectionId)
                              .AsEnumerable()
                              .FirstOrDefault();

            if (col == null)
            {
                col = client.CreateDocumentCollectionAsync(databaseLink, new DocumentCollection { Id = CollectionId }).Result;
            }

            return col;
        }

        /// <summary>If a Stored Procedure is found on the DocumentCollection for the Id supplied it is deleted.</summary>
        /// <param name="colSelfLink">DocumentCollection to search for the Stored Procedure</param>
        /// <param name="sprocId">Id of the Stored Procedure to delete</param>
        /// <returns>An ResourceResponse which can be ignored.</returns>
        public static async Task TryDeleteStoredProcedure(this DocumentClient client, string colSelfLink, string sprocId)
        {
            StoredProcedure sproc = client.CreateStoredProcedureQuery(colSelfLink).Where(s => s.Id == sprocId).AsEnumerable().FirstOrDefault();
            if (sproc != null)
            {
                await client.DeleteStoredProcedureAsync(sproc.SelfLink);
            }
        }


        /// <summary>If a Trigger is found on the DocumentCollection for the Id supplied it is deleted.</summary>
        /// <param name="colSelfLink">DocumentCollection to search for the Trigger</param>
        /// <param name="triggerId">Id of the Trigger to delete</param>
        /// <returns>An ResourceResponse which can be ignored.</returns>
        public static async Task TryDeleteTrigger(this DocumentClient client, string colSelfLink, string triggerId)
        {
            Trigger trigger = client.CreateTriggerQuery(colSelfLink).Where(t => t.Id == triggerId).AsEnumerable().FirstOrDefault();
            if (trigger != null)
            {
                await client.DeleteTriggerAsync(trigger.SelfLink);
            }
        }

    }
}