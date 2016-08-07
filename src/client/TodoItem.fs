module TodoItem

open System
open Fable.Core
open Fable.Import
open Fable.Core.JsInterop

module React = Fable.Import.React
module Redux = Fable.Helpers.Redux
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

open Model

open Fable.Helpers

type Props(maybeStore, maybeChildren, todo, user) =

  member val todo : Todo = todo
  member val user : User.T = user

  interface ReactRedux.Property<Props> with
    member val store = maybeStore
    member val children = maybeChildren

type TodoItem(props) =
  inherit React.Component<Props, Todo>(props)

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

// let stateMapper = Func<Todo, Props option, obj>( fun a b -> JsInterop.createObj [] )
let stateMapper =
  Func<Todo, Props option, obj>(
    fun a b ->
      let res =
        createObj [
          "state" ==> box a;
          "user" ==> box b.Value.user]
      res)
let make (props : Props) = ReactRedux.factory TodoItem stateMapper props

//
//type State = {
//  todo : Todo
//  user : User.T
//}
//
//type Event = {
//  change : React.FormEvent -> unit
//}
//
//type props
//
//let TodoItem (s : State, e : Event) =
//  let todo = s.todo
//
//  let check =
//    match Perm.requestComplete s.user todo with
//      | None -> Tag.div [] []
//      | Some perm ->
//        Tag.input [Attr.Type "checkbox"; Attr.Checked todo.Done; Attr.OnChange e.change] []
//
//  Tag.li [] [
//    check;
//    unbox <| sprintf "User %O - " todo.Owner
//    unbox todo.Text;
//  ]
//
//
//let make (props : Props) = ReactRedux.factory TodoItem props
