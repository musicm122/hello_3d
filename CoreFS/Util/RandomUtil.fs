namespace CoreFS.Util

open System

[<AutoOpen>]
type RandomUtil =

    static member getRandIntInRange min max =
        let r = new Godot.RandomNumberGenerator()
        r.RandiRange(min, max)

    static member getDoubleInRange min max =
        (Random().NextDouble() * max - min) + min

    static member getRandFloat32InRange (min: float32) (max: float32) =
        let r = new Godot.RandomNumberGenerator()
        r.RandfRange(min, max)

    static member getRandFloat32() =
        let r = new Godot.RandomNumberGenerator()
        r.Randf()

    static member getRandFloat() = float (Random().NextDouble())
