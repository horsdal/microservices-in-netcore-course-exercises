using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LoyaltyProgram;
using LoyaltyProgram.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace LoyaltyProgramTests
{
    public class UserEndpoints_should : IDisposable
    {
        private readonly IHost _host;
        private readonly HttpClient _sut;

        public UserEndpoints_should()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(x => x.UseStartup<Startup>().UseTestServer())
                .Start();
            _sut = _host.GetTestClient();
        }

        [Fact]
        public async Task allow_registering_user()
        {
            var newuser = new LoyaltyProgramUser {Name = "Foo", LoyaltyPoints = 42};
            var actual = await _sut.PostAsync(
                "/users",
                new StringContent(JsonConvert.SerializeObject(newuser), Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
            var actualUser = JsonConvert.DeserializeObject<LoyaltyProgramUser>(await actual.Content.ReadAsStringAsync());
            Assert.Equal(newuser.Name, actualUser.Name);
            Assert.Equal(newuser.LoyaltyPoints, actualUser.LoyaltyPoints);
        }

        public void Dispose()
        {
            _host?.Dispose();
            _sut?.Dispose();
        }
    }
}
