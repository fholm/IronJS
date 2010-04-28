namespace IronJS.Ast

type BinaryOp 
  = Lt        // <
  | LtEq      // <=
  | Gt        // >
  | GtEq      // >=
  | Eq        // ==
  | NotEq     // !=
  | Add       // +
  | AddAsn    // +=
  | Sub       // -
  | SubAsn    // -=
  | Mul       // *
  | MulAsn    // *=
  | Div       // /
  | DivAsn    // /=
  | Mod       // %
  | ModAsn    // %=

type UnaryOp 
  = PreInc    // ++
  | PreDec    // --