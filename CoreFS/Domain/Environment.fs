namespace CoreFS.Domain

type PositionSpace =
    | Ground
    | Air

type IsShortPress = string -> bool
type IsLongPress = string -> bool


type EnvironmentState =
    { playerSpace: PositionSpace
      isShortPressedPredicate: IsShortPress
      isLongPressedPredicate: IsLongPress }
    static member Default() =
        { playerSpace = Ground
          isLongPressedPredicate = fun _ -> failwith "not implemented"
          isShortPressedPredicate = fun _ -> failwith "not implemented" }
