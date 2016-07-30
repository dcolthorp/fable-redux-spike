module TodoItem

open System
open Fable.Core
open Fable.Import

module React = Fable.Import.React
module Redux = Fable.Helpers.Redux
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

open Model

open Fable.Helpers

// TODO: Create a subcomponent for todo item

type TodoItemProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodoItemProps> option) =
  abstract todo : Todo.T
  abstract user : User.T

  interface ReactRedux.Property<TodoItemProps> with
    member val store = maybeStore
    member val children = maybeChildren

type TodoList(props) as this =
  inherit React.Component<TodoItemProps, Todo.T>(props)

  let todo = props.todo

  member self.render() =
    let check = match Perm.requestComplete props.user todo with
      | None -> Tag.div [] []
      | Some perm ->
        let toggle (_ : React.FormEvent) = ReactRedux.dispatch props (ToggleComplete perm)
        Tag.input [Attr.Type "checkbox"; Attr.Checked todo.Done; Attr.OnChange toggle] []

    Tag.li [] [
      check;
      unbox <| sprintf "User %O - " todo.Owner
      unbox todo.Text;
    ]
