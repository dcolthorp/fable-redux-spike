namespace Fable.Helpers
open System
open System.Text.RegularExpressions
open Fable.Core
open Fable.Import.JS

// Type definitions for redux v3.3.1, generated from Typescript
module Redux =

  let [<Literal>] APP_KEY = "app"

  type Action<'A> = {
    ``type`` : string;
    payload: 'A;
  }

  let inline action a =
    {
      ``type`` = APP_KEY;
      payload = a;
    }

  let inline payload a = a.payload

  let reducer f = Func<'S, Action<'A>, 'S>(fun state act ->
      match act.``type`` with
      | APP_KEY -> f state (act.payload)
      | _ -> state)


  let toObj ({ ``type`` = actionType; payload = p } : Action<'A>) =
    [ ("type", actionType :> obj); ("payload", p :> obj) ]
    |> List.toSeq
    |> Fable.Core.Operators.createObj
