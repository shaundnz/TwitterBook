using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TwitterBook.DTO.V1.Requests;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Models;
using Xunit;

namespace TwitterBook.IntegrationTests
{
    public class PostControllerTests: IntegrationTests
    {
        [Trait("Posts", "Get")]
        [Fact(DisplayName = "GET Posts with no posts returns empty response")]
        public async Task GetAll_WithoutAnyPosts()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await TestClient.GetAsync("/api/v1/posts");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<List<Post>>()).Should().BeEmpty();
            
        }

        [Trait("Posts", "Get")]
        [Fact(DisplayName = "GET Posts returns posts when posts exists")]
        public async Task GetAll_WithPosts()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(new CreatePostRequestDTO { Name = "Created test post" });

            // Act
            var res = await TestClient.GetAsync($"/api/v1/posts/{createdPost.Id}");

            // Assert
            res.StatusCode.Should().Be(HttpStatusCode.OK);
            var returnedPost = await res.Content.ReadFromJsonAsync<Post>();
            returnedPost.Id.Should().Be(createdPost.Id);
            returnedPost.Name.Should().Be("Created test post");
           
        }
    }
}
