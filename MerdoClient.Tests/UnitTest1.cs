using MerdoClient;

namespace MerdoClient.Tests;

public class AccountServiceTests
{
    [Fact]
    public void RegisterAndLogin_WithValidCredentials_ShouldSucceed()
    {
        var service = new AccountService();
        service.ResetForTesting();

        var result = service.Register("merdo", "123456");

        Assert.True(result);
        Assert.True(service.Login("merdo", "123456"));
    }

    [Fact]
    public void Register_WithExistingUser_ShouldFail()
    {
        var service = new AccountService();
        service.ResetForTesting();

        Assert.True(service.Register("merdo", "123456"));
        Assert.False(service.Register("merdo", "123456"));
    }
}