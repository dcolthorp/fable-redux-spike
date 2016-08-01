namespace Model

open System

// Simple User ADT
// Nothing special
type UserId =
  | UID of int // Good idea? Implications for e.g. Data access?
  override this.ToString() =
    match this with
      | (UID i) -> i.ToString()


module User =
  type T = {
    Id : UserId
  }

  let make id = {Id = id}
  let idValue {Id = UID i} = i


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
type TodosState = {
  List : Todo.T list;
  UserId : UserId;
}

// Permissions Are TYPES. We want static enforcement of verification. They can
// be whatever they need to be to represent
module Perm =
  //
  type ToEdit = CanEdit_ of Todo.T
  type ToComplete = CanComplete_ of Todo.T

  // Check for permission gets an option
  let requestEdit user todo =
    None

  let requestComplete (user : User.T) (todo : Todo.T) =
    if user.Id = todo.Owner then
      Some <| CanComplete_ todo
    else
      None


// Application actions to be executed against a state. Actions which need
// authorization have a permission type as their first constituent. This forces
// us to check for permission at the time when we construct them.
type TodoAction =
  | Create of Todo.T
  | ChangeText of Perm.ToEdit * string
  | MarkComplete of Perm.ToComplete
  | ToggleComplete of Perm.ToComplete
  | CheckAll of Perm.ToComplete list
  | ChangeUser of UserId


module Actions =
  let replaceItem current updated list =
    let update =
      function
        | t when t = current -> updated
        | a -> a
    List.map update list

  let rec perform state act =
    printfn "Performing action %A" act
    match act with
      | Create (todo) ->
        printfn "Creating todo"
        { state with List = todo :: state.List}

      | ChangeText (Perm.CanEdit_ todo, s) ->
        let updated = { todo with Text = s }
        { state with List = replaceItem todo updated state.List }

      | MarkComplete (Perm.CanComplete_ todo) ->
        { state with List = replaceItem todo (Todo.setComplete true todo) state.List }

      | ToggleComplete (Perm.CanComplete_ todo) ->
        { state with List = replaceItem todo (Todo.toggleComplete todo) state.List }

      | CheckAll todos ->
        List.fold perform state (List.map MarkComplete todos)

      | ChangeUser uid ->
        { state with UserId = uid }
