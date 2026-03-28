using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TechBlogApi.Dtos.Comment;
using TechBlogApi.Dtos.Post;
using TechBlogApi.Dtos.Tag;
using TechBlogApi.Helpers;
using TechBlogApi.Hubs;
using TechBlogApi.Mappers;
using TechBlogApi.Models;
using TechBlogApi.Services.Abstracts;
using TechBlogApi.UnitOfWorks;

namespace TechBlogApi.Services.Concretes
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IHubContext<NotificationHub> hubContext;

        public PostService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IHubContext<NotificationHub> hubContext)
        {
            this.unitOfWork = unitOfWork;
            contextAccessor = httpContext;
            this.hubContext = hubContext;
        }

        public async Task<ApiResult> AddCommentToPost(CreateCommentDto dto)
        {
            int userId = int.Parse(contextAccessor?.HttpContext?.User.FindFirstValue("userId") ?? "0");

            Comment comment = new()
            {
                Content = dto.Content,
                UserId = userId,
                PostId = dto.PostId
            };
            await unitOfWork.GetWriteRepository<Comment>().AddAsync(comment);
            int result = await unitOfWork.SaveAsync();
            if (result > 0)
            {
                await hubContext.Clients.Group($"post-{dto.PostId}")
            .SendAsync("ReceiveComment", dto.Content);

                return new ApiResult(true, "Created Successfully");
            }
            return new ApiResult(false, "Failed");
        }

        public async Task<ApiResult> AddTagToPost(int postId, int tagId)
        {
            Post post = await unitOfWork.GetReadRepository<Post>().GetAsync(x => x.Id == postId);
            if (post == null) return new ApiResult(false, "Post cant Found");

            Tag tag = await unitOfWork.GetReadRepository<Tag>().GetAsync(x => x.Id == tagId);
            if (tag == null) return new ApiResult(false, "Tag cant Found");

            if (post.Tags != null && post.Tags?.Any(x => x.Id == tagId) == true)
            {
                return new ApiResult(false, "This tag also exisist");
            }
            post.Tags?.Add(tag);
            unitOfWork.GetWriteRepository<Post>().Update(post);
            int result = await unitOfWork.SaveAsync();

            return result > 0
                ? new ApiResult(true, "Tag added successfully")
                : new ApiResult(false, "Failed to add tag");
        }

        public async Task<ApiResult> BookmarkPost(int postId, int userId)
        {
            var bookmark = await unitOfWork.GetReadRepository<PostBookMark>()
                .GetAsync(x => x.PostId == postId && x.UserId == userId);
                
            if (bookmark != null)
                return new ApiResult(false, "Post already bookmarked");
                
            PostBookMark newBookmark = new()
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            
            await unitOfWork.GetWriteRepository<PostBookMark>().AddAsync(newBookmark);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Bookmark added successfully")
                : new ApiResult(false, "Failed to add bookmark");
        }

        public async Task<ApiResult> CreatePost(CreatePostDto dto)
        {
            int userId = int.Parse(contextAccessor?.HttpContext?.User.FindFirstValue("userId") ?? "0");
            HashSet<int>? tagIds = dto.TagIds?.ToHashSet();
            Post post = new()
            {
                Content = dto.Content,
                CategoryId = dto.CategoryId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = userId
            };
            if (tagIds != null && dto.TagIds!.Any())
            {
                List<Tag> tags = await unitOfWork.GetReadRepository<Tag>().GetAllQueryable().Where(x => tagIds.Contains(x.Id)).ToListAsync();
                post.Tags = tags;
            }

            await unitOfWork.GetWriteRepository<Post>().AddAsync(post);
            int result = await unitOfWork.SaveAsync();

            return result > 0
                ? new ApiResult(true, "Post added successfully")
                : new ApiResult(false, "Failed to add post");
        }

        public async Task<ApiResult> DeleteComment(int commentId)
        {
            Comment comment = await unitOfWork.GetReadRepository<Comment>().GetAsync(x => x.Id == commentId);
            if (comment == null) return new ApiResult(false, "Comment cant Found");

            unitOfWork.GetWriteRepository<Comment>().Remove(comment);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Deleted Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult> DeletePost(int id)
        {
            await unitOfWork.GetWriteRepository<Post>().SoftDeleteAsync(id);
            int result = await unitOfWork.SaveAsync();
            return result > 0 ? new ApiResult(true, "Deleted Successfully") : new ApiResult(false, "Failed");
        }

        public async Task<ApiResult<IList<PostDto>>> GetAllPostsAsync(QueryObject query)
        {
            IQueryable<Post> posts = unitOfWork.GetReadRepository<Post>()
            .GetAllQueryable()
            .Include(x => x.User)
            .Include(x => x.Category)
            .Include(x => x.Comments!)
            .ThenInclude(x => x.User)
            .Include(x => x.Tags)
            .OrderByDescending(x => x.CreatedDate);

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                posts = posts.Where(p => p.Content.Contains(query.SearchTerm));
            }
            if (!string.IsNullOrEmpty(query.SortBy))
            {
                posts = query.IsDescending
            ? posts.OrderByDescending(e => EF.Property<object>(e, query.SortBy))
            : posts.OrderBy(e => EF.Property<object>(e, query.SortBy));
            }

            int totalCounts = await posts.CountAsync();
            posts = posts.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize);
            IList<PostDto> postDtos = await posts.Select(x => new PostDto
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                CategoryName = x.Category!.Name,
                UserId = x.UserId,
                UserName = x.User.UserName,
                Content = x.Content,
                Comments = x.Comments!.Select(a => a.ToDto()).ToList(),
                Tags = x.Tags!.Select(x => x.ToDto()).ToList(),
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            }).ToListAsync();
            return new ApiResult<IList<PostDto>>(true, postDtos, totalCount: totalCounts, query.PageNumber, query.PageSize);
        }

        public async Task<ApiResult<PostDto>> GetByIdPostAsync(int id)
        {
            Post post = await unitOfWork.GetReadRepository<Post>()
                .GetAllQueryable()
                .Include(x => x.User)
                .Include(x => x.Category)
                .Include(x => x.Comments!)
                .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == id);
                
            if (post == null) return new ApiResult<PostDto>(false, null!);
            return new ApiResult<PostDto>(true, post.ToDto());
        }

        public async Task<ApiResult<IList<CommentDto>>> GetCommentsByPostAsync(int postId)
        {
            var comments = await unitOfWork.GetReadRepository<Comment>()
                .GetAllQueryable()
                .Include(x => x.User)
                .Where(x => x.PostId == postId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => x.ToDto())
                .ToListAsync();
                
            return new ApiResult<IList<CommentDto>>(true, comments);
        }

        public async Task<ApiResult<IList<PostDto>>> GetPostsByCategoryAsync(int categoryId)
        {
            var posts = await unitOfWork.GetReadRepository<Post>()
                .GetAllQueryable()
                .Include(x => x.User)
                .Include(x => x.Category)
                .Include(x => x.Comments!)
                .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Where(x => x.CategoryId == categoryId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new PostDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category!.Name,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    Content = x.Content,
                    Comments = x.Comments!.Select(a => a.ToDto()).ToList(),
                    Tags = x.Tags!.Select(x => x.ToDto()).ToList(),
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync();
                
            return new ApiResult<IList<PostDto>>(true, posts);
        }

        public async Task<ApiResult<IList<PostDto>>> GetPostsByUserAsync(int userId)
        {
            var posts = await unitOfWork.GetReadRepository<Post>()
                .GetAllQueryable()
                .Include(x => x.User)
                .Include(x => x.Category)
                .Include(x => x.Comments!)
                .ThenInclude(x => x.User)
                .Include(x => x.Tags)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(x => new PostDto
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category!.Name,
                    UserId = x.UserId,
                    UserName = x.User.UserName,
                    Content = x.Content,
                    Comments = x.Comments!.Select(a => a.ToDto()).ToList(),
                    Tags = x.Tags!.Select(x => x.ToDto()).ToList(),
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync();
                
            return new ApiResult<IList<PostDto>>(true, posts);
        }

        public async Task<ApiResult<IList<TagDto>>> GetTagsByPostAsync(int postId)
        {
            var post = await unitOfWork.GetReadRepository<Post>()
                .GetAllQueryable()
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == postId);
                
            if (post == null)
                return new ApiResult<IList<TagDto>>(false, null!);
                
            var tags = post.Tags?.Select(x => x.ToDto()).ToList() ?? new List<TagDto>();
            return new ApiResult<IList<TagDto>>(true, tags);
        }

        public async Task<ApiResult> LikePost(int postId, int userId)
        {
            var existingLike = await unitOfWork.GetReadRepository<PostLike>()
                .GetAsync(x => x.PostId == postId && x.UserId == userId);
                
            if (existingLike != null)
                return new ApiResult(false, "Post already liked");
                
            PostLike like = new()
            {
                PostId = postId,
                UserId = userId,
                CreatedDate = DateTime.UtcNow
            };
            
            await unitOfWork.GetWriteRepository<PostLike>().AddAsync(like);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Post liked successfully")
                : new ApiResult(false, "Failed to like post");
        }

        public async Task<ApiResult> RemoveBookmark(int postId, int userId)
        {
            var bookmark = await unitOfWork.GetReadRepository<PostBookMark>()
                .GetAsync(x => x.PostId == postId && x.UserId == userId);
                
            if (bookmark == null)
                return new ApiResult(false, "Bookmark not found");
                
            unitOfWork.GetWriteRepository<PostBookMark>().Remove(bookmark);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Bookmark removed successfully")
                : new ApiResult(false, "Failed to remove bookmark");
        }

        public async Task<ApiResult> RemoveTagFromPost(int postId, int tagId)
        {
            Post post = await unitOfWork.GetReadRepository<Post>()
                .GetAllQueryable()
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == postId);
                
            if (post == null) 
                return new ApiResult(false, "Post not found");
                
            var tag = post.Tags?.FirstOrDefault(x => x.Id == tagId);
            if (tag == null)
                return new ApiResult(false, "Tag not found on this post");
                
            post.Tags?.Remove(tag);
            unitOfWork.GetWriteRepository<Post>().Update(post);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Tag removed successfully")
                : new ApiResult(false, "Failed to remove tag");
        }

        public async Task<ApiResult> UnlikePost(int postId, int userId)
        {
            var like = await unitOfWork.GetReadRepository<PostLike>()
                .GetAsync(x => x.PostId == postId && x.UserId == userId);
                
            if (like == null)
                return new ApiResult(false, "Like not found");
                
            unitOfWork.GetWriteRepository<PostLike>().Remove(like);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Post unliked successfully")
                : new ApiResult(false, "Failed to unlike post");
        }

        public async Task<ApiResult> UpdateComment(UpdateCommentDto dto)
        {
            var comment = await unitOfWork.GetReadRepository<Comment>()
                .GetAsync(x => x.Id == dto.Id);
                
            if (comment == null)
                return new ApiResult(false, "Comment not found");
                
            int userId = int.Parse(contextAccessor?.HttpContext?.User.FindFirstValue("userId") ?? "0");
            
            // Check if user is the comment owner
            if (comment.UserId != userId)
                return new ApiResult(false, "You can only update your own comments");
                
            comment.Content = dto.Content;
            comment.UpdatedDate = DateTime.UtcNow;
            comment.UpdatedBy = userId;
            
            unitOfWork.GetWriteRepository<Comment>().Update(comment);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Comment updated successfully")
                : new ApiResult(false, "Failed to update comment");
        }

        public async Task<ApiResult> UpdatePost(UpdatePostDto dto)
        {
            var post = await unitOfWork.GetReadRepository<Post>()
                .GetAllQueryable()
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);
                
            if (post == null)
                return new ApiResult(false, "Post not found");
                
            int userId = int.Parse(contextAccessor?.HttpContext?.User.FindFirstValue("userId") ?? "0");
            
            if (post.UserId != userId)
                return new ApiResult(false, "You can only update your own posts");
                
            post.Content = dto.Content;
            post.CategoryId = dto.CategoryId;
            post.UpdatedDate = DateTime.UtcNow;
            post.UpdatedBy = userId;
            
            if (dto.TagIds != null && dto.TagIds.Any())
            {
                HashSet<int> tagIds = dto.TagIds.ToHashSet();
                List<Tag> tags = await unitOfWork.GetReadRepository<Tag>()
                    .GetAllQueryable()
                    .Where(x => tagIds.Contains(x.Id))
                    .ToListAsync();
                post.Tags = tags;
            }
            
            unitOfWork.GetWriteRepository<Post>().Update(post);
            int result = await unitOfWork.SaveAsync();
            
            return result > 0
                ? new ApiResult(true, "Post updated successfully")
                : new ApiResult(false, "Failed to update post");
        }
    }
}