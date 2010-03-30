module IronJS.Fsi

let dbgViewProp = typeof<System.Linq.Expressions.Expression>.GetProperty("DebugView", System.Reflection.BindingFlags.NonPublic ||| System.Reflection.BindingFlags.Instance)