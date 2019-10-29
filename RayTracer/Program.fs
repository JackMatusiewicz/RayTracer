// Learn more about F# at http://fsharp.org

open RayTracer

let hackyScene () =
    let x = 1920.
    let y = 1080.
    let llc = { X = -2.; Y = -1.; Z = -1. }
    let hori = { X = 4.; Y = 0.; Z = 0. }
    let vert = { X = 0.; Y = 2.; Z = 0. }
    let origin = { X = 0.; Y = 0.; Z = 0. }
    let rays =
        Array2D.init
            (int y)
            (int x)
            (fun v u ->
                let v = (int y) - v - 1
                let u = (float u) / x
                let v = (float v) / y
                let direction =
                    let a = Vector.scalarMultiply u hori
                    let b = Vector.scalarMultiply v vert
                    Vector.add a b
                    |> Vector.add llc
                    |> Vector.unitVector
                { Position = origin; Direction = direction }
            )
    Array2D.map Shape.hackySphereScene rays
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
