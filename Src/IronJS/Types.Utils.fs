namespace IronJS

module ConvertTypes =

  (*Converts a ClrType object to JsType enum*)
  let clrToJs typ = 
    if   typ = typeof<double>             then Types.Double
    elif typ = typeof<int>                then Types.Integer
    elif typ = typeof<string>             then Types.String
    elif typ = typeof<bool>               then Types.Boolean
    elif typ = typeof<Runtime.Object>     then Types.Object
    elif typ = typeof<Runtime.Function>   then Types.Function
    elif typ = typeof<Runtime.Undefined>  then Types.Undefined
    elif typ = typeof<ClrObject>          then Types.Clr
    elif typ = typeof<Runtime.Box>        then Types.Dynamic
    else failwithf "Invalid type '%s'" typ.Name

  let normalizeJsType typ =
    match typ with
    #if ONLY_DOUBLE
    | Types.Number
    | Types.Double    
    | Types.Integer   -> Types.Double
    #else
    | Types.Number
    | Types.Double    -> Types.Double
    | Types.Integer   -> Types.Integer
    #endif

    | Types.Boolean   -> Types.Boolean
    
    | Types.StringNull 
    | Types.String    -> Types.String

    | Types.ObjectNull
    | Types.Object    -> Types.Object
    
    | Types.ArrayNull
    | Types.Array     -> Types.Array

    | Types.FunctionNull
    | Types.Function  -> Types.Function

    | Types.UndefinedNull
    | Types.Undefined -> Types.Undefined
    
    | Types.Null
    | Types.Clr       -> Types.Clr
    | Types.ClrNull   -> Types.Clr

    //Special cases that all result in Object
    | Types.ArrFunc
    | Types.ArrFuncNull
    | Types.ObjArr
    | Types.ObjArrNull
    | Types.ObjFunc
    | Types.ObjFuncNull
    | Types.ObjFuncArr
    | Types.ObjFuncArrNull -> Types.Object

    //Full dynamic typing
    | _ -> Types.Dynamic

  (*Converts a JsType enum to ClrType object*)
  let rec jsToClr typ =
    match typ with
    #if ONLY_DOUBLE
    | Types.Double 
    | Types.Integer   -> typeof<double>
    #else
    | Types.Double    -> typeof<double>
    | Types.Integer   -> typeof<int>
    #endif
    | Types.Boolean   -> typeof<bool>
    | Types.String    -> typeof<string>
    | Types.Object    -> typeof<Runtime.Object>
    | Types.Function  -> typeof<Runtime.Function>
    | Types.Undefined -> typeof<Runtime.Undefined>
    | Types.Dynamic   -> typeof<Runtime.Box>
    | Types.Null      
    | Types.Clr       -> typeof<ClrObject>
    | _               -> jsToClr (normalizeJsType typ)
