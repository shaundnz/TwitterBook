using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Services;

namespace TwitterBook.Controllers.V1
{
    [ApiController]
    [Route("/api/v1/tags")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TagsController : Controller
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;

        public TagsController(IPostService postService, IMapper mapper)
        {
            _postService = postService;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Policy = "TagViewer")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _postService.GetAllTagsAsync();
            var response = _mapper.Map<List<TagResponseDTO>>(tags);
            return Ok(response);
        }

        [HttpGet("secret")]
        [Authorize(Policy = "MustWorkForChapsas")]
        public async Task<IActionResult> ProtectedRoute()
        {
            return Ok("You can see this route because you have an email ending with chapsas.com");
        }
    }
}
