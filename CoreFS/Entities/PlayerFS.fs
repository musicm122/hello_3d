namespace CoreFS.Entities

open CoreFS.Constants
open CoreFS.Domain
open CoreFS.Scenes
open CoreFS.Util
open Godot

type PlayerFS() =
    inherit KinematicBody()

    let hitEvent = Event<unit>()

    member this.mobDetector =
        this.GetNode<Area>("MobDetector")

    member this.pivot =
        this.GetNode<Spatial>("Pivot")

    member this.animPlayer =
        this.GetNode<AnimationPlayer>("AnimationPlayer")

    member val getCollidersInGroup: Option<string -> seq<KinematicCollision>> = None with get, set
    member val die: Option<unit -> unit> = None with get, set
    member val currentState = PlayerState.Default with get, set
    member val OnPlayerHit = hitEvent.Publish with get, set
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

    member this.onMobDetector_BodyEntered(body: Node) =
        GD.Print("onMobDetector_BodyEntered name: " + body.Name)

        match body.Name.ToLower() with
        | name when name.Contains("mob") -> hitEvent.Trigger()
        | _ -> ()

    member this.colliderCheck() =
        let mobStepHandler (slideCollision: KinematicCollision) =
            let mob = slideCollision.Collider :?> MobFS

            if Vector3.Up.Dot(slideCollision.Normal) > 0.1f then
                mob.squash ()
                this.velocity <- Physics.applySquashPhysics this.velocity this.BounceImpulse

        match this.getCollidersInGroup with
        | Some getColliders ->
            let colliders = getColliders "Mobs"

            match colliders with
            | _ -> colliders |> Seq.iter mobStepHandler
            | ActivePatterns.EmptySeq -> ()
        | None -> ()
    // takes player state and preform some actions

    member this.processGameState() =
        let env =
            Singleton.GlobalStateInstance().GameEnvironment

        match env.gameState with
        | Quit ->
            GD.Print("Quit Game")
            this.GetTree().Quit()
        | YouDied ->
            GD.Print("You Died")
            this.GetTree().ReloadCurrentScene() |> ignore
        | _ -> ()

    override this._Ready() : unit =
        this.mobDetector.Connect("body_entered", this, nameof this.onMobDetector_BodyEntered)
        |> ignore

        this.getCollidersInGroup <- Some(this.GetAllCollidersInGroup)


    member this.movementInputCheck() =
        let dir = Input.getDirectionFromInput ()

        if dir <> Vector3.Zero then
            this.pivot.LookAt(this.Translation + dir, Vector3.Up)
            this.animPlayer.PlaybackSpeed <- float32 DefaultPlayerValues.MovementPlaybackAnimationSpeed
        else
            this.animPlayer.PlaybackSpeed <- float32 DefaultPlayerValues.IdlePlaybackAnimationSpeed

        this.velocity <- this.velocity.WithX(dir.x * this.Speed)
        this.velocity <- this.velocity.WithZ(dir.z * this.Speed)

    member this.actionInputCheck() =
        if this.IsOnFloor() && Input.IsActionPressed("jump") then
            this.velocity <- this.velocity.AddToY(this.JumpImpulse)

    //input -> playerState-> side effects
    override this._PhysicsProcess(delta: float32) =
        this.processGameState ()
        this.movementInputCheck ()
        this.actionInputCheck ()
        //gravity
        this.velocity <- this.velocity.WithY(this.velocity.y - (this.FallAcceleration * delta))
        this.velocity <- this.MoveAndSlide(this.velocity, Vector3.Up)
        this.colliderCheck ()
        //	# This makes the character follow a nice arc when jumping
        let xRotation =
            Mathf.Pi / 6f * this.velocity.y / this.JumpImpulse

        this.pivot.Rotation <- this.pivot.Rotation.WithX(xRotation)
