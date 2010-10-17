namespace IronJS.Compiler

open System
open IronJS
open IronJS.Expr.Patterns

module Object =
  
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
    
    //--------------------------------------------------------------------------
    let putBox expr index value =
      match index with
      | Index ->
        Expr.blockTmpT<IjsObj> expr (fun tmp -> 
          [Dlr.invoke
            (Expr.Object.Methods.putBoxIndex tmp)
            [tmp; index; value]])

      | TypeCode -> failwith "Que?"
        

