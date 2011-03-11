namespace IronJS.Compiler

open System
open IronJS
open IronJS.Compiler.Utils
open IronJS.Dlr
open IronJS.Dlr.Operators
open IronJS.Dlr.ExtensionMethods

(*
//
*)
module Object =
  
  (*  
  //
  *)
  module Property = 

    //
    let putBox expr name value =
      tempBlockT<CO> expr (fun tmp -> 
        let args = [name; value]
        [call tmp "Put" args; value]
      )
    
    //
    let putRef expr name (value:Dlr.Expr) =
      tempBlockT<CO> expr (fun tmp -> 
        let tag = value.Type |> TypeTag.OfType |> Dlr.const'
        let args = [name; value; tag]
        [call tmp "Put" args; value]
      )
    
    //
    let putVal expr name (value:Dlr.Expr) =
      tempBlockT<CO> expr (fun tmp -> 
        let args = [name; Utils.normalizeVal value]
        [call tmp "Put" args; value]
      )

    //
    let put name (value:Dlr.Expr) expr = 
      match value with
      | IsBox -> putBox expr name value
      | IsRef -> putRef expr name value
      | IsVal -> putVal expr name value

    //
    let putName expr name (value:Dlr.Expr) = 
      let name = Dlr.const' name
      match value with
      | IsBox -> putBox expr name value
      | IsRef -> putRef expr name value
      | IsVal -> putVal expr name value
  
    //
    let get name expr = 
      tempBlockT<CommonObject> expr (fun tmp -> 
        [call tmp "Get" [name]]
      )

    //
    let delete expr name = 
      tempBlockT<CommonObject> expr (fun tmp -> 
        [call tmp "Delete" [name]]
      )
   
  (*  
  //
  *)
  module Index =
  
    //
    let private putConvert expr index value tag =
      Utils.tempBlockT<CommonObject> expr (fun tmp -> 
        let normalizedValue = normalizeVal value
        let args =
          match tag with
          | None -> [index; normalizedValue] 
          | Some tag -> [index; normalizedValue; tag] 

        [call tmp "Put" args; value]
      )
    
    //
    let putBox expr index value =
      if Dlr.Utils.isT<uint32> index then
        Utils.tempBlockT<CO> expr (fun tmp -> 
          let args = [index; value]
          [Dlr.call tmp "Put" args; value]
        )

      else
        putConvert expr index value None

    //
    let putVal expr index value =
      if Dlr.Utils.isT<uint32> index then
        Utils.tempBlockT<CO> expr (fun tmp -> 
          let args = [index; Utils.normalizeVal value]
          [Dlr.call tmp "Put" args; value]
        )
         
      else
        putConvert expr index value None

    //
    let putRef expr index (value:Dlr.Expr) =
      let tag = value.Type |> TypeTag.OfType |> Dlr.const'
      
      if Dlr.Utils.isT<uint32> index then
        Utils.tempBlockT<CO> expr (fun tmp -> 
          let args =  [index; value; tag]
          [Dlr.call tmp "Put" args; value]
        )
            
      else
        putConvert expr index value (Some tag)
      
    //
    let put index value expr =
      match value with
      | IsBox -> putBox expr index value
      | IsVal -> putVal expr index value
      | IsRef -> putRef expr index value

    //
    let get expr index = 
      tempBlockT<CO> expr (fun tmp -> 
        [tmp.CallMember("Get", [index])]
      )

    //
    let delete expr index = 
      tempBlockT<CO> expr (fun tmp -> 
        [tmp.CallMember("Delete", [index])]
      )

  // MemberExpression . Identifier
  let getProperty (ctx:Ctx) object' (name:string) =
    let object' = object' |> ctx.Compile

    ensureObject ctx object'
      (Property.get !!!name)
      (fun x -> Constants.Boxed.undefined)

  // MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    let index = index |> Utils.compileIndex ctx
    let object' = object' |> ctx.Compile

    ensureObject ctx object'
      (fun x -> Index.get x index)
      (fun x -> Constants.Boxed.undefined)

  // 11.1.4 array initialiser
  let literalArray (ctx:Ctx) (indexes:Ast.Tree list) = 
    let length = indexes.Length |> uint32

    blockTmpT<CO> (fun tmp ->
      [ 
        tmp .= ctx.Env.CallMember("NewArray", [!!!length])

        (List.mapi (fun i value ->
          let value = 
            match value with
            | Ast.Pass -> Ast.Undefined
            | value -> value

          let value = ctx.Compile value
          let index = !!!(uint32 i)

          tmp |> Index.put index value
        ) indexes) |> blockSimple

        tmp :> Dlr.Expr
      ] |> Seq.ofList)
      
  // 11.1.5 object initialiser
  let literalObject (ctx:Ctx) properties =
    let newExpr = ctx.Env.CallMember("NewObject")

    let setProperty tmp assign =
      match assign with
      | Ast.Assign(Ast.String name, expr) -> 
        let value = ctx.Compile expr
        let name = name.Trim('"')
        tmp |> Property.put !!!name value

      | _ -> failwithf "Que? %A" assign

    Dlr.blockTmpT<CO> (fun tmp -> 
      let initExprs = List.map (setProperty tmp) properties
      (Dlr.assign tmp newExpr :: initExprs) @ [tmp] |> Seq.ofList
    )