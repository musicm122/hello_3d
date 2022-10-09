namespace CoreFS.Util

open CoreFS.Constants
open CoreFS.DU
open CoreFS.Domain
open Godot

module InputEvents =
    type SystemInputEvent() =
        inherit Event<InputEvent>()

module Input =
    let getDirectionFromInput () =
        let mutable direction = Vector3.Zero

        if Input.IsActionPressed(MovementConstants.MoveRight) then
            direction <- direction.AddToX(1f)

        if Input.IsActionPressed(MovementConstants.MoveLeft) then
            direction <- direction.AddToX(-1f)

        if Input.IsActionPressed(MovementConstants.MoveBack) then
            direction <- direction.AddToZ(1f)

        if Input.IsActionPressed(MovementConstants.MoveForward) then
            direction <- direction.AddToZ(-1f)

        direction.Normalized()

    let detectActiveInput inputCheck =
        let applyInputCheck checkTestFun (input: InputType) : PlayerState = checkTestFun input

        let testAnyInput =
            applyInputCheck inputCheck

        InputConstants.availableInputs
        |> Array.map InputType.toInput
        |> Array.map (testAnyInput)

    let detectAction actionCheck =
        let applyInputCheck checkTestFun (input: ActionInput) : PlayerState = checkTestFun input

        let testActionInput =
            applyInputCheck actionCheck

        ActionConstants.availableActions
        |> Array.map ActionInput.toActionInput
        |> Array.map (testActionInput)

    let detectMovement inputCheck =
        let applyInputCheck checkTestFun (input: MovementInput) : PlayerState = checkTestFun input

        let testMoveInput =
            applyInputCheck inputCheck

        MovementConstants.availableMovementInputs
        |> Array.map MovementInput.toMovementInput
        |> Array.map (testMoveInput)
