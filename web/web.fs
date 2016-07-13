
namespace Web

open System
open Fable.Core

module Main =
  open Tree

  let t =
    Node (1, Leaf, Leaf)

  printfn "%A" t
