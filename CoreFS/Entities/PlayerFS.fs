namespace CoreFS.Entities

open CoreFS.Domain
open CoreFS.Util
open Godot

type PlayerFS() =
    inherit KinematicBody()

    member this.applyRotation(target: Spatial) =
        Physics.applyRotation target this.velocity this.JumpImpulse

    member this.applyGravity(delta) =
        Physics.applyGravity this.velocity this.FallAcceleration delta

    member this.applySquashPhysics() =
        Physics.applySquashPhysics this.velocity this.BounceImpulse

    member val velocity = Vector3.Zero with get, set

    /// How fast the player moves in meters per second.
    [<Export>]
    member val Speed = 14.0f with get, set

    /// Vertical impulse applied to the character upon jumping in meters per second.
    [<Export>]
    member val JumpImpulse = 20.0f with get, set

    /// Vertical impulse applied to the character upon bouncing over a mob in meters per second.
    [<Export>]
    member val BounceImpulse = 16.0f with get, set

    /// The downward acceleration when in the air, in meters per second per second.
    [<Export>]
    member val FallAcceleration = 75.0f with get, set

    member this.AnimationPlayer =
        this.GetNode<AnimationPlayer>("AnimationPlayer")

    member this.pivot =
        this.GetNode<Spatial>("Pivot")

    member this.die() =
        this.EmitSignal(nameof HitSignal)
        this.QueueFree()

    member this.onMobDetector_BodyEntered _ = this.die ()

    override this._Ready() : unit = GD.Print "OnReady Player"

    // takes player state and preform some actions

    member this.processState (state: PlayerState) (delta: float32) =
        let mobStepHandler (slideCollision: KinematicCollision) =
            let mob = slideCollision.Collider :?> MobFS

            if Vector3.Up.Dot(slideCollision.Normal) > 0.1f then
                mob.squash ()
                this.velocity <- this.applySquashPhysics ()

        let colliderCheck () =
            let colliders =
                this.GetAllCollidersInGroup("mob")

            if Seq.isEmpty colliders = false then
                colliders |> Seq.iter mobStepHandler

        match state with
        | Idle idleState -> this.AnimationPlayer.PlaybackSpeed <- float32 idleState.PlaybackSpeed
        | Moving movingState ->
            this.pivot.LookAt(this.Translation + movingState.Direction, Vector3.Up)
            this.AnimationPlayer.PlaybackSpeed <- float32 movingState.PlaybackSpeed
        | _ -> ()

        this.velocity <- Physics.calcVelocity state this.velocity this.Speed
        this.velocity <- this.applyGravity delta
        this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
        colliderCheck ()
        this.pivot.Rotation <- this.applyRotation this.pivot

    //input -> playerState-> side effects
    override this._PhysicsProcess(delta: float32) =
        Input.pollForInput
            { playerSpace =
                match this.IsOnFloor() with
                | true -> PositionSpace.Ground
                | false -> PositionSpace.Air
              isShortPressedPredicate = Input.IsActionJustPressed
              isLongPressedPredicate = Input.IsActionPressed }
        |> Array.iter (fun playerState -> this.processState playerState delta)
