namespace CoreFS.Util.Extensions

open Godot

[<AutoOpen>]
module Extensions =
    type KinematicCollision with
        member this.ColliderIsInGroup(groupName: string) : bool =
            if this <> null then
                let body = this.Collider :?> PhysicsBody
                body.IsInGroup groupName
            else
                false

    type KinematicBody with
        member this.GetAllColliders() : Option<seq<KinematicCollision>> =
            if this = null then
                None
            else
                let slideCount = this.GetSlideCount()

                match slideCount with
                | 0 -> None
                | _ ->
                    let result =
                        seq {
                            for x in 0 .. this.GetSlideCount() - 1 do
                                yield this.GetSlideCollision(x)
                        }

                    Some(result)

        member this.GetAllCollidersInGroup(groupName: string) =
            let inGroup (collider: KinematicCollision) : bool = collider.ColliderIsInGroup(groupName)

            match this.GetAllColliders() with
            | Some colliders -> colliders |> Seq.filter inGroup
            | None -> Seq.empty
