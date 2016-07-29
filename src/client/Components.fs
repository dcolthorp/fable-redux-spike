module Components

open System
open Fable.Core
open Fable.Import

module React = Fable.Import.React
module ReactRedux = Fable.Import.ReactRedux
module Tag = Fable.Helpers.React
module Attr = Tag.Props
module ReactRedux = Fable.Import.ReactRedux

// open ActionCreators
open Properties
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


let store = Fable.Import.Redux.Globals.createStore(reducer, {List = []})

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
        |> (fun a -> printfn "dispatcing, current store state is %A" ( store.getState()))
        |> ignore
    actionCreator () |> actionDispatcher

  let createActionButton (actionLabel, dispatcher) =
    Tag.button [ Attr.Key actionLabel
                 Attr.OnClick dispatcher
               ]
      [ Tag.h1 [] [unbox actionLabel]]

  let buttons =
    [ nothing; addTodo ]
    |> List.map (fun actionCreator -> (labelFrom actionCreator, dispatcherFrom actionCreator))
    |> List.map createActionButton

  // let appState = match state with
  //   | Some s -> s
  //   | None -> failwith "huh?"

  member self.render() =
    // let appState : TodosState =
    //   match (props :> ReactRedux.Property<TodosProps>).store with
    //     | Some store -> store.getState() |> unbox
    //     | None -> failwith "Cannot get state without a Redux store"

    // printfn "Rendering with appstate %A" appState
    Tag.div [] [
      Tag.h1 [] [unbox (sprintf "%A" props)];
      Tag.div [] buttons
    ]


TodoList?props <- createObj [
  "State" ==> React.PropTypes.object
]

let stateMapper =
  Func<TodosState, obj>(fun a ->
                printfn "statemapper\n%A" a
                {List = a.List} |> Serialize.toPlainJsObj)

let mapDispatchToProps<'a> =
  Func<'a, obj>(fun a ->
                printfn "mapDispatchToProps<'a>"
                createObj [])

[<Emit("$0")>]
let coerce<'a> f : 'a = failwith "js"

[<Emit("debugger")>]
let debugger() =
  failwith "js only"



//////////////////////////////////////////////////////
// Broken trying to minimize changes

let stateMapper' : ReactRedux.MapStateToProps =
  Func<obj, obj option, obj>(
    fun a b ->
      let a' = unbox a : TodosState
      createObj [
        "state" ==> a
      ])

let com : React.ComponentClass<obj> =
    ReactRedux.Globals.connect(stateMapper').Invoke(TodoList)

//////////////////////////////////////////////////

type Provider = ReactRedux.Provider<TodosState, TodosProps>

let provider (props : TodosProps) =
  Tag.com<Provider,ReactRedux.Property<TodosProps>,TodosState> props [
    React.createElement(Case1 com, props |> Serialize.toPlainJsObj)
    ]
