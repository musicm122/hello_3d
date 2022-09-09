namespace CoreFS.Util

open Godot

module Vector3Util =

    let Add (v1: Vector3) (v2: Vector3) =
        let x = v1.x + v2.x
        let y = v1.y + v2.y
        let z = v1.z + v2.z
        Vector3(x, y, z)

    let Subtract (v1: Vector3) (v2: Vector3) =
        let x = v1.x - v2.x
        let y = v1.y - v2.y
        let z = v1.z - v2.z
        Vector3(x, y, z)
