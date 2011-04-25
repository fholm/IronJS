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

///
module internal Object =

  ///
  module Property = 

    ///
    let private makeInlineCache() =
      let cache = !!!(new Runtime.Optimizations.InlinePropertyPutCache())
      let cacheId = cache .-> "CachedId"
      let cacheIndex = cache .-> "CachedIndex"
      cache, cacheId, cacheIndex

    ///
    let putBox expr name value =
      let cache, cacheId, cacheIndex = makeInlineCache()

      tempBlockT<CO> expr (fun tmp -> 
        [tempBlock value (fun valueTmp ->
          let args = [expr; name; valueTmp]

          [
            Dlr.ternary 
              (cacheId .== (expr .-> "PropertySchema" .-> "Id"))
              (Dlr.block [] [(Dlr.index (expr .-> "Properties") [cacheIndex] .-> "Value") .= valueTmp; Dlr.void'])
              (call cache "Put" args)

            valueTmp
          ]
        )]
      )
    
    ///
    let putRef expr name (value:Dlr.Expr) =
      let cache, cacheId, cacheIndex = makeInlineCache()

      tempBlockT<CO> expr (fun tmp -> 
        [tempBlock value (fun valueTmp ->
          let tag = valueTmp.Type |> TypeTag.OfType |> Dlr.const'
          let args = [expr; name; valueTmp; tag]

          [
            Dlr.ternary 
              (cacheId .== (expr .-> "PropertySchema" .-> "Id"))
              (Dlr.block [] [
                Dlr.index (expr .-> "Properties") [cacheIndex] .-> "Value" .-> "Clr" .= valueTmp
                Dlr.index (expr .-> "Properties") [cacheIndex] .-> "Value" .-> "Tag" .= tag
                Dlr.index (expr .-> "Properties") [cacheIndex] .-> "HasValue" .= !!!true
                Dlr.void'
              ])
              (call cache "Put" args)

            valueTmp
          ]
        )]
      )
    
    ///
    let putVal expr name (value:Dlr.Expr) =
      let cache, cacheId, cacheIndex = makeInlineCache()

      tempBlockT<CO> expr (fun tmp -> 
        [tempBlock value (fun valueTmp ->
          let args = [expr; name; Utils.normalizeVal valueTmp]
          
          let idx = Dlr.paramT<int> "~index"
          let prp = Dlr.paramT<Descriptor array> "~properties"

          [
            Dlr.ternary 
              (cacheId .== (expr .-> "PropertySchema" .-> "Id"))

              (Dlr.block [idx; prp] [
                idx .= cacheIndex
                prp .= (expr .-> "Properties")
                Dlr.index prp [idx] .-> "Value" .-> "Number" .= (Utils.normalizeVal valueTmp)
                Dlr.index prp [idx] .-> "HasValue" .= !!!true
                Dlr.void'
              ])

              (call cache "Put" args)

            valueTmp
          ]
        )]
      )

    ///
    let put name (value:Dlr.Expr) expr = 
      match value with
      | IsBox -> putBox expr name value
      | IsRef -> putRef expr name value
      | IsVal -> putVal expr name value

    ///
    let putName expr name (value:Dlr.Expr) = 
      let name = Dlr.const' name
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

  /// MemberExpression [ Expression ]
  let getIndex (ctx:Ctx) object' index =
    let index = index |> Utils.compileIndex ctx
    let object' = ctx $ compile object'

    ensureObject ctx object'
      (fun x -> Index.get x index)
      (fun x -> 
        (Dlr.ternary 
          (Dlr.callStaticT<Object> "ReferenceEquals" [Dlr.castT<obj> x; Dlr.null'])
          (Dlr.callGeneric ctx.Env "RaiseTypeError" [typeof<BV>] [!!!ErrorUtils.nextErrorId()])
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

  /// 
  let toStatic (vars:Dlr.ParameterList) (body:Dlr.ExprList) (expr:Dlr.Expr) =
    if expr |> Dlr.isStatic then
      expr

    else
      let temp = Dlr.tempFor expr
      vars.Add(temp)
      body.Add(temp .= expr)
      temp :> Dlr.Expr

  ///
  let makePropertyGetCache (throwOnMissing:bool) (env:Env) =
    let cache = !!!Runtime.Optimizations.InlinePropertyGetCache(env, throwOnMissing)
    let cacheId = cache .-> "CachedId"
    let cacheIndex = cache .-> "CachedIndex"
    cache, cacheId, cacheIndex

  ///
  let getMember (ctx:Ctx) (expr:Dlr.Expr) (name:string) (throwOnMissing:bool) =
    
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
      Utils.Constants.Boxed.undefined
      
    //
    let fromJsValue (jsval:Dlr.Expr) (name:string) (ctx:Ctx) =
      Dlr.call (TC.ToObject(ctx.Env, jsval)) "Get" [!!!name]
      
    let body = new Dlr.ExprList(2)
    let vars = new Dlr.ParameterList(2)
    let expr = expr |> toStatic vars body

    match TypeTag.OfType(expr.Type) with
    | TypeTags.Box -> 
      body.Add(
        Dlr.ternary
          (Utils.Box.isObject expr)
          (fromJsObject (Utils.Box.unboxObject expr) name throwOnMissing ctx)
          (Dlr.ternary
            (Utils.Box.isClr expr)
            (fromClrObject (Utils.Box.unboxClr expr) name ctx)
            (fromJsValue expr name ctx)
          )
      )

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

  ///
  let getMember_Ast (ctx:Ctx) (ast:Ast.Tree) (name:string) (throwOnMissing:bool) =
    getMember ctx (ctx |> compile ast) name throwOnMissing
