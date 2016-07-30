namespace Fable.Helpers

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS
open Fable.Import.React
open Fable.Import.Redux
open Fable.Import.ReactRedux

module Redux = Fable.Helpers.Redux

module ReactRedux =
  let dispatchAction (props : Property<'T>) (action : Redux.Action<'A>) =
    match (props :> Property<'T>).store with
      | None -> failwith "Cannot create action dispatcher without a Redux store"
      | Some store ->
        action
        |> Redux.toObj
        |> store.dispatch
        |> ignore

  let dispatch (props : Property<'T>) (payload : 'A) =
    dispatchAction props (Redux.action payload)
