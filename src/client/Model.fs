namespace Model

open System
open Aether


// Simple User ADT
// Nothing special
[<StructuredFormatDisplay("{intValue}")>]
type UserId =
  | UID of int // Good idea? Implications for e.g. Data access?
  with
  member this.intValue =
    let (UID i) = this
    i

  member this.toString() =
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
type Todo = {
  Id : TodoId
  Text : String
  Done : bool
  Owner : UserId
  Protected : bool
} with
  static member text_ : Lens<Todo, String> = (fun a -> a.Text), (fun v a -> {a with Text = v})
  static member done_ : Lens<Todo, bool> = (fun a -> a.Done), (fun v a -> {a with Done = v})


[<CompilationRepresentation (CompilationRepresentationFlags.ModuleSuffix)>]
module Todo =
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
  List : Todo list;
  UserId : UserId;
} with
  static member userId_ : Lens<TodosState, UserId>=
    (fun a -> a.UserId), (fun v a -> {a with UserId = v})

  static member list_ : Lens<TodosState, Todo list> =
    (fun a -> a.List), (fun v a -> {a with List = v})

  static member todoWithId_(id : TodoId) : Lens<TodosState, Todo> =
    let get = fun (list : Todo list) ->
      List.find (fun t -> t.Id = id) list
    let set = fun value list ->
      let rec set' l =
        match l with
          | [] -> []
          | a::b when a.Id = id -> value :: (set' b)
          | a::b -> a :: (set' b)
      set' list

    let inlist : Lens<Todo list, Todo> = get, set
    Compose.lens TodosState.list_ inlist
// Permissions Are TYPES. We want static enforcement of verification. They can
// be whatever they need to be to represent
module Perm =
  //
  type ToEdit = CanEdit of Todo
  type ToComplete = CanComplete of Todo

  // Check for permission gets an option
  let requestEdit user todo =
    None

  let requestComplete (user : User.T) (todo : Todo) =
    if user.Id = todo.Owner then
      Some <| CanComplete todo
    else
      None


// Application actions to be executed against a state. Actions which need
// authorization have a permission type as their first constituent. This forces
// us to check for permission at the time when we construct them.
type TodoAction =
  | Create of Todo
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

      | ChangeText (Perm.CanEdit todo, s) ->
        let updated = { todo with Text = s }
        { state with List = replaceItem todo updated state.List }

      | MarkComplete (Perm.CanComplete todo) ->
        let l = Compose.lens (TodosState.todoWithId_ todo.Id) Todo.done_
        Optic.set l true state

      | ToggleComplete (Perm.CanComplete todo) ->
        let l = Compose.lens (TodosState.todoWithId_ todo.Id) Todo.done_
        printfn "Hello!"
        Optic.set l (not <| Optic.get l state) state

      | CheckAll todos ->
        List.fold perform state (List.map MarkComplete todos)

      | ChangeUser uid ->
        { state with UserId = uid }
