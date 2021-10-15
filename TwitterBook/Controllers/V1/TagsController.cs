using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.Services;

namespace TwitterBook.Controllers.V1
{
    [ApiController]
    [Route("/api/v1/tags")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;

        public TagsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetTags()
        {
            return Ok(await _postService.GetAllTagsAsync());
        }
    }
}
