using babbly_like_service.Models;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.Extensions.Options;

namespace babbly_like_service.Services
{
    public class CassandraService
    {
        private readonly ICluster _cluster;
        private readonly string _keyspace;

        public CassandraService(IOptions<CassandraSettings> cassandraSettings)
        {
            var settings = cassandraSettings.Value;
            _keyspace = settings.Keyspace;

            var clusterBuilder = Cluster.Builder()
                .AddContactPoints(settings.Hosts)
                .WithDefaultKeyspace(_keyspace);

            if (!string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password))
            {
                clusterBuilder.WithCredentials(settings.Username, settings.Password);
            }

            _cluster = clusterBuilder.Build();
            
            // Register mappings
            MappingConfiguration.Global.Define<LikeMappings>();
        }

        public Cassandra.ISession GetSession()
        {
            return _cluster.Connect(_keyspace);
        }
    }
} 