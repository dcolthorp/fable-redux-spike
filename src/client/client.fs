module Client

open System
open System.Diagnostics
open Fable.Core

open Fable.Import
open Fable.Import.Redux
open Fable.Import.ReactRedux
open Fable.Import.Browser

module R = Fable.Helpers.React
open R.Props

open Model
open Components
open Properties

type ActionType = Increment | Decrement
type Action = { ``type`` : TodoAction }

let toString = function
  | Increment -> "Increment"
  | Decrement -> "Decrement"

let toObj { ``type`` = actionType } =
  [ ("type", actionType :> obj) ]
  |> List.toSeq
    |> Fable.Core.Operators.createObj

let perform state act =
  printfn "%A" act
  match act with
  | {``type`` = Something} -> state

let reducer = Func<TodosState, Action, TodosState>(perform)
let store = Globals.createStore(reducer, {List = []})
let initialProps = TodosProps(Some store, None)


// let CommentList(props: CBState) =
//   R.div [ClassName "commentList"] []


// type CommentBox(props) =
//     inherit R.Component<CBProps, CBState>(props, { data = [||] })

//     // member x.loadCommentsFromServer () =
//     //     ajax (Get x.props.url)
//     //         (fun data -> x.setState { data = data })
//     //         (fun status ->
//     //             Browser.console.error(x.props.url, status))

//     // member x.handleCommentSubmit (comment: Comment) =
//     //     let comments = x.state.data
//     //     // Optimistically set an id on the new comment. It will be replaced by an
//     //     // id generated by the server. In a production application you would likely
//     //     // not use Date.now() for this and would have a more robust system in place.
//     //     x.setState { data = [] }
//     //     ajax (Post (x.props.url, comment))
//     //         (fun data -> x.setState { data = data })
//     //         (fun status ->
//     //             x.setState { data = comments }
//     //             Browser.console.error(x.props.url, status))

//     member x.componentDidMount () =
//       null
//         // x.loadCommentsFromServer ()
//         // Browser.window.setInterval(
//         //      x.loadCommentsFromServer, x.props.pollInterval)

//     member x.render () =
//         R.div [ClassName "commentBox"] [
//             R.h1 [] [unbox "Comnts"]
//             R.fn CommentList {data = x.state.data} []
//             // Use ReactHelper.com to build a React Component from a type
//             // R.com<CommentForm,_,_> {onCommentSubmit = x.handleCommentSubmit} []
//         ]

ReactDom.render(
    Components.provider initialProps,
    Browser.document.getElementById "content")
|> ignore
