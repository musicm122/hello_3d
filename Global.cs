using System;
using CoreFS.Constants;
using CoreFS.Domain;
using CoreFS.Util;
using Godot;
using Microsoft.FSharp.Control;
using Microsoft.FSharp.Core;

namespace CoreFS.Scenes;

[CompilationMapping(SourceConstructFlags.ObjectType)]
[Serializable]
public class Global : Node
{
    internal EnvironmentState gameEnvironment;
    internal Events.SystemInitialize InitEventPublisher;
    internal InputEvents.SystemInputEvent InputEventPublisher;
    internal Events.ProcessFrameEvent ProcessFrameEventPublisher;
    internal Events.ProcessPhysicsEvent ProcessPhysicsEventPublisher;


    public Global()
    {
        bool ShortInputCheck(string s)
        {
            return false;
        }

        var shortCheck = new Converter<string, bool>((Func<string, bool>)ShortInputCheck);
        var fsharpShortCheck = FSharpFunc<string, bool>.FromConverter(shortCheck);

        bool LongInputCheck(string s)
        {
            return false;
        }

        var longCheck = new Converter<string, bool>((Func<string, bool>)LongInputCheck);
        var fsharpLongCheck = FSharpFunc<string, bool>.FromConverter(longCheck);


        var globalFs = this;
        InitEventPublisher = new Events.SystemInitialize();
        InputEventPublisher = new InputEvents.SystemInputEvent();
        ProcessPhysicsEventPublisher = new Events.ProcessPhysicsEvent();
        ProcessFrameEventPublisher = new Events.ProcessFrameEvent();
        // \u0024GlobalFS.\u002Dctor\u004054.\u0040_instance, (FSharpFunc<string, bool>) \u0024GlobalFS.\u002Dctor\u004055\u002D1.\u0040_instance);

        EnvironmentState.Default();
        GameEnvironment =
            new EnvironmentState(
                GameState.Playing,
                PositionSpace.Ground,
                fsharpShortCheck,
                fsharpLongCheck);
    }

    public IEvent<FSharpHandler<Unit>, Unit> InitEvent => InitEventPublisher.Publish;

    public IEvent<FSharpHandler<InputEvent>, InputEvent> NewInput => InputEventPublisher.Publish;

    public IEvent<FSharpHandler<float>, float> processPhysicsPublisher => ProcessPhysicsEventPublisher.Publish;

    public IEvent<FSharpHandler<float>, float> processFramePublisher => ProcessFrameEventPublisher.Publish;

    public EnvironmentState GameEnvironment
    {
        get => gameEnvironment;
        set => gameEnvironment = value;
    }

    public IEvent<FSharpHandler<Unit>, Unit> triggerInitEvent()
    {
        return InitEventPublisher.Publish;
    }

    public void triggerPhysicsFrame(float delta)
    {
        ProcessPhysicsEventPublisher.Trigger(delta);
    }

    public void triggerFrame(float delta)
    {
        ProcessFrameEventPublisher.Trigger(delta);
    }

    public Error initSceneTree()
    {
        return GetTree().Connect("idle_frame", this, "triggerFrame");
    }

    public override void _Ready()
    {
        triggerInitEvent();
    }

    public override void _Process(float delta)
    {
        triggerFrame(delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        triggerPhysicsFrame(delta);
    }
}