namespace IronJS.Api

open IronJS

module Expr =

  //----------------------------------------------------------------------------
  //
  // Object property/index
  //
  //----------------------------------------------------------------------------

  //----------------------------------------------------------------------------
  let private _jsObjectPropertyCacheExprs expr =
    Dlr.field expr "PropertyIndex", Dlr.field expr "PropertyClassId"

  //----------------------------------------------------------------------------
  let jsObjectGetProperty expr name = 
    let cache = Dlr.const' (new IronJS.GetPropertyCache(name))
    let propertyIndex, propertyClassId = _jsObjectPropertyCacheExprs cache
    Expr.blockTmpT<IronJS.Object> expr (fun tmp -> 
      [
        (Dlr.ternary
          (Dlr.eq propertyClassId (Dlr.field tmp "PropertyClassId"))
          (Expr.propertyValue tmp propertyIndex)
          (Dlr.callStaticT<Api.GetPropertyCache> "update" [cache; tmp])
        )
      ]
    )
      
  //----------------------------------------------------------------------------
  let jsObjectPutProperty expr name value = 
    let cache = Dlr.const' (new IronJS.PutPropertyCache(name))
    let propertyIndex, propertyClassId = _jsObjectPropertyCacheExprs cache
    Expr.blockTmpT<IronJS.Object> expr (fun tmp -> 
      [
        (Dlr.ternary
          (Dlr.eq propertyClassId (Dlr.field tmp "PropertyClassId"))
          (Dlr.blockTmpT<int> (fun index ->
            [
              (Dlr.assign index propertyIndex)
              (Expr.assignBoxValue
                (Expr.propertyValue tmp index)
                (value)
              )
            ] |> Seq.ofList
          ))
          (Dlr.callStaticT<Api.PutPropertyCache> "update" [cache; tmp; value])
        )
      ]
    )
      
  //----------------------------------------------------------------------------
  let jsObjectUpdateProperty expr name value = 
    let cache = Dlr.const' (new IronJS.PutPropertyCache(name))
    let propertyIndex, propertyClassId = _jsObjectPropertyCacheExprs cache
    Expr.blockTmpT<IronJS.Object> expr (fun tmp -> 
      [
        (Dlr.ternary
          (Dlr.eq propertyClassId (Dlr.field tmp "PropertyClassId"))
          (Expr.updateBoxValue 
            (Expr.propertyValue tmp propertyIndex)
            (value)
          )
          (Dlr.callStaticT<Api.PutPropertyCache> "update" [cache; tmp; value])
        )
      ]
    )

  let isIndex (casted:Dlr.Expr) (original:Dlr.Expr) =
    if original.Type = typeof<uint32> 
      then Dlr.gt casted Dlr.neg1
      else Dlr.eq (Dlr.castT<IjsNum> casted) original
        
  //----------------------------------------------------------------------------
  let jsObjectPutIndex expr index value =
    match Utils.expr2tc index with
    | TypeCodes.Number ->
      Expr.blockTmpT<IjsObj> expr (fun tmp ->
        [
          Expr.blockTmpT<int> index (fun i ->
            [
              (Dlr.ternary
                (Dlr.andChain [
                  (isIndex i index)
                  (Dlr.lt (Dlr.castT<uint32> i) (Dlr.field tmp "IndexLength"))
                  (Dlr.isNotNull (Dlr.field tmp "IndexValues"))
                ])
                (Expr.assignValue
                  (Dlr.index (Dlr.field tmp "IndexValues") [i])
                  (value)
                )
                (Dlr.callStaticT<Api.Object> "putIndex" [tmp; index; value])
              )
            ]
          )
        ]
      )

    | _ -> 
      Expr.blockTmpT<IronJS.Object> expr (fun tmp ->
        [Dlr.callStaticT<Api.Object> "putIndex" [tmp; index; value]]
      )
      
  //----------------------------------------------------------------------------
  let jsObjectGetIndex expr index =
    Expr.blockTmpT<IjsObj> expr (fun tmp ->
      [
        Expr.blockTmpT<int> index (fun i ->
          [
            (Dlr.ternary
              (Dlr.andChain [
                (isIndex i index)
                (Dlr.lt (Dlr.castT<uint32> i) (Dlr.field tmp "IndexLength"))
                (Dlr.isNotNull (Dlr.field tmp "IndexValues"))
              ])
              (Dlr.index (Dlr.field tmp "IndexValues") [i])
              (Dlr.callStaticT<Api.Object> "getIndex" [tmp; index])
            )
          ]
        )
      ]
    )

  //----------------------------------------------------------------------------
  //
  // Functions/methods
  //
  //----------------------------------------------------------------------------
      
  //----------------------------------------------------------------------------
  let private jsFunctionType args =
      Utils.createDelegate (
        List.foldBack (fun (itm:Dlr.Expr) types ->
          let type' = 
            if itm.Type = typeof<IjsBox> 
              then itm.Type.MakeByRefType() 
              else itm.Type

          type'::types
        ) args [typeof<IronJS.Box>]
      )
        
  //----------------------------------------------------------------------------
  let jsFunctionInvoke func this' args =
    Expr.blockTmpT<IjsFunc> func (fun f -> 
      let args = f :: this' :: args
      let t = jsFunctionType args
      let c = Dlr.const' (Api.InvokeCache.create t)
      let cached = Dlr.field c "Cached"
      let cachedId = Dlr.field c "FunctionId"
      [
        (Dlr.ternary
          (Dlr.eq cachedId (Dlr.field f "FunctionId"))
          (Dlr.invoke cached args)
          (Dlr.invoke
            (Dlr.callStaticGenericT<Api.InvokeCache> "update" [t] [c; f])
            (args)
          )
        )
      ]
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

