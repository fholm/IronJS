namespace IronJS.Compiler

open System
open IronJS
open IronJS.Compiler.Utils.Patterns
open IronJS.Dlr.Operators

//------------------------------------------------------------------------------
module Object =
  
  //----------------------------------------------------------------------------
  module Property = 

    //--------------------------------------------------------------------------
    let putBox expr name value =
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        let args = [name; value]
        [Dlr.call tmp "Put" args; value]
      )
    
    //--------------------------------------------------------------------------
    let putRef expr name (value:Dlr.Expr) =
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        let tag = value.Type |> Utils.type2tag |> Dlr.const'
        let args = [name; value; tag]
        [Dlr.call tmp "Put" args; value]
      )
    
    //--------------------------------------------------------------------------
    let putVal expr name (value:Dlr.Expr) =
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        let args = [name; Utils.normalizeVal value]
        [Dlr.call tmp "Put" args; value]
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
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        [Dlr.call tmp "Get" [name]]
      )

    //--------------------------------------------------------------------------
    let delete expr name = 
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        [Dlr.call tmp "Delete" [name]]
      )
      
  //----------------------------------------------------------------------------
  module Index =
  
    //--------------------------------------------------------------------------
    let private putConvert expr index value tag =
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        let normalizedValue = Utils.normalizeVal value
        let args =
          match tag with
          | None -> [ index; normalizedValue] 
          | Some tag -> [index; normalizedValue; tag] 

        [Dlr.call tmp "Put" args; value]
      )
    
    //--------------------------------------------------------------------------
    let putBox expr index value =
      match index with
      | IsIndex ->
        Utils.blockTmpT<CommonObject> expr (fun tmp -> 
          let args = [index; value]
          [Dlr.call tmp "Put" args; value]
        )

      | _ -> putConvert expr index value None

    //--------------------------------------------------------------------------
    let putVal expr index value =
      match index with
      | IsIndex ->
        Utils.blockTmpT<CommonObject> expr (fun tmp -> 
          let args = [index; Utils.normalizeVal value]
          [Dlr.call tmp "Put" args; value]
        )
            
      | _ -> putConvert expr index value None

    //--------------------------------------------------------------------------
    let putRef expr index (value:Dlr.Expr) =
      let tag = value.Type |> Utils.type2tag |> Dlr.const'

      match index with
      | IsIndex ->
        Utils.blockTmpT<CommonObject> expr (fun tmp -> 
          let args =  [index; value; tag]
          [Dlr.call tmp "Put" args; value]
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
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        [Dlr.call tmp "Get" [index]]
      )

    //--------------------------------------------------------------------------
    let delete expr index = 
      Utils.blockTmpT<CommonObject> expr (fun tmp -> 
        [Dlr.call tmp "Delete" [index]])

  //----------------------------------------------------------------------------
  // 11.2.1 Property Accessors
      
  // MemberExpression . Identifier
  let getProperty (ctx:Ctx) object' name =
    Utils.ensureObject ctx (object' |> ctx.Compile)
      (Property.get !!!name)
      (fun x -> Utils.Constants.Boxed.undefined)

  // MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    let indexExpr = index |> Utils.compileIndex ctx
    Utils.ensureObject ctx (object' |> ctx.Compile)
      (Index.get indexExpr)
      (fun x -> Utils.Constants.Boxed.undefined)

  //----------------------------------------------------------------------------
  // 11.1.4 array initialiser
  let literalArray (ctx:Ctx) (indexes:Ast.Tree list) = 
    let length = indexes.Length

    Dlr.blockTmpT<CommonObject> (fun tmp ->
      [ (Dlr.assign tmp
          (Dlr.call ctx.Env "NewArray" [!!!(uint32 length)]))

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
    let newExpr = Dlr.call ctx.Env "NewObject" []

    let setProperty tmp assign =
      match assign with
      | Ast.Assign(Ast.String name, expr) -> 
        let value = ctx.Compile expr
        let name = Dlr.const' name
        tmp |> Property.put name value

      | _ -> failwith "Que?"

    Dlr.blockTmpT<CommonObject> (fun tmp -> 
      let initExprs = List.map (setProperty tmp) properties
      (Dlr.assign tmp newExpr :: initExprs) @ [tmp] |> Seq.ofList)