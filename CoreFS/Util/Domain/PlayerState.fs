namespace CoreFS.Util.Domain

open CoreFS.Util.Constants
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
        { PlaybackSpeed = DefaultsValues.PlaybackAnimationSpeed
          Direction = Vector3.Zero }

type GroundedMovingData =
    { MoveSpeed: Speed
      Bounce: BounceImpulse
      PlaybackSpeed: AnimationPlaybackSpeed
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultsValues.PlaybackAnimationSpeed
          Direction = Vector3.Zero
          Bounce = DefaultsValues.BounceImpulse
          MoveSpeed = DefaultsValues.Speed }

type JumpingMovingData =
    { MoveSpeed: Speed
      Bounce: BounceImpulse
      JumpImpulse: JumpImpulse
      FallAcceleration: FallAcceleration
      PlaybackSpeed: AnimationPlaybackSpeed
      Direction: Direction }
    static member Default() =
        { PlaybackSpeed = DefaultsValues.PlaybackAnimationSpeed
          Direction = Vector3.Zero
          Bounce = DefaultsValues.BounceImpulse
          MoveSpeed = DefaultsValues.Speed
          FallAcceleration = DefaultsValues.FallAcceleration
          JumpImpulse = DefaultsValues.JumpImpulse }

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
