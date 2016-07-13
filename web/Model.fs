namespace Web

open System


module Tree =
  type T<'a> =
    | Node of 'a * T<'a> * T<'a>
    | Leaf
