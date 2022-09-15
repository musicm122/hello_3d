namespace CoreFS.Util.Domain

open Godot


[<Signal>]
type SquashedSignal = delegate of unit -> unit
