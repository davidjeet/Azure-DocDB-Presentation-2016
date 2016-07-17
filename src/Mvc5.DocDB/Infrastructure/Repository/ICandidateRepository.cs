namespace Mvc5.DocDB.Infrastructure.Repository
{
    public interface ICandidateRepository : System.IDisposable
    {
        System.Threading.Tasks.Task<Microsoft.Azure.Documents.Client.ResourceResponse<Microsoft.Azure.Documents.Document>> Create(Mvc5.DocDB.Models.Dating.Candidate candidate);
        System.Threading.Tasks.Task CreateCandidateTrigger(string appFolder);
        Models.Dating.Candidate Get(string id);
        System.Collections.Generic.List<Models.Dating.Candidate> GetAll();
        System.Collections.Generic.List<Models.Dating.Candidate> Match(string candidateId, string species, string gender, string currentLocation, string politicalAffiliation);
        System.Threading.Tasks.Task<byte[]> ReadAttachment(string id);
        System.Threading.Tasks.Task SaveAttachment(string id, System.IO.Stream attachment);
        System.Threading.Tasks.Task<Microsoft.Azure.Documents.Client.ResourceResponse<Microsoft.Azure.Documents.Document>> Update(Mvc5.DocDB.Models.Dating.Candidate candidate);
    }
}
