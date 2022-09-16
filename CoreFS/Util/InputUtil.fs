namespace CoreFS.Util

open CoreFS.DU
open CoreFS.Constants
open CoreFS.Domain

module InputUtil =

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
