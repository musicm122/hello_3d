namespace CoreFS.Domain

open CoreFS.Constants
open Godot

[<Signal>]
type HitSignal = delegate of unit -> unit

type AnimationPlaybackSpeed = int
type Speed = float32
type Damage = int
type BounceImpulse = float32
type JumpImpulse = float32
type FallAcceleration = float32
type Direction = Vector3
type HP = int

type TakeDamageData = { DamageAmount: Damage; CurrentHP: HP }

type IdleData =
    { PlaybackSpeed: AnimationPlaybackSpeed
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultPlayerValues.PlaybackAnimationSpeed
          Direction = Vector3.Zero }

type GroundedMovingData =
    { MoveSpeed: Speed
      Bounce: BounceImpulse
      PlaybackSpeed: AnimationPlaybackSpeed
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultPlayerValues.PlaybackAnimationSpeed
          Direction = Vector3.Zero
          Bounce = DefaultPlayerValues.BounceImpulse
          MoveSpeed = DefaultPlayerValues.Speed }

type JumpingMovingData =
    { MoveSpeed: Speed
      Bounce: BounceImpulse
      JumpImpulse: JumpImpulse
      FallAcceleration: FallAcceleration
      PlaybackSpeed: AnimationPlaybackSpeed
      Position: PositionSpace
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultPlayerValues.PlaybackAnimationSpeed
          Direction = Vector3.Zero
          Bounce = DefaultPlayerValues.BounceImpulse
          MoveSpeed = DefaultPlayerValues.Speed
          FallAcceleration = DefaultPlayerValues.FallAcceleration
          Position = Ground
          JumpImpulse = DefaultPlayerValues.JumpImpulse }


type MovementData =
    | Idle of IdleData
    | Jumping of JumpingMovingData
    | Moving of GroundedMovingData
    static member (+)(x, y) =
        let newDir = x.Direction + y.Direction
        { y with Direction = newDir }

type PlayerState =
    | Idle of IdleData
    | Jumping of JumpingMovingData
    | Moving of GroundedMovingData
    | TakingDamage of TakeDamageData
    | Paused
    | Unpaused

type PlayerModel =
    { onFrameTick: Option<(float32 -> unit)>
      onPhysicsTick: Option<(float32 -> unit)>
      onMobCollision: Option<(Spatial -> unit)>
      getCollidersInGroup: Option<string -> Option<seq<KinematicCollision>>>
      die: Option<(unit -> unit)>
      speed: float32
      velocity: Vector3
      jumpImpulse: float32
      bounceImpulse: float32
      fallAcceleration: float32
      pivot: Spatial
      self: KinematicBody
      animationPlayer: AnimationPlayer }
    static member Default (owner: KinematicBody) (pivot: Spatial) (animPlayer: AnimationPlayer) =
        { onFrameTick = None
          onPhysicsTick = None
          onMobCollision = None
          getCollidersInGroup = None
          die = None
          speed = DefaultPlayerValues.Speed
          velocity = Vector3.Zero
          jumpImpulse = DefaultPlayerValues.JumpImpulse
          bounceImpulse = DefaultPlayerValues.BounceImpulse
          fallAcceleration = DefaultPlayerValues.FallAcceleration
          pivot = pivot
          self = owner
          animationPlayer = animPlayer }
