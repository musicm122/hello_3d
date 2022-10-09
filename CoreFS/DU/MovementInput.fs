namespace CoreFS.DU

open CoreFS.Constants
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

    static member toMovementInput(name: string) =
        match name with
        | "move_left" -> MovementInput.Left
        | "move_right" -> MovementInput.Right
        | "move_forward" -> MovementInput.Forward
        | "move_back" -> MovementInput.Back
        | _ -> MovementInput.Unsupported

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
    static member toInput(name: string) : InputType =
        match name with
        | "jump" -> Action(ActionInput.Jump)
        | "pause" -> Action(ActionInput.Pause)
        | "move_left" -> Movement(MovementInput.Left)
        | "move_right" -> Movement(MovementInput.Right)
        | "move_forward" -> Movement(MovementInput.Forward)
        | "move_back" -> Movement(MovementInput.Back)
        | _ -> Movement(MovementInput.Unsupported)
