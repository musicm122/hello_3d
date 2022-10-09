namespace CoreFS.Domain

open Godot

[<Signal>]
type HitSignal = delegate of unit -> unit

[<Signal>]
type SquashedSignal = delegate of unit -> unit

type AnimationPlaybackSpeed = int
type Speed = float32
type Damage = int
type BounceImpulse = float32
type JumpImpulse = float32
type FallAcceleration = float32
type Direction = Vector3
type HP = int

type TakeDamageData = { DamageAmount: Damage; CurrentHP: HP }

type HitEvent() =
    let hitEvent = Event<_>()

    [<CLIEvent>]
    member this.HitEvent = hitEvent.Publish
