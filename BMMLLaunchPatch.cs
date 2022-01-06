using BepInEx.Preloader.Core.Patching;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;
//using Mono.Cecil;
//using BepInEx;
//using BepInEx.NetLauncher;
//using System;
//using BepInEx.Bootstrap;
//using BepInEx.Configuration;
//using BepInEx.Logging;


[PatcherPluginInfo("BMMLLaunchPatch", "BmmL Launch Patch", "1.0.0")]
public class BMMLLaunchPatch : BasePatcher
{
	[TargetAssembly("EMMA.Desktop.exe")]
	public void PatchMain(AssemblyDefinition ass)
	{
		//string n = "null";
		//foreach (var x in ass.MainModule.Types) {
		//	Console.WriteLine($"{x.Name} from {(x.BaseType != null ? x.BaseType.Name : n)}");
		//}

		// The method that needs to be patched is EMMA.Desktop.TheGame..ctor

		TypeDefinition theGame = ass.MainModule.Types.First(
			x => x.Name == "TheGame" && x.BaseType != null && x.BaseType.Name == "TheGame");

		//foreach (var m in theGame.Methods) {
		//	Console.WriteLine(m.Name);
		//}

		MethodDefinition method = theGame.Methods.First(x => x.Name == ".ctor");

		var il = method.Body.GetILProcessor();

		// Insertion point is the ldarg.0 opcode that's directly before ldc.i4 0xC549A
		var ins = il.Body.Instructions.First(
			x => x.OpCode == OpCodes.Ldarg_0 && 
			x.Next.OpCode == OpCodes.Ldc_I4 && 
			(int)x.Next.Operand == 808090);

		//Console.WriteLine(ins.OpCode.Name);

		// Insert a return opcode before initialization of the steam Client can happen
		il.InsertBefore(ins, il.Create(OpCodes.Ret));

		// Remove all opcodes after the first return
		var ret = il.Body.Instructions.First(x => x.OpCode == OpCodes.Ret);

		ins = il.Body.Instructions.Last();
		while (ins != ret) {
			var prev = ins.Previous;
			il.Remove(ins);
			ins = prev;
		}
	}
}



/*

namespace proj
{
	public static class Patcher
	{
		// List of assemblies to patch
		public static IEnumerable<string> TargetDLLs { get; } = new[] {"EMMA.Desktop.exe"};

		// Patches the assemblies
		public static void Patch(AssemblyDefinition assembly)
		{
			// Patcher code here
			Console.WriteLine("test");
		}
	}
}
*/

/*
using BepInEx;
using BepInEx.NetLauncher;

namespace proj
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        public override void Load()
        {
            // Plugin startup logic
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
*/