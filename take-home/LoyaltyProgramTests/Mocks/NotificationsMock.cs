using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoyaltyProgramServiceTests.Mocks
{
  using Microsoft.AspNetCore.Mvc;

  public class NotificationsMock : ControllerBase
  {
    public static bool ReceivedNotification = false;

    [HttpPost("/notifications")]
    public async Task<OkResult> Notify()
    {
      ReceivedNotification = true;
      using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
      {
        var body = await reader.ReadToEndAsync();
        await System.IO.File.WriteAllTextAsync(
          $"{Environment.CurrentDirectory}./../../../recorded-contracts/post-notifications.json", body);
      }
      return Ok();
    }
  }
}