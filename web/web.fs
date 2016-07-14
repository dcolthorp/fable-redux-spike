namespace Web

open System
open System.Diagnostics
open Fable.Core

open Fable.Import
open Fable.Import.Browser

open Fable.Helpers.Virtualdom
open Fable.Helpers.Virtualdom.App
open Fable.Helpers.Virtualdom.Html

module Main =
  open Tree

  Debug.WriteLine "Yo"
  let t =
    Node (1, Leaf, Leaf)

  printfn "Hello will, this is a value %A" Leaf
