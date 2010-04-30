namespace IronJS.Ast.Flags

  type Variable
    = Parameter       = 1
    | ClosedOver      = 2
    | InitToUndefined = 4
    | TypeResolved    = 8
    | NeedProxy       = 16
  
  type Scope
    = HasDS = 1
    | InLocalDS = 2
    | NeedGlobals = 4
    | NeedClosure = 8
