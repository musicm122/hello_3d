namespace CoreFS.Util

open System

module RandomUtil =

    let intInRange min max = Random().Next(min, max)

    let doubleInRange min max =
        (Random().NextDouble() * max - min) + min

    let float32InRange (min: float32) (max: float32) =
        let minDouble = (float) min
        let maxDouble = (float) max

        let result =
            doubleInRange minDouble maxDouble

        float32 (result)
