using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TwitterBook.Data;
using TwitterBook.DTO.V1.Requests;
using TwitterBook.DTO.V1.Responses;
using TwitterBook.Models;

namespace TwitterBook.IntegrationTests
{
    public class IntegrationTests {

        protected readonly HttpClient TestClient;
        

        public IntegrationTests()
        {
           
            var appFactory = new WebApplicationFactory<Startup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                    services.Remove(descriptor);
                    services.AddDbContext<DataContext>(options =>
                    { 
                        options.UseSqlite("Data Source=TestingDatabase.sqlite");
                    });

                    var sp = services.BuildServiceProvider();
                    using (var scope = sp.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<DataContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                    }
                });
               
            });
            TestClient = appFactory.CreateClient();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var res = await TestClient.PostAsJsonAsync("/api/identity/register", new UserRegistrationRequestDTO
            {
                Email = "test@email.com",
                Password = "Abc@123"
            });

            var loginResponseBody = await res.Content.ReadFromJsonAsync<AuthSuccessResponseDTO>();

            return loginResponseBody.Token;

        }

        protected async Task<CreatePostResponseDTO> CreatePostAsync(CreatePostRequestDTO req)
        {
            var res = await TestClient.PostAsJsonAsync("/api/v1/posts", req);
            var resContent = await res.Content.ReadFromJsonAsync<CreatePostResponseDTO>();
            return resContent;
        }
    }
}
