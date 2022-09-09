namespace CoreFS.Entities

open CoreFS.Util
open Godot

[<Signal>]
type SquashedSignal = delegate of unit -> unit

type MobFS() =
    inherit KinematicBody()

    /// Minimum speed of the mob in meters per second.
    [<Export>]
    member val minSpeed = 10.0f with get, set

    /// Maximum speed of the mob in meters per second.
    [<Export>]
    member val maxSpeed = 18.0f with get, set

    [<Export>]
    member val velocity = Vector3.Zero with get, set

    member this.AnimationPlayer =
        this.GetNode<Spatial>("AnimationPlayer")

    member this.squash() =
        this.EmitSignal(nameof (SquashedSignal))
        this.QueueFree()

    member this.onVisibilityNotifier_ScreenExited() = this.QueueFree()

    member this.init (startPos: Vector3) (playerPos: Vector3) =
        this.LookAtFromPosition(startPos, playerPos, Vector3.Up)
        let min = -Mathf.Pi / 4.0f
        let max = Mathf.Pi / 4.0f
        this.RotateY(RandomUtil.float32InRange min max)

        let randomSpeed =
            RandomUtil.float32InRange this.minSpeed this.maxSpeed

        // We calculate a forward velocity first, which represents the speed.
        this.velocity <- Vector3.Forward * randomSpeed

        // We then rotate the vector based on the mob's Y rotation to move in the direction it's looking.
        this.velocity <- this.velocity.Rotated(Vector3.Up, this.Rotation.y)
        this.AnimationPlayer.Set("playback_speed", randomSpeed / min)

    override this._PhysicsProcess(_: float32) =
        this.MoveAndSlide(this.velocity) |> ignore
