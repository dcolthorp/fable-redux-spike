module TodoItem

open System
open Fable.Core
open Fable.Import

module React = Fable.Import.React
module Redux = Fable.Helpers.Redux
module ReactRedux = Fable.Helpers.ReactRedux
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

open Model


// TODO: Create a subcomponent for todo item

// type TodoItemProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodosProps> option) =
//   member val todo : Todo.T option = None
//   member val user = {Id = UID 1}

//   interface ReactRedux.Property<TodosProps> with
//     member val store = maybeStore
//     member val children = maybeChildren
