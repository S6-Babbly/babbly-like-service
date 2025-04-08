using Cassandra.Mapping;

namespace babbly_like_service.Models
{
    public class LikeMappings : Mappings
    {
        public LikeMappings()
        {
            For<Like>()
                .TableName("likes")
                .PartitionKey(l => l.Id)
                .Column(l => l.Id, cm => cm.WithName("id"))
                .Column(l => l.PostId, cm => cm.WithName("post_id"))
                .Column(l => l.UserId, cm => cm.WithName("user_id"))
                .Column(l => l.CreatedAt, cm => cm.WithName("created_at"))
                .Column(l => l.LikeType, cm => cm.WithName("like_type"));
        }
    }
} 