namespace IronJS.Compiler

open System
open IronJS
open IronJS.Dlr
open IronJS.Dlr.Operators
open IronJS.Dlr.ExtensionMethods
open IronJS.Support.CustomOperators
open IronJS.Compiler
open IronJS.Compiler.Utils
open IronJS.Compiler.Context

module Object =

  module Property = 

    //
    let putBox expr name value =
      tempBlockT<CO> expr (fun tmp -> 
        [tempBlock value (fun valueTmp ->
          let args = [name; valueTmp]
          [call tmp "Put" args; valueTmp]
        )]
      )
    
    //
    let putRef expr name (value:Dlr.Expr) =
      tempBlockT<CO> expr (fun tmp -> 
        [tempBlock value (fun valueTmp ->
          let tag = valueTmp.Type |> TypeTag.OfType |> Dlr.const'
          let args = [name; valueTmp; tag]
          [call tmp "Put" args; valueTmp]
        )]
      )
    
    //
    let putVal expr name (value:Dlr.Expr) =
      tempBlockT<CO> expr (fun tmp -> 
        [tempBlock value (fun valueTmp ->
          let args = [name; Utils.normalizeVal valueTmp]
          [call tmp "Put" args; valueTmp]
        )]
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
      tempBlockT<CO> expr (fun tmp -> 
        [call tmp "Get" [name]]
      )

    //
    let delete expr name = 
      tempBlockT<CO> expr (fun tmp -> 
        [call tmp "Delete" [name]]
      )

    //
    let attr name (attr:uint16) cobj =
      tempBlockT<CO> cobj (fun tmp -> 
        [call tmp "SetAttrs" [name; !!!attr]]
      )
   
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
    let object' = ctx $ compile object'

    ensureObject ctx object'
      (Property.get !!!name)
      (fun x -> 
        Constants.Boxed.undefined
      )

  // MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    let index = index |> Utils.compileIndex ctx
    let object' = ctx $ compile object'

    ensureObject ctx object'
      (fun x -> Index.get x index)
      (fun x -> 
        (Dlr.ternary 
          (Dlr.callStaticT<Object> "ReferenceEquals" [Dlr.castT<obj> x; Dlr.null'])
          (Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] [])
          (Utils.Constants.Boxed.undefined)
        )
      )

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

          let value = ctx $ compile value
          let index = !!!(i |> uint32)

          tmp |> Index.put index value
        ) indexes) |> blockSimple

        tmp :> Dlr.Expr
      ] |> Seq.ofList)
      
  // 11.1.5 object initialiser
  let literalObject (ctx:Ctx) properties =
    let newExpr = ctx.Env.CallMember("NewObject")

    let setProperty tmp (name:string, value) =
      let value = ctx $ compile value
      tmp |> Property.put !!!name value

    Dlr.blockTmpT<CO> (fun tmp -> 
      let initExprs = properties |> List.map (setProperty tmp)
      (Dlr.assign tmp newExpr :: initExprs) @ [tmp] |> Seq.ofList
    )