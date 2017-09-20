using System.Net;
using System.Threading.Tasks;
using Amazon.S3;
using PollyTimeout.Domain;

namespace PollyTimeout.Persistence.Adapter
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ResiliencePolicy _resiliencePolicy;
        private readonly IAmazonS3 _s3Client;

        public DocumentRepository(IAmazonS3 s3Client, ResiliencePolicy resiliencePolicy)
        {
            _s3Client = s3Client;
            _resiliencePolicy = resiliencePolicy;
        }

        public async Task<Document> GetDocumentAsync(string bucket, string objectName)
        {
            Document document = null;

            var response =
                await _resiliencePolicy.Policy.ExecuteAsync(async () =>
                    await _s3Client.GetObjectAsync(bucket, objectName));

            if (response.HttpStatusCode == HttpStatusCode.OK)
                document = new Document
                {
                    Name = objectName,
                    Version = response.VersionId
                };

            return document;
        }
    }
}