module Components

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


let nothing () = Nothing
let addTodo (user : User.T) () = Create (Todo.make user.Id "Hello!")

let toString = function
  | Nothing -> "Nothing"
  | Create _ -> "Create"

type TodosProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodosProps> option) =
  let state' =
    match maybeStore with
      | None -> failwith "Need an initial state"
      | Some store ->
        store.getState() : TodosState

  member val state = state'
  member val user = User.make state'.UserId

  interface ReactRedux.Property<TodosProps> with
    member val store = maybeStore
    member val children = maybeChildren


let createActionButton (actionLabel, dispatcher) =
  Tag.button [ Attr.Key actionLabel
               Attr.OnClick dispatcher
             ]
    [ Tag.h1 [] [unbox actionLabel]]

let renderTodo (props : TodosProps) (todo : Todo.T) =
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

type TodoList(props) as this =
  inherit React.Component<TodosProps, TodosState>(props)

  let labelFrom actionCreator =  actionCreator() |> toString

  let dispatcherFrom actionCreator (_:React.MouseEvent) =
    let act = actionCreator()
    ReactRedux.dispatch props act


  let buttons =
    [ nothing; addTodo props.user ]
    |> List.map (fun actionCreator -> (labelFrom actionCreator, dispatcherFrom actionCreator))
    |> List.map createActionButton

  member self.render() =
    let {List = a} =  props.state in
      Tag.div [] [
        Tag.ul []
          (List.map (renderTodo props) a)
        Tag.div [] buttons
        Tag.div [] [unbox <| sprintf "Logged in as User %O" props.user.Id]
      ]


TodoList?props <- createObj [
  "state" ==> React.PropTypes.object
  "user" ==> React.PropTypes.object
]

let stateMapper' =
  Func<TodosState, TodosProps option, obj>(
    fun a b ->
      let res = createObj [
          "state" ==> box a;
          "user" ==> box b.Value.user]
      res)


let com : React.ComponentClass<obj> =
    ReactRedux.Globals.connect(stateMapper').Invoke(TodoList)

type Provider = ReactRedux.Provider<TodosState, TodosProps>

let provider (props : TodosProps) =
  Tag.com<Provider,ReactRedux.Property<TodosProps>,TodosState> props [
    React.createElement(Case1 com, props |> Serialize.toPlainJsObj)
    ]
