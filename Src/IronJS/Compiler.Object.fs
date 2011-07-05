namespace IronJS.Compiler

open System
open IronJS
open IronJS.Dlr
open IronJS.Dlr.Operators
open IronJS.Dlr.ExtensionMethods
open IronJS.Runtime
open IronJS.Support.CustomOperators
open IronJS.Compiler
open IronJS.Compiler.Utils
open IronJS.Compiler.Context

///
module internal Object =

  ///
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

    ///
    let put name (value:Dlr.Expr) expr = 
      match value with
      | IsBox -> putBox expr name value
      | IsRef -> putRef expr name value
      | IsVal -> putVal expr name value

    ///
    let get name expr = 
      tempBlockT<CO> expr (fun tmp -> 
        [call tmp "Get" [name]]
      )

    ///
    let delete expr name = 
      tempBlockT<CO> expr (fun tmp -> 
        [call tmp "Delete" [name]]
      )

    ///
    let attr name (attr:uint16) cobj =
      tempBlockT<CO> cobj (fun tmp -> 
        [call tmp "SetAttrs" [name; !!!attr]]
      )
   
  ///
  module Index =
  
    ///
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

  /// MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    ensureObject ctx object'
      (fun x -> Index.get x index)
      (fun x -> 
        (Dlr.ternary 
          (Dlr.callStaticT<Object> "ReferenceEquals" [Dlr.castT<obj> x; Dlr.null'])
          (Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] [!!!ErrorUtils.nextErrorId()])
          (Utils.Constants.Boxed.undefined)
        )
      )

  ///
  let getIndex_Ast (ctx:Ctx) (expr:Ast.Tree) (index:Ast.Tree) =
    getIndex ctx (ctx |> compile expr) (ctx |> compile index)

  /// MemberExpression . String = Expression
  let putMember (ctx:Ctx) (expr:Dlr.Expr) (name:string) (value:Dlr.Expr) =
    
    //
    let makePropertyPutCache (env:Env) =
      let cache = !!!(new Runtime.Optimizations.InlinePropertyPutCache())
      let cacheId = cache .-> "CachedId"
      let cacheIndex = cache .-> "CachedIndex"
      cache, cacheId, cacheIndex

    //
    let fromJsObject (jsobj:Dlr.Expr) (name:string) (value:Dlr.Expr) (ctx:Ctx) =
      let env = ctx.Target.Environment
      let cache, cacheId, cacheIndex = env |> makePropertyPutCache

      match name with
      | "length" -> Property.put !!!name value jsobj
      | _ ->
        let index = Dlr.paramT<int> "~index"
        let properties = Dlr.paramT<Descriptor array> "~properties"
        let fallbackArgs =
          match value with
          | IsBox -> [|jsobj; !!!name; value|]
          | IsVal -> [|jsobj; !!!name; value |> Utils.normalizeVal|]
          | IsRef -> [|jsobj; !!!name; value; !!!TypeTag.OfType(value.Type)|] 

        Dlr.Fast.block [||] [|
          Dlr.ifElse 
            (cacheId .== jsobj .-> "PropertySchema" .-> "Id")
            (Dlr.Fast.block [|index; properties|] [|
              properties .= jsobj .-> "Properties"
              index .= cacheIndex
              Utils.assign (Dlr.index properties [index] .-> "Value") value
            |])
            (Dlr.call cache "Put" fallbackArgs)
          value
        |]

    //
    let fromClrObject (clrobj:Dlr.Expr) (name:string)  (value:Dlr.Expr) (ctx:Ctx) = 
      let env = ctx.Target.Environment
      value
      
    //
    let fromJsValue (jsval:Dlr.Expr) (name:string)  (value:Dlr.Expr) (ctx:Ctx) =
      Dlr.Fast.block [||] [|value|]

    //
    let fromBox (expr:Dlr.Expr) (name:string) throw (ctx:Ctx) =
      Dlr.ternary
        (Utils.Box.isObject expr)
        (fromJsObject (Utils.Box.unboxObject expr) name throw ctx)
        (Dlr.ternary
          (Utils.Box.isClr expr)
          (fromClrObject (Utils.Box.unboxClr expr) name value ctx)
          (fromJsValue expr name value ctx)
        )

    let body = new Dlr.ExprList(3)
    let vars = new Dlr.ParameterList(2)
    let expr = expr |> Utils.toStatic vars body
    let value = value |> Utils.toStatic vars body

    match TypeTag.OfType(expr.Type) with
    | TypeTags.Box -> 
      body.Add(fromBox expr name value ctx)

    | TypeTags.Object
    | TypeTags.Function ->
      body.Add(fromJsObject expr name value ctx)

    | TypeTags.String
    | TypeTags.SuffixString
    | TypeTags.Undefined
    | TypeTags.Bool
    | TypeTags.Number ->
      body.Add(fromJsValue expr name value ctx)

    | TypeTags.Clr ->
      body.Add(fromClrObject expr name value ctx)

    Dlr.block vars body
    
  /// MemberExpression . String = Expression
  let putMember_Ast (ctx:Ctx) (ast:Ast.Tree) (name:string) (value:Ast.Tree) =
    putMember ctx (ctx |> compile ast) name (ctx |> compile value)

  /// MemberExpression . String
  let getMember (ctx:Ctx) (expr:Dlr.Expr) (name:string) (throwOnMissing:bool) =
    
    //
    let makePropertyGetCache (throwOnMissing:bool) (env:Env) =
      let cache = !!!(new Runtime.Optimizations.InlinePropertyGetCache(env, throwOnMissing))
      let cacheId = cache .-> "CachedId"
      let cacheIndex = cache .-> "CachedIndex"
      cache, cacheId, cacheIndex

    //
    let fromJsObject (jsobj:Dlr.Expr) (name:string) throw (ctx:Ctx) =
      let env = ctx.Target.Environment
      let cache, cacheId, cacheIndex = env |> makePropertyGetCache throw

      Dlr.ternary 
        (cacheId .== jsobj .-> "PropertySchema" .-> "Id")
        (Dlr.index (jsobj .-> "Properties") [cacheIndex] .-> "Value")
        (Dlr.call cache "Get" [|jsobj; !!!name|])

    //
    let fromClrObject (clrobj:Dlr.Expr) (name:string) (ctx:Ctx) = 
      let env = ctx.Target.Environment
      Dlr.callStaticT<BoxingUtils> "JsBox" [
        Dlr.dynamic typeof<obj> (new Runtime.Binders.GetMemberBinder(name,env)) [clrobj]
      ]
      
    //
    let fromJsValue (jsval:Dlr.Expr) (name:string) (ctx:Ctx) =
      Dlr.call (Utils.Convert.toObject ctx jsval) "Get" [!!!name]

    //
    let fromBox (expr:Dlr.Expr) (name:string) throw (ctx:Ctx) =
      Dlr.ternary
        (Utils.Box.isObject expr)
        (fromJsObject (Utils.Box.unboxObject expr) name throw ctx)
        (Dlr.ternary
          (Utils.Box.isClr expr)
          (fromClrObject (Utils.Box.unboxClr expr) name ctx)
          (fromJsValue expr name ctx)
        )

    let body = new Dlr.ExprList(2)
    let vars = new Dlr.ParameterList(1)
    let expr = expr |> Utils.toStatic vars body

    match TypeTag.OfType(expr.Type) with
    | TypeTags.Box -> 
      body.Add(fromBox expr name throwOnMissing ctx)

    | TypeTags.Object
    | TypeTags.Function ->
      body.Add(fromJsObject expr name throwOnMissing ctx)

    | TypeTags.String
    | TypeTags.SuffixString
    | TypeTags.Undefined
    | TypeTags.Bool
    | TypeTags.Number ->
      body.Add(fromJsValue expr name ctx)

    | TypeTags.Clr ->
      body.Add(fromClrObject expr name ctx)

    Dlr.block vars body

  /// MemberExpression . String
  let getMember_Ast (ctx:Ctx) (ast:Ast.Tree) (name:string) (throwOnMissing:bool) =
    getMember ctx (ctx |> compile ast) name throwOnMissing
