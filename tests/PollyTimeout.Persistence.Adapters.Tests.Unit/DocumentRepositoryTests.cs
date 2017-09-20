using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Moq;
using PollyTimeout.Persistence.Adapter;
using PollyTimeout.Persistence.Adapters.Tests.Unit.Stubs;
using Xunit;

namespace PollyTimeout.Persistence.Adapters.Tests.Unit
{
    public class DocumentRepositoryTests
    {
        public DocumentRepositoryTests()
        {
            _clientMock = new Mock<IAmazonS3>();
            _policy = new ResiliencePolicyStub();
            _sut = new DocumentRepository(_clientMock.Object, _policy);
        }

        private readonly Mock<IAmazonS3> _clientMock;
        private readonly ResiliencePolicyStub _policy;
        private readonly DocumentRepository _sut;

        [Fact]
        public async Task GetDocumentAsync_WhenCallAboveThreshold_TriggersTimeout()
        {
            _clientMock.Setup(x =>
                    x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback((string b, string o, CancellationToken c) => { Thread.Sleep(2000); })
                .ReturnsAsync(new GetObjectResponse());

            await Record.ExceptionAsync(async () => await _sut.GetDocumentAsync("dummy", "document.json"));

            _policy.TimeoutTriggered.Should().BeTrue();
        }
    }
}