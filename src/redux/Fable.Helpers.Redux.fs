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

  let action a =
    {
      ``type`` = APP_KEY;
      payload = a;
    }

  let payload a = a.payload

  let toObj ({ ``type`` = actionType; payload = p } : Action<'A>) =
    [ ("type", actionType :> obj); ("payload", p :> obj) ]
    |> List.toSeq
    |> Fable.Core.Operators.createObj
