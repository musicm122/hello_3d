namespace CoreFS.Util

open Godot
open CoreFS.Util.Extensions
open CoreFS.Domain

module Physics =
    let applyVelocity (velocity: Vector3) (direction: Vector3) (speed: float32) =
        velocity
            .WithX(direction.x * speed)
            .WithZ(direction.z * speed)

    let applyGravity (currentVelocity: Vector3) fallAcceleration delta =
        currentVelocity.WithY((currentVelocity.y - (fallAcceleration * delta)))

    let applyRotation (spatial: Spatial) (velocity: Vector3) jumpImpulse =
        spatial.Rotation.WithX(Mathf.Pi / 6f * velocity.y / jumpImpulse)

    let applySquashPhysics (velocity: Vector3) bounceImpulse = velocity.WithY(bounceImpulse)

    let calcVelocity (state: PlayerState) currVelocity speed =
        match state with
        | PlayerState.Idle idleState -> applyVelocity currVelocity idleState.Direction speed
        | PlayerState.Moving movingState -> applyVelocity currVelocity movingState.Direction speed
        | PlayerState.Jumping jumpData ->
            match jumpData.Position with
            | Ground ->
                let newV =
                    applyVelocity currVelocity jumpData.Direction speed

                newV.AddToY(jumpData.JumpImpulse)
            | _ -> applyVelocity currVelocity jumpData.Direction speed
        | _ -> currVelocity
