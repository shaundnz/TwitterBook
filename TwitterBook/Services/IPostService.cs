using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.Models;

namespace TwitterBook.Services
{
    public interface IPostService
    {

        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(Guid postId);

        Task<bool> CreatePostAsync(Post post);

        Task<bool> UpdatedPostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> UserOwnsPostAsync(Guid postId, string userId);

        Task<List<Tag>> GetAllTagsAsync();
    }
}
