using babbly_like_service.Models;
using babbly_like_service.Services;
using Cassandra;
using Cassandra.Mapping;

namespace babbly_like_service.Repositories
{
    public class CassandraLikeRepository : ILikeRepository
    {
        private readonly CassandraService _cassandraService;
        private readonly IMapper _mapper;

        public CassandraLikeRepository(CassandraService cassandraService)
        {
            _cassandraService = cassandraService;
            _mapper = new Mapper(_cassandraService.GetSession());
        }

        public async Task<Like?> GetLikeAsync(Guid id)
        {
            return await _mapper.SingleOrDefaultAsync<Like>("SELECT * FROM likes WHERE id = ?", id);
        }

        public async Task<Like?> GetLikeByUserAndPostAsync(string userId, Guid postId)
        {
            var result = await _mapper.FetchAsync<Like>("SELECT * FROM likes WHERE user_id = ? AND post_id = ? ALLOW FILTERING", userId, postId);
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Like>> GetLikesByPostAsync(Guid postId)
        {
            return await _mapper.FetchAsync<Like>("SELECT * FROM likes WHERE post_id = ?", postId);
        }

        public async Task<IEnumerable<Like>> GetLikesByUserAsync(string userId)
        {
            return await _mapper.FetchAsync<Like>("SELECT * FROM likes WHERE user_id = ?", userId);
        }

        public async Task<int> GetLikeCountByPostAsync(Guid postId)
        {
            var result = await _mapper.FetchAsync<Like>("SELECT * FROM likes WHERE post_id = ?", postId);
            return result.Count();
        }

        public async Task<IEnumerable<string>> GetUserIdsByPostAsync(Guid postId)
        {
            var result = await _mapper.FetchAsync<Like>("SELECT user_id FROM likes WHERE post_id = ?", postId);
            return result.Select(like => like.UserId);
        }

        public async Task<IEnumerable<Guid>> GetPostIdsByUserAsync(string userId)
        {
            var result = await _mapper.FetchAsync<Like>("SELECT post_id FROM likes WHERE user_id = ?", userId);
            return result.Select(like => like.PostId);
        }

        public async Task AddLikeAsync(Like like)
        {
            // Check if the like already exists
            var existingLike = await GetLikeByUserAndPostAsync(like.UserId, like.PostId);
            if (existingLike != null)
            {
                // If it exists, we don't need to do anything
                return;
            }

            // If it doesn't exist, create a new one
            if (like.Id == Guid.Empty)
            {
                like.Id = Guid.NewGuid();
            }
            
            if (like.CreatedAt == DateTime.MinValue)
            {
                like.CreatedAt = DateTime.UtcNow;
            }

            // Set default like type if not specified
            if (string.IsNullOrEmpty(like.LikeType))
            {
                like.LikeType = "default";
            }

            await _mapper.InsertAsync(like);
        }

        public async Task UpdateLikeAsync(Like like)
        {
            await _mapper.UpdateAsync<Like>(
                "UPDATE likes SET created_at = ?, like_type = ? WHERE id = ?",
                like.CreatedAt, like.LikeType, like.Id);
        }

        public async Task<bool> DeleteLikeAsync(Guid id)
        {
            // Use simple execute statement instead of DeleteAsync
            var session = _cassandraService.GetSession();
            var preparedStatement = await session.PrepareAsync("DELETE FROM likes WHERE id = ?");
            var boundStatement = preparedStatement.Bind(id);
            await session.ExecuteAsync(boundStatement);
            return true;
        }

        public async Task<bool> DeleteLikeByUserAndPostAsync(string userId, Guid postId)
        {
            var like = await GetLikeByUserAndPostAsync(userId, postId);
            if (like == null)
            {
                return false;
            }
            
            await DeleteLikeAsync(like.Id);
            return true;
        }
    }
} 