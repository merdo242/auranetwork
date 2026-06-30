using System;
using System.IO;
using System.Threading.Tasks;
using CmlLib.Core;
using CmlLib.Core.Auth;

class Program {
    static async Task Main() {
        try {
            var path = new MinecraftPath();
            var launcher = new MinecraftLauncher(path);
            var version = "fabric-loader-0.19.3-1.21.1";
            var session = new MSession("testuser", "offline_token", Guid.NewGuid().ToString("N"));
            var launchOptions = new MLaunchOption {
                Session = session
            };
            var process = await launcher.CreateProcessAsync(version, launchOptions);
            Console.WriteLine("Java Path: " + process.StartInfo.FileName);
            Console.WriteLine("Arguments Length: " + process.StartInfo.Arguments.Length);
            // File.WriteAllText("args.txt", process.StartInfo.Arguments);
        } catch (Exception ex) {
            Console.WriteLine(ex.ToString());
        }
    }
}
