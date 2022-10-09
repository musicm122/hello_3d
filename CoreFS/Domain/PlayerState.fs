namespace CoreFS.Domain

open CoreFS.Constants
open Godot

type IdleData =
    { PlaybackSpeed: AnimationPlaybackSpeed
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultPlayerValues.IdlePlaybackAnimationSpeed
          Direction = Vector3.Zero }

type MovingData =
    { MoveSpeed: Speed
      Bounce: BounceImpulse
      PlaybackSpeed: AnimationPlaybackSpeed
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultPlayerValues.IdlePlaybackAnimationSpeed
          Direction = Vector3.Zero
          Bounce = DefaultPlayerValues.BounceImpulse
          MoveSpeed = DefaultPlayerValues.Speed }

    static member (+)(x, y) =
        let newDir = x.Direction + y.Direction
        { y with Direction = newDir }


type JumpingMovingData =
    { MoveSpeed: Speed
      Bounce: BounceImpulse
      JumpImpulse: JumpImpulse
      FallAcceleration: FallAcceleration
      PlaybackSpeed: AnimationPlaybackSpeed
      Position: PositionSpace
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultPlayerValues.IdlePlaybackAnimationSpeed
          Direction = Vector3.Zero
          Bounce = DefaultPlayerValues.BounceImpulse
          MoveSpeed = DefaultPlayerValues.Speed
          FallAcceleration = DefaultPlayerValues.FallAcceleration
          Position = Ground
          JumpImpulse = DefaultPlayerValues.JumpImpulse }



type PlayerHealthState =
    | Unharmed
    | TakeHit

type PlayerState =
    | Idle
    | Jumping
    | Moving of MovingData
    | TakeHit
    | Paused
    | Unpaused
    | Default
    | Death

type PlayerModel =
    { onMobCollision: Option<(Spatial -> unit)>
      getCollidersInGroup: Option<string -> Option<seq<KinematicCollision>>>
      speed: float32
      velocity: Vector3
      jumpImpulse: float32
      bounceImpulse: float32
      fallAcceleration: float32 }
    static member Default =
        { onMobCollision = None
          getCollidersInGroup = None
          speed = DefaultPlayerValues.Speed
          velocity = Vector3.Zero
          jumpImpulse = DefaultPlayerValues.JumpImpulse
          bounceImpulse = DefaultPlayerValues.BounceImpulse
          fallAcceleration = DefaultPlayerValues.FallAcceleration }

type PlayerDetails =
    { state: PlayerState
      health: PlayerHealthState
      model: PlayerModel }
