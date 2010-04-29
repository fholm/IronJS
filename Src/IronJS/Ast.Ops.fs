namespace IronJS.Ast

type BinaryOp 
  //Logical
  = AndAlso     // &&
  | OrElse      // ||
  | Lt          // <
  | LtEq        // <=
  | Gt          // >
  | GtEq        // >=
  | Eq          // ==
  | NotEq       // !=
  | EqEq        // ===
  | NotEqEq     // !==
  | InstanceOf  // instanceof
  | In          // in

  //Math
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

  //Bit
  | And       // &
  | AndAsn    // &=
  | Or        // |
  | OrAsn     // |=
  | Xor       // ^
  | XorAsn    // ^=
  | LShift    // <<
  | LShiftAsn // <<=
  | RShift    // >>
  | RShiftAsn // >>=
  | UShift    // >>>
  | UShiftAsn // >>>=

type UnaryOp 
  = PreInc    // ++
  | PreDec    // --
  | Plus      // +
  | Minus     // -
  | Not       // !
  | Comp      // ~
  | Void      // void
  | Delete    // delete
  | TypeOf    // typeof