using Xunit;

namespace Serilog.Sinks.OpenSearch.Tests
{
    public class OpenSearchSinkTests
    {
        [Theory]
        [InlineData("my-logevent", null)]
        [InlineData("my-logevent", "my-logevent")]
        [InlineData(null, null)]
        [InlineData(null, "logevent")]
        public void Ctor_DetectElasticsearchVersionSetToTrue_SetsTypeName(string configuredTypeName, string expectedTypeName)
        {
            /* ARRANGE */
            var options = new OpenSearchSinkOptions
            {
                TypeName = configuredTypeName
            };

            /* ACT */
            _ = OpenSearchSinkState.Create(options);

            /* Assert */
            Assert.Equal(expectedTypeName, options.TypeName);
        }

        [Theory]
        [InlineData("my-logevent", null)]
        [InlineData(null, null)]
        public void Ctor_DetectElasticsearchVersionSetToFalseAssumesVersion7_SetsTypeNameToNull(string configuredTypeName, string expectedTypeName)
        {
            /* ARRANGE */
            var options = new OpenSearchSinkOptions
            {
                DetectOpenSearchVersion = false,
                TypeName = configuredTypeName
            };

            /* ACT */
            _ = OpenSearchSinkState.Create(options);

            /* Assert */
            Assert.Equal(expectedTypeName, options.TypeName);
        }

        [Theory]
        [InlineData("my-logevent", null)]
        [InlineData("my-logevent", "my-logevent")]
        [InlineData(null, null)]
        [InlineData(null, "logevent")]
        public void CreateLogger_DetectElasticsearchVersionSetToTrue_SetsTypeName(string configuredTypeName, string expectedTypeName)
        {
            /* ARRANGE */
            var options = new OpenSearchSinkOptions
            {
                DetectOpenSearchVersion = true,
                TypeName = configuredTypeName
            };

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.OpenSearch(options);

            /* ACT */
            _ = loggerConfig.CreateLogger();

            /* Assert */
            Assert.Equal(expectedTypeName, options.TypeName);
        }

    }
}
