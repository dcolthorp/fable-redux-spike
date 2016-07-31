module App

open System
open Fable.Core
open Fable.Import

module React = Fable.Import.React
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

open Fable.Helpers

open Model

let nothing () = Nothing
let addTodo (user : User.T) () = Create (Todo.make user.Id "Hello!")

let toString = function
  | Nothing -> "Nothing"
  | Create _ -> "Create"

type TodoListProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodoListProps> option) =
  let state' =
    match maybeStore with
      | None -> failwith "Need an initial state"
      | Some store ->
        store.getState() : TodosState

  interface ReactRedux.Property<TodoListProps>

  member val state = state'
  member val user = User.make state'.UserId

  member val store = maybeStore
  member val children = maybeChildren


let createActionButton (actionLabel, dispatcher) =
  Tag.button [ Attr.Key actionLabel
               Attr.OnClick dispatcher
             ]
    [ Tag.h1 [] [unbox actionLabel]]

type TodoList(props) as this =
  inherit React.Component<TodoListProps, TodosState>(props)

  let labelFrom actionCreator =  actionCreator() |> toString

  let dispatcherFrom actionCreator (_:React.MouseEvent) =
    let act = actionCreator()
    ReactRedux.dispatch props act

  let makeTodo (t : Todo.T) =
    TodoItem.make <|
      TodoItem.TodoItemProps(
        todo = t,
        user = props.user,
        maybeStore = props.store,
        maybeChildren = None)

  let buttons =
    [ nothing; addTodo props.user ]
    |> List.map (fun actionCreator -> (labelFrom actionCreator, dispatcherFrom actionCreator))
    |> List.map createActionButton

  member self.render() =

    let {List = a} =  props.state in
      Tag.div [] [
        Tag.ul [] (List.map makeTodo a)
        Tag.div [] buttons
        Tag.div [] [unbox <| sprintf "Logged in as User %O" props.user.Id]
      ]


TodoList?props <- createObj [
  "state" ==> React.PropTypes.object
  "user" ==> React.PropTypes.object
]

let stateMapper =
  Func<TodosState, TodoListProps option, obj>(
    fun a b ->
      let res = createObj [
          "state" ==> box a;
          "user" ==> box b.Value.user]
      res)


let provider = ReactRedux.buildProvider TodoList stateMapper
