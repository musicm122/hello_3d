namespace CoreFS.Util

open CoreFS.Constants
open CoreFS.DU
open CoreFS.Domain

module Input =

    // takes user input state and returns player state
    let pollForInput (env: EnvironmentState) =
        let testForAction (action: ActionInput) : PlayerState =
            let name = action.asString ()

            match InputConstants.availableInputs
                  |> Array.contains name
                  //&& env.isLongPressedPredicate name
                  && env.isShortPressedPredicate name
                with
            | true ->
                match action with
                | Jump jumpData -> PlayerState.Jumping({ jumpData with Position = env.playerSpace })
                | Pause pauseData ->
                    if pauseData = true then
                        PlayerState.Paused
                    else
                        PlayerState.Unpaused
                | _ -> PlayerState.Idle(IdleData.Default())
            | _ -> PlayerState.Idle(IdleData.Default())

        let testForMovement (movement: MovementInput) : PlayerState =
            let vec = movement.AsVector().Normalized()
            let name = movement.AsString()

            match InputConstants.availableInputs
                  |> Array.contains name
                  && env.isLongPressedPredicate name
                with
            | true -> PlayerState.Moving { GroundedMovingData.Default() with GroundedMovingData.Direction = vec }
            | _ -> PlayerState.Idle(IdleData.Default())

        let testForInput inputType =
            match inputType with
            | Action actionInput -> testForAction actionInput
            | Movement movementInput -> testForMovement movementInput

        InputUtil.detectActiveInput testForInput
