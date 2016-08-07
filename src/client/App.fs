module App

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

module React = Fable.Import.React
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

open Fable.Helpers

open Model

let addTodo (user : User.T) () = Create (Todo.make user.Id "Hello!")
let toggleUser (user : User.T) () =
  let newId =
    match User.idValue user with
      | 1 -> UID 2
      | 2 -> UID 1
      | x -> failwithf "Unknown id %A" x
  ChangeUser newId

type TodoListProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodoListProps> option) =
  let state' =
    match maybeStore with
      | None -> failwith "Need an initial state"
      | Some store ->
        store.getState() : TodosState


  member val state = state'
  member val user = User.make state'.UserId

  interface ReactRedux.Property<TodoListProps> with
    member val store = maybeStore
    member val children = None



type TodoList(props) =
  inherit React.Component<TodoListProps, TodosState>(props)

  member self.render() =
    let toString =
      function
        | ChangeUser _ -> "Swap User"
        | Create _ -> "Create"

    let labelFrom actionCreator =  actionCreator() |> toString
    let createActionButton (actionLabel, dispatcher) =
      Tag.button [ Attr.Key actionLabel; Attr.OnClick dispatcher] [unbox actionLabel]

    let dispatcherFrom actionCreator (_:React.MouseEvent) =
      let act = actionCreator()
      ReactRedux.dispatch props act

    let buttons =
      [ toggleUser props.user; addTodo props.user ]
      |> List.map (fun actionCreator -> (labelFrom actionCreator, dispatcherFrom actionCreator))
      |> List.map createActionButton

    let makeTodo (t : Todo) =
      TodoItem.make <|
        TodoItem.Props(
          todo = t,
          user = props.user,
          maybeStore = (props :> ReactRedux.Property<TodoListProps>).store,
          maybeChildren = None)

    let {List = a} =  props.state in
      Tag.div [] <| List.concat [
        [props.user.Id.toString() |> sprintf "Logged in as User with id %O" |> unbox ]
        buttons
        (List.map makeTodo a)
      ]



let stateMapper =
  Func<TodosState, TodoListProps option, obj>(
    fun a b ->
      let res = createObj [
                  "state" ==> box a;
                  "user" ==> (box <| User.make a.UserId)]
      res)


let provider = ReactRedux.buildProvider TodoList stateMapper

