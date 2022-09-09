namespace CoreFS.Util

open System
open CoreFS.Util.Constants
open CoreFS.Util.DU
open CoreFS.Util.Domain
open Godot
open Microsoft.FSharp.Reflection

module InputUtil =
    let toActionInput (name: string) : ActionInput =
        match name with
        | "jump" ->
            let defaults = JumpingMovingData.Default()
            ActionInput.Jump defaults
        | "pause" -> ActionInput.Pause true
        | _ -> ActionInput.Unsupported


    let toMovementInput (name: string) =
        match name with
        | "move_left" -> MovementInput.Left
        | "move_right" -> MovementInput.Right
        | "move_forward" -> MovementInput.Forward
        | "move_back" -> MovementInput.Back
        | _ -> MovementInput.Unsupported


    let detectAction actionCheck =
        let applyInputCheck checkTestFun (input: ActionInput) : PlayerState = checkTestFun input

        let testActionInput =
            applyInputCheck actionCheck

        ActionConstants.availableActions
        |> Array.map (toActionInput)
        |> Array.map (testActionInput)

    let detectMovement inputCheck =
        let applyInputCheck checkTestFun (input: MovementInput) : PlayerState   = checkTestFun input

        let testMoveInput =
            applyInputCheck inputCheck

        MovementConstants.availableMovementInputs
        |> Array.map (toMovementInput)
        |> Array.map (testMoveInput)
