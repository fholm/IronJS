namespace IronJS

type Types 
  = Nothing   = 0   // NOT null

  | Double    = 1
  | Integer   = 2
  | Number    = 3   // Double | Integer

  | Boolean   = 4
  | String    = 8

  | Object    = 16
  | Function  = 32
  | Array     = 64

  | Undefined = 128
  | Null      = 256
  | Dynamic   = 512
  | Clr       = 1024
  | ClrNull   = 1280 // Clr | Null
  
  // Special combined types to allow us to keep 
  // strong typing if the only non-typed value is null
  | StringNull    = 264 // String | Null
  | ObjectNull    = 272 // Object | Null
  | FunctionNull  = 288 // Function | Null
  | ArrayNull     = 320 // Array | Null
  | UndefinedNull = 384 // Undefined | Null

  // Special combined types for JavaScript objects
  // to allow strong typing as Runtime.Object
  | ObjFuncArr      = 112 // Object | Function | Array
  | ObjFuncArrNull  = 368 // Object | Function | Array | Null
  | ObjArr          = 80  // Object | Array
  | ObjArrNull      = 336 // Object | Array | Null
  | ObjFunc         = 48  // Object | Function
  | ObjFuncNull     = 304 // Object | Function | Null
  | ArrFunc         = 96  // Array | Function
  | ArrFuncNull     = 352 // Array | Function | Null