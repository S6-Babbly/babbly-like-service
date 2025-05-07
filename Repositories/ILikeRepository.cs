using babbly_like_service.Models;

namespace babbly_like_service.Repositories
{
    public interface ILikeRepository
    {
        Task<Like?> GetLikeAsync(Guid id);
        Task<Like?> GetLikeByUserAndPostAsync(string userId, Guid postId);
        Task<IEnumerable<Like>> GetLikesByPostAsync(Guid postId);
        Task<IEnumerable<Like>> GetLikesByUserAsync(string userId);
        Task<int> GetLikeCountByPostAsync(Guid postId);
        Task<IEnumerable<string>> GetUserIdsByPostAsync(Guid postId);
        Task<IEnumerable<Guid>> GetPostIdsByUserAsync(string userId);
        Task AddLikeAsync(Like like);
        Task UpdateLikeAsync(Like like);
        Task<bool> DeleteLikeAsync(Guid id);
        Task<bool> DeleteLikeByUserAndPostAsync(string userId, Guid postId);
    }
} 