namespace CoreFS.Util

open Godot

module Physics =
    let applyVelocity (velocity: Vector3) (direction: Vector3) (speed: float32) =
        Vector3(direction.x * speed, velocity.y, direction.z * speed)

    let applyGravity (currentVelocity: Vector3) fallAcceleration delta =
        let newY =
            (currentVelocity.y - (fallAcceleration * delta))

        Vector3(currentVelocity.x, newY, currentVelocity.z)

    let applyRotation (spatial: Spatial) (velocity: Vector3) jumpImpulse =
        let newRotation =
            Mathf.Pi / 6f * velocity.y / jumpImpulse

        spatial.Rotation <- Vector3(newRotation, spatial.Rotation.y, spatial.Rotation.z)
        spatial.Rotation

    let applySquashPhysics (velocity: Vector3) bounceImpulse =
        Vector3(velocity.x, bounceImpulse, velocity.z)
