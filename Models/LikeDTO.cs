namespace babbly_like_service.Models
{
    public class LikeRequest
    {
        public Guid PostId { get; set; }
        public string LikeType { get; set; } = "default";
    }

    public class LikeResponse
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string LikeType { get; set; } = "default";
    }

    public class LikeCountResponse
    {
        public Guid PostId { get; set; }
        public int Count { get; set; }
    }

    public class UserLikesResponse
    {
        public string UserId { get; set; } = string.Empty;
        public List<Guid> LikedPosts { get; set; } = new List<Guid>();
    }

    public class PostLikesResponse
    {
        public Guid PostId { get; set; }
        public List<string> UserIds { get; set; } = new List<string>();
    }
} 