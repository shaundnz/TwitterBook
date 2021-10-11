using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.Models;

namespace TwitterBook.Services
{
    public interface IPostService
    {

        List<Post> GetPosts();

        Post GetPostById(Guid postId);

        bool UpdatedPost(Post postToUpdate);

        bool DeletePost(Guid postId);
    }
}
