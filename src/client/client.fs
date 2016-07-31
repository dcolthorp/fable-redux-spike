module Client

open System
open System.Diagnostics
open Fable.Core

open Fable.Import
open Fable.Import.Redux
open Fable.Import.ReactRedux
open Fable.Import.Browser

open Fable.Helpers

module R = Fable.Helpers.React
open R.Props

open Model
open App


let reducer = Redux.reducer Actions.perform
let store = Fable.Import.Redux.Globals.createStore(reducer, {
  List = [Todo.make (UID 2) "Someone else's"]
  UserId = UID 1;
})

printfn "Store state is %A" <| (store.getState() : TodosState)

let initialProps = TodoListProps(Some store, None)

ReactDom.render(
    App.provider initialProps,
    Browser.document.getElementById "content")
|> ignore
