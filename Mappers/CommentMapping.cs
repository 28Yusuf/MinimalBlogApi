using TechBlogApi.Dtos.Comment;
using TechBlogApi.Models;

namespace TechBlogApi.Mappers
{
    public static class CommentMapping
    {
        public static CommentDto ToDto(this Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                UserName = comment.User?.UserName!,
                PostId = comment.PostId,
                CreatedBy = comment.CreatedBy,
                CreatedDate = comment.CreatedDate,
                UpdatedBy = comment.UpdatedBy,
                UpdatedDate = comment.UpdatedDate
            };
        }
    }
}