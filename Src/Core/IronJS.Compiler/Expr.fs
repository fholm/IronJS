namespace IronJS.Compiler

  open System
  open IronJS
  open IronJS.Aliases
  open IronJS.Utils
  open IronJS.Compiler

  //-------------------------------------------------------------------------
  // Utility functions for compiling IronJS ast trees
  module Expr =

    let returnBoxedBool (ctx:Ctx) boolExpr =
      Dlr.blockSimple [
        (Dlr.assign (Expr.unboxBool ctx.Env_Temp_Bool) boolExpr)
        (ctx.Env_Temp_Bool)
      ]

    let returnBoxedNumber (ctx:Ctx) number =
      Dlr.blockSimple [
        (Dlr.assign (Expr.unboxNumber ctx.Env_Temp_Number) number)
        (ctx.Env_Temp_Number)
      ]

    let returnBoxedClr (ctx:Ctx) clr =
      Dlr.blockSimple [
        (Dlr.assign (Expr.unboxClr ctx.Env_Temp_Clr) clr)
        (ctx.Env_Temp_Clr)
      ]

    let returnBoxedString (ctx:Ctx) str =
      Dlr.blockSimple [
        (Dlr.assign (Expr.unboxString ctx.Env_Temp_String) str)
        (ctx.Env_Temp_String)
      ]

    let returnBoxedObject (ctx:Ctx) object' =
      Dlr.blockSimple [
        (Dlr.assign (Expr.unboxObject ctx.Env_Temp_Object) object')
        (ctx.Env_Temp_Object)
      ]
      
    let returnBoxedFunction (ctx:Ctx) function' =
      Dlr.blockSimple [
        (Dlr.assign (Expr.unboxObject ctx.Env_Temp_Function) function')
        (ctx.Env_Temp_Function)
      ]