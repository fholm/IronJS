namespace IronJS.Api

open IronJS

module Expr =

  //----------------------------------------------------------------------------
  let jsObjectGetProperty expr name = 
    let name = Dlr.const' name
    Expr.blockTmpT<IronJS.Object> expr (fun tmp -> 
      [Dlr.callStaticT<Api.Object> "getProperty" [tmp; name]]
    )
    
  //----------------------------------------------------------------------------
  let jsObjectPutLength expr value =
    Expr.blockTmpT<IronJS.Object> expr (fun tmp -> 
      [Dlr.callStaticT<Api.Object> "putLength" [tmp; value]]
    )
      
  //----------------------------------------------------------------------------
  let jsObjectPutProperty expr name value = 
    match name with
    | "length" -> 
      jsObjectPutLength expr value

    | _ ->
      let name = Dlr.const' name
      Expr.blockTmpT<IronJS.Object> expr (fun tmp -> 
        [Dlr.callStaticT<Api.Object> "putProperty" [tmp; name; value]]
      )
      
  //----------------------------------------------------------------------------
  let jsObjectUpdateProperty expr name value = 
    match name with
    | "length" -> jsObjectPutLength expr value
    | _ -> jsObjectPutProperty expr name value
        
  //----------------------------------------------------------------------------
  let jsObjectPutIndex expr index value =
    Expr.blockTmpT<IronJS.Object> expr (fun tmp ->
      [Dlr.callStaticT<Api.Object> "putIndex" [tmp; index; value]]
    )
      
  //----------------------------------------------------------------------------
  let jsObjectGetIndex expr index =
    Expr.blockTmpT<IjsObj> expr (fun tmp ->
      [Dlr.callStaticT<Api.Object> "getIndex" [tmp; index]]
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

