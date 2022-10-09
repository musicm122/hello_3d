namespace CoreFS.DU

open CoreFS.Constants
open Microsoft.FSharp.Reflection

type ActionInput =
    | Jump
    | Pause
    | Idle
    member this.asString() =
        match this with
        | Jump (_) -> ActionConstants.JumpAction
        | _ -> Idle.ToString()

    static member toActionInput(name: string) : ActionInput =
        match name with
        | "jump" -> ActionInput.Jump
        | "pause" -> ActionInput.Pause
        | _ -> ActionInput.Idle

    static member ActionInputs =
        let castCaseToActionInput case =
            let unionCase =
                FSharpValue.MakeUnion(case, [||])

            let actionInput = unionCase :?> ActionInput
            actionInput

        let cases =
            FSharpType.GetUnionCases typeof<ActionInput>

        let retval =
            cases |> Array.map (castCaseToActionInput)

        retval
