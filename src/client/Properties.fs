module Properties

open Fable.Core
open Fable.Import
open Fable.Import.JS
module React = Fable.Import.React
module Redux = Fable.Import.Redux
module ReactRedux = Fable.Import.ReactRedux
module FsMap = FSharp.Collections.Map

open Model

type TodosProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodosProps> option) =
  // member this.increment = ActionCreators.increment
  // member this.decrement = ActionCreators.decrement
  // member this.count = initialState

  interface ReactRedux.Property<TodosProps> with
    member val store = maybeStore
    member val children = maybeChildren
