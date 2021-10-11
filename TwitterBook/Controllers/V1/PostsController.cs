using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitterBook.DTO.V1.Requests;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Models;
using TwitterBook.Services;

namespace TwitterBook.Controllers.V1
{
    [ApiController]
    [Route("/api/v1/posts")]
    public class PostsController : Controller
    {

        private readonly IPostService _postService;


        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public IActionResult GetAllPosts()
        {
            return Ok(_postService.GetPosts());
        }

        [HttpPost]
        public IActionResult CreatePost([FromBody] CreatePostRequestDTO createPostRequestDTO)
        {

            var post = new Post { Id = createPostRequestDTO.Id };

            if (post.Id == Guid.Empty)
            {
                post.Id = Guid.NewGuid();
            }

            _postService.GetPosts().Add(post);

            var response = new CreatePostResponseDTO { Id = post.Id };
            return CreatedAtAction(nameof(GetPost), new { postId = post.Id}, response);
        }

        [HttpGet("{postId}")]
        public IActionResult GetPost([FromRoute] Guid postId)
        {
            var post = _postService.GetPostById(postId);
            if (post == null) return NotFound();

            return Ok(post);
        }

        [HttpPut("{postId}")]
        public IActionResult UpdatePost([FromRoute] Guid postId, [FromBody] UpdatePostRequestDTO updatePostRequestDTO)
        { 

            var post = new Post
            {
                Id = postId,
                Name = updatePostRequestDTO.Name
            };

            var updated = _postService.UpdatedPost(post);

            if (!updated) return NotFound();

            return Ok(post);
        }

        [HttpDelete("{postId}")]
        public IActionResult DeletePost([FromRoute] Guid postId)
        {
            var deleted = _postService.DeletePost(postId);

            // Could return 200 OK with item in the body here
            if (!deleted) return NotFound();

            return NoContent();
        }

    }
}