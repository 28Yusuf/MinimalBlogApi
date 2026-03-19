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

        public Task<ApiResult> BookmarkPost(int postId, int userId)
        {
            throw new NotImplementedException();
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
            };
            if (tagIds != null && dto.TagIds.Any())
            {
                List<Tag> tags = await unitOfWork.GetReadRepository<Tag>().GetAllQueryable().Where(x => tagIds!.Contains(x.Id)).ToListAsync();
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
                posts.Where(p => p.Content.Contains(query.SearchTerm));
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

        public Task<ApiResult<PostDto>> GetByIdPostAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<IList<CommentDto>>> GetCommentsByPostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<IList<PostDto>>> GetPostsByCategoryAsync(int categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<IList<PostDto>>> GetPostsByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<IList<TagDto>>> GetTagsByPostAsync(int postId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> LikePost(int postId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> RemoveBookmark(int postId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> RemoveTagFromPost(int postId, int tagId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> UnlikePost(int postId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> UpdateComment(UpdateCommentDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult> UpdatePost(UpdatePostDto dto)
        {
            throw new NotImplementedException();
        }
    }
}