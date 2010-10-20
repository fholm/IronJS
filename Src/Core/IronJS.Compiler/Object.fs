namespace IronJS.Compiler

open System
open IronJS
open IronJS.Expr.Patterns

module Object =

  //----------------------------------------------------------------------------
  let ghd expr method' name = 
    Expr.blockTmpT<IjsObj> expr (fun tmp -> 
      [Dlr.invoke (method' tmp) [tmp; name]])
  
  //----------------------------------------------------------------------------
  module Property = 

    //--------------------------------------------------------------------------
    let putBox expr name value =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke
          (Expr.Object.Methods.putBoxProperty tmp)
          [tmp; name; value]])
    
    //--------------------------------------------------------------------------
    let putRef expr name value =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke
          (Expr.Object.Methods.putRefProperty tmp)
          [tmp; name; value; value |> Utils.expr2tc |> Dlr.const']])
    
    //--------------------------------------------------------------------------
    let putVal expr name (value:Dlr.Expr) =
      Expr.blockTmpT<IjsObj> expr (fun tmp -> 
        [Dlr.invoke
          (Expr.Object.Methods.putValProperty tmp)
          [tmp; name; Expr.normalizeVal value]])

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
        [Dlr.callStaticT<Api.ObjectModule.Index.Converters> 
          "put" [tmp; index; value]])
    
    //--------------------------------------------------------------------------
    let putBox expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [Dlr.invoke
            (Expr.Object.Methods.putBoxIndex tmp)
            [tmp; index; value]])

      | TypeCode -> convertIndex_BoxVal expr index value

    //--------------------------------------------------------------------------
    let putVal expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [Dlr.invoke
            (Expr.Object.Methods.putValIndex tmp)
            [tmp; index; Expr.normalizeVal value]])
            
      | TypeCode -> convertIndex_BoxVal expr index value

    //--------------------------------------------------------------------------
    let putRef expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [Dlr.invoke
            (Expr.Object.Methods.putRefIndex tmp)
            [tmp; index; value; value |> Utils.expr2tc |> Dlr.const']])
            
      | TypeCode -> 
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [Dlr.callStaticT<Api.ObjectModule.Index.Converters> 
            "put" [tmp; index; value; value |> Utils.expr2tc |> Dlr.const']])
      
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