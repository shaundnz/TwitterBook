using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitterBook.DTO.V1.Requests;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Extensions;
using TwitterBook.Models;
using TwitterBook.Services;

namespace TwitterBook.Controllers.V1
{
    [ApiController]
    [Route("/api/v1/posts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {

        private readonly IPostService _postService;


        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts()
        {
            return Ok(await _postService.GetPostsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestDTO createPostRequestDTO)
        {

            var post = new Post { Name = createPostRequestDTO.Name, 
                Tags = createPostRequestDTO.Tags.Select(t => new Tag { TagString = t }).ToList(), 
                UserId = HttpContext.GetUserId() 
            };
            await _postService.CreatePostAsync(post);

            var response = new CreatePostResponseDTO { Id = post.Id, Tags = post.Tags.Select(t => t.TagString).ToList() };
            return CreatedAtAction(nameof(GetPost), new { postId = post.Id}, response);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetPost([FromRoute] Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null) return NotFound();

            return Ok(post);
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost([FromRoute] Guid postId, [FromBody] UpdatePostRequestDTO updatePostRequestDTO)
        {

            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost) return Unauthorized(new { error = "You cannot modify this post" });

            var post = await _postService.GetPostByIdAsync(postId);
            post.Name = updatePostRequestDTO.Name;

            var updated = await _postService.UpdatedPostAsync(post);

            if (!updated) return NotFound();

            return Ok(post);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postService.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost) return Unauthorized(new { error = "You cannot delete this post" });

            var deleted = await _postService.DeletePostAsync(postId);

            // Could return 200 OK with item in the body here
            if (!deleted) return NotFound();

            return NoContent();
        }

    }
}