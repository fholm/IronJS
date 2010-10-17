namespace IronJS.Api

open IronJS
open IronJS.Expr.Patterns

module Expr =
        
  //----------------------------------------------------------------------------
  let jsObjectPutIndex expr index (value:Dlr.Expr) =
    let methodName =
      if value.Type = typeof<IjsNum>
        then "PutValIndex"
        else "PutBoxIndex"

    Expr.blockTmpT<IjsObj> expr (fun tmp ->
      [Dlr.invoke
        (Dlr.property (Dlr.field tmp "Methods") methodName)
        [tmp; index; value]
      ]
    )
      
  //----------------------------------------------------------------------------
  let jsObjectGetIndex expr index =
    Expr.blockTmpT<IjsObj> expr (fun tmp ->
      [Dlr.invoke 
        (Dlr.property (Dlr.field tmp "Methods") "GetIndex")
        [tmp; index]
      ]
    )
      
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

