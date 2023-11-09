using System.Threading.Tasks;
using System;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LoyaltyProgramServiceTests.Mocks;

public class MocksHost : IAsyncDisposable
{
  private readonly IHost hostForMocks;

  public MocksHost(int port, string scenario)
  {
    this.hostForMocks =
      Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(x => x
          .ConfigureServices(services => services.AddSingleton(new Scenario(scenario)).AddControllers())
          .Configure(app => app.UseRouting().UseEndpoints(opt => opt.MapControllers()))
          .UseUrls($"http://localhost:{port}"))
        .Build();

    new Thread(() => this.hostForMocks.Run()).Start();
  }

  public async ValueTask DisposeAsync()
  {
    await hostForMocks.StopAsync();
    hostForMocks.Dispose();
  }
}
  
public record Scenario(string Name);