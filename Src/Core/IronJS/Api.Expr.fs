namespace IronJS.Api

open IronJS
open IronJS.Expr.Patterns

module Expr =
      
  //----------------------------------------------------------------------------
  let jsFunctionInvoke func this' args =
    Expr.blockTmpT<IjsFunc> func (fun f -> 
      let argTypes = [for (a:Dlr.Expr) in args -> a.Type]
      let args = f :: this' :: args
      [Dlr.callStaticGenericT<Api.Function> "call" argTypes args]
    )
      
  //----------------------------------------------------------------------------
  let jsMethodInvoke target f args =
    Expr.blockTmpT<IjsObj> target (fun object' ->
      [
        Expr.blockTmpT<IjsBox> (f object') (fun method' ->
          [
            (Expr.testIsFunction
              (method')
              (fun x -> jsFunctionInvoke x object' args)
              (fun x -> Expr.undefinedBoxed)
            )
          ]
        )
      ]
    )

