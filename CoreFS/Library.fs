namespace CoreFS

open Godot

type MainFs() =
    inherit Node()
    override this._Ready() =
        GD.Print "E L L O"        