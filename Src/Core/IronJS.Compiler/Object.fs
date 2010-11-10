namespace IronJS.Compiler

open System
open IronJS
open IronJS.Expr.Patterns
open IronJS.Dlr.Operators

//------------------------------------------------------------------------------
module Object =
  
  //----------------------------------------------------------------------------
  module Property = 

    //--------------------------------------------------------------------------
    let putBox expr name value =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        let args = [tmp; name; value]
        let method' = Expr.Object.Methods.putBoxProperty tmp
        [Dlr.invoke method' args; value]
      )
    
    //--------------------------------------------------------------------------
    let putRef expr name (value:Dlr.Expr) =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        let tag = value.Type |> Utils.type2tag |> Dlr.const'
        let args = [tmp; name; value; tag]
        let method' = Expr.Object.Methods.putRefProperty tmp
        [Dlr.invoke method' args; value]
      )
    
    //--------------------------------------------------------------------------
    let putVal expr name (value:Dlr.Expr) =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        let args = [tmp; name; Expr.normalizeVal value]
        let method' = Expr.Object.Methods.putValProperty tmp
        [Dlr.invoke method' args; value]
      )

    //--------------------------------------------------------------------------
    let put name (value:Dlr.Expr) expr = 
      match value with
      | IsBox -> putBox expr name value
      | IsRef -> putRef expr name value
      | IsVal -> putVal expr name value

    //--------------------------------------------------------------------------
    let putName expr name (value:Dlr.Expr) = 
      let name = Dlr.const' name
      match value with
      | IsBox -> putBox expr name value
      | IsRef -> putRef expr name value
      | IsVal -> putVal expr name value
  
    //--------------------------------------------------------------------------
    let get name expr = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        let method' = Expr.Object.Methods.getProperty tmp
        [Dlr.invoke method' [tmp; name]]
      )

    //--------------------------------------------------------------------------
    let delete expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        let method' = Expr.Object.Methods.deleteProperty tmp
        [Dlr.invoke method' [tmp; name]]
      )
      
  //----------------------------------------------------------------------------
  module Index =
  
    //--------------------------------------------------------------------------
    let private putConvert expr index value tag =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        let normalizedValue = Expr.normalizeVal value
        let args =
          match tag with
          | None -> [tmp; index; normalizedValue] 
          | Some tag -> [tmp; index; normalizedValue; tag] 

        [Dlr.callStaticT<Api.Object.Index.Converters> "put" args; value]
      )
    
    //--------------------------------------------------------------------------
    let putBox expr index value =
      match index with
      | IsIndex ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          let args = [tmp; index; value]
          let method' = Expr.Object.Methods.putBoxIndex tmp
          let invoke = Dlr.invoke method' args
          [invoke; value]
        )

      | _ -> putConvert expr index value None

    //--------------------------------------------------------------------------
    let putVal expr index value =
      match index with
      | IsIndex ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          let args = [tmp; index; Expr.normalizeVal value]
          let method' = Expr.Object.Methods.putValIndex tmp
          let invoke = Dlr.invoke method' args
          [invoke; value]
        )
            
      | _ -> putConvert expr index value None

    //--------------------------------------------------------------------------
    let putRef expr index (value:Dlr.Expr) =
      let tag = value.Type |> Utils.type2tag |> Dlr.const'

      match index with
      | IsIndex ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          let args =  [tmp; index; value; tag]
          let method' = Expr.Object.Methods.putRefIndex tmp
          let invoke = Dlr.invoke method' args
          [invoke; value]
        )
            
      | _ -> putConvert expr index value (Some tag)
      
    //--------------------------------------------------------------------------
    let put index value expr =
      match value with
      | IsBox -> putBox expr index value
      | IsVal -> putVal expr index value
      | IsRef -> putRef expr index value

    //--------------------------------------------------------------------------
    let get (index:Dlr.Expr) expr = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        match index with
        | IsIndex -> 
          [Dlr.invoke (Expr.Object.Methods.getIndex tmp) [tmp; index]]

        | _ ->
          [Dlr.callStaticT<Api.Object.Index.Converters> "get" [tmp; index]]
      )

    //--------------------------------------------------------------------------
    let delete expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke 
          (Expr.Object.Methods.deleteIndex tmp)
          [tmp; name]])

  //----------------------------------------------------------------------------
  // 11.2.1 Property Accessors
      
  // MemberExpression . Identifier
  let getProperty (ctx:Ctx) object' name =
    Utils.ensureObject ctx (object' |> ctx.Compile)
      (Property.get !!!name)
      (fun x -> Expr.BoxedConstants.undefined)

  // MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    let indexExpr = index |> Utils.compileIndex ctx
    Utils.ensureObject ctx (object' |> ctx.Compile)
      (Index.get indexExpr)
      (fun x -> Expr.BoxedConstants.undefined)

  //----------------------------------------------------------------------------
  // 11.1.4 array initialiser
  let literalArray (ctx:Ctx) (indexes:Ast.Tree list) = 
    let length = indexes.Length
    let args = [ctx.Env; Dlr.const' (uint32 length)]

    Dlr.blockTmpT<IjsObj> (fun tmp ->
      [ (Dlr.assign tmp
          (Dlr.callMethod
            Api.Environment.Reflected.createArray args))

        (List.mapi (fun i value ->
          let index = uint32 i |> Dlr.const'
          let value = ctx.Compile value
          tmp |> Index.put index value
        ) indexes) |> Dlr.blockSimple

        (tmp :> Dlr.Expr)
      ] |> Seq.ofList)
      
  //----------------------------------------------------------------------------
  // 11.1.5 object initialiser
  let literalObject (ctx:Ctx) properties =
    let method' = Api.Environment.Reflected.createObject
    let newExpr = Dlr.callMethod method' [ctx.Env]

    let setProperty tmp assign =
      match assign with
      | Ast.Assign(Ast.String name, expr) -> 
        let value = ctx.Compile expr
        let name = Dlr.const' name
        tmp |> Property.put name value

      | _ -> failwith "Que?"

    Dlr.blockTmpT<IjsObj> (fun tmp -> 
      let initExprs = List.map (setProperty tmp) properties
      (Dlr.assign tmp newExpr :: initExprs) @ [tmp] |> Seq.ofList)