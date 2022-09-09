namespace CoreFS.Util.DU

open CoreFS.Util.Constants
open CoreFS.Util.Domain
open Godot
open Microsoft.FSharp.Reflection

type ActionInput =
    | Jump of JumpingMovingData
    | Pause of bool
    | Unsupported
    member this.AsString() =
        match this with
        | Jump (_) -> ActionConstants.JumpAction
        | _ -> MovementConstants.UnsupportedInput

    static member ActionInputs =
        FSharpType.GetUnionCases typeof<ActionInput>
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> ActionInput)
