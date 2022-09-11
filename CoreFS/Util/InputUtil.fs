namespace CoreFS.Util

open CoreFS.Util.Constants
open CoreFS.Util.DU
open CoreFS.Util.Domain

module InputUtil =
    let toActionInput (name: string) : ActionInput =
        match name with
        | "jump" ->
            let defaults = JumpingMovingData.Default()
            ActionInput.Jump defaults
        | "pause" -> ActionInput.Pause true
        | _ -> ActionInput.Idle

    let toMovementInput (name: string) =
        match name with
        | "move_left" -> MovementInput.Left
        | "move_right" -> MovementInput.Right
        | "move_forward" -> MovementInput.Forward
        | "move_back" -> MovementInput.Back
        | _ -> MovementInput.Unsupported

    let toInput (name: string) : InputType =
        match name with
        | "jump" ->
            let defaults = JumpingMovingData.Default()
            Action(ActionInput.Jump defaults)
        | "pause" -> Action(ActionInput.Pause true)
        | "move_left" -> Movement(MovementInput.Left)
        | "move_right" -> Movement(MovementInput.Right)
        | "move_forward" -> Movement(MovementInput.Forward)
        | "move_back" -> Movement(MovementInput.Back)
        | _ -> Movement(MovementInput.Unsupported)

    let detectActiveInput inputCheck =
        let applyInputCheck checkTestFun (input: InputType) : PlayerState = checkTestFun input

        let testAnyInput =
            applyInputCheck inputCheck

        InputConstants.availableInputs
        |> Array.map (toInput)
        |> Array.map (testAnyInput)


    let detectAction actionCheck =
        let applyInputCheck checkTestFun (input: ActionInput) : PlayerState = checkTestFun input

        let testActionInput =
            applyInputCheck actionCheck

        ActionConstants.availableActions
        |> Array.map (toActionInput)
        |> Array.map (testActionInput)

    let detectMovement inputCheck =
        let applyInputCheck checkTestFun (input: MovementInput) : PlayerState = checkTestFun input

        let testMoveInput =
            applyInputCheck inputCheck

        MovementConstants.availableMovementInputs
        |> Array.map (toMovementInput)
        |> Array.map (testMoveInput)
