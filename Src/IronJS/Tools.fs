module IronJS.Tools

//Imports
open IronJS.Utils
open IronJS.EtTools

let closureVal (parm:EtParam) (num:int) =
  field (field parm (sprintf "Item%i" num)) "Value"

