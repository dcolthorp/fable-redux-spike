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

type Props(maybeStore, maybeChildren, todo, user) =

  member val todo : Todo.T = todo
  member val user : User.T = user

  interface ReactRedux.Property<Props> with
    member val store = maybeStore
    member val children = maybeChildren

type TodoList(props) =
  inherit React.Component<Props, Todo.T>(props)

  let todo = props.todo

  member self.render() =
    let check =
      match Perm.requestComplete props.user todo with
        | None -> Tag.div [] []
        | Some perm ->
          let toggle (_ : React.FormEvent) = ReactRedux.dispatch props (ToggleComplete perm)
          Tag.input [Attr.Type "checkbox"; Attr.Checked todo.Done; Attr.OnChange toggle] []

    Tag.li [] [
      check;
      unbox <| sprintf "User %O - " todo.Owner
      unbox todo.Text;
    ]

let stateMapper =
  Func<Todo.T, Props option, obj>(
    fun a b ->
      let res =
        createObj [
          "state" ==> box a;
          "user" ==> box b.Value.user]
      res)

let make (props : Props) = ReactRedux.factory TodoList stateMapper props
