namespace CoreFS.Scenes

open CoreFS.Constants.Events
open CoreFS.Domain
open Godot
open CoreFS.Util.InputEvents

open FSharp.Core

// GetNode<PlayerVariables>("/root/PlayerVariables");
type GlobalFS() =
    inherit Node()
    let initEventPublisher = SystemInitialize()
    let inputEventPublisher = SystemInputEvent()

    let processPhysicsEventPublisher =
        ProcessPhysicsEvent()

    let processFrameEventPublisher =
        ProcessFrameEvent()

    member this.InitEvent =
        initEventPublisher.Publish

    member this.NewInput =
        inputEventPublisher.Publish

    member this.processPhysicsPublisher =
        processPhysicsEventPublisher.Publish

    member this.processFramePublisher =
        processFrameEventPublisher.Publish

    member this.triggerInitEvent() = initEventPublisher.Publish

    member this.triggerPhysicsFrame(delta) =
        processPhysicsEventPublisher.Trigger(delta)

    member this.triggerFrame(delta) =
        processFrameEventPublisher.Trigger(delta)

    member this.initSceneTree() =
        let sceneTree = this.GetTree()
        sceneTree.Connect("idle_frame", this, nameof this.triggerFrame)

    member val GameEnvironment =
        { EnvironmentState.Default() with
            gameState = GameState.Playing
            playerSpace = PositionSpace.Ground
            isShortPressedPredicate = Input.IsActionJustPressed
            isLongPressedPredicate = Input.IsActionPressed } with get, set

    //public EnvironmentState EnvironmentState { get; set; } = EnvironmentState.Default();

    override this._Ready() =
        this.triggerInitEvent () |> ignore
        ()
    //this.initSceneTree()

    override this._Process(delta: float32) = this.triggerFrame (delta)

    override this._PhysicsProcess(delta: float32) = this.triggerPhysicsFrame (delta)


module Singleton =
    let private g =
        Lazy.Create(fun () -> new GlobalFS())

    let GlobalStateInstance () = g.Value
