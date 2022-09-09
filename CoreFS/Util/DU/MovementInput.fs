namespace CoreFS.Util.DU

open CoreFS.Util.Constants
open Godot
open Microsoft.FSharp.Reflection

[<Struct>]
type MovementInput =
    | Right
    | Left
    | Forward
    | Back
    | Unsupported

    member this.AsString() =
        match this with
        | Left -> MovementConstants.MoveLeft
        | Right -> MovementConstants.MoveRight
        | Forward -> MovementConstants.MoveForward
        | Back -> MovementConstants.MoveBack
        | _ -> MovementConstants.UnsupportedInput

    member this.AsVector() =
        match this with
        | Right -> Vector3(1.0f, 0.0f, 0.0f)
        | Left -> Vector3(-1.0f, 0.0f, 0.0f)
        | Forward -> Vector3(0.0f, 0.0f, -1.0f)
        | Back -> Vector3(0.0f, 0.0f, 1.0f)
        | _ -> Vector3(0.0f, 0.0f, 0.0f)

    static member MovementInputs =
        FSharpType.GetUnionCases typeof<MovementInput>
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> MovementInput)

type InputType =
    | Movement of MovementInput
    | Action of ActionInput
    