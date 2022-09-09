namespace CoreFS

open Godot

type MainFS() =
    inherit Node()
    override this._Ready() = GD.Print "E L L O"
