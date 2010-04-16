let typ = Et.GetDelegateType([| typeof<int>.MakeByRefType(); typeof<int>  |])
let prm = Dlr.Expr.param "~in" (typeof<int>.MakeByRefType())
let lmb = Et.Lambda(typ, Dlr.Expr.block [
  Dlr.Expr.assign prm (Dlr.Expr.constant 4)
], [prm])

let i = Dlr.Expr.param "~i" (typeof<int>)
let outer = Dlr.Expr.lambdaWithLocals [] [i] [
  Dlr.Expr.assign i (Dlr.Expr.constant 1)
  Et.Invoke(lmb, i)
  i
]

let cmp = outer.Compile()

cmp.DynamicInvoke()