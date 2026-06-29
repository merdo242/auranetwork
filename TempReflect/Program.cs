using System; using System.Reflection; using System.Linq;
var asm = Assembly.LoadFrom(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.nuget\\packages\\cmllib.core\\4.0.6\\lib\\netstandard2.0\\CmlLib.Core.dll");
var launcher = asm.GetType("CmlLib.Core.MinecraftLauncher");
foreach(var m in launcher.GetMethods()) { Console.WriteLine(m.ReturnType.Name + " " + m.Name + "(" + string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name)) + ")"); }
