namespace Model

open System

// Simple User ADT
// Nothing special
type UserId = UID of int // Good idea? Implications for e.g. Data access?
type User = {
  Id : UserId
}


// Simple Todo ADT
// Nothing special
type TodoId = int
module Todo =
  type T = {
    Id : TodoId
    Text : String
    Done : bool
    Owner : UserId
    Protected : bool
  }

  let mutable nextId = 0
  let make uid text =
    let id = nextId
    nextId <- id+1
    {
      Id = nextId
      Text = text
      Done = false
      Owner = uid
      Protected = false
    }
  let makeProtected uid text =
    { make uid text with Protected = true}
  let setComplete b todo = { todo with Done = b}
  let toggleComplete todo = setComplete (not todo.Done) todo


// App state
type TodosState = {List : Todo.T list}

// Permissions Are TYPES. We want static enforcement of verification. They can
// be whatever they need to be to represent
module Perm =
  //
  type ToEdit = CanEdit of Todo.T
  type ToComplete = CanComplete of Todo.T

  // Check for permission gets an option
  let requestEdit user todo =
    None

  let requestComplete (user : User) (todo : Todo.T) =
    if user.Id = todo.Owner then
      Some <| CanComplete todo
    else
      None


// Application actions to be executed against a state. Actions which need
// authorization have a permission type as their first constituent. This forces
// us to check for permission at the time when we construct them.
type TodoAction =
  | Create of Todo.T
  | ChangeText of Perm.ToEdit * string
  | MarkComplete of Perm.ToComplete
  | CheckAll of Perm.ToComplete list


module Actions =
  let replaceItem current updated list =
    let update =
      function
        | t when t = current -> updated
        | a -> a
    List.map update list

  let rec perform state act =
    printfn "%A" act
    match act with
    | Create (todo) ->
      { state with List = todo :: state.List}

    | ChangeText (Perm.CanEdit todo, s) ->
      let updated = { todo with Text = s }
      { state with List = replaceItem todo updated state.List }

    | MarkComplete (Perm.CanComplete todo) ->
      { state with List = replaceItem todo (Todo.setComplete true todo) state.List }

    | CheckAll todos ->
      List.fold perform state (List.map MarkComplete todos)


module Tests =
  let ``example`` =
    let user1 = { Id = UID 1 }
    let user2 = { Id = UID 2 }

    let s1 = { List = [] }

    let todo1 = Todo.make user1.Id "Buy some milk"
    let todo2 = Todo.makeProtected user1.Id "For my eyes only"
    let todo3 = Todo.make user2.Id "Do a little dance"

    let createActions =
          [
            Create (todo1)
            Create (todo2)
            Create (todo3)
          ]
    let check1 =
      match Perm.requestComplete user1 todo1 with
        | Some perm -> List.singleton <| MarkComplete perm
        | None -> []
    let check2 =
      match Perm.requestComplete user2 todo2 with
        | Some perm -> List.singleton <| MarkComplete perm
        | None -> []
    let actions = (List.reduce List.append [createActions; check1; check2])
    printfn "%A" actions
    List.fold Actions.perform s1 actions
