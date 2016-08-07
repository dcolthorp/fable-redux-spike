namespace Fable.Helpers

open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import

open Fable.Import.JS
open Fable.Import.Redux
open Fable.Import.ReactRedux

module React = Fable.Import.React
module Redux = Fable.Helpers.Redux
module Tag = Fable.Helpers.React
module RR = Fable.Import.ReactRedux

module ReactRedux =
  type Globals() =
    member this.dispatchAction (props : Property<'T>) (action : Redux.Action<'A>) =
      match props.store with
        | None -> failwith "Cannot create action dispatcher without a Redux store"
        | Some store ->
          action
          |> Redux.toObj
          |> store.dispatch
          // |> (fun x -> printfn "Store value dispatched to %A" (store.getState()))
          |> ignore

    member this.dispatch (props : Property<'T>) (payload : 'A) =
      this.dispatchAction props (Redux.action payload)

    member this.factory comp stateMapper =
      let com = RR.Globals.connect(stateMapper).Invoke(comp)
      fun props ->
        React.createElement(Case1 com, props |> JsInterop.toPlainJsObj)

    member this.buildProvider comp (stateMapper : MapStateToProps<'S, 'P>) =
      let f = this.factory comp stateMapper
      fun (props : 'P) ->
        Tag.com<RR.Provider<'S, 'P>, RR.Property<'P>, 'S> props [
          f props
        ]

[<AutoOpen>]
module ReactRedux_extensions =
  let ReactRedux = new ReactRedux.Globals()
