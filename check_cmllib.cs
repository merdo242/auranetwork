using System;
using System.Reflection;
using CmlLib.Core;

class Program {
    static void Main() {
        var t = typeof(MinecraftLauncher);
        foreach(var m in t.GetMethods()) {
            Console.WriteLine(m.Name);
        }
    }
}
