namespace CoreFS.Constants

module ActivePatterns =
    let (|EmptySeq|_|) a = if Seq.isEmpty a then Some() else None


module Events =

    type SystemInitialize() =
        inherit Event<unit>()

    type ProcessFrameEvent() =
        inherit Event<float32>()

    type ProcessPhysicsEvent() =
        inherit Event<float32>()


module DefaultPlayerValues =
    [<Literal>]
    let IdlePlaybackAnimationSpeed = 1

    [<Literal>]
    let MovementPlaybackAnimationSpeed = 4

    [<Literal>]
    let Speed = 14.0f

    [<Literal>]
    let JumpImpulse = 20.0f

    [<Literal>]
    let BounceImpulse = 16.0f

    [<Literal>]
    let FallAcceleration = 75.0f

module DefaultMobValues =
    let MinSpeed = 10.0f
    let MaxSpeed = 18.0f



module ActionConstants =
    [<Literal>]
    let JumpAction = "jump"

    [<Literal>]
    let PauseAction = "pause"

    let availableActions =
        [| JumpAction; PauseAction |]

module MovementConstants =
    [<Literal>]
    let MoveLeft = "move_left"

    [<Literal>]
    let MoveRight = "move_right"

    [<Literal>]
    let MoveForward = "move_forward"

    [<Literal>]
    let MoveBack = "move_back"

    [<Literal>]
    let UnsupportedInput = "Unsupported Input"

    let availableMovementInputs =
        [| MoveLeft
           MoveRight
           MoveForward
           MoveBack |]

module InputConstants =
    let availableInputs =
        Array.concat [ MovementConstants.availableMovementInputs
                       ActionConstants.availableActions ]
