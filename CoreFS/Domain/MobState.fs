namespace CoreFS.Domain

open CoreFS.Constants
open CoreFS.Util
open Godot

type MobState =
    | Aggro
    | Squashed
    | OffScreen

type MobModel =
    { onFrameTick: Option<(float32 -> unit)>
      onPhysicsTick: Option<(float32 -> unit)>
      squashed: Option<(unit -> unit)>
      onScreenExit: Option<(unit -> unit)>
      onInit: Option<(Vector3 -> Vector3 -> unit)>
      animationPlayer: AnimationPlayer
      self: KinematicBody
      minSpeed: float32
      maxSpeed: float32
      speed: float32
      mutable velocity: Vector3 }
    static member Default (owner: KinematicBody) (animPlayer: AnimationPlayer) =
        { onFrameTick = None
          onPhysicsTick = None
          squashed = None
          onScreenExit = None
          onInit = None
          velocity = Vector3.Zero
          animationPlayer = animPlayer
          self = owner
          minSpeed = DefaultMobValues.MinSpeed
          maxSpeed = DefaultMobValues.MaxSpeed
          speed = DefaultMobValues.MinSpeed }

    static member init
        (startPos: Vector3)
        (playerPos: Vector3)
        (self: KinematicBody)
        (animPlayer: AnimationPlayer)
        (minSpeed: float32)
        (maxSpeed: float32)
        : MobModel =
        let getRandomSpeed =
            getRandFloat32InRange minSpeed maxSpeed

        let calcVelocityWithRotation speed (yRotation: float32) =
            // We calculate a forward velocity first, which represents the speed.
            let (newVel: Vector3) =
                Vector3.Forward * speed
            // We then rotate the vector based on the mob's Y rotation to move in the direction it's looking.
            newVel.Rotated(Vector3.Up, yRotation)

        self.LookAtFromPosition(startPos, playerPos, Vector3.Up)
        let min = -Mathf.Pi / 4.0f
        let max = Mathf.Pi / 4.0f
        self.RotateY(RandomUtil.getRandFloat32InRange min max)


        let randomSpeed =
            getRandFloat32InRange minSpeed maxSpeed

        // We calculate a forward velocity first, which represents the speed.
        let velocity =
            calcVelocityWithRotation randomSpeed self.Rotation.y

        // We then rotate the vector based on the mob's Y rotation to move in the direction it's looking.
        animPlayer.PlaybackSpeed <- (randomSpeed / min)
        { MobModel.Default self animPlayer with speed = getRandomSpeed }
