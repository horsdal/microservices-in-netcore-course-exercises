using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NotificationsContractTest;

public class VerifyLoyaltyProgramContract
{
    [Theory]
    [MemberData(nameof(RecordedContracts))]
    public async Task VerifyNotificationsEndpoint(string recordingPath, HttpStatusCode expected)
    {
        var sut = new HttpClient();
        var recordedBody = await File.ReadAllTextAsync(recordingPath);
        
        var actual = await sut.PostAsync(
            "http://localhost:5000/notifications", 
            new StringContent(recordedBody, Encoding.UTF8, "application/json"));
        
        Assert.Equal(expected, actual.StatusCode);
    }
    
    private static IEnumerable<object[]> RecordedContracts() => 
        Directory
            .EnumerateFiles("../../../../LoyaltyProgramTests/recorded-contracts/")
            .Where(x => x.EndsWith(".json"))
            .Select(x => new object[] { x, HttpStatusCode.OK })
            .ToList();
}