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
            var like = new Like
            {
                PostId = request.PostId,
                UserId = request.UserId,
                LikeType = string.IsNullOrEmpty(request.LikeType) ? "default" : request.LikeType
            };

            await _likeRepository.AddLikeAsync(like);
            return CreatedAtAction(nameof(GetLike), new { id = like.Id }, MapToLikeResponse(like));
        }

        // DELETE api/likes/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLike(Guid id)
        {
            var success = await _likeRepository.DeleteLikeAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        // POST api/likes/unlike
        [HttpPost("unlike")]
        public async Task<ActionResult> UnlikeLike([FromBody] LikeRequest request)
        {
            var success = await _likeRepository.DeleteLikeByUserAndPostAsync(request.UserId, request.PostId);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
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