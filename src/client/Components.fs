module Components

open System
open Fable.Core
open Fable.Import

module React = Fable.Import.React
module ReactRedux = Fable.Import.ReactRedux
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

open Model


type Action<'a> = {
  ``type`` : String;
  payload : 'a
  }


let nothing () = {``type`` = "app"; payload = Nothing }
let addTodo () = {``type`` = "app"; payload = Create (Todo.make (UID 1) "Hello!") }

let toString = function
  | Nothing -> "Nothing"
  | Create _ -> "Create"

let toObj { ``type`` = actionType; payload = p } =
  [ ("type", actionType :> obj); ("payload", p :> obj) ]
  |> List.toSeq
  |> Fable.Core.Operators.createObj


let reducer = Func<TodosState, Action<TodoAction>, TodosState>(fun state act ->
  printfn "reducer %A %A" state act
  match act.``type`` with
  | "app" ->
    Actions.perform state (act.payload)
  | _ -> state)


let createActionButton (actionLabel, dispatcher) =
  Tag.button [ Attr.Key actionLabel
               Attr.OnClick dispatcher
             ]
    [ Tag.h1 [] [unbox actionLabel]]

let renderTodo (todo : Todo.T) =
  Tag.li [] [
    unbox todo.Text
  ]


let store = Fable.Import.Redux.Globals.createStore(reducer, {List = []})

type TodosProps(maybeStore : Redux.Store option, maybeChildren : React.ReactElement<TodosProps> option) =
  member val state = {List = []}
  interface ReactRedux.Property<TodosProps> with
    member val store = maybeStore
    member val children = maybeChildren

type TodoList(props) as this =
  inherit React.Component<TodosProps, TodosState>(props)

  do this.state <- {List = []}

  let labelFrom actionCreator =  actionCreator().``payload`` |> toString

  let dispatcherFrom actionCreator (_:React.MouseEvent) =
    let actionDispatcher action =
      match (props :> ReactRedux.Property<TodosProps>).store with
        | None -> failwith "Cannot create action dispatcher without a Redux store"
        | Some store -> action
        |> toObj
        |> store.dispatch
        |> ignore
    actionCreator () |> actionDispatcher


  let buttons =
    [ nothing; addTodo ]
    |> List.map (fun actionCreator -> (labelFrom actionCreator, dispatcherFrom actionCreator))
      |> List.map createActionButton

  member self.render() =
    let {List = a} =  props.state in
      Tag.div [] [
        // Tag.h1 [] [Todos];
        Tag.ul []
          (List.map renderTodo a)
        Tag.div [] buttons
      ]


TodoList?props <- createObj [
  "state" ==> React.PropTypes.object
]

[<Emit("debugger")>]
let debugger() =
  failwith "js only"

let stateMapper' : ReactRedux.MapStateToProps =
  Func<obj, obj option, obj>(
    fun a b ->
      let a' = unbox a : TodosState
      createObj [
        "state" ==> a
      ])

let com : React.ComponentClass<obj> =
    ReactRedux.Globals.connect(stateMapper').Invoke(TodoList)

type Provider = ReactRedux.Provider<TodosState, TodosProps>

let provider (props : TodosProps) =
  Tag.com<Provider,ReactRedux.Property<TodosProps>,TodosState> props [
    React.createElement(Case1 com, props |> Serialize.toPlainJsObj)
    ]
