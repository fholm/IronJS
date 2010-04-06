module IronJS.Operators

let inline flip f a b = f b a
let inline pair f (a, b) = f a b