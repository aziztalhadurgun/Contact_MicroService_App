using Contact.Users.DataAccess;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace Contact.UnitTests
{
    public class UserTest : WebApplicationFactory<User>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var root = new InMemoryDatabaseRoot();

            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<UserDbContext>));
                services.AddDbContext<UserDbContext>(options =>
                    options.UseInMemoryDatabase("Testing", root));
            });

            return base.CreateHost(builder);
        }

        [Fact]
        public async Task GetUsers()
        {
            await using var application = new UserTest();

            var client = application.CreateClient();
            var users = await client.GetFromJsonAsync<List<User>>("/users");

            Assert.Empty(users);
        }

        [Fact]
        public async Task GetUserDetail()
        {
            await using var application = new UserTest();

            var client = application.CreateClient();
            var users = await client.GetFromJsonAsync<List<User>>("/userDetails");

            Assert.Empty(users);
        }


        [Fact]
        public async Task AddUser()
        {
            await using var application = new UserTest();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Name1",
                Surname = "Surname1",
                Company = "Company1"
            };

            var client = application.CreateClient();
            var response = await client.PostAsJsonAsync("/userAdd", user);
            var result = await response.Content.ReadFromJsonAsync<User>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
        }

    }
}