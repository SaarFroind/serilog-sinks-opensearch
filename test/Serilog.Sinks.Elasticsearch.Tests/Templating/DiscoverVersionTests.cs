using System;
using FluentAssertions;
using Serilog.Sinks.OpenSearch.Tests.Stubs;
using Xunit;

namespace Serilog.Sinks.OpenSearch.Tests.Templating
{
    public class DiscoverVersionTests : OpenSearchSinkTestsBase
    {
        private readonly Tuple<Uri, int> _templateGet;

        public DiscoverVersionTests()
        {
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.OpenSearch(_options);

            var logger = loggerConfig.CreateLogger();
            using ((IDisposable)logger)
            {
                logger.Error("Test exception. Should not contain an embedded exception object.");
            }

            _seenHttpGets.Should().NotBeNullOrEmpty().And.HaveCount(1);
            _templateGet = _seenHttpGets[0];
        }


        [Fact]
        public void TemplatePutToCorrectUrl()
        {
            var uri = _templateGet.Item1;
            uri.AbsolutePath.Should().Be("/");
        }
    }
}