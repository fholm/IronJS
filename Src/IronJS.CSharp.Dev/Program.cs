using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace IronJS.CSharp.Dev {
	class Program {
		static void Main(string[] args) {
            var asm = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("ironjs_runtime_assembly"),
                AssemblyBuilderAccess.RunAndSave
            );

            var mod = asm.DefineDynamicModule("module");
            var typ = mod.DefineType("type1", TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);
            var gens = typ.DefineGenericParameters(new[] { "T0", "T1" });

            typ.DefineField("test1", gens[0], FieldAttributes.Public);

            var dynTyp = typ.CreateType();

            var typs = asm.GetTypes();
            asm.Save("ironjs_runtime_assembly.dll");
		}
	}
}
