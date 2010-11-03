namespace IronJS.Compiler

open System
open IronJS
open IronJS.Expr.Patterns

//------------------------------------------------------------------------------
module Object =
  
  //----------------------------------------------------------------------------
  module Property = 

    //--------------------------------------------------------------------------
    let putBox expr name value =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [ (Dlr.invoke
            (Expr.Object.Methods.putBoxProperty tmp)
            [tmp; name; value])
          (value)])
    
    //--------------------------------------------------------------------------
    let putRef expr name value =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [ (Dlr.invoke
            (Expr.Object.Methods.putRefProperty tmp)
            [tmp; name; value; value |> Utils.expr2tc |> Dlr.const'])
          (value)])
    
    //--------------------------------------------------------------------------
    let putVal expr name (value:Dlr.Expr) =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [ (Dlr.invoke
            (Expr.Object.Methods.putValProperty tmp)
            [tmp; name; Expr.normalizeVal value])
          (value)])

    //--------------------------------------------------------------------------
    let put expr name (value:Dlr.Expr) = 
      match value with
      | Box -> putBox expr name value
      | Ref -> putRef expr name value
      | Val -> putVal expr name value

    //--------------------------------------------------------------------------
    let putName expr name (value:Dlr.Expr) = 
      let name = Dlr.const' name
      match value with
      | Box -> putBox expr name value
      | Ref -> putRef expr name value
      | Val -> putVal expr name value
  
    //--------------------------------------------------------------------------
    let get expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke 
          (Expr.Object.Methods.getProperty tmp)
          [tmp; name]])
  
    //--------------------------------------------------------------------------
    let has expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke 
          (Expr.Object.Methods.hasProperty tmp)
          [tmp; name]])
  
    //--------------------------------------------------------------------------
    let delete expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke 
          (Expr.Object.Methods.deleteProperty tmp)
          [tmp; name]])
      
  //----------------------------------------------------------------------------
  module Index =

    let convertIndex_BoxVal expr index value =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [ (Dlr.callStaticT<Api.Object.Index.Converters> 
            "put" [tmp; index; value])
          (value)])
    
    //--------------------------------------------------------------------------
    let putBox expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [ (Dlr.invoke
              (Expr.Object.Methods.putBoxIndex tmp)
              [tmp; index; value])
            (value)])

      | TypeCode -> convertIndex_BoxVal expr index value

    //--------------------------------------------------------------------------
    let putVal expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [ (Dlr.invoke
              (Expr.Object.Methods.putValIndex tmp)
              [tmp; index; Expr.normalizeVal value])
            (value)])
            
      | TypeCode -> convertIndex_BoxVal expr index value

    //--------------------------------------------------------------------------
    let putRef expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [ (Dlr.invoke
              (Expr.Object.Methods.putRefIndex tmp)
              [tmp; index; value; value |> Utils.expr2tc |> Dlr.const'])
            (value)])
            
      | TypeCode -> 
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [ (Dlr.callStaticT<Api.Object.Index.Converters> 
              "put" [tmp; index; value; value |> Utils.expr2tc |> Dlr.const'])
            (value)])
      
    //--------------------------------------------------------------------------
    let put expr index value =
      match value with
      | Box -> putBox expr index value
      | Val -> putVal expr index value
      | Ref -> putRef expr index value
  
    //--------------------------------------------------------------------------
    let get expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke 
          (Expr.Object.Methods.getIndex tmp)
          [tmp; name]])
  
    //--------------------------------------------------------------------------
    let has expr name = 
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke 
          (Expr.Object.Methods.hasIndex tmp)
          [tmp; name]])
  
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
    let name = Dlr.const' name
    (Expr.testIsObject
      (ctx.Compile object')
      (fun x -> Property.get x name)
      (fun x -> Expr.BoxedConstants.undefined))

  // MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    let index = Utils.compileIndex ctx index
    (Expr.testIsObject
      (ctx.Compile object')
      (fun x -> Index.get x index)
      (fun x -> Expr.BoxedConstants.undefined))

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
          Index.put tmp index value
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
        Property.put tmp name value

      | _ -> failwith "Que?"

    Dlr.blockTmpT<IjsObj> (fun tmp -> 
      let initExprs = List.map (setProperty tmp) properties
      (Dlr.assign tmp newExpr :: initExprs) @ [tmp] |> Seq.ofList)