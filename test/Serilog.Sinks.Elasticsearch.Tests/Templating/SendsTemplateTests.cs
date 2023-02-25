using System;
using FluentAssertions;
using Serilog.Sinks.OpenSearch.Tests.Stubs;
using Xunit;

namespace Serilog.Sinks.OpenSearch.Tests.Templating
{
    public class SendsTemplateTests : OpenSearchSinkTestsBase
    {
        private readonly Tuple<Uri, string> _templatePut;

        public SendsTemplateTests()
        {
            _options.AutoRegisterTemplate = true;

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.OpenSearch(_options);

            var logger = loggerConfig.CreateLogger();
            using (logger as IDisposable)
            {
                logger.Error("Test exception. Should not contain an embedded exception object.");
            }

            _seenHttpPosts.Should().NotBeNullOrEmpty().And.HaveCount(1);
            _seenHttpPuts.Should().NotBeNullOrEmpty().And.HaveCount(1);
            _templatePut = _seenHttpPuts[0];
        }

        [Fact]
        public void ShouldRegisterTheVersion7TemplateOnRegistrationWhenDetectElasticsearchVersionFalse()
        {
            JsonEquals(_templatePut.Item2, "template_v7_no-aliases.json");
        }

        [Fact]
        public void TemplatePutToCorrectUrl()
        {
            var uri = _templatePut.Item1;
            uri.AbsolutePath.Should().Be("/_template/serilog-events-template");
        }
    }
}