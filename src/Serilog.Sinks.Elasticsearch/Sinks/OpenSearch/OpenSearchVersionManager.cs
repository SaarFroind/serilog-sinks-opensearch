#nullable enable
using OpenSearch.Net;
using Serilog.Debugging;
using System;

namespace Serilog.Sinks.OpenSearch
{
    /// <summary>
    /// Encapsulates detection of Elasticsearch version
    /// and fallback in case of detection failiure.
    /// </summary>
    internal class OpenSearchVersionManager
    {
        private readonly bool _detectElasticsearchVersion;
        private readonly IOpenSearchLowLevelClient _client;

        /// <summary>
        /// We are defaulting to version 1.2.0
        /// </summary>
        public readonly Version DefaultVersion = new(1, 2);
        public Version? DetectedVersion { get; private set; }
        public bool DetectionAttempted { get; private set; }

        public OpenSearchVersionManager(
            bool detectElasticsearchVersion,
            IOpenSearchLowLevelClient client)
        {
            _detectElasticsearchVersion = detectElasticsearchVersion;
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Version EffectiveVersion
        {
            get
            {
                if (DetectedVersion is not null)
                    return DetectedVersion;

                if (_detectElasticsearchVersion == false
                    || DetectionAttempted == true)
                    return DefaultVersion;

                // Attemp once
                DetectedVersion = DiscoverClusterVersion();

                return DetectedVersion ?? DefaultVersion;
            }
        }

        internal Version? DiscoverClusterVersion()
        {
            try
            {
                var response = _client.DoRequest<DynamicResponse>(HttpMethod.GET, "/");
                if (!response.Success) return null;

                var discoveredVersion = response.Dictionary["version"]["number"];

                if (!discoveredVersion.HasValue)
                    return null;

                if (discoveredVersion.Value is not string strVersion)
                    return null;

                return new Version(strVersion);

            }
            catch (Exception ex)
            {
                SelfLog.WriteLine("Failed to discover the cluster version. {0}", ex);
                return null;
            }
            finally
            {
                DetectionAttempted = true;
            }
        }
    }
}
