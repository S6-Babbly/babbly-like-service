using System;

namespace babbly_like_service.Models
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string LikeType { get; set; } = "default";
    }
} 