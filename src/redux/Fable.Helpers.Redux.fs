namespace Fable.Helpers
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS

// Type definitions for redux v3.3.1, generated from Typescript
module Redux =

  type Action<'A> = {
    ``type`` : string;
    payload: 'A;
  }

  let [<Literal>] APP_KEY = "app"

  type Globals() =
    member __.action a =
      {
        ``type`` = APP_KEY;
        payload = a;
      }

    member __.payload a = a.payload

    member __.reducer f = Func<'S, Action<'A>, 'S>(fun state act ->
        match act.``type`` with
        | APP_KEY -> f state (act.payload)
        | _ -> state)


    member __.toObj ({ ``type`` = actionType; payload = p } : Action<'A>) =
      [ ("type", actionType :> obj); ("payload", p :> obj) ]
      |> List.toSeq
      |> Fable.Core.JsInterop.createObj

[<AutoOpen>]
module Redux_extensions =
  let Redux = Redux.Globals()
