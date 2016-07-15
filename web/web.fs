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

  type Model = Yo
  type Action = Do

  let initModel = Yo
  let todoView a = div [] [text "hello!"]


  let todoUpdate model msg =
    Yo, []

  let act () = async {
    do! Async.Sleep 1
    printfn "Yo!"
    }

  let app : App<Model, Action> = createApp {Model = initModel; View = todoView; Update = todoUpdate}
  let processor = app |> start renderer
