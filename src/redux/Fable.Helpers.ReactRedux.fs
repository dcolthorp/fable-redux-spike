namespace Fable.Helpers

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Import.React
open Fable.Import.Redux
open Fable.Import.ReactRedux

module ReactRedux =
  let dispatch (props : Property<'T>) action =
    match (props :> Property<'T>).store with
      | None -> failwith "Cannot create action dispatcher without a Redux store"
      | Some store ->
        action
        |> Redux.toObj
        |> store.dispatch
        |> ignore
