module Client

open System
open System.Diagnostics
open Fable.Core

open Fable.Import
open Fable.Import.Redux
open Fable.Import.ReactRedux
open Fable.Import.Browser

module R = Fable.Helpers.React
open R.Props

open Model
open Components

let initialProps = TodosProps(Some store, None)

ReactDom.render(
    Components.provider initialProps,
    Browser.document.getElementById "content")
|> ignore
