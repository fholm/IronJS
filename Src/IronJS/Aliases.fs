namespace IronJS.Aliases

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

type ClrType        = System.Type
type Dynamic        = System.Object
type StrongBox<'a>  = System.Runtime.CompilerServices.StrongBox<'a>
type IEnum<'a>      = System.Collections.Generic.IEnumerable<'a>
type Dict<'a, 'b>   = System.Collections.Generic.Dictionary<'a, 'b>