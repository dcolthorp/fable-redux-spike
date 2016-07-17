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


type TodoList(props, ?state) =
  inherit Tag.Component<TodosProps, TodosState>(props, ?state = state)

  member self.render() =
    Tag.div [] [
      Tag.h1 [] [unbox "Hello"]
    ]



let stateMapper<'a> = Func<'a, obj>(fun a -> Tag.toPlainJsObj a)


[<Emit("$0")>]
let coerce<'a> f : 'a = failwith "js"

let counter props = Tag.com<TodoList,TodosProps,TodosState> props []

// [<Emit("React.createElement($0,$1)")>]
//  let instantiate<'props> (a:React.ReactElement<obj>) (p:'props) =
//    failwith "js only"

[<Emit("debugger")>]
  let debugger() =
   failwith "js only"

// [<Emit("$0")>]
//   let tocom<'a> a : U2<React.ComponentClass<'a>,React.StatelessComponent<'a>> = failwith "js only"
[<Emit("$0")>]
  let tocom<'a> a : 'a -> React.ReactElement<obj> list -> React.ReactElement<obj> = failwith "js only"

[<Emit("$0")>]
  let wipe a : React.ReactElement<obj> = failwith "js only"

let counterContainer (props: TodosProps) =
  let inner = counter props
  let connect = ReactRedux.Globals.connect(coerce<ReactRedux.MapStateToProps> stateMapper).Invoke(inner)

  let comp = tocom<obj> connect
  // React.createElement(comp, (Tag.toPlainJsObj props), [| |])
  // inner
  comp

type Provider = ReactRedux.Provider<TodosState, TodosProps>
let provider (props : TodosProps) =
  let el = counterContainer props
  debugger()
  Tag.com<Provider,ReactRedux.Property<TodosProps>,TodosState> props [el props []]
