using BepInEx.Preloader.Core.Patching;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using System.Linq;

[PatcherPluginInfo("BMMLLaunchPatch", "BmmL Launch Patch", "1.0.0")]
public class BMMLLaunchPatch : BasePatcher
{
	[TargetAssembly("EMMA.Desktop.exe")]
	public void PatchMain(AssemblyDefinition asmDef)
	{
		// The method that needs to be patched is EMMA.Desktop.TheGame..ctor
		TypeDefinition theGame = asmDef.MainModule.Types.First(
			x => x.Name == "TheGame" && x.BaseType != null && x.BaseType.Name == "TheGame");

		MethodDefinition method = theGame.Methods.First(x => x.Name == ".ctor");


		var il = method.Body.GetILProcessor();

		// Insertion point is the ldarg.0 opcode that's directly before ldc.i4 0xC549A
		var ins = il.Body.Instructions.First(
			x => x.OpCode == OpCodes.Ldarg_0 && 
			x.Next.OpCode == OpCodes.Ldc_I4 && 
			(int)x.Next.Operand == 808090);

		// Insert a return opcode before initialization of the Client object can happen
		il.InsertBefore(ins, il.Create(OpCodes.Ret));

		// Remove all opcodes after the first return opcode
		var ret = il.Body.Instructions.First(x => x.OpCode == OpCodes.Ret);

		ins = il.Body.Instructions.Last();
		while (ins != ret) {
			var prev = ins.Previous;
			il.Remove(ins);
			ins = prev;
		}
	}
}
