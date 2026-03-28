using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechBlogApi.Dtos.Post;
using TechBlogApi.Models;

namespace TechBlogApi.Mappers
{
    public static class PostMapping
    {
        public static PostDto ToDto(this Post post)
        {
            return new PostDto
            {
                Id = post.Id,
                CategoryId = post.CategoryId,
                Content = post.Content,
                CreatedBy = post.CreatedBy,
                CreatedDate = post.CreatedDate,
                UpdatedBy = post.UpdatedBy,
                UpdatedDate = post.UpdatedDate
            };
        }
    }
}