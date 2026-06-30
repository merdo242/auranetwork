using System;
using System.Reflection;
using CmlLib.Core;

class Program {
    static void Main() {
        var t = typeof(MinecraftLauncher);
        foreach(var e in t.GetEvents()) {
            Console.WriteLine(e.Name + " - " + e.EventHandlerType.FullName);
        }
    }
}
