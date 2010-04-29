namespace IronJS.Aliases

type Lexer  = IronJS.Parser.ES3Lexer
type Parser = IronJS.Parser.ES3Parser

type MetaObj        = System.Dynamic.DynamicMetaObject

type Et             = System.Linq.Expressions.Expression
type EtParam        = System.Linq.Expressions.ParameterExpression
type EtLambda       = System.Linq.Expressions.LambdaExpression

type AstUtils       = Microsoft.Scripting.Ast.Utils
type DynamicUtils   = Microsoft.Scripting.Utils.DynamicUtils

type CtorInfo       = System.Reflection.ConstructorInfo
type ParmInfo       = System.Reflection.ParameterInfo
type FieldInfo      = System.Reflection.FieldInfo
type MethodInfo     = System.Reflection.MethodInfo

type AstTree        = Antlr.Runtime.Tree.CommonTree

type ClrType          = System.Type
type ClrObject        = System.Object
type StrongBox<'a>    = System.Runtime.CompilerServices.StrongBox<'a>
type IEnum<'a>        = System.Collections.Generic.IEnumerable<'a>
type Dict<'a, 'b>     = System.Collections.Generic.Dictionary<'a, 'b>
type SafeDict<'a, 'b> = System.Collections.Concurrent.ConcurrentDictionary<'a, 'b>
type Delegate         = System.Delegate
type DebuggerDisplayAttribute = System.Diagnostics.DebuggerDisplayAttribute