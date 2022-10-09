namespace CoreFS.Util

open Godot
open CoreFS.Util.Extensions

module Physics =
    let applyVelocity (velocity: Vector3) (direction: Vector3) (speed: float32) =
        velocity
            .WithX(direction.x * speed)
            .WithZ(direction.z * speed)

    let applySpeedToDirection (direction: Vector3) (speed: float32) =
        direction
            .WithX(direction.x * speed)
            .WithZ(direction.z * speed)

    let applyGravity (currentVelocity: Vector3) fallAcceleration delta =
        currentVelocity.WithY((currentVelocity.y - (fallAcceleration * delta)))

    let applyRotation (spatial: Spatial) (velocity: Vector3) jumpImpulse =
        spatial.Rotation.WithX(Mathf.Pi / 6f * velocity.y / jumpImpulse)

    let applySquashPhysics (velocity: Vector3) bounceImpulse = velocity.WithY(bounceImpulse)
