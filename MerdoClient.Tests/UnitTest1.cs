using MerdoClient;
using System.IO;

namespace MerdoClient.Tests;

public class AccountServiceTests
{
    [Fact]
    public void RegisterAndLogin_WithValidCredentials_ShouldSucceed()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var service = new AccountService(tempPath);

        var result = service.Register("merdo", "123456");

        Assert.True(result);
        Assert.True(service.Login("merdo", "123456"));

        // Clean up
        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }
    }

    [Fact]
    public void Register_WithExistingUser_ShouldFail()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var service = new AccountService(tempPath);

        Assert.True(service.Register("merdo", "123456"));
        Assert.False(service.Register("merdo", "123456"));

        // Clean up
        if (Directory.Exists(tempPath))
        {
            Directory.Delete(tempPath, true);
        }
    }
}