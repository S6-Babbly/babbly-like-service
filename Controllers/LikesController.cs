using babbly_like_service.Models;
using babbly_like_service.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace babbly_like_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LikesController : ControllerBase
    {
        private readonly ILikeRepository _likeRepository;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikeRepository likeRepository, ILogger<LikesController> logger)
        {
            _likeRepository = likeRepository;
            _logger = logger;
        }

        // GET api/likes/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LikeResponse>> GetLike(Guid id)
        {
            var like = await _likeRepository.GetLikeAsync(id);
            if (like == null)
            {
                return NotFound();
            }

            return Ok(MapToLikeResponse(like));
        }

        // GET api/likes/post/{postId}
        [HttpGet("post/{postId}")]
        public async Task<ActionResult<IEnumerable<LikeResponse>>> GetLikesByPost(Guid postId)
        {
            var likes = await _likeRepository.GetLikesByPostAsync(postId);
            return Ok(likes.Select(like => MapToLikeResponse(like)));
        }

        // GET api/likes/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<LikeResponse>>> GetLikesByUser(string userId)
        {
            var likes = await _likeRepository.GetLikesByUserAsync(userId);
            return Ok(likes.Select(like => MapToLikeResponse(like)));
        }

        // GET api/likes/post/{postId}/count
        [HttpGet("post/{postId}/count")]
        public async Task<ActionResult<LikeCountResponse>> GetLikeCountByPost(Guid postId)
        {
            var count = await _likeRepository.GetLikeCountByPostAsync(postId);
            return Ok(new LikeCountResponse { PostId = postId, Count = count });
        }

        // GET api/likes/post/{postId}/users
        [HttpGet("post/{postId}/users")]
        public async Task<ActionResult<PostLikesResponse>> GetUserIdsByPost(Guid postId)
        {
            var userIds = await _likeRepository.GetUserIdsByPostAsync(postId);
            return Ok(new PostLikesResponse { PostId = postId, UserIds = userIds.ToList() });
        }

        // GET api/likes/user/{userId}/posts
        [HttpGet("user/{userId}/posts")]
        public async Task<ActionResult<UserLikesResponse>> GetPostIdsByUser(string userId)
        {
            var postIds = await _likeRepository.GetPostIdsByUserAsync(userId);
            return Ok(new UserLikesResponse { UserId = userId, LikedPosts = postIds.ToList() });
        }

        // GET api/likes/check
        [HttpGet("check")]
        public async Task<ActionResult<bool>> CheckLikeExists([FromQuery] string userId, [FromQuery] Guid postId)
        {
            var like = await _likeRepository.GetLikeByUserAndPostAsync(userId, postId);
            return Ok(like != null);
        }

        // POST api/likes
        [HttpPost]
        public async Task<ActionResult<LikeResponse>> AddLike(LikeRequest request)
        {
            // Get authenticated user ID from JWT headers (forwarded by API Gateway)
            var userId = Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { error = "Authentication required. User ID not found in token." });
            }

            try
            {
                // Check if user already liked this post
                var existingLike = await _likeRepository.GetLikeByUserAndPostAsync(userId, request.PostId);
                if (existingLike != null)
                {
                    return Conflict(new { error = "User has already liked this post" });
                }

                var like = new Like
                {
                    PostId = request.PostId,
                    UserId = userId,
                    LikeType = string.IsNullOrEmpty(request.LikeType) ? "default" : request.LikeType
                };

                await _likeRepository.AddLikeAsync(like);
                
                // Get updated like count for the post
                var updatedLikeCount = await _likeRepository.GetLikeCountByPostAsync(request.PostId);
                
                _logger.LogInformation("User {UserId} liked post {PostId}", userId, request.PostId);
                
                // Return response with like count that frontend expects
                var response = new
                {
                    like = MapToLikeResponse(like),
                    likes = (int)updatedLikeCount,
                    postId = request.PostId
                };
                
                return CreatedAtAction(nameof(GetLike), new { id = like.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding like for user {UserId} and post {PostId}", userId, request.PostId);
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE api/likes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLike(Guid id)
        {
            // Get authenticated user ID from JWT headers
            var userId = Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { error = "Authentication required. User ID not found in token." });
            }

            try
            {
                // Check if like exists and belongs to the user
                var like = await _likeRepository.GetLikeAsync(id);
                if (like == null)
                {
                    return NotFound(new { error = "Like not found" });
                }

                if (like.UserId != userId)
                {
                    return Forbid("You can only delete your own likes");
                }

                var success = await _likeRepository.DeleteLikeAsync(id);
                if (!success)
                {
                    return NotFound(new { error = "Like not found" });
                }

                _logger.LogInformation("User {UserId} deleted like {LikeId}", userId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting like {LikeId} for user {UserId}", id, userId);
                return StatusCode(500, "Internal server error");
            }
        }

        // POST api/likes/unlike
        [HttpPost("unlike")]
        public async Task<ActionResult> UnlikeLike([FromBody] LikeRequest request)
        {
            // Get authenticated user ID from JWT headers
            var userId = Request.Headers["X-User-Id"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new { error = "Authentication required. User ID not found in token." });
            }

            try
            {
                var success = await _likeRepository.DeleteLikeByUserAndPostAsync(userId, request.PostId);
                if (!success)
                {
                    return NotFound(new { error = "Like not found or user has not liked this post" });
                }

                _logger.LogInformation("User {UserId} unliked post {PostId}", userId, request.PostId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unliking post {PostId} for user {UserId}", request.PostId, userId);
                return StatusCode(500, "Internal server error");
            }
        }

        private static LikeResponse MapToLikeResponse(Like like)
        {
            return new LikeResponse
            {
                Id = like.Id,
                PostId = like.PostId,
                UserId = like.UserId,
                CreatedAt = like.CreatedAt,
                LikeType = like.LikeType
            };
        }
    }
} 