using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using OpenSearch.Net;
using Serilog.Formatting.Elasticsearch;

namespace Serilog.Sinks.OpenSearch
{
    internal class SerializerAdapter : ISerializer, IOpenSearchSerializer
    {
        private readonly IOpenSearchSerializer _elasticsearchSerializer;

        internal SerializerAdapter(IOpenSearchSerializer elasticsearchSerializer)
        {
            _elasticsearchSerializer = elasticsearchSerializer ??
                                       throw new ArgumentNullException(nameof(elasticsearchSerializer));
        }

        public object Deserialize(Type type, Stream stream)
        {
            return _elasticsearchSerializer.Deserialize(type, stream);
        }

        public T Deserialize<T>(Stream stream)
        {
            return _elasticsearchSerializer.Deserialize<T>(stream);
        }

        public Task<object> DeserializeAsync(Type type, Stream stream,
           CancellationToken cancellationToken = default)
        {
            return _elasticsearchSerializer.DeserializeAsync(type, stream, cancellationToken);
        }

        public Task<T> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
        {
            return _elasticsearchSerializer.DeserializeAsync<T>(stream, cancellationToken);
        }

        public void Serialize<T>(T data, Stream stream,
           SerializationFormatting formatting = SerializationFormatting.Indented)
        {
            _elasticsearchSerializer.Serialize(data, stream, formatting);
        }

        public Task SerializeAsync<T>(T data, Stream stream,
           SerializationFormatting formatting = SerializationFormatting.Indented,
           CancellationToken cancellationToken = default)
        {
            return _elasticsearchSerializer.SerializeAsync(data, stream, formatting, cancellationToken);
        }

        public string SerializeToString(object value)
        {
            return _elasticsearchSerializer.SerializeToString(value, formatting: SerializationFormatting.None);
        }
    }
}