namespace CoreFS.Entities

open Godot

type ScoreLabelFS() =
    inherit Label()

    member val score = 0 with get, set

    member this.onMobSquashed() =
        this.score <- this.score + 1
        this.Text <- "Score: " + this.score.ToString()
