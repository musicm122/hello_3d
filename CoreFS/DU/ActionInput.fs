namespace CoreFS.DU

open CoreFS.Constants
open CoreFS.Domain
open Microsoft.FSharp.Reflection

type ActionInput =
    | Jump of JumpingMovingData
    | Pause of bool
    | Idle
    member this.asString() =
        match this with
        | Jump (_) -> ActionConstants.JumpAction
        | _ -> Idle.ToString()

    static member toActionInput(name: string) : ActionInput =
        match name with
        | "jump" ->
            let defaults = JumpingMovingData.Default()
            ActionInput.Jump defaults
        | "pause" -> ActionInput.Pause true
        | _ -> ActionInput.Idle

    static member ActionInputs =
        FSharpType.GetUnionCases typeof<ActionInput>
        |> Array.map (fun case -> FSharpValue.MakeUnion(case, [||]) :?> ActionInput)
