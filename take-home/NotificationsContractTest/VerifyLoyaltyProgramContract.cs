using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NotificationsContractTest;

public class VerifyLoyaltyProgramContract
{
    [Theory]
    [InlineData("../../../../LoyaltyProgramTests/recorded-contracts/post-notifications.json", HttpStatusCode.OK)]
    public async Task VerifyNotificatoinsEndpoint(string recordingPath, HttpStatusCode expected)
    {
        var sut = new HttpClient();
        var recordedBody = await File.ReadAllTextAsync(recordingPath);
        
        var actual = await sut.PostAsync(
            "http://localhost:5001/notifications", 
            new StringContent(recordedBody, Encoding.UTF8, "application/json"));
        
        Assert.Equal(expected, actual.StatusCode);
    }
}