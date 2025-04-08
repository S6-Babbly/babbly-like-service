namespace babbly_like_service.Services
{
    public class CassandraSettings
    {
        public string[] Hosts { get; set; } = Array.Empty<string>();
        public string Keyspace { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
} 