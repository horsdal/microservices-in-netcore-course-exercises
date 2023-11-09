using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using static System.IO.File;

namespace LoyaltyProgramServiceTests.Mocks;

public class NotificationsMock : ControllerBase
{
  public static bool ReceivedNotification = false;
  private readonly Scenario _currentScenario;

  public NotificationsMock(Scenario currentScenario)
  {
    _currentScenario = currentScenario;
  }

  [HttpPost("/notifications")]
  public async Task<OkResult> Notify()
  {
    ReceivedNotification = true;
    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
    {
      var body = await reader.ReadToEndAsync();
      await WriteAllTextAsync($"{Environment.CurrentDirectory}./../../../recorded-contracts/post-notifications-{_currentScenario.Name}.json", body);
    }
    return Ok();
  }
}