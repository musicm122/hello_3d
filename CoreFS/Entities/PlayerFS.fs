namespace CoreFS.Entities

open CoreFS.Util.Constants
open CoreFS.Util.DU
open CoreFS.Util.Domain
open CoreFS.Util.InputUtil
open Godot

type PlayerFS() =
    inherit KinematicBody()
    let applyGravity (currentVelocity: Vector3) fallAcceleration delta =
        let newY =
            (currentVelocity.y - (fallAcceleration * delta))
        Vector3(currentVelocity.x, newY, currentVelocity.z)

    let applyVelocity
        (fallAcceleration: float32)
        (speed: float32)
        (currentVelocity: Vector3)
        (direction: Vector3)
        (delta: float32)
        =
        let mutable result: Vector3 =
            currentVelocity

        result.x <- direction.x * speed
        result.z <- direction.z * speed
        result <- applyGravity result fallAcceleration delta
        result

    let getDirectionInput () =
        let testForAction () (action: ActionInput) : PlayerState =
            let name = action.AsString()

            match Input.IsActionJustPressed(name) with
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

            match Input.IsActionPressed(name) with
            | true -> PlayerState.Moving { GroundedMovingData.Default() with GroundedMovingData.Direction = vec }
            | _ -> PlayerState.Idle(IdleData.Default())

        detectMovement testForMovement


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
    //    let applyVelocity (fallAcceleration:float32) (speed:float32) (direction:Vector3) (currentVelocity:Vector3) (delta:float32)  =

    member this.applyPhysics =
        applyVelocity this.FallAcceleration this.Speed


    member this.die() =
        this.EmitSignal(nameof (HitSignal))
        this.QueueFree()

    member this.onMobDetector_BodyEntered(_) = this.die ()

    override this._Ready() : unit = GD.Print "OnReady Player"

    // takes player state and preform some actions
    member this.processState (state: PlayerState) (delta: float32) =
        match state with
        | Idle idleState ->
            //this.pivot.LookAt(this.Translation + idleState.Direction, Vector3.Up)
            this.AnimationPlayer.PlaybackSpeed <- (float32) idleState.PlaybackSpeed
            this.velocity <- applyVelocity this.FallAcceleration this.Speed this.velocity idleState.Direction delta

            //this.velocity <- this.applyPhysics idleState.Direction this.velocity delta
            this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
        | Moving movingState ->
            this.pivot.LookAt(this.Translation + movingState.Direction, Vector3.Up)
            this.AnimationPlayer.PlaybackSpeed <- (float32) movingState.PlaybackSpeed
            //this.velocity <- this.applyPhysics movingState.Direction this.velocity delta
            this.velocity <- applyVelocity this.FallAcceleration this.Speed this.velocity movingState.Direction delta

            this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)

            GD.Print(
                $"moving in direction : {movingState.Direction.ToString()} with velocity :{this.velocity} Speed : {movingState.MoveSpeed}"
            )

        | Jumping jumpData ->
            this.velocity <- applyVelocity this.FallAcceleration this.Speed this.velocity jumpData.Direction delta

            let newYVelocity =
                this.velocity.y + jumpData.JumpImpulse

            this.velocity <- Vector3(this.velocity.x, newYVelocity, this.velocity.z)
            this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
            GD.Print($"jumping in direction : {jumpData.Direction.ToString()} with Speed : {jumpData.MoveSpeed}")
        | _ -> ignore ()


    //input -> playerState-> side effects
    override this._PhysicsProcess(delta: float32) =
        getDirectionInput ()
        |> Array.iter (fun state -> this.processState state delta)


// this.velocity <- this.applyPhysics direction this.velocity delta
// this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
