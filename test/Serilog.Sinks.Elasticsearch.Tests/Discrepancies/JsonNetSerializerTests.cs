using OpenSearch.Net;
using OpenSearch.Client;
using OpenSearch.Client.JsonNetSerializer;
using Xunit;

namespace Serilog.Sinks.OpenSearch.Tests.Discrepancies
{
    public class JsonNetSerializerTests : OpenSearchSinkUniformityTestsBase
    {
        public JsonNetSerializerTests() : base(JsonNetSerializer.Default(LowLevelRequestResponseSerializer.Instance, new ConnectionSettings())) { }

        [Fact]
        public void Should_SerializeToExpandedExceptionObjectWhenExceptionIsSet()
        {
            ThrowAndLogAndCatchBulkOutput("test_with_jsonnet_serializer");
        }
    }

}
