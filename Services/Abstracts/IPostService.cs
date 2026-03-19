using TechBlogApi.Dtos.Comment;
using TechBlogApi.Dtos.Post;
using TechBlogApi.Dtos.Tag;
using TechBlogApi.Helpers;

namespace TechBlogApi.Services.Abstracts
{
    public interface IPostService
    {
        Task<ApiResult<IList<PostDto>>> GetAllPostsAsync(QueryObject query);
        Task<ApiResult<PostDto>> GetByIdPostAsync(int id);
        Task<ApiResult> CreatePost(CreatePostDto dto);
        Task<ApiResult> UpdatePost(UpdatePostDto dto);
        Task<ApiResult> DeletePost(int id);
        Task<ApiResult<IList<PostDto>>> GetPostsByUserAsync(int userId);
        Task<ApiResult<IList<PostDto>>> GetPostsByCategoryAsync(int categoryId);
        Task<ApiResult> LikePost(int postId, int userId);
        Task<ApiResult> UnlikePost(int postId, int userId);
        Task<ApiResult> BookmarkPost(int postId, int userId);
        Task<ApiResult> RemoveBookmark(int postId, int userId);
        Task<ApiResult<IList<CommentDto>>> GetCommentsByPostAsync(int postId);
        Task<ApiResult> AddCommentToPost(CreateCommentDto dto);
        Task<ApiResult> UpdateComment(UpdateCommentDto dto);
        Task<ApiResult> DeleteComment(int commentId);
        Task<ApiResult<IList<TagDto>>> GetTagsByPostAsync(int postId);
        Task<ApiResult> AddTagToPost(int postId, int tagId);
        Task<ApiResult> RemoveTagFromPost(int postId, int tagId);
    }
}