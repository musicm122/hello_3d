namespace CoreFS.Entities

open CoreFS.Domain
open Godot

type MobFS() =
    inherit KinematicBody()

    let squashEvent = Event<unit>()
    member this.OnMobSquashed = squashEvent.Publish

    member val state = MobState.Aggro with get, set

    /// Minimum speed of the mob in meters per second.
    [<Export>]
    member val minSpeed = 10.0f with get, set

    /// Maximum speed of the mob in meters per second.
    [<Export>]
    member val maxSpeed = 18f with get, set

    [<Export>]
    member val velocity = Vector3.Zero with get, set

    member this.AnimationPlayer =
        this.GetNode<AnimationPlayer>("AnimationPlayer")

    member this.VisibilityNotifier =
        this.GetNode<VisibilityNotifier>("VisibilityNotifier")

    member this.squash() =
        squashEvent.Trigger()
        //this.EmitSignal(nameof (SquashedSignal))
        this.state <- MobState.Squashed
        ()

    member this.onVisibilityNotifier_ScreenExited() = this.state <- MobState.OffScreen

    member this.Initialize (startPos: Vector3) (playerPos: Vector3) = this.init startPos playerPos

    override this._Ready() : unit =
        this.VisibilityNotifier.Connect("screen_exited", this, nameof this.onVisibilityNotifier_ScreenExited)
        |> ignore

    override this._PhysicsProcess(delta: float32) =
        match this.state with
        | Aggro -> this.MoveAndSlide this.velocity |> ignore
        | Squashed -> this.QueueFree()
        | OffScreen -> this.QueueFree()


    member this.init startPos playerPos : unit =
        this.LookAtFromPosition(startPos, playerPos, Vector3.Up)

        let calcNewRotation () =
            GD.Randomize()
            let min = (float) (-Mathf.Pi / 4.0f)
            let max = (float) (Mathf.Pi / 4.0f)
            (float32) (GD.RandRange(min, max))

        let calcRandomSpeed () =
            GD.Randomize()
            // We calculate a forward velocity first, which represents the speed.
            let min = float this.minSpeed
            let max = float this.maxSpeed
            float32 (GD.RandRange(min, max))

        this.RotateY(calcNewRotation ())
        let randomSpeed = calcRandomSpeed ()
        this.velocity <- Vector3.Forward * randomSpeed
        this.velocity <- this.velocity.Rotated(Vector3.Up, this.Rotation.y)

        // We then rotate the vector based on the mob's Y rotation to move in the direction it's looking.
        this.AnimationPlayer.PlaybackSpeed <- (randomSpeed / this.minSpeed)
        ()
