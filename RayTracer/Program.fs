// Learn more about F# at http://fsharp.org

open RayTracer

let hackySphere = { Center = { X = 0.; Y = 0.; Z = -1. }; Radius = 0.5 }
let hackySphereScene (r : Ray) : Colour =
    let v = Shape.sphereCollides { Min = -100.; Max = 500. } hackySphere r
    match v with
    | Some v ->
        Ray.getPosition v.T r
        |> Vector.sub hackySphere.Center
        |> Vector.unitVector
        |> UnitVector.toVector
        |> (fun {X=x;Y=y;Z=z} -> { X = x+1.; Y = y+1.; Z = z+1. })
        |> Vector.scalarMultiply 0.5
    | None ->
        let dir = UnitVector.toVector r.Direction
        let t = 0.5 * (dir.Y + 1.)
        let a =
            {X = 1.; Y = 1.; Z = 1.}
            |> Vector.scalarMultiply (1. - t)
        let b =
            {X = 0.5; Y = 0.7; Z = 1.}
            |> Vector.scalarMultiply t
        Vector.add a b
    |> fun r ->
        {
            R = r.X * 255.99 |> uint8
            G = r.Y * 255.99 |> uint8
            B = r.Z * 255.99 |> uint8
        }

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
    Array2D.map hackySphereScene rays
    |> Ppm.toPpm
    |> Ppm.toDisk "testImage"

[<EntryPoint>]
let main argv =
    hackyScene ()
    0
