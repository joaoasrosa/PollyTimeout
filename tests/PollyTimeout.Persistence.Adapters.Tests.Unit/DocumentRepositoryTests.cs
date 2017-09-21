using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Moq;
using Polly.Timeout;
using PollyTimeout.Persistence.Adapter;
using PollyTimeout.Persistence.Adapters.Tests.Unit.Stubs;
using Xunit;

namespace PollyTimeout.Persistence.Adapters.Tests.Unit
{
    public class DocumentRepositoryTests
    {
        public DocumentRepositoryTests()
        {
            var clientMock = new Mock<IAmazonS3>();

            clientMock.Setup(x =>
                    x.GetObjectAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(async () =>
                {
                    await Task.Delay(2000);
                    return new GetObjectResponse();
                });

            _policy = new ResiliencePolicyStub();
            _sut = new DocumentRepository(clientMock.Object, _policy);
        }

        private readonly ResiliencePolicyStub _policy;
        private readonly DocumentRepository _sut;

        [Fact]
        public async Task GetDocumentAsync_WhenCallAboveThreshold_ThrowsTimeoutRejectedException()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var exception =
                await Record.ExceptionAsync(async () => await _sut.GetDocumentAsync("dummy", "document.json"));

            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(100), 30);
            exception.Should().BeOfType<TimeoutRejectedException>();
        }

        [Fact]
        public async Task GetDocumentAsync_WhenCallAboveThreshold_TriggersTimeout()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            await Record.ExceptionAsync(async () => await _sut.GetDocumentAsync("dummy", "document.json"));

            stopWatch.Stop();

            stopWatch.Elapsed.Should().BeCloseTo(TimeSpan.FromMilliseconds(100), 30);
            _policy.TimeoutTriggered.Should().BeTrue();
        }
    }
}