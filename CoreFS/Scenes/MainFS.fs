namespace CoreFS

open CoreFS.Domain
open CoreFS.Entities
open CoreFS.Util
open Godot

type MainFS() =
    inherit Node()

    member val GameEnvironment =
        { EnvironmentState.Default() with
            gameState = GameState.Playing
            playerSpace = PositionSpace.Ground
            isShortPressedPredicate = Input.IsActionJustPressed
            isLongPressedPredicate = Input.IsActionPressed } with get, set


    [<Export>]
    member val mobScene: PackedScene = ResourceLoader.Load("res://Mob.tscn") :?> PackedScene with get, set

    member this.Timer =
        this.GetNode<Timer>("MobTimer")

    member this.Player =
        this.GetNode<PlayerFS>("Player")

    member this.UserInterface =
        this.GetNode<Control>("UserInterface")

    member this.RetryButton =
        this.GetNode<ColorRect>("UserInterface/Retry")

    member this.ScoreLabel =
        this.GetNode<ScoreLabelFS>("UserInterface/ScoreLabel")

    member this.onPlayerHit() =
        GD.Print("Player Hit")
        this.Timer.Stop()
        this.RetryButton.Show()

    member this.processGameState(gameState: GameState) =
        match gameState with
        | StartMenu -> failwith "nav to start menu"
        | YouDied -> failwith "Show retry menu"
        | Playing -> failwith "Play game"
        | Quit -> this.GetTree().Quit()
        | _ -> failwith "todo"

    member this.onMobTimeout() =
        // Create a new instance of the Mob scene.
        let mob = this.mobScene.Instance() :?> MobFS
        // Choose a random location on the SpawnPath.
        let mobSpawnLocation =
            this.GetNode("SpawnPath/SpawnLocation") :?> PathFollow

        mobSpawnLocation.UnitOffset <- GD.Randf()

        let playerPos = this.Player.Transform.origin
        mob.Initialize mobSpawnLocation.Translation playerPos
        mob.AddToGroup("Mobs")
        this.AddChild(mob)
        mob.OnMobSquashed.Add(fun _ -> this.ScoreLabel.onMobSquashed ())

    override this._UnhandledInput(inputEvent: InputEvent) =
        match inputEvent with
        | :? InputEventKey as ie when
            ie.IsActionPressed("ui_accept")
            && this.RetryButton.Visible
            ->
            this.GetTree().ReloadCurrentScene() |> ignore
        | _ -> ()

    override this._Ready() =
        GD.Randomize()

        this.Timer.Connect("timeout", this, nameof this.onMobTimeout)
        |> ignore
        //this.mobDetector.Connect("body_entered", this, nameof this.onMobDetector_BodyEntered)
        this.Player.OnPlayerHit.Add(this.onPlayerHit)
        this.RetryButton.Hide()
