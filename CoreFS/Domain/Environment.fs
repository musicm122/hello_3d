namespace CoreFS.Domain

type GameState =
    | StartMenu
    | Playing
    | Paused
    | YouDied
    | Quit

type PositionSpace =
    | Ground
    | Air

type IsShortPress = string -> bool
type IsLongPress = string -> bool


type EnvironmentState =
    { gameState: GameState
      playerSpace: PositionSpace
      isShortPressedPredicate: IsShortPress
      isLongPressedPredicate: IsLongPress }
    static member Default() =
        { gameState = GameState.Playing
          playerSpace = Ground
          isLongPressedPredicate = fun _ -> failwith "not implemented"
          isShortPressedPredicate = fun _ -> failwith "not implemented" }
