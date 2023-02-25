using System;
using FluentAssertions;
using Serilog.Sinks.OpenSearch.Tests.Stubs;
using Xunit;

namespace Serilog.Sinks.OpenSearch.Tests.Templating
{
    public class TemplateMatchTests : OpenSearchSinkTestsBase
    {
        private readonly Tuple<Uri, string> _templatePut;

        public TemplateMatchTests()
            : base("7.0.0")
        {
            _options.AutoRegisterTemplate = true;
            _options.IndexFormat = "dailyindex-{0:yyyy.MM.dd}-mycompany";
            _options.TemplateName = "dailyindex-logs-template";
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
            _seenHttpHeads.Should().NotBeNullOrEmpty().And.HaveCount(1);
            _templatePut = _seenHttpPuts[0];

        }

        [Fact]
        public void TemplatePutToCorrectUrl()
        {
            var uri = _templatePut.Item1;
            uri.AbsolutePath.Should().Be("/_template/dailyindex-logs-template");
        }

        [Fact]
        public void TemplateMatchShouldReflectConfiguredIndexFormat()
        {
            var json = _templatePut.Item2;
            json.Should().Contain(@"""index_patterns"":[""dailyindex-*-mycompany""]");
        }

    }
}