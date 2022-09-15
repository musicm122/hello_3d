namespace CoreFS.Entities

open CoreFS.Util
open CoreFS.Util.Constants
open CoreFS.Util.DU
open CoreFS.Util.Domain
open CoreFS.Util.InputUtil
open Godot
open CoreFS.Util.Extensions

type PlayerFS() =
    inherit KinematicBody()

    member this.applyVelocity(direction: Vector3) =
        Physics.applyVelocity this.velocity direction this.Speed

    member this.applyRotation(target: Spatial) =
        Physics.applyRotation target this.velocity this.JumpImpulse

    member this.applyGravity(delta) =
        Physics.applyGravity this.velocity this.FallAcceleration delta

    member this.applySquashPhysics() =
        Physics.applySquashPhysics this.velocity this.BounceImpulse

    member this.pollForInput() =
        let testForAction (action: ActionInput) : PlayerState =
            let name = action.AsString()

            match
                InputConstants.availableInputs
                |> Array.contains name
                && Input.IsActionJustPressed(name)
                with
            | true ->
                match action with
                | Jump jumpData -> PlayerState.Jumping(jumpData)
                | Pause pauseData ->
                    if pauseData = true then
                        PlayerState.Paused
                    else
                        PlayerState.Unpaused
                | _ -> PlayerState.Idle(IdleData.Default())
            | _ -> PlayerState.Idle(IdleData.Default())

        let testForMovement (movement: MovementInput) : PlayerState =
            let vec = movement.AsVector().Normalized()
            let name = movement.AsString()

            match
                InputConstants.availableInputs
                |> Array.contains name
                && Input.IsActionPressed(name)
                with
            | true -> PlayerState.Moving { GroundedMovingData.Default() with GroundedMovingData.Direction = vec }
            | _ -> PlayerState.Idle(IdleData.Default())

        let testForInput inputType =
            match inputType with
            | Action actionInput -> testForAction actionInput
            | Movement movementInput -> testForMovement movementInput

        detectActiveInput testForInput


    // takes user input state and returns player state
    member this.processInput(inputState: InputType) =
        let currentSpeed = this.Speed
        let bounce = this.BounceImpulse
        let playbackSpeed = 4

        match inputState with
        | Movement moveState ->
            let dir = moveState.AsVector()

            PlayerState.Moving
                { MoveSpeed = currentSpeed
                  Bounce = bounce
                  PlaybackSpeed = playbackSpeed
                  Direction = dir }
        | Action actionState ->
            match actionState with
            | Pause isPaused when isPaused = true -> PlayerState.Paused
            | Pause isPaused when isPaused = false -> PlayerState.Unpaused
            | Jump jumpState -> PlayerState.Jumping jumpState
            | _ -> failwith "Unsupported Action"

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

    member this.die =
        this.EmitSignal(nameof (HitSignal))
        this.QueueFree

    member this.onMobDetector_BodyEntered _ = this.die ()

    override this._Ready() : unit = GD.Print "OnReady Player"

    // takes player state and preform some actions
    member this.processState (state: PlayerState) (isOnFloor: bool) (delta: float32) =
        match state with
        | Idle idleState ->
            this.AnimationPlayer.PlaybackSpeed <- float32 idleState.PlaybackSpeed
            this.velocity <- this.applyVelocity idleState.Direction
            this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
        | Moving movingState ->
            this.pivot.LookAt(this.Translation + movingState.Direction, Vector3.Up)
            this.AnimationPlayer.PlaybackSpeed <- float32 movingState.PlaybackSpeed
            this.velocity <- this.applyVelocity movingState.Direction
            this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)

        | Jumping jumpData -> // todo: jump input height is super height
            if isOnFloor then
                this.velocity <- this.applyVelocity jumpData.Direction

                let newYVelocity =
                    this.velocity.y + jumpData.JumpImpulse

                this.velocity <- Vector3(this.velocity.x, newYVelocity, this.velocity.z)
                this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
        | _ -> ()

    member this.MobStepHandler(slideCollision: KinematicCollision) =
        let mob = slideCollision.Collider :?> MobFS

        if Vector3.Up.Dot(slideCollision.Normal) > 0.1f then
            mob.squash ()
            this.velocity <- this.applySquashPhysics ()

    //input -> playerState-> side effects
    override this._PhysicsProcess(delta: float32) =
        let isOnFloor = this.IsOnFloor()
        //if isOnFloor then GD.Print isOnFloor

        this.pollForInput ()
        |> Array.iter (fun state -> this.processState state isOnFloor delta)

        this.velocity <- this.applyGravity delta
        this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)

        //todo: convert to state
        let colliders =
            this.GetAllCollidersInGroup("mob")

        if Seq.isEmpty colliders = false then
            colliders |> Seq.iter (this.MobStepHandler)

        this.pivot.Rotation <- this.applyRotation this.pivot


// this.velocity <- this.applyPhysics direction this.velocity delta
// this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
