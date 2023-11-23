using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PactNet;
using PactNet.Output.Xunit;
using PactNet.Verifier;
using Xunit;
using Xunit.Abstractions;

namespace NotificationsContractTest;

public class VerifyLoyaltyProgramContract
{
    private readonly PactVerifier _verifier;

    public VerifyLoyaltyProgramContract(ITestOutputHelper output)
    {
        _verifier =  new PactVerifier("Notifications", new PactVerifierConfig
        {
            LogLevel = PactLogLevel.Debug,
            Outputters = new []{ new XunitOutput(output)}
        });
        
    }
    
    [Fact]
    public void VerifyNotificationsEndpoint()
    {
        _verifier
            .WithHttpEndpoint(new Uri("http://localhost:5001"))
            .WithDirectorySource(new DirectoryInfo( "../../../../LoyaltyProgramTests/pacts/"))
            .Verify();
    }
}