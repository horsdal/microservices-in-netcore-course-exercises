namespace LoyaltyProgramServiceTests.Mocks
{
  using Microsoft.AspNetCore.Mvc;

  public class NotificationsMock : ControllerBase
  {
    public static bool ReceivedNotification = false;

    [HttpPost("/notifications")]
    public OkResult Notify()
    {
      ReceivedNotification = true;
      return Ok();
    }
  }
}