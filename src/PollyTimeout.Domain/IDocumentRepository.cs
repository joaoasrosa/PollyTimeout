using System.Threading.Tasks;

namespace PollyTimeout.Domain
{
    public interface IDocumentRepository
    {
        Task<Document> GetDocumentAsync(string bucket, string objectName);
    }
}