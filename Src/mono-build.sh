#!/bin/bash

platform='x86'
monopath='/usr/lib/mono/4.0/'
fscorepath='/usr/local/lib/mono/4.0/'

`fsc -o:IronJS.dll --debug:pdbonly --noframework --optimize+ --define:BIGINTEGER --platform:$platform -r:$fscorepath'FSharp.Core.dll' -r:$monopath'mscorlib.dll' -r:$monopath'System.Core.dll' -r:$monopath'System.dll' -r:$monopath'System.Numerics.dll' --target:library --warn:4 --warnaserror:76 --vserrors --LCID:1033 --utf8output --fullpaths --flaterrors IronJS/AssemblyInfo.fs IronJS/CLR2.fs IronJS/FSharp.fs IronJS/Dlr.fs IronJS/Support.fs IronJS/Error.fs IronJS/Core.fs IronJS/Core.Helpers.fs IronJS/Core.Operators.fs IronJS/Compiler.Ast.fs IronJS/Compiler.Lexer.fs IronJS/Compiler.Parser.fs IronJS/Compiler.Analyzer.fs IronJS/Compiler.Context.fs IronJS/Compiler.Utils.fs IronJS/Compiler.HostFunction.fs IronJS/Compiler.Object.fs IronJS/Compiler.Identifier.fs IronJS/Compiler.Function.fs IronJS/Compiler.Exception.fs IronJS/Compiler.Operators.fs IronJS/Compiler.ControlFlow.fs IronJS/Compiler.Scope.fs IronJS/Compiler.Core.fs IronJS/Native.Utils.fs IronJS/Native.Global.fs IronJS/Native.Math.fs IronJS/Native.Object.fs IronJS/Native.Function.fs IronJS/Native.Array.fs IronJS/Native.RegExp.fs IronJS/Native.String.fs IronJS/Native.Number.fs IronJS/Native.Date.fs IronJS/Native.Boolean.fs IronJS/Native.Error.fs IronJS/Hosting.fs > /dev/null`

