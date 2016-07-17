using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Mvc5.DocDB.Models.Dating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mvc5.DocDB.Infrastructure.Repository.DocumentDB
{
    public class CandidateRepository : BaseRepository, ICandidateRepository
    {
        private static readonly string collectionName = DocDBCollection.Candidates; // "candidates"
        private static readonly string triggerId = DocDBCollection.MyPostTrigger; // "UpdateCandidateTrigger"

        public CandidateRepository()
        {
            Initialize(collectionName).Wait();
        }

        //GetAll
        public List<Candidate> GetAll()
        {
            var query = client.CreateDocumentQuery<Candidate>(collection.SelfLink, "SELECT * FROM " + collectionName);
            return query.ToList();

            //// * Or, alternative implementation using fluent syntax *
            ////return client.CreateDocumentQuery<Candidate>(collection.DocumentsLink).AsEnumerable().ToList();
        }

        //Get
        public Candidate Get(string id)
        {
            return client.CreateDocumentQuery<Candidate>(collection.DocumentsLink)
                         .Where(d => d.Id == id)
                         .AsEnumerable()
                         .FirstOrDefault();
        }

        //Create
        public async Task<ResourceResponse<Document>> Create(Candidate candidate)
        {
            return await client.CreateDocumentAsync(collection.SelfLink, candidate);
        }

        //Update
        public async Task<ResourceResponse<Document>> Update(Candidate candidate)
        {
            Document doc = client.CreateDocumentQuery(collection.DocumentsLink)
                                .Where(d => d.Id == candidate.Id)
                                .AsEnumerable()
                                .FirstOrDefault();

            return await client.ReplaceDocumentAsync(doc.SelfLink, candidate);
        }

        //Match
        public List<Candidate> Match(string candidateId, string species, string gender, string currentLocation, string politicalAffiliation)
        {
            var sql = new System.Text.StringBuilder();
            sql.AppendFormat("SELECT * FROM {0} c WHERE c.id <> '{1}'", collectionName, candidateId);
            if (!String.IsNullOrEmpty(gender)) sql.AppendFormat(" AND c.Gender = '{0}'", gender);
            if (!String.IsNullOrEmpty(species)) sql.AppendFormat(" AND c.Species = '{0}'", species.Trim());
            if (!String.IsNullOrEmpty(currentLocation)) sql.AppendFormat(" AND c.CurrentLocation = '{0}'", currentLocation.Trim());
            if (!String.IsNullOrEmpty(politicalAffiliation)) sql.AppendFormat(" AND c.PoliticalAffiliation = '{0}'", politicalAffiliation.Trim());

            var query = client.CreateDocumentQuery<Candidate>(collection.SelfLink, sql.ToString());

            return query.AsEnumerable().ToList();
        }

        public async Task CreateCandidateTrigger(string appFolder)
        {
            string triggerPath = Path.Combine(appFolder, "UpdateCandidateTrigger.js");
            string triggerBody = File.ReadAllText(triggerPath);
            var trigger = new Trigger
            {
                Id = Path.GetFileName(triggerId),
                Body = triggerBody,
                TriggerOperation = TriggerOperation.Update,
                TriggerType = TriggerType.Post
            };

            client.TryDeleteTrigger(collection.SelfLink, triggerId).Wait();
            await client.CreateTriggerAsync(collection.SelfLink, trigger);
        }

        public async Task SaveAttachment(string id, Stream attachment)
        {
            Document doc = client.CreateDocumentQuery(collection.DocumentsLink)
                                .Where(d => d.Id == id)
                                .AsEnumerable()
                                .FirstOrDefault();

            await client.CreateAttachmentAsync(doc.AttachmentsLink, attachment);
        }

        public async Task<byte[]> ReadAttachment(string id)
        {
            Document doc = client.CreateDocumentQuery(collection.DocumentsLink)
                                .Where(d => d.Id == id)
                                .AsEnumerable()
                                .FirstOrDefault();

            if (doc == null) return null;
            //Query for document for attachment for attachments
            Attachment attachment = client.CreateAttachmentQuery(doc.SelfLink).AsEnumerable().FirstOrDefault();

            //Use DocumentClient to read the Media content
            if (attachment == null) return null;
            MediaResponse content = await client.ReadMediaAsync(attachment.MediaLink);

            byte[] bytes = new byte[content.ContentLength];
            await content.Media.ReadAsync(bytes, 0, (int)content.ContentLength);
            return bytes;
        }

        #region IDisposable Implementation
        private bool _disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (client != null)
                    {
                        client.Dispose();
                        client = null;
                    }
                    database = null;
                    collection = null;
                }

                _disposed = true;
            }
        }
        #endregion
    }
}