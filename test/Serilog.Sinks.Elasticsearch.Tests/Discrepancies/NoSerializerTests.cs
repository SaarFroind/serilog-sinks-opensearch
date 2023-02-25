using Xunit;

namespace Serilog.Sinks.OpenSearch.Tests.Discrepancies
{
    public class NoSerializerTests : OpenSearchSinkUniformityTestsBase
    {
        public NoSerializerTests() : base(null) { }

        [Fact]
        public void Should_SerializeToExpandedExceptionObjectWhenExceptionIsSet()
        {
            ThrowAndLogAndCatchBulkOutput("test_with_no_serializer");
        }
    }

}
